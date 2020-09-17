using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore.Examples.Common;
using GoogleARCore.Examples.HelloAR;

public class SubOptions : MonoBehaviour
{

    [SerializeField] private string objName = null;
    [SerializeField] private GameObject mesh = null;
    [SerializeField] private Transform meshParent = null;

    [Space(5)]
    [Header("OBJECT VIEW MODES - PREFABS:")]
    [Space(15)]
    [SerializeField] private GameObject[] objectViewModes = null; //scrollmenu with quiz/animations options for a specific mesh
    private AppController appController;

    private void Start()
    {
        appController = FindObjectOfType<AppController>();
        GetComponentInChildren<Text>().text = objName;
    }

    public void GoToObjectView()
    {
        StartCoroutine(LockButton());
        for (int i = 0; i < objectViewModes.Length; i++)
        {
            appController.CreateDropMenuList(objectViewModes[i]);
        }
        appController.UpdateObjectViewMode(objectViewModes[0].GetComponent<ObjectViewModes>().viewModeName, objectViewModes[0].GetComponent<ObjectViewModes>());
        //appController.InstantiateObjectMesh(mesh);
        appController.GoToObjectViewScreen(objName);

        PlaneDiscoveryGuide.HandAnimationAR = true;
    }

    private IEnumerator LockButton()
    {
        GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(0.5f);
        GetComponent<Button>().interactable = true;
    }

    public void SetObjInstance()
    {
        FindObjectOfType<HelloARController>().GameObjectHorizontalPlanePrefab = mesh;
        Debug.Log("To entrando aqui");
    }
}


