using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutPanel : MonoBehaviour
{

    public GameObject aboutPanel;

    public void ClosePanel()
    {
        aboutPanel.SetActive(false);
    }
    public void OpenPanel()
    {
        aboutPanel.SetActive(true);
    }
}
