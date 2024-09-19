using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//For Picking files
using System.IO;

//For firebase storage
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;

public class UploadManager : MonoBehaviour
{
    public PostManager postManager;
    public ProfileManager profileManager;

    FirebaseStorage storage;
    StorageReference storageReference;
    
    void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://a-nice-fisherman.appspot.com");
    }

    public IEnumerator ImageUpload(Texture2D tex, string date)
    {
        yield return null;
        byte[] bytes = tex.EncodeToJPG();
        //Editing Metadata
        var newMetadata = new MetadataChange();
        newMetadata.ContentType = "image/jpeg";

        //Create a reference to where the file needs to be uploaded
        StorageReference uploadRef = storageReference.Child("Images/" + PlayerPrefs.GetString("Username") + "/" + PlayerPrefs.GetString("Username") + "-" + date + ".jpeg");
        Debug.Log("File upload started");
        uploadRef.PutBytesAsync(bytes, newMetadata).ContinueWithOnMainThread((task) => 
        { 
            if(task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
            }
            else
            {
                Debug.Log("File Uploaded Successfully!");
                postManager.UploadSuccess();
            }
        });
    }

    public IEnumerator ProfileImageUpload(Texture2D tex)
    {
        yield return null;
        byte[] bytes = tex.EncodeToJPG();
        //Editing Metadata
        var newMetadata = new MetadataChange();
        newMetadata.ContentType = "image/jpeg";

        //Create a reference to where the file needs to be uploaded
        StorageReference uploadRef = storageReference.Child("ProfileImages/" + PlayerPrefs.GetString("Username") + ".jpeg");
        Debug.Log("File upload started");
        uploadRef.PutBytesAsync(bytes, newMetadata).ContinueWithOnMainThread((task) => 
        { 
            if(task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
            }
            else
            {
                Debug.Log("File Uploaded Successfully!");
                profileManager.UploadSuccess();
            }
        });
    }
}
