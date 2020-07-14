using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryOption : MonoBehaviour
{
    [SerializeField] private Sprite categoryImageSprite = null;
    [SerializeField] private string categoryName = null;

    [Space(5)]
    [Header("SUB-OPTIONS - PREFABS:")]
    [Space(15)]
    [SerializeField] private SubOptions[] subOptionsList = null;
    private AppController appController;

    private void Start()
    {
        GetComponentInChildren<Text>().text = categoryName;
        appController = FindObjectOfType<AppController>();
    }

    #region CategorySelectionButton
    public void ButtonAction()
    {
        StartCoroutine(LockButton());
        StartCoroutine(ButtonActionCorroutine());
    }

    private IEnumerator ButtonActionCorroutine()
    {
        UpdateImageDisplay();
        yield return new WaitForSeconds(0.5f);
        CreateSubOptionsList();
        appController.GoToSubOptionsMenu();
    }
    #endregion


    #region Update SubOptionsMenu for the selected category
    private void CreateSubOptionsList()
    {
        for (int i = 0; i < subOptionsList.Length; i++)
        {
            GameObject subOptionInst = Instantiate(subOptionsList[i].gameObject, appController.subOptionsParent);
            appController.instButtonsInScene.Add(subOptionInst);
        }
    }
    private void UpdateImageDisplay()
    {
        appController.categoryImageSlot.sprite = categoryImageSprite;
        appController.categoryImageSlot.SetNativeSize();
    }
    #endregion

    private IEnumerator LockButton()
    {
        GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(0.5f);
        GetComponent<Button>().interactable = true;
    }
}
