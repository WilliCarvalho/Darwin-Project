using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGrowndScroller : MonoBehaviour
{
    private RawImage backGrownd;
    // Start is called before the first frame update
    void Awake()
    {
        backGrownd = GetComponent<RawImage>(); 
    }

    // Update is called once per frame
    void Update()
    {
        backGrownd.uvRect = new Rect(backGrownd.uvRect.x, backGrownd.uvRect.y+Time.deltaTime*0.05f, backGrownd.uvRect.width, backGrownd.uvRect.height);
    }
}
