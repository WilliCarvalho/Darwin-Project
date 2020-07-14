using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectViewModes : MonoBehaviour
{

    [System.Serializable]
    public class ObjectModeQuiz
    {
        [Space(20)]
        [TextArea(2, 4)] public string question;
        public int timer;
        [Space(5)]
        [Header("ANSWERS: MAX = 3")]
        [Space(5)]
        [TextArea(1, 3)] public string[] answers;

        [Space(5)]
        public int correctAnswerIndex;
    }

    private AppController appController;
    public string viewModeName;

    [Space(5)]
    [Header("QUESTIONS OF THIS MODE:")]
    [Space(15)]
    public List<ObjectModeQuiz> questionList;


    private void Start()
    {
        appController = FindObjectOfType<AppController>();
        GetComponentInChildren<Text>().text = viewModeName;
    }
    
    public void UpdateViewMode()
    {
        StartCoroutine(LockButton());
        appController.UpdateObjectViewMode(GetComponentInChildren<Text>().text, this);
        appController.UpdateQuizInfo(this);
        appController.OpenCloseObjectViewDropMenu();
    }
    private IEnumerator LockButton()
    {
        GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(0.5f);
        GetComponent<Button>().interactable = true;
    }
}
