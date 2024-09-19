using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;

public class ProfileManager : MonoBehaviour
{
    [Header("Manager")]
    public DatabaseManager databaseManager;
    public UploadManager uploadManager;
    public ToastManager toastManager;
    public PostManager postManager;
    public PlaceManager placeManager;

    [Header("UI")]
    public Text Username;
    public RawImage profileImage;
    
    [Header("My Post")]
    public GameObject myPost_prefab;
    public Transform myPost_parent;

    [Header("My Post")]
    public GameObject myStar_prefab;
    public Transform myStar_parent;

    [Header("Animator")]
    public Animator canvasAnim;

    [Header("Profile Image")]
    public GameObject Loading;

    [Header("Variable")]
    [HideInInspector]
    public bool doRefresh_myPost;

    [Header("Firebase")]
    FirebaseStorage storage;
    StorageReference storageReference;

    void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://a-nice-fisherman.appspot.com");
        Username.text = PlayerPrefs.GetString("Username");
        getProfileImage();
    }

    void Update()
    {
        if (doRefresh_myPost)
        {
            foreach(Transform child in myPost_parent)
            {
                if (child.gameObject.tag == "My Post")
                Destroy(child.gameObject);
            }
            
            for (int i = 0; i < databaseManager.posts.Length; i++)
            {
                if (databaseManager.posts[i].userName == PlayerPrefs.GetString("Username"))
                {
                    var myPost = Instantiate(myPost_prefab);
                    myPost.transform.SetParent(myPost_parent, false);
                    myPost.transform.GetChild(1).GetComponent<Text>().text = databaseManager.posts[i].date;
                    StorageReference image = storageReference.Child("Images").Child(databaseManager.posts[i].userName).Child(databaseManager.posts[i].fileName);
                    image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
                    {
                        if(!task.IsFaulted && !task.IsCanceled)
                        {
                            StartCoroutine(LoadImage(Convert.ToString(task.Result), myPost.transform.GetChild(0).GetComponent<RawImage>(), 470));
                        }
                        else
                        {
                            Debug.Log(task.Exception);
                        }
                    });
                }
            }
            doRefresh_myPost = false;
        }
    }

    public void MyStar_Refresh()
    {
        foreach(Transform child in myStar_parent)
        {
            if (child.gameObject.tag == "My Star")
            Destroy(child.gameObject);
        }
        
        for (int i = 0; i < databaseManager.p.Length; i++)
        {
            if (PlayerPrefs.GetInt("Like" + databaseManager.p[i].spotName) == 1)
            {
                var myStar = Instantiate(myStar_prefab);
                myStar.transform.SetParent(myStar_parent, false);
                myStar.transform.GetChild(1).GetComponent<Text>().text = databaseManager.p[i].spotName;
                StartCoroutine(LoadImage(databaseManager.p[i].representImage, myStar.transform.GetChild(0).GetComponent<RawImage>(), 470));
            }
        }
    }

    void getProfileImage()
    {
        StorageReference image = storageReference.Child("ProfileImages").Child(PlayerPrefs.GetString("Username") + ".jpeg");
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if(!task.IsFaulted && !task.IsCanceled)
            {
                StartCoroutine(LoadImage(Convert.ToString(task.Result), profileImage, 300));
            }
            else
            {
                Debug.Log(task.Exception);
            }
        });
    }
    
    public void Submit(Texture2D tex)
    {
        Loading.SetActive(true);
        StartCoroutine(uploadManager.ProfileImageUpload(tex));
    }

    public void UploadSuccess()
    {
        StorageReference image = storageReference.Child("ProfileImages").Child(PlayerPrefs.GetString("Username") + ".jpeg");
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if(!task.IsFaulted && !task.IsCanceled)
            {
                StartCoroutine(LoadImage(Convert.ToString(task.Result), profileImage, 300));
            }
            else
            {
                Debug.Log(task.Exception);
            }
        });
        Loading.SetActive(false);
        toastManager.doToast("프로필 이미지가 변경되었습니다.");
    }
    
    IEnumerator LoadImage(string MediaUrl, RawImage rawImage, int fitSize)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if(request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            rawImage.SetNativeSize();

            float width, height;

            if (rawImage.GetComponent<RectTransform>().sizeDelta.x > rawImage.GetComponent<RectTransform>().sizeDelta.y)
            {
                height = 1;
                width = (rawImage.GetComponent<RectTransform>().sizeDelta.x / rawImage.GetComponent<RectTransform>().sizeDelta.y);

                width *= fitSize;
                height *= fitSize;

                rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            }
            else
            {
                width = 1;
                height = (rawImage.GetComponent<RectTransform>().sizeDelta.y / rawImage.GetComponent<RectTransform>().sizeDelta.x);

                width *= fitSize;
                height *= fitSize;

                rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            }
        }
    }

/* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */
    //Buttons
    public void MyPost()
    {
        canvasAnim.SetBool("MyPost_Open", true);
    }

    public void MyPost_Back()
    {
        canvasAnim.SetBool("MyPost_Open", false);
    }

    public void MyStar()
    {
        foreach(Transform child in myStar_parent)
        {
            if (child.gameObject.tag == "My Star")
            Destroy(child.gameObject);
        }
        
        for (int i = 0; i < databaseManager.p.Length; i++)
        {
            if (PlayerPrefs.GetInt("Like" + databaseManager.p[i].spotName) == 1)
            {
                var myStar = Instantiate(myStar_prefab);
                myStar.transform.SetParent(myStar_parent, false);
                myStar.transform.GetChild(1).GetComponent<Text>().text = databaseManager.p[i].spotName;
                StartCoroutine(LoadImage(databaseManager.p[i].representImage, myStar.transform.GetChild(0).GetComponent<RawImage>(), 470));
            }
        }
        canvasAnim.SetBool("MyStar_Open", true);
    }

    public void MyStar_Back()
    {
        canvasAnim.SetBool("MyStar_Open", false);
    }
}
