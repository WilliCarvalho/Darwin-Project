using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GoogleARCore.Examples.Common;
using GoogleARCore.Examples.HelloAR;

public class AppController : MonoBehaviour
{

    public Image categoryImageSlot;
    public Transform subOptionsParent;

    [Space(5)]
    [Header("SCREEN CONTAINERS:")]
    [Space(15)]
    [SerializeField] private RectTransform catgoryMenu = null;
    [SerializeField] private RectTransform subOptionsMenu = null;
    [SerializeField] private RectTransform objectViewMenu = null;
    [SerializeField] private RectTransform objectViewTopBar = null;
    [SerializeField] private RectTransform objectViewBottomBar = null;
    [SerializeField] private RectTransform objectMeshRectTransform = null;
    [SerializeField] private Canvas canvas = null;

    [Space(5)]
    [Header("OBJECT VIEW SCREEN:")]
    [Space(15)]
    [SerializeField] private RectTransform objectViewDropMenu = null;
    [SerializeField] private Transform dropMenuItensParent = null;
    [SerializeField] private Text objectViewScreenName = null;
    [SerializeField] private Text selectedObjectViewModeText = null;
    [SerializeField] private GameObject staticBG = null;

    [HideInInspector] public List<GameObject> instButtonsInScene; // meshes options
    private HelloARController aRControllerInstance;
    private DetectedPlaneGenerator detectedPlaneGenerator;

    private void Awake()
    {
        /*
        canvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 720);
        canvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvas.rect.height);
        */
    }

    private void Start()
    {
        aRControllerInstance = FindObjectOfType<HelloARController>();
        detectedPlaneGenerator = FindObjectOfType<DetectedPlaneGenerator>();
        canvas.renderMode = RenderMode.WorldSpace;
    }

    public enum GameScreen
    {
        catgoryMenu,
        subOptionsMenu,
        objectViewMenu,
    }
    public GameScreen gameScreen;

    

    #region ScreenTransitions

    IEnumerator DeactivateScreenWithDelay(GameObject screenContainer)
    {
        yield return new WaitForSeconds(0.5f);
        screenContainer.SetActive(false);
    }

    public void GoToSubOptionsMenu()
    {
        switch(gameScreen)
        {
            case GameScreen.catgoryMenu:
                gameScreen = GameScreen.subOptionsMenu;
                subOptionsMenu.gameObject.SetActive(true);
                catgoryMenu.DOAnchorPosX(-2000, 0.5f);
                subOptionsMenu.DOAnchorPosX(0, 0.5f);
                StartCoroutine(DeactivateScreenWithDelay(catgoryMenu.gameObject));
                break;

            case GameScreen.objectViewMenu:
                staticBG.SetActive(true);
                Invoke("DestroyOldDropMenuItens", 0.45f);
                Destroy(currentObjectMesh, 0.45f);
                gameScreen = GameScreen.subOptionsMenu;
                subOptionsMenu.gameObject.SetActive(true);
                subOptionsMenu.DOAnchorPosX(0, 0.5f);
                objectViewMenu.DOAnchorPosX(-2000, 0.5f);
                StartCoroutine(DeactivateScreenWithDelay(objectViewMenu.gameObject));
                PlaneDiscoveryGuide.HandAnimationAR = false;
                Destroy(aRControllerInstance._gameObject);
                break;
        }
        if (isDropMenuOpen)
        {
            Invoke("OpenCloseObjectViewDropMenu", 0.45f);
        }
    }
    public void GoToCatgoryMenu()
    {
        gameScreen = GameScreen.catgoryMenu;
        catgoryMenu.gameObject.SetActive(true);
        catgoryMenu.DOAnchorPosX(0, 0.5f);
        subOptionsMenu.DOAnchorPosX(2000, 0.5f);
        Invoke("DestroyOldSubOptionsButtons", 0.25f);
        StartCoroutine(DeactivateScreenWithDelay(subOptionsMenu.gameObject));
    }
    public void GoToObjectViewScreen(string objName)
    {
        staticBG.SetActive(false);
        gameScreen = GameScreen.objectViewMenu;
        objectViewMenu.gameObject.SetActive(true);
        subOptionsMenu.DOAnchorPosX(2000, 0.5f);
        objectViewMenu.DOAnchorPosX(0, 0.5f);
        StartCoroutine(DeactivateScreenWithDelay(subOptionsMenu.gameObject));
        objectViewScreenName.text = objName;
        detectedPlaneGenerator.instPlaneObj.SetActive(true);
        
    }

    #endregion



    private void DestroyOldSubOptionsButtons()
    {
        if(instButtonsInScene != null)
        {
            for (int i = 0; i < instButtonsInScene.Count; i++)
            {
                Destroy(instButtonsInScene[i]);
            }
            instButtonsInScene.Clear();
        }

    }




    #region ObjectView Screen

    [HideInInspector] public List<GameObject> viewModes; //different view modes of a specific object (dropmenu)
    public void CreateDropMenuList(GameObject viewMode)
    {
        if(viewMode != null)
        {
            GameObject viewModeButtonInst = Instantiate(viewMode, dropMenuItensParent);
            viewModes.Add(viewModeButtonInst);
        }

    }

    private void DestroyOldDropMenuItens()
    {
        if(viewModes != null)
        {
            for (int i = 0; i < viewModes.Count; i++)
            {
                Destroy(viewModes[i]);
            }
            viewModes.Clear();
        }

    }



    private bool isDropMenuOpen;
    public void OpenCloseObjectViewDropMenu()
    {
        if(!isDropMenuOpen)
        {
            isDropMenuOpen = true;
            objectViewDropMenu.DOAnchorPosY(0, 0.5f);
        }
        else
        {
            isDropMenuOpen = false;
            objectViewDropMenu.DOAnchorPosY(810, 0.5f);
        }
    }
    public void UpdateObjectViewMode(string modeButtonName, ObjectViewModes objViewMode)
    {
        selectedObjectViewModeText.text = modeButtonName;
        UpdateQuizInfo(objViewMode);
        //atualiza mesh e/ou animação
    }


    [SerializeField] private Transform objectMeshPosition = null;
    private GameObject currentObjectMesh;
    public void InstantiateObjectMesh(GameObject mesh)
    {
        currentObjectMesh = Instantiate(mesh, objectMeshPosition);
    }

    #endregion


    

    #region Quiz

    [Space(5)]
    [Header("QUIZ: ")]
    [Space(15)]
    [SerializeField] RectTransform quizScreen = null;
    [SerializeField] RectTransform transitionPanel = null;
    [SerializeField] Text timerUI = null, questionUI = null, progressNumberUI = null;
    private ObjectViewModes questions;
    [HideInInspector] public List<GameObject> answers = null;
    [HideInInspector] public float timer = 10, startTimer;
    private bool shouldTimerRun;
    private int questionIndex = 0;
    private int finalQuestionIndex = -100; //random number that will never correspond to any question

    
    private void Update()
    {
        if(shouldTimerRun)
        {
            timer -= Time.deltaTime;
            timerUI.text = Mathf.Round(timer).ToString();
            if(timer <= 0)
            {
                shouldTimerRun = false;
                timer = 0;
                StartCoroutine(QuizTranstition());
            }
        }
    } 

    public void UpdateQuizInfo(ObjectViewModes objectVMode)
    {
        questions = objectVMode;
    }

    public void StartQuiz()
    {
        StartCoroutine(QuizTranstition());
        FindObjectOfType<SoundManager>().Play_2();
    }

    public void CheckAnswer(bool isCorrect, Image answerButtonImage)
    {
        shouldTimerRun = false;
        if (isCorrect)
        {
            answerButtonImage.color = Color.green;
        }
        else
        {
            answerButtonImage.color = Color.red;
        }
        StartCoroutine(QuizTranstition());
    }

    private IEnumerator NextQuestion()
    {
        yield return new WaitForSeconds(0.5f);
        if(questionIndex < questions.questionList.Count)
        {
            questionIndex++;
            progressNumberUI.text = (questionIndex.ToString() + "/" + questions.questionList.Count.ToString());
            timer = questions.questionList[questionIndex-1].timer;
            shouldTimerRun = true;
            if(questionIndex != 1)
            {
                yield return new WaitForSeconds(0.3f);
                DestroyAnswersList();
            }
            questionUI.text = questions.questionList[questionIndex-1].question;
            CreateAnswersList();
        }
        else
        {
            EndQuiz();
        }

    }

    public void EndQuiz()
    {
        questionIndex = finalQuestionIndex;
        ClosePausedPanel();
        StartCoroutine(QuizTranstition());
        FindObjectOfType<SoundManager>().Play_1();
    }

    [SerializeField] private GameObject incorrectAnswerOption = null, correctAnswerOption = null;
    [SerializeField] private Transform answerOptionsParent = null;

    private void CreateAnswersList()
    {
        for (int i = 0; i < questions.questionList[questionIndex-1].answers.Length ; i++)
        {
            if(questions.questionList[questionIndex-1].correctAnswerIndex == i)
            {
                GameObject answerButton = Instantiate(correctAnswerOption, answerOptionsParent);
                answers.Add(answerButton);
                answerButton.GetComponentInChildren<Text>().text = questions.questionList[questionIndex-1].answers[i];
            }
            else
            {
                GameObject answerButton = Instantiate(incorrectAnswerOption, answerOptionsParent);
                answers.Add(answerButton);
                answerButton.GetComponentInChildren<Text>().text = questions.questionList[questionIndex-1].answers[i];
            }
        }
    }

    public IEnumerator QuizTranstition()
    {
        if(questionIndex == 0) //Start QUiz
        {
            quizScreen.gameObject.SetActive(true);
            if(Screen.height / Screen.width >= 1.8f )
            {
                objectMeshRectTransform.DOAnchorPosY(544, 0.25f);
            }
            else
            {
                objectMeshRectTransform.DOAnchorPosY(531, 0.25f);
                objectMeshRectTransform.DOScale(new Vector3(0.84f, 0.84f, 0.84f), 0.25f);
            }
            

            objectViewBottomBar.DOAnchorPosX(-2000, 0.5f);
            objectViewTopBar.DOAnchorPosX(-2000, 0.5f);
            StartCoroutine(DeactivateScreenWithDelay(objectViewBottomBar.gameObject));
            StartCoroutine(DeactivateScreenWithDelay(objectViewTopBar.gameObject));
            quizScreen.DOAnchorPosX(0, 0.5f);
            StartCoroutine(NextQuestion());
        }
        else if (questionIndex == finalQuestionIndex) //End Quiz
        {
            objectViewBottomBar.gameObject.SetActive(true);
            objectViewTopBar.gameObject.SetActive(true);
            quizScreen.DOAnchorPosX(2000, 0.5f);
            StartCoroutine(DeactivateScreenWithDelay(quizScreen.gameObject));
            objectViewBottomBar.DOAnchorPosX(0, 0.5f);
            objectViewTopBar.DOAnchorPosX(0, 0.5f);
            objectMeshRectTransform.DOAnchorPosY(0, 0.25f);
            objectMeshRectTransform.DOScale(new Vector3(1, 1, 1), 0.25f);
            questionIndex = 0;
            DestroyAnswersList();
        }
        else //Between Questions
        {
            transitionPanel.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.25f);
            transitionPanel.DOAnchorPosY(0, 0.25f);
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(NextQuestion());
            yield return new WaitForSeconds(1f);
            transitionPanel.DOAnchorPosY(3500, 0.5f);
            StartCoroutine(DeactivateScreenWithDelay(transitionPanel.gameObject));
            yield return new WaitForSeconds(0.5f);
            transitionPanel.anchoredPosition = new Vector2(transitionPanel.anchoredPosition.x, -3500);
        }

    }

    [SerializeField] GameObject pausedPanel = null;

    public void PauseQuiz()
    {
        pausedPanel.SetActive(true);
    }

    public void ClosePausedPanel()
    {
        pausedPanel.SetActive(false);
    }

    private void DestroyAnswersList()
    {
        for (int i = 0; i < answers.Count; i++)
        {
            Destroy(answers[i]);
        }
        answers.Clear();
    }
    #endregion
}