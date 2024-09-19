using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GuideController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Scrollbar scrollbar;
    float curPos, targetPos;
    float[] pos = {1.0f, 0.0f};
    float val;
    bool isDrag;

    void Start()
    {
        targetPos = pos[0];
    }

    void Update()
    {
        InitPos();
    }

    void InitPos()
    {
        if (!isDrag)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);
        }
    }

    public void OnDrag(PointerEventData eventData) => isDrag = true;

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;

        if (scrollbar.value >= 0.5f && scrollbar.value <= 1.0f)
        {
            if (eventData.delta.y > 10)
            {
                targetPos = pos[1];
            }
            else
            {
                targetPos = pos[0];
            }
        }
        else
        {
            if (eventData.delta.y < -10)
            {
                targetPos = pos[0];
            }
            else
            {
                targetPos = pos[1];
            }
        }
    }
}
