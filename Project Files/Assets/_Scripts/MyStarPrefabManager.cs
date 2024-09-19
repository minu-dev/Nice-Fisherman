using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyStarPrefabManager : MonoBehaviour
{
    Animator canvasAnim;
    PlaceManager placeManager;
    string path;

    void Start()
    {
        placeManager = GameObject.FindWithTag("PlaceManager").GetComponent<PlaceManager>();
        canvasAnim = GameObject.FindWithTag("Canvas").GetComponent<Animator>();
        path = this.transform.parent.GetChild(1).GetComponent<Text>().text.ToString();
        this.transform.GetComponent<Button>().onClick.AddListener(doDetails);
    }

    void doDetails()
    {
        canvasAnim.SetBool("Place_Open", true);
        placeManager.Details_Instance(path);
    }
}