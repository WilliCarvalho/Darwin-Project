using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    public GameObject parent;
    public Image ui_img;
    public Text ui_text;
    public Text ui_pag;
    [Space(10)]
    public TutorialPag[] tutorialPags;

    [System.Serializable]
    public class TutorialPag
    {
        public Sprite image;
        [TextArea(2, 3)]public string text;
    }

    private void Start()
    {
        if(tutorialPags.Length != 0)
        {
            ShowPageInfo(tutorialPags[pageIndex]);
        }
    }


    int pageIndex;
    public void NextInfoPage()
    {
        if (pageIndex < tutorialPags.Length - 1)
        {
            pageIndex++;
            ShowPageInfo(tutorialPags[pageIndex]);
        }
    }
    public void PreviousInfoPage()
    {
        if (pageIndex > 0)
        {
            pageIndex--;
            ShowPageInfo(tutorialPags[pageIndex]);
        }
    }
    void ShowPageInfo(TutorialPag pag)
    {
        ui_img.sprite = pag.image;
        ui_text.text = pag.text;
        ui_pag.text = (pageIndex+1).ToString() + "/" + tutorialPags.Length.ToString();
    }

    public void CloseTutorial()
    {
        parent.gameObject.SetActive(false);
        
    }
}
