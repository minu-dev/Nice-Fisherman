using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;

public class PostManager : MonoBehaviour
{
    [Header("Managers")]
    public DatabaseManager databaseManager;
    public UploadManager uploadManager;
    public NativeManager nativeManager;
    public ToastManager toastManager;

    [Header("Post Prefab")]
    public GameObject post_prefab;
    public Transform post_parent;
    public GameObject comment_prefab;
    public Transform comment_parent;

    [Header("New Post")]
    public Animator newPostAnim;
    public RawImage uploadedImage;
    public InputField inputDescription;
    public GameObject uploadGuide;
    public GameObject Loading;
    public Text limitCount;

    [Header("Details")]
    public Animator canvasAnim;
    public RawImage DET_profileImage;
    public Text DET_name;
    public Text DET_date;
    public Text DET_heart;
    public Text DET_description;
    public GameObject DET_heartFilled;
    public GameObject DET_heartOutlined;
    public GameObject DET_heartAnim;
    public GameObject DET_delete;
    public GameObject DET_confirmDelete;
    public GameObject DET_confirmCommentDelete;
    public GameObject DET_scrollView;
    [HideInInspector]
    public int openIndex;

    [Header("Comment")]
    public InputField commentField;

    [Header("Variable")]
    int count;
    string date;
    bool doRefresh_post;
    GameObject selecetedPost;
    string selecetedPostID;
    int selecetedPostIndex;
    string selectedKey;
    [HideInInspector]
    public bool doRefresh_like;
    [HideInInspector]
    public bool doRefresh_like_details;
    [HideInInspector]
    public bool doRefresh_comment;
    

    [Header("Firebase")]
    FirebaseStorage storage;
    StorageReference storageReference;

    void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://a-nice-fisherman.appspot.com");
    }

/* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */
    //Add New Post
    public void NewPost()
    {
        uploadedImage.texture = null;
        inputDescription.text = "";
        uploadGuide.SetActive(true);
        newPostAnim.SetBool("Open", true);
    }

    public void ReadGallery(Texture2D tex)
    {
        uploadGuide.SetActive(false);

        uploadedImage.texture = tex;
        uploadedImage.SetNativeSize();

        float width, height;

        if (uploadedImage.GetComponent<RectTransform>().sizeDelta.x > uploadedImage.GetComponent<RectTransform>().sizeDelta.y)
        {
            height = 1;
            width = (uploadedImage.GetComponent<RectTransform>().sizeDelta.x / uploadedImage.GetComponent<RectTransform>().sizeDelta.y);

            width *= 1240;
            height *= 1240;

            uploadedImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }
        else
        {
            width = 1;
            height = (uploadedImage.GetComponent<RectTransform>().sizeDelta.y / uploadedImage.GetComponent<RectTransform>().sizeDelta.x);

            width *= 1240;
            height *= 1240;

            uploadedImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }
    }

    public void Submit()
    {
        if (uploadedImage.texture != null)
        {
            date = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
            Loading.SetActive(true);
            StartCoroutine(uploadManager.ImageUpload(uploadedImage.texture as Texture2D, date));
        }
        else
        {
            toastManager.doToast("이미지를 첨부해주세요.");
        }
    }

    public void UploadSuccess()
    {
        databaseManager.AddPost(PlayerPrefs.GetString("Username"), inputDescription.text.ToString(), PlayerPrefs.GetString("Username") + "-" + date + ".jpeg", date, 0);
        Loading.SetActive(false);
        newPostAnim.SetBool("Open", false);
        toastManager.doToast("게시글이 작성되었습니다.");
        databaseManager.ReadPostOnly();
    }

    public void Close()
    {
        newPostAnim.SetBool("Open", false);
    }

/* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */
    //Read Post
    public void InitPost(int receive)
    {
        //Change Variable;
        count = receive;
        doRefresh_post = true;
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
    //Like
    public void Like()
    {
        Transform curClick = EventSystem.current.currentSelectedGameObject.transform;
        selecetedPost = curClick.parent.gameObject.transform.parent.gameObject;
        selecetedPostID = curClick.parent.gameObject.transform.parent.gameObject.transform.GetChild(7).GetComponent<Text>().text.ToString();
        selecetedPostIndex = int.Parse(curClick.parent.gameObject.transform.parent.gameObject.transform.GetChild(8).GetComponent<Text>().text.ToString());
        databaseManager.ReadLike("Post", selecetedPostID);
    }

    public void Like_Details()
    {
        databaseManager.ReadLike_Details("Post", databaseManager.posts[openIndex].date);
    }

/* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */
    //Details
    public void Details(string path)
    {
        //Destroy old Comments
        foreach(Transform child in comment_parent)
        {
            if (child.gameObject.tag == "Comment")
                Destroy(child.gameObject);
        }

        canvasAnim.SetBool("Post_Open", true);

        for (int i = 0; i < databaseManager.posts.Length; i++)
        {
            if (path == databaseManager.posts[i].date)
            {
                openIndex = i;
                break;
            }
        }

        if (databaseManager.posts[openIndex].userName == PlayerPrefs.GetString("Username"))
            DET_delete.SetActive(true);
        else DET_delete.SetActive(false);

        DET_name.text = databaseManager.posts[openIndex].userName;
        string[] tmp_1 = databaseManager.posts[openIndex].date.Split('-');
        string[] tmp_2 = tmp_1[0].Split('_');
        string tmp_3 = tmp_2[0] + "년 " + tmp_2[1] + "월 " + tmp_2[2] + "일";
        DET_date.text = tmp_3;
        DET_heart.text = databaseManager.posts[openIndex].like.ToString() + "명의 사용자가 이 게시물을 좋아합니다.";
        DET_description.text = databaseManager.posts[openIndex].description;
        if (PlayerPrefs.GetInt("Like" + databaseManager.posts[openIndex].date) == 0)
        {
            DET_heartFilled.SetActive(false);
            DET_heartOutlined.SetActive(true);
        }
        else if (PlayerPrefs.GetInt("Like" + databaseManager.posts[openIndex].date) == 1)
        {
            DET_heartFilled.SetActive(true);
            DET_heartOutlined.SetActive(false);
        }
            
        //Create Comments
        for (int i = 0; i < databaseManager.comments.Length; i++)
        {
            if (databaseManager.comments[i].postID == path)
            {
                var newComment = Instantiate(comment_prefab);
                newComment.transform.SetParent(comment_parent, false);
                newComment.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = databaseManager.comments[i].userName;
                string[] DET_tmp_1 = databaseManager.comments[i].date.Split('-');
                string[] DET_tmp_2 = DET_tmp_1[0].Split('_');
                string DET_tmp_3 = DET_tmp_2[0] + "년 " + DET_tmp_2[1] + "월 " + DET_tmp_2[2] + "일";
                newComment.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = DET_tmp_3;
                newComment.transform.GetChild(1).GetComponent<Text>().text = databaseManager.comments[i].content;
                newComment.transform.GetChild(3).GetComponent<Text>().text = databaseManager.comments[i].keyID;
                
                if (databaseManager.comments[i].userName == PlayerPrefs.GetString("Username"))
                {
                    newComment.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                }
            }
        }
        DET_scrollView.SetActive(false);
        DET_scrollView.SetActive(true);
    }

    public void Back_Details()
    {
        databaseManager.ReadPostOnly();
        canvasAnim.SetBool("Post_Open", false);
    }

/* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */
    //Comment
    public void Create_Comment()
    {
        if (commentField.text != "")
        {
            date = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
            databaseManager.AddComment(databaseManager.posts[openIndex].date, PlayerPrefs.GetString("Username"), commentField.text, date);
            databaseManager.ReadCommentOnly(databaseManager.posts[openIndex].date);
            commentField.text = "";
            #if UNITY_ANDROID
            toastManager.doToast("댓글이 작성되었습니다.");
            #endif
        }
    }

/* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */
    //Refresh
    public void Refresh_Button()
    {
        databaseManager.ReadPostOnly();
    }
    
/* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */
    //Delete
    public void Delete_Start()
    {
        DET_confirmDelete.SetActive(true);
    }

    public void Delete_Confirm()
    {
        databaseManager.RemovePost(databaseManager.posts[openIndex].date);
        for (int i = 0; i < databaseManager.comments.Length; i++)
        {
            if (databaseManager.comments[i].postID == databaseManager.posts[openIndex].date)
            {
                databaseManager.RemoveComment(databaseManager.comments[i].keyID);
            }
        }
        databaseManager.ReadPostOnly();
        canvasAnim.SetBool("Post_Open", false);
        DET_confirmDelete.SetActive(false);
    }

    public void Delete_Cancel()
    {
        DET_confirmDelete.SetActive(false);
    }

/* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */
    //Comment Delete
    public void Comment_Delete_Start(string key)
    {
        selectedKey = key;
        DET_confirmCommentDelete.SetActive(true);
    }

    public void Comment_Delete_Confirm()
    {
        databaseManager.RemoveComment(selectedKey);
        selectedKey = "";
        databaseManager.ReadCommentOnly(databaseManager.posts[openIndex].date);
        DET_confirmCommentDelete.SetActive(false);
    }

    public void Comment_Delete_Cancel()
    {
        selectedKey = "";
        DET_confirmCommentDelete.SetActive(false);
    }

/* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */
    void Update()
    {
        if (doRefresh_post)
        {
            //Destroy Old Posts
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Post");
            for (int j = 0; j < objects.Length; j++) 
                Destroy(objects[j]);
            
            //Create New Posts
            for (int i = count - 1; i >= 0; i--)
            {
                var newPost = Instantiate(post_prefab);
                newPost.transform.SetParent(post_parent, false);
                newPost.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = databaseManager.posts[i].userName;
                string[] tmp_1 = databaseManager.posts[i].date.Split('-');
                string[] tmp_2 = tmp_1[0].Split('_');
                string tmp_3 = tmp_2[0] + "년 " + tmp_2[1] + "월 " + tmp_2[2] + "일";
                newPost.transform.GetChild(3).GetChild(2).GetComponent<Text>().text = tmp_3;
                newPost.transform.GetChild(4).GetComponent<Text>().text = databaseManager.posts[i].description;
                newPost.transform.GetChild(5).GetChild(3).GetComponent<Text>().text = databaseManager.posts[i].like.ToString();
                newPost.transform.GetChild(7).GetComponent<Text>().text = databaseManager.posts[i].date;
                newPost.transform.GetChild(8).GetComponent<Text>().text = i.ToString();
                if (PlayerPrefs.GetInt("Like" + databaseManager.posts[i].date) == 0)
                {
                    newPost.transform.GetChild(5).GetChild(1).gameObject.SetActive(true);
                    newPost.transform.GetChild(5).GetChild(2).gameObject.SetActive(false);
                }
                else if (PlayerPrefs.GetInt("Like" + databaseManager.posts[i].date) == 1)
                {
                    newPost.transform.GetChild(5).GetChild(1).gameObject.SetActive(false);
                    newPost.transform.GetChild(5).GetChild(2).gameObject.SetActive(true);
                }
                StorageReference image = storageReference.Child("Images").Child(databaseManager.posts[i].userName).Child(databaseManager.posts[i].fileName);
                image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
                {
                    if(!task.IsFaulted && !task.IsCanceled)
                    {
                        StartCoroutine(LoadImage(Convert.ToString(task.Result), newPost.transform.GetChild(0).GetChild(0).GetComponent<RawImage>(), 1740));
                    }
                    else
                    {
                        Debug.Log(task.Exception);
                    }
                });
                image = storageReference.Child("ProfileImages").Child(databaseManager.posts[i].userName + ".jpeg");
                image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
                {
                    if(!task.IsFaulted && !task.IsCanceled)
                    {
                        StartCoroutine(LoadImage(Convert.ToString(task.Result), newPost.transform.GetChild(2).GetChild(0).GetComponent<RawImage>(), 150));
                    }
                    else
                    {
                        Debug.Log(task.Exception);
                    }
                });
            }
            doRefresh_post = false;
        }

        if (doRefresh_like)
        {
            if (PlayerPrefs.GetInt("Like" + selecetedPostID) == 0)
            {
                //Check and Update
                int like = databaseManager.like_count;
                like++;
                databaseManager.LikeUpdate("Post", selecetedPostID, like.ToString());
                databaseManager.posts[selecetedPostIndex].like = like;

                //Action
                PlayerPrefs.SetInt("Like" + selecetedPostID, 1);
                selecetedPost.transform.GetChild(5).GetChild(0).gameObject.SetActive(true);
                selecetedPost.transform.GetChild(5).GetChild(1).gameObject.SetActive(false);
                selecetedPost.transform.GetChild(5).GetChild(2).gameObject.SetActive(true);
                StartCoroutine(HeartAnim_Off(selecetedPost.transform.GetChild(5).GetChild(0).gameObject));
                selecetedPost.transform.GetChild(5).GetChild(3).gameObject.GetComponent<Text>().text = like.ToString();
                #if UNITY_ANDROID
                toastManager.doToast("좋아요가 반영되었습니다.");
                #endif
            }
            else if (PlayerPrefs.GetInt("Like" + selecetedPostID) == 1)
            {
                //Check and Update
                int like = databaseManager.like_count;
                like--;
                databaseManager.LikeUpdate("Post", selecetedPostID, like.ToString());
                databaseManager.posts[selecetedPostIndex].like = like;

                //Action
                PlayerPrefs.SetInt("Like" + selecetedPostID, 0);
                selecetedPost.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
                selecetedPost.transform.GetChild(5).GetChild(1).gameObject.SetActive(true);
                selecetedPost.transform.GetChild(5).GetChild(2).gameObject.SetActive(false);
                selecetedPost.transform.GetChild(5).GetChild(3).gameObject.GetComponent<Text>().text = like.ToString();
                #if UNITY_ANDROID
                toastManager.doToast("좋아요가 취소되었습니다.");
                #endif
            }
            doRefresh_like = false;
        }

        if (doRefresh_like_details)
        {
            if (PlayerPrefs.GetInt("Like" + databaseManager.posts[openIndex].date) == 0)
            {
                //Check and Update
                int like = databaseManager.like_count;
                like++;
                databaseManager.LikeUpdate("Post", databaseManager.posts[openIndex].date, like.ToString());
                databaseManager.posts[openIndex].like = like;

                //Action
                PlayerPrefs.SetInt("Like" + databaseManager.posts[openIndex].date, 1);
                DET_heartFilled.SetActive(true);
                DET_heartOutlined.SetActive(false);
                DET_heartAnim.SetActive(true);
                StartCoroutine(HeartAnim_Off(DET_heartAnim));
                DET_heart.text = like.ToString() + "명의 사용자가 이 게시물을 좋아합니다.";
                #if UNITY_ANDROID
                toastManager.doToast("좋아요가 반영되었습니다.");
                #endif
            }
            else if (PlayerPrefs.GetInt("Like" + databaseManager.posts[openIndex].date) == 1)
            {
                //Check and Update
                int like = databaseManager.like_count;
                like--;
                databaseManager.LikeUpdate("Post", databaseManager.posts[openIndex].date, like.ToString());
                databaseManager.posts[openIndex].like = like;

                //Action
                PlayerPrefs.SetInt("Like" + databaseManager.posts[openIndex].date, 0);
                DET_heartFilled.SetActive(false);
                DET_heartOutlined.SetActive(true);
                DET_heart.text = like.ToString() + "명의 사용자가 이 게시물을 좋아합니다.";
                #if UNITY_ANDROID
                toastManager.doToast("좋아요가 취소되었습니다.");
                #endif
            }
            doRefresh_like_details = false;
        }

        if (doRefresh_comment)
        {
            //Destroy old Comments
            foreach(Transform child in comment_parent)
            {
                if (child.gameObject.tag == "Comment")
                Destroy(child.gameObject);
            }

            //Create Comments
            for (int i = 0; i < databaseManager.comments.Length; i++)
            {
                if (databaseManager.comments[i].postID == databaseManager.posts[openIndex].date)
                {
                    var newComment = Instantiate(comment_prefab);
                    newComment.transform.SetParent(comment_parent, false);
                    newComment.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = databaseManager.comments[i].userName;
                    string[] DET_tmp_1 = databaseManager.comments[i].date.Split('-');
                    string[] DET_tmp_2 = DET_tmp_1[0].Split('_');
                    string DET_tmp_3 = DET_tmp_2[0] + "년 " + DET_tmp_2[1] + "월 " + DET_tmp_2[2] + "일";
                    newComment.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = DET_tmp_3;
                    newComment.transform.GetChild(1).GetComponent<Text>().text = databaseManager.comments[i].content;
                    newComment.transform.GetChild(3).GetComponent<Text>().text = databaseManager.comments[i].keyID;
                    
                    if (databaseManager.comments[i].userName == PlayerPrefs.GetString("Username"))
                    {
                        newComment.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                    }
                }
            }
            DET_scrollView.SetActive(false);
            DET_scrollView.SetActive(true);

            doRefresh_comment = false;
        }

        limitCount.text = inputDescription.text.Length + "/250";
    }
    
    IEnumerator HeartAnim_Off(GameObject heartAnim)
    {
        yield return new WaitForSeconds(0.5f);
        heartAnim.SetActive(false);
    }
}
