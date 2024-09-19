using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLoginManager : MonoBehaviour
{
    public LoadScene sceneManager;

    void Start()
    {
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        yield return new WaitForSeconds(2f);

        if (PlayerPrefs.GetInt("Auto Login") == 1)
            sceneManager.FadeInTo("Main");
        else
            sceneManager.JustLoad("Auth");
    }
}
