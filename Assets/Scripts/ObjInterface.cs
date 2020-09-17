using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjInterface : MonoBehaviour
{
    public RectTransform container = null;
    public GameObject undoButton = null;
    public Text tittle = null;
    public TextMeshProUGUI info = null;
    public Text infoPage = null;

    ObjManipulator objDisplayed;

    public void ReceiveObjDisplayed(ObjManipulator obj)
    {
        objDisplayed = obj;
    }

    public void GoBack()
    {
        if(objDisplayed != null)
        {
            objDisplayed.GoBack();
        }
    }
    public void NextInfoPage()
    {
        objDisplayed.NextInfoPage();
    }
    public void PreviousInfoPage()
    {
        objDisplayed.PreviousInfoPage();
    }

    public void UpdateUndoButton(bool boolValue)
    {
        undoButton.SetActive(boolValue);
    }
}
