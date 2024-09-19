using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpotPrefabManager : MonoBehaviour
{
    PlaceManager placeManager;

    void Start()
    {
        placeManager = GameObject.FindWithTag("PlaceManager").GetComponent<PlaceManager>();

        this.transform.GetComponent<Button>().onClick.AddListener(doDetails);
    }

    void doDetails()
    {
        placeManager.Details(0);
    }
}
