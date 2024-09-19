using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using UnityEngine.Android;

public class Tensorflow : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")] string fileName = "fish_model.tflite";
    [SerializeField] TextAsset labelMap = null;
    [SerializeField] TextAsset scientificLabelMap = null;
    [SerializeField] TextAsset descriptionMap = null;
    private WebCamTexture camTexture;
    public RawImage cameraView;
    public ResultManager resultManager;
    public GameObject resultPanel;
    public RawImage InputBlurred;
    public Animator canvasAnim;
    Predictor pd;

    public string[] labels;
    public string[] labels_scientific;
    public string[] labels_description;
    int index;
    float score;

    public LoadScene sceneManager;

    void Start()
    {
        sceneManager.FadeOutTo();

        //Init Resolution
        Screen.SetResolution(1440, 2960, true);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        pd = new Predictor(path);

        #if UNITY_ANDROID
        InitCam_Android();
        #elif UNITY_IOS
        //InitCam_Android();
        #endif

        labels = labelMap.text.Split('\n');
        labels_scientific = scientificLabelMap.text.Split('\n');
        labels_description = descriptionMap.text.Split('§');
    }

    void InitCam_PC()
    {
        string cameraName = WebCamUtil.FindName();
        camTexture = new WebCamTexture(cameraName, 1280, 720, 30);
        cameraView.texture = camTexture;
        camTexture.Play();
        Debug.Log($"Starting camera: {cameraName}");
    }

    void InitCam_Android()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            Permission.RequestUserPermission(Permission.Camera);

        if (WebCamTexture.devices.Length == 0)
        {
            Debug.Log("No Camera!");
            return;
        }

        WebCamDevice[] devices = WebCamTexture.devices;
        int selectedCameraIndex = -1;

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
            {
                selectedCameraIndex = i;
                break;
            }
        }

        if (selectedCameraIndex >= 0)
        {
            camTexture = new WebCamTexture(devices[selectedCameraIndex].name);
            camTexture.requestedFPS = 60;
            cameraView.texture = camTexture;
            camTexture.Play();
        }
    }

    void OnDestroy()
    {
        camTexture?.Stop();
        pd?.Dispose();
    }

    public void OnClick()
    {
        pd.Invoke(camTexture);
        var result = pd.GetResults();
        index = result.index;
        score = result.score;
        resultPanel.SetActive(true);
        InputBlurred.texture = camSnap();
        resultManager.InitResult(labels[index], labels_scientific[index], labels_description[index], index, score);
        canvasAnim.SetBool("Open", true);
    }

    public void FromGallery(Texture2D inputTex)
    {
        pd.Invoke(inputTex);
        var result = pd.GetResults();
        index = result.index;
        score = result.score;
        resultPanel.SetActive(true);
        InputBlurred.texture = null;
        resultManager.InitResult(labels[index], labels_scientific[index], labels_description[index], index, score);
        canvasAnim.SetBool("Open", true);
    }

    Texture2D camSnap()
    {
        Texture2D snap = new Texture2D(camTexture.width, camTexture.height);
        snap.SetPixels(camTexture.GetPixels());
        snap.Apply();

        return snap;
    }
}
