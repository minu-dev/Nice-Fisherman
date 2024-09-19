using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NestedScrollManager : MonoBehaviour, IDragHandler, IEndDragHandler
{    
    public Scrollbar scrollbar;

    const int SIZE = 3;
    float[] pos = new float[SIZE];
    float distance, curPos, targetPos;
    bool isDrag;
    int targetIndex;

    public Image[] buttonIco;

    void Start()
    {
        distance = 1f / (SIZE - 1);
        for (int i = 0; i < SIZE; i++) pos[i] = distance * i;

        scrollbar.value = 0.5f;

        curPos = SetPos();
        targetIndex = 1;
        targetPos = pos[targetIndex];
        scrollbar.value = 0.5f;
    }

    float SetPos()
    {
        for (int i = 0; i < SIZE; i++)
            if (scrollbar.value < pos[i] + distance * 0.5f && scrollbar.value > pos[i] - distance * 0.5f)
            {
                targetIndex = i;
                return pos[i];
            }
        return 0;
    }


    public void OnBeginDrag(PointerEventData eventData) => curPos = SetPos();

    public void OnDrag(PointerEventData eventData) => isDrag = true;

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        targetPos = SetPos();

        if (curPos == targetPos)
        {
            if (eventData.delta.x > 10 && curPos - distance >= 0)
            {
                --targetIndex;
                targetPos = curPos - distance;
            }

            else if (eventData.delta.x < -10 && curPos + distance <= 1.01f)
            {
                ++targetIndex;
                targetPos = curPos + distance;
            }
        }
    }

    void Update()
    {
        if(!isDrag)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);
        }
        Gradient();
        UnGradient();
    }

    void Gradient()
    {
        float alpha = buttonIco[targetIndex].color.a;
        alpha = Mathf.Lerp(alpha, 0, 0.1f);
        buttonIco[targetIndex].color = new Color(200/225f, 200/255f, 200/255f, alpha);
    }

    void UnGradient()
    {
        int rest_1 = -1;
        int rest_2 = -1;

        if (targetIndex == 0)
        {
            rest_1 = 1;
            rest_2 = 2;
        }
        else if (targetIndex == 1)
        {
            rest_1 = 0;
            rest_2 = 2;
        }
        else if (targetIndex == 2)
        {
            rest_1 = 0;
            rest_2 = 1;
        }

        float alpha_1 = buttonIco[rest_1].color.a;
        float alpha_2 = buttonIco[rest_2].color.a;
        alpha_1 = Mathf.Lerp(alpha_1, 1, 0.1f);
        alpha_2 = Mathf.Lerp(alpha_2, 1, 0.1f);
        buttonIco[rest_1].color = new Color(200/225f, 200/255f, 200/255f, alpha_1);
        buttonIco[rest_2].color = new Color(200/225f, 200/255f, 200/255f, alpha_2);
    }

    public void Select(int index)
    {
        curPos = SetPos();
        targetIndex = index;
        targetPos = pos[index];
    }
}