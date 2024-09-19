using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class PlaceManager : MonoBehaviour
{
    [Header("Managers")]
    public DatabaseManager db;
    public GoogleMapAPI gMap;
    public GPSManager gps;
    public ToastManager toastManager;
    public ProfileManager profileManager;
    public Animator canvasAnim;

    [Header("Details")]
    public GameObject GameObject_Details;
    public RawImage thumbnail_Details;
    public Text spotName_Details;
    public Text address_Details;
    public Text heart_Details;
    public Text fish_Details;
    public RawImage map_Details;
    public GameObject heartOn_Details;
    public GameObject heartOff_Details;
    public GameObject heartAnim_Details;
    string[] species_Details;

    [Header("Search")]
    public InputField searchField;

    int openIndex = 0;

    [System.Serializable]
    public class Popular
    {
        public RawImage thumbnail;
        public Text spotName;
    }
    public Popular[] popular;

    int[] value = new int[10];
    int[] index = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};

    public GameObject spot_prefab;
    public Transform spot_parent;

    int count;

    bool doRefresh_all = false;
    bool doRefresh_popular = false;
    public bool doRefresh_like = false;

    string[] species;

    public void CreateNewSpot(int i)
    {
        count = i;
        doRefresh_all = true;
    }

    public void Init_Popular()
    {
        for (int i = 0; i < db.p.Length; i++)
        {
            value[i] = db.p[i].like;
        }

        for (int i = 0; i < db.p.Length; i++)
        {
            for (int j = 0; j < (db.p.Length - 1) - i; j++)
            {
		        if (value[j] < value[j + 1])
                {
			        int temp = value[j];
			        value[j] = value[j + 1];
			        value[j + 1] = temp;

                    temp = index[j];
			        index[j] = index[j + 1];
			        index[j + 1] = temp;
		        }
	        }
        }
        doRefresh_popular = true;
    }

    void Update()
    {
        if (doRefresh_popular)
        {
            for (int i = 0; i < 4; i++)
            {
                StartCoroutine(LoadImage(popular[i].thumbnail, db.p[index[i]].representImage, 700));
                popular[i].spotName.text = db.p[index[i]].spotName;
            }
            doRefresh_popular = false;
        }

        if(doRefresh_all)
        {
            for (int j = 0; j < count; j++)
            {
                var newSpot = Instantiate(spot_prefab);
                newSpot.transform.SetParent(spot_parent, false);
                StartCoroutine(LoadImage(newSpot.transform.GetChild(0).GetChild(0).GetComponent<RawImage>(), db.p[j].representImage, 400));
                newSpot.transform.GetChild(1).GetComponent<Text>().text = db.p[j].spotName;
                newSpot.transform.GetChild(2).GetComponent<Text>().text = db.p[j].displayLocation;
                species = db.p[j].fish.Split('+');
                string tmp = "";
                for (int m = 0; m < species.Length; m++)
                {
                    tmp += ("#" + species[m] + " ");
                }
                newSpot.transform.GetChild(3).GetComponent<Text>().text = tmp;
            }
            doRefresh_all = false;
        }

        if(doRefresh_like)
        {
            if (PlayerPrefs.GetInt("Like" + db.p[openIndex].spotName) == 0)
            {
                //Check and Update
                int like = db.like_count;
                like++;
                db.LikeUpdate("Places", db.p[openIndex].spotName, like.ToString());
                db.p[openIndex].like = like;

                //Action
                PlayerPrefs.SetInt("Like" + db.p[openIndex].spotName, 1);
                heartOn_Details.SetActive(true);
                heartOff_Details.SetActive(false);
                heartAnim_Details.SetActive(true);
                StartCoroutine(HeartAnim_Off());
                heart_Details.text = like.ToString();
                profileManager.MyStar_Refresh();
                #if UNITY_ANDROID
                toastManager.doToast("즐겨찾기가 반영되었습니다.");
                #endif
            }
            else if (PlayerPrefs.GetInt("Like" + db.p[openIndex].spotName) == 1)
            {
                //Check and Update
                int like = db.like_count;
                like--;
                db.LikeUpdate("Places", db.p[openIndex].spotName, like.ToString());
                db.p[openIndex].like = like;

                //Action
                PlayerPrefs.SetInt("Like" + db.p[openIndex].spotName, 0);
                heartOn_Details.SetActive(false);
                heartOff_Details.SetActive(true);
                heart_Details.text = like.ToString();
                profileManager.MyStar_Refresh();
                #if UNITY_ANDROID
                toastManager.doToast("즐겨찾기가 취소되었습니다.");
                #endif
            }
            doRefresh_like = false;
        }
    }

    IEnumerator LoadImage(RawImage rawImage, string url, float fitSize)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            float width, height;

            rawImage.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            rawImage.SetNativeSize();
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

    public void Details(int c)
    {
        canvasAnim.SetBool("Place_Open", true);

        GameObject thisObject = EventSystem.current.currentSelectedGameObject;

        string name = thisObject.transform.GetChild(c + 1).GetComponent<Text>().text;
        openIndex = 0;
        for(int i = 0; i < count; i++)
        {
            if (db.p[i].spotName == name)
            {
                openIndex = i;
                break;
            }
        }

        StartCoroutine(LoadImage(thumbnail_Details, db.p[openIndex].representImage, 1450));
        spotName_Details.text = db.p[openIndex].spotName;
        address_Details.text = db.p[openIndex].displayLocation;
        if (PlayerPrefs.GetInt("Like" + db.p[openIndex].spotName) == 0)
        {
            heartOn_Details.SetActive(false);
            heartOff_Details.SetActive(true);
        }
        else if (PlayerPrefs.GetInt("Like" + db.p[openIndex].spotName) == 1)
        {
            heartOn_Details.SetActive(true);
            heartOff_Details.SetActive(false);
        }
        heart_Details.text = db.p[openIndex].like.ToString();
        species_Details = db.p[openIndex].fish.Split('+');
        string tmp = "";
        for (int m = 0; m < species_Details.Length; m++)
        {
            if (m < species_Details.Length - 1)
                tmp += (species_Details[m] + ", ");
            else
                tmp += species_Details[m];
        }
        fish_Details.text = tmp;
        gMap.Maps(map_Details, db.p[openIndex].lat, db.p[openIndex].lot, 15, 1, GoogleMapAPI.mapType.roadmap);
    }

    public void Details_Instance(string path)
    {
        canvasAnim.SetBool("Place_Open", true);

        openIndex = 0;
        for(int i = 0; i < count; i++)
        {
            if (db.p[i].spotName == path)
            {
                openIndex = i;
                break;
            }
        }

        StartCoroutine(LoadImage(thumbnail_Details, db.p[openIndex].representImage, 1450));
        spotName_Details.text = db.p[openIndex].spotName;
        address_Details.text = db.p[openIndex].displayLocation;
        if (PlayerPrefs.GetInt("Like" + db.p[openIndex].spotName) == 0)
        {
            heartOn_Details.SetActive(false);
            heartOff_Details.SetActive(true);
        }
        else if (PlayerPrefs.GetInt("Like" + db.p[openIndex].spotName) == 1)
        {
            heartOn_Details.SetActive(true);
            heartOff_Details.SetActive(false);
        }
        heart_Details.text = db.p[openIndex].like.ToString();
        species_Details = db.p[openIndex].fish.Split('+');
        string tmp = "";
        for (int m = 0; m < species_Details.Length; m++)
        {
            if (m < species_Details.Length - 1)
                tmp += (species_Details[m] + ", ");
            else
                tmp += species_Details[m];
        }
        fish_Details.text = tmp;
        gMap.Maps(map_Details, db.p[openIndex].lat, db.p[openIndex].lot, 15, 1, GoogleMapAPI.mapType.roadmap);
    }

    public void Close_Details()
    {
        canvasAnim.SetBool("Place_Open", false);
    }

    public void PathButton()
    {
        double lat;
        double lot;
        gps.RetrieveGPSData();
        lat = gps.latitude;
        lot = gps.longitude;

        Application.OpenURL("https://www.google.com/maps/dir/" + lat + "," + lot + db.p[openIndex].webLocation);
    }

    public void Like()
    {
        db.ReadLike("Places", db.p[openIndex].spotName);
    }

    IEnumerator HeartAnim_Off()
    {
        yield return new WaitForSeconds(0.5f);
        heartAnim_Details.SetActive(false);
    }

    public void Search_Button()
    {
        for (int i = 0; i < db.p.Length; i++)
        {
            if (searchField.text == db.p[i].spotName)
            {
                canvasAnim.SetBool("Place_Open", true);
                string name = searchField.text;
                openIndex = i;

                StartCoroutine(LoadImage(thumbnail_Details, db.p[openIndex].representImage, 1450));
                spotName_Details.text = db.p[openIndex].spotName;
                address_Details.text = db.p[openIndex].displayLocation;
                if (PlayerPrefs.GetInt("Like" + db.p[openIndex].spotName) == 0)
                {
                    heartOn_Details.SetActive(false);
                    heartOff_Details.SetActive(true);
                }
                else if (PlayerPrefs.GetInt("Like" + db.p[openIndex].spotName) == 1)
                {
                    heartOn_Details.SetActive(true);
                    heartOff_Details.SetActive(false);
                }
                heart_Details.text = db.p[openIndex].like.ToString();
                species_Details = db.p[openIndex].fish.Split('+');
                string tmp = "";
                for (int m = 0; m < species_Details.Length; m++)
                {
                    if (m < species_Details.Length - 1)
                        tmp += (species_Details[m] + ", ");
                    else
                        tmp += species_Details[m];
                }
                fish_Details.text = tmp;
                gMap.Maps(map_Details, db.p[openIndex].lat, db.p[openIndex].lot, 15, 1, GoogleMapAPI.mapType.roadmap);
                searchField.text = "";
                return;
            }
        }
        #if UNITY_ANDROID
        toastManager.doToast("존재하지 않는 장소명입니다.");
        #endif
    }
}
