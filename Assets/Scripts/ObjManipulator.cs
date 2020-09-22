using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjManipulator : MonoBehaviour
{
    ObjInterface objUI;
    AppController appController;

    private void Start()
    {
        objUI = FindObjectOfType<ObjInterface>();
        appController = FindObjectOfType<AppController>();
    }


    int selectedIndex;
    bool isSeparated;
    public GameObject closedObj;
    public GameObject openedObj;
    [SerializeField] GameObject[] openedObjParts = null;
    private Vector3 selectedPartOriginalPos;
    public LayerMask layerMask;

    public void SeparateParts()
    {
        if(!isSeparated)
        {
            isSeparated = true;
            closedObj.SetActive(false);
            openedObj.SetActive(true);
            for (int i = 0; i < openedObjParts.Length; i++)
            {
                openedObjParts[i].GetComponent<Collider>().enabled = true;
            }
        }
    }
    public void ReuniteParts()
    {
        if(isSeparated)
        {
            isSeparated = false;
            for (int i = 0; i < openedObjParts.Length; i++)
            {
                openedObjParts[i].GetComponent<Collider>().enabled = false;
            }
            openedObj.SetActive(false);
            closedObj.SetActive(true);
        }

    }



    bool isScaling;
    Vector2 touch0Pos;
    Vector2 touch1Pos;
    float startDistance;

    bool canSelect;

    private void Update()
    {

        #region Scale Obj
        if (Input.touchCount != 2)
        {
            isScaling = false;
        }
        if (Input.touchCount > 0)
        {
            Touch touch0 = Input.GetTouch(0);
            touch0Pos = touch0.position;

        }
        if (Input.touchCount > 1)
        {
            Touch touch1 = Input.GetTouch(1);
            touch1Pos = touch1.position;
            if (touch1.phase == TouchPhase.Began)
            {
                startDistance = Vector2.Distance(touch0Pos, touch1Pos);
                isScaling = true;
            }
        }

        if (isScaling)
        {
            float scaleValue = 0.02f * Time.deltaTime;
            if (startDistance > Vector2.Distance(touch0Pos, touch1Pos))
            {
                //scale down
                if(transform.localScale.x > 0)
                {
                    transform.localScale = new Vector3(transform.localScale.x - scaleValue, transform.localScale.y - scaleValue, transform.localScale.z - scaleValue);
                }
            }
            else if (startDistance < Vector2.Distance(touch0Pos, touch1Pos))
            {
                //scale up
                transform.localScale = new Vector3(transform.localScale.x + scaleValue, transform.localScale.y + scaleValue, transform.localScale.z + scaleValue);
            }
        }
        #endregion


        #region Select/Expand - Obj/Part 
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.touchCount < 2)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, layerMask))
                {
                    if (!canSelect)
                    {
                        StartCoroutine(SelectTime());
                    }
                    else
                    {
                        if (!isSeparated)
                        {
                            SeparateParts();
                            objUI.UpdateUndoButton(true);
                        }
                        else
                        {
                            // SELECT SINGLE OBJ PART:
                            if (selectedPart == null && appController.canSelectObjPart)
                            {
                                // hide other parts
                                for (int i = 0; i < openedObjParts.Length; i++)
                                {
                                    if (openedObjParts[i] != hitInfo.collider.gameObject)
                                    {
                                        openedObjParts[i].SetActive(false);
                                    }
                                    else
                                    {
                                        selectedIndex = i;
                                    }
                                }

                                openedObjParts[selectedIndex].SetActive(true);

                                // move selected part to center
                                selectedPartOriginalPos = openedObjParts[selectedIndex].transform.position;
                                openedObjParts[selectedIndex].transform.position = transform.position;


                                objUI.container.gameObject.SetActive(true);
                                pageIndex = 0;
                                //ShowInfoOfSelectedPart(hitInfo.collider.GetComponent<ObjPartData>());
                                ShowInfoOfSelectedPart(openedObjParts[selectedIndex].GetComponent<ObjPartData>());

                                #region OLD_METHOD
                                /*
                                if(selectedPart == null && appController.canSelectObjPart)
                                {
                                    // hide other parts
                                    for (int i = 0; i < objParts.Length; i++)
                                    {
                                        if(objParts[i].objPart != hitInfo.collider.gameObject)
                                        {
                                            objParts[i].objPart.SetActive(false);
                                        }
                                        else
                                        {
                                            selectedIndex = i;
                                        }
                                    }

                                    objParts[selectedIndex].objPart.SetActive(true);

                                    // move selected part to center
                                    objParts[selectedIndex].objPart.transform.position = transform.position;


                                    objUI.container.gameObject.SetActive(true);
                                    pageIndex = 0;
                                    //ShowInfoOfSelectedPart(hitInfo.collider.GetComponent<ObjPartData>());
                                    ShowInfoOfSelectedPart(objParts[selectedIndex].objPart.GetComponent<ObjPartData>());
                                    */
                                #endregion
                            }
                    }
                    }
                }
            }
        }
        #endregion

        if(!isReferenced)
        {
            objUI.ReceiveObjDisplayed(this);
            isReferenced = true;
        }
    }

    IEnumerator SelectTime()
    {
        canSelect = true;
        yield return new WaitForSeconds(0.5f);
        canSelect = false;
    }

    public void GoBack()
    {
        if(selectedPart == null)
        {
            ReuniteParts();
            objUI.UpdateUndoButton(false);
        }
        else if(selectedPart != null)
        {
            HideInfoOfSelectedPart();
        }
    }

    bool isReferenced;
    int pageIndex;
    ObjPartData selectedPart;

    void ShowInfoOfSelectedPart(ObjPartData part)
    {
        selectedPart = part;
        objUI.tittle.text = part.tittle;
        objUI.info.text = part.info[pageIndex];
        objUI.infoPage.text = (pageIndex + 1).ToString() + " / " + part.info.Length.ToString();
    }
    void HideInfoOfSelectedPart()
    {
        objUI.container.gameObject.SetActive(false);
        selectedPart = null;
        openedObjParts[selectedIndex].transform.position = selectedPartOriginalPos;

        // Show other parts
        for (int i = 0; i < openedObjParts.Length; i++)
        {
            openedObjParts[i].SetActive(true);
        }
    }
    public void NextInfoPage()
    {
        if(pageIndex < selectedPart.info.Length-1)
        {
            pageIndex++;
            ShowInfoOfSelectedPart(selectedPart);
        }
    }
    public void PreviousInfoPage()
    {
        if (pageIndex > 0)
        {
            pageIndex--;
            ShowInfoOfSelectedPart(selectedPart);
        }
    }


}
