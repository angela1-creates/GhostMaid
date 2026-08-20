using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;


public class ButtonManager : MonoBehaviour
{
    public static ButtonManager instance;

    [Header("Loading screen stuff")]
    [SerializeField] private GameObject loadingScreenCanvas;
    [SerializeField] private Image progressBar;
    [SerializeField] private GameObject titleScreen;


    [Header("Scenes to go")]
    private string startScene = "StartScene";
    private string tutorialScene = "TutorialScene";
    private string gameScene = "GameScene";
    private string shopScene = "ShopScene";
    private string cutsceneScene = "CutsceneScene";

    [Header("Number of player button stuff")]
    [SerializeField] private GameObject playerButtons; // chosing player amount
    [SerializeField] private GameObject playButton; // for hiding
    [SerializeField] private GameObject tutorialWarning;
    private StatsManager statsManager;

    [Header("Audio")]
    [SerializeField] private AudioClip clickSfx;

    private void Awake()
    {
        if(instance==null){instance=this;}
        else{Destroy(gameObject);}
    }

    void Start()
    {
        if (tutorialWarning != null)
        {
            tutorialWarning.SetActive(false);
        }
        if (playerButtons)
        {
            playerButtons.SetActive(false);
        }
        try
        {
            statsManager = FindFirstObjectByType<StatsManager>();
        }
        catch
        {
            // pass
        }
    }


    public void button_GoToStartMenu()
    {
        if (playButton != null)
        {
            playButton.SetActive(true);
        }
        AudioManager.instance.PlayAudioClip(clickSfx);
        OpenLoadingScreen(false);
        SceneManager.LoadScene(startScene);
    }

    public void button_GoToShop()
    {
        AudioManager.instance.PlayAudioClip(clickSfx);
        SceneManager.LoadScene(shopScene);
    }

    public void button_GoToGame()
    {
        if (clickSfx)
        {
            AudioManager.instance.PlayAudioClip(clickSfx);
        }
        else
        {
            UnityEngine.Debug.Log("No clicksfx btw");
        }

        if (statsManager)  // if we are at the start with the manager
        {
            if(statsManager.activateCutscene == true)
            {
                StartCoroutine(LoadSceneAsync(cutsceneScene));
            }
            else
            {
                StartCoroutine(LoadSceneAsync(gameScene));
            }
        }
        else
        {
            StartCoroutine(LoadSceneAsync(gameScene));
        }
    }

    public void button_GoToTutorial()
    {
        AudioManager.instance.PlayAudioClip(clickSfx);
        SceneManager.LoadScene(tutorialScene);
    }


    // private void GoToCutscene()
    // {
    //     AudioManager.instance.PlayAudioClip(clickSfx);
    //     StartCoroutine(LoadSceneAsync(cutsceneScene));
    // }


    private IEnumerator LoadSceneAsync(string scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        while (!operation.isDone)
        {
            if (loadingScreenCanvas && progressBar && titleScreen)
            {
                OpenLoadingScreen(true);
                progressBar.fillAmount = operation.progress; 
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void OpenLoadingScreen(bool isOpen)
    {
        if (loadingScreenCanvas && progressBar && titleScreen)
        {
            titleScreen.SetActive(!isOpen);
            loadingScreenCanvas.SetActive(isOpen);
        }
    }


    // chosing amount of player 
    public void button_ShowPlayerButtons()
    {
        if (playerButtons != null)
        {
            AudioManager.instance.PlayAudioClip(clickSfx);
            tutorialWarning.SetActive(true);
            playerButtons.SetActive(true);
            playButton.SetActive(false);
        }
    }
    
    public void button_OnePlayer()
    {
        StatsManager.Instance.SetPlayerNum(1);
        AudioManager.instance.PlayAudioClip(clickSfx);
        button_GoToGame();
    }
    public void button_TwoPlayer()
    {
        StatsManager.Instance.SetPlayerNum(2);
        AudioManager.instance.PlayAudioClip(clickSfx);
        button_GoToGame();
    }
}
