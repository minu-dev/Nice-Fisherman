using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentDeleteManager : MonoBehaviour
{
    PostManager postManager;
    string key;

    void Start()
    {
        postManager = GameObject.FindWithTag("PostManager").GetComponent<PostManager>();
        
        this.transform.GetComponent<Button>().onClick.AddListener(doDelete);
        key = this.transform.parent.parent.parent.transform.GetChild(3).GetComponent<Text>().text.ToString();
    }

    void doDelete()
    {
        postManager.Comment_Delete_Start(key);
    }
}
