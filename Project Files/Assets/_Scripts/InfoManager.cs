using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{
    [Header("Animators")]
    public Animator animator;

    public void Info_Open()
    {
        animator.SetBool("Open", true);
    }

    public void Info_Close()
    {
        animator.SetBool("Open", false);
    }
}
