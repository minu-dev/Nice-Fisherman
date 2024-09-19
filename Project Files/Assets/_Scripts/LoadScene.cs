using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public Image panel;
    public string startAction = "";

    void Start()
    {
        if (startAction == "Fade Out")
            FadeOutTo();
    }

    public void JustLoad(string nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }

    public void FadeInTo(string nextScene)
    {
        StartCoroutine(FadeIn(nextScene));
    }

    IEnumerator FadeIn(string nextScene)
    {
        panel.gameObject.SetActive(true);

        float alpha = 0;
        panel.color = new Color(1, 1, 1, alpha);
        
        while (alpha <= 0.99f)
        {
            yield return new WaitForFixedUpdate();
            alpha = Mathf.Lerp(alpha, 1, 0.1f);
            panel.color = new Color(1, 1, 1, alpha);
        }
        SceneManager.LoadScene(nextScene);
    }

    public void FadeOutTo()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        panel.gameObject.SetActive(true);

        float alpha = 1;
        panel.color = new Color(1, 1, 1, alpha);
        
        while (alpha >= 0.01f)
        {
            yield return new WaitForFixedUpdate();
            alpha = Mathf.Lerp(alpha, 0, 0.1f);
            panel.color = new Color(1, 1, 1, alpha);
        }

        panel.gameObject.SetActive(false);
    }
}
