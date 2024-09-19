using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostPrefabManager : MonoBehaviour
{
    PostManager postManager;
    string path;

    void Start()
    {
        postManager = GameObject.FindWithTag("PostManager").GetComponent<PostManager>();
        
        if (this.gameObject.tag == "Heart")
        {
            this.transform.GetComponent<Button>().onClick.AddListener(doLike);
        }
        else
        {
            this.transform.GetComponent<Button>().onClick.AddListener(doDetails);
            path = this.transform.parent.transform.GetChild(7).GetComponent<Text>().text.ToString();
        }
    }

    void doLike()
    {
        postManager.Like();
    }

    void doDetails()
    {
        postManager.Details(path);
    }
}
