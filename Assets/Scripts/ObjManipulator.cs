using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjManipulator : MonoBehaviour
{
    ObjInterface objUI;
    private void Start()
    {
        objUI = FindObjectOfType<ObjInterface>();
    }

    #region ObjExtensor
    int selectedIndex;
    Vector3 centeredPosition = Vector3.zero;
    bool isSeparated;
    [SerializeField] ObjPart[] objParts = null;

    [System.Serializable]
    public class ObjPart
    {
        public GameObject objPart;
        public Transform newPosition;
        [HideInInspector] public Vector3 originalPosition;
    }

    public void SeparateParts()
    {
        if(!isSeparated)
        {
            isSeparated = true;
            for (int i = 0; i < objParts.Length; i++)
            {
                objParts[i].originalPosition = objParts[i].objPart.transform.position;
                objParts[i].objPart.transform.position = objParts[i].newPosition.position;
                objParts[i].objPart.GetComponent<Collider>().enabled = true;
            }
        }
    }
    public void ReuniteParts()
    {
        if(isSeparated)
        {
            isSeparated = false;
            for (int i = 0; i < objParts.Length; i++)
            {
                objParts[i].objPart.transform.position = objParts[i].originalPosition;
                objParts[i].objPart.GetComponent<Collider>().enabled = false;
            }
        }

    }
    #endregion

    #region ObjManipulation
    bool isScaling;
    Vector2 touch0Pos;
    Vector2 touch1Pos;
    float startDistance;
    #endregion

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
            float scaleValue = 0.5f * Time.deltaTime;
            if (startDistance > Vector2.Distance(touch0Pos, touch1Pos))
            {
                //scale down
                transform.localScale = new Vector3(transform.localScale.x - scaleValue, transform.localScale.y - scaleValue, transform.localScale.z - scaleValue);
            }
            else if (startDistance < Vector2.Distance(touch0Pos, touch1Pos))
            {
                //scale up
                transform.localScale = new Vector3(transform.localScale.x + scaleValue, transform.localScale.y + scaleValue, transform.localScale.z + scaleValue);
            }
        }
        #endregion


        #region Select/Expand Obj/Part 
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.touchCount < 2)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
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
                            if(selectedPart == null)
                            {
                                // was being used for ReuniteParts(); 
                                //-> instead use this to zoom in obj's part


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
                                // move selected part to screen's center
                                objParts[selectedIndex].objPart.transform.position = centeredPosition;


                                objUI.container.gameObject.SetActive(true);
                                pageIndex = 0;
                                ShowInfoOfSelectedPart(hitInfo.collider.GetComponent<ObjPartData>());
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
        objParts[selectedIndex].objPart.transform.position = objParts[selectedIndex].newPosition.position;

        // Show other parts
        for (int i = 0; i < objParts.Length; i++)
        {
            objParts[i].objPart.SetActive(true);
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
