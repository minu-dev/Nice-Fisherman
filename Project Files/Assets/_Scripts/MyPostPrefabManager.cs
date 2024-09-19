using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyPostPrefabManager : MonoBehaviour
{
    Animator canvasAnim;
    PostManager postManager;
    string path;

    void Start()
    {
        postManager = GameObject.FindWithTag("PostManager").GetComponent<PostManager>();
        canvasAnim = GameObject.FindWithTag("Canvas").GetComponent<Animator>();
        path = this.transform.parent.GetChild(1).GetComponent<Text>().text.ToString();
        this.transform.GetComponent<Button>().onClick.AddListener(doDetails);
    }

    void doDetails()
    {
        canvasAnim.SetBool("Post_Open", true);
        postManager.Details(path);
    }
}