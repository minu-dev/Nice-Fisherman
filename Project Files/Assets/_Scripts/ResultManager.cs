using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [System.Serializable]
    public class information
    {
        public string name;
        public Sprite illustration;
        public bool disrupt = false;
        public string term = null;
        public string length = null;
        public bool female = false;
        public bool external_female = false;
        public Sprite[] ex_images;
    }
    public information[] info;

    public GameObject[] subject_1;
    public GameObject isDisrupt;
    public Image fishImage;
    public Text resultLabel;
    public Text resultScientific;
    public Text resultDescription;
    public GameObject cantFind;
    public GameObject scrollView;
    public Scrollbar scBar;
    public GameObject isTaboo;

    public Text term_text;
    public Text length_text;
    public Text female_text;
    
    public Image[] photos;
    
    public Animator canvasAnim;

    private void Start()
    {
    }

    public void InitResult(string label, string scientific, string description, int index, float score)
    {
        if (score > 2.8)
        {
            scBar.value = 1;
            scrollView.SetActive(true);
            cantFind.SetActive(false);
            for (int i = 0; i < subject_1.Length; i++)
            {
                subject_1[i].SetActive(true);
            }
            isDisrupt.SetActive(false);

            resultLabel.text = label;
            resultScientific.text = scientific;
            resultDescription.text = description;

            fishImage.sprite = info[index].illustration;

            if (info[index].term != "X")
                term_text.text = info[index].term;
            else term_text.text = "금어기 없음";

            if (info[index].length != "X")
                length_text.text = info[index].length + "cm 이하";
            else term_text.text = "포획금지체장 없음";

            if (info[index].female)
                female_text.text = "암컷 포획 금지";

            if (info[index].external_female)
                female_text.text = "복부 외포란 암컷 포획 금지";

            if (!info[index].female && !info[index].external_female)
                female_text.text = "해당 없음";

            if (info[index].disrupt)
            {
                for (int i = 0; i < subject_1.Length; i++)
                {
                    subject_1[i].SetActive(false);
                }
                isDisrupt.SetActive(true);
            }

            for (int i = 0; i < 4; i++)
            {
                photos[i].sprite = info[index].ex_images[i];
            }

            if (info[index].term != "X" && !info[index].disrupt)
                isInTaboo(index);
        }

        else if (score <= 2.8)
        {
            scrollView.SetActive(false);
            cantFind.SetActive(true);
        }
    }

    void isInTaboo(int index)
    {
        float nowDate = float.Parse(DateTime.Now.ToString("MM.dd"));
        
        string[] tmp_1 = info[index].term.Split(' ');
        float start = float.Parse(tmp_1[0]);
        float end = float.Parse(tmp_1[2]);

        //2021 ~ 2021
        if (start < end)
        {
            //Taboo
            if (start <= nowDate && nowDate <= end)
            {
                isTaboo.SetActive(true);
            }
            //Non-Taboo
            else
            {
                isTaboo.SetActive(false);
            }
        }
        //2021 ~ 2022
        else if (start > end)
        {
            //Taboo
            if (start <= nowDate)
            {
                isTaboo.SetActive(true);
            }
            //Non-Taboo
            else
            {
                isTaboo.SetActive(false);
            }
        }
    }

    public void closeIsTaboo()
    {
        isTaboo.SetActive(false);
    }

    public void LearnMore()
    {
        Application.OpenURL("https://search.naver.com/search.naver?sm=tab_hty.top&where=nexearch&query=" + resultLabel.text.ToString());
    }

    public void Close()
    {
        canvasAnim.SetBool("Open", false);
    }
}
