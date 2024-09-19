﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Animator canvasAnim;

    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    void Start()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        StartCoroutine(AnimatorOff());
    }

    //Functions to change the login screen UI
    public void LoginScreen() //Back button
    {
        loginUI.SetActive(true);
        registerUI.SetActive(false);
    }
    public void RegisterScreen() // Regester button
    {
        loginUI.SetActive(false);
        registerUI.SetActive(true);
    }

    IEnumerator AnimatorOff()
    {
        yield return new WaitForSeconds(1f);
        loginUI.SetActive(true);
        registerUI.SetActive(false);
        canvasAnim.enabled = false;
    }
}
