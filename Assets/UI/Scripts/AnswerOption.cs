using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerOption : MonoBehaviour
{
    AppController appController;
    Image buttonImage;

    private void Start()
    {
        appController = FindObjectOfType<AppController>();
        buttonImage = GetComponent<Image>();
    }

    private void Update()
    {
        // Gives 'wrong answer' feedback when the timer <= 0
        if(appController.timer <= 0 && buttonImage.color != Color.red)
        {
            buttonImage.color = Color.red;
            StartCoroutine(LockButton());
        }
    }

    public void CheckSelectedAnswer(bool isCorrect)
    {
        StartCoroutine(LockButton());
        appController.CheckAnswer(isCorrect, buttonImage);
    }

    private IEnumerator LockButton()
    {
        GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(1.75f);
        GetComponent<Button>().interactable = true;
        buttonImage.color = Color.gray;
    }
}
