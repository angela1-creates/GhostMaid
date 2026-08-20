using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Board.Input;
using UnityEngine.UI;

public class StartManager: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TestCoinsText;
    [SerializeField] private Image check;
    private StatsManager statsManager;

    void Start()
    {
        statsManager = FindFirstObjectByType<StatsManager>();
        TestCoinsText.text = $"Coins: {statsManager.coins}";

        if (statsManager.activateCutscene == true)  // default want cutscene
        {
            check.color = new Color(1f, 1f, 1f, 1f);
        }
        else  // default dont want cutscene
        {
            check.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    public void button_ActivateCutscene()
    {
        try
        {
            if (statsManager.activateCutscene == true)  // when clicked when we have check activated
            {
                check.color = new Color(1f, 1f, 1f, 0f);
                statsManager.SetActivateCutscene(false);
            }
            else  // when clicked when we don't want cutscene
            {
                check.color = new Color(1f, 1f, 1f, 1f);
                statsManager.SetActivateCutscene(true);
            }
        }
        catch
        {
            UnityEngine.Debug.Log("activate cutscene function error");
        }

    }
}

/*
public class StartManager : MonoBehaviour
{
    public static StartManager Instance {get; private set;}

    void Awake()
    {
        if (Instance == null)
        {
            Instance=this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // [SerializeField] private TextMeshProUGUI TestText;
    [SerializeField] private TextMeshProUGUI TestCoinsText;
    [SerializeField] private Collider2D StartCollider;
    [SerializeField] private Collider2D ExitCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StatsManager statsManager = FindFirstObjectByType<StatsManager>();
        TestCoinsText.text = $"Coins: {statsManager.coins}";
    }

    // Update is called once per frame
    void Update()
    {
        var contacts = BoardInput.GetActiveContacts(BoardContactType.Finger);

        foreach(var contact in contacts)
        {
            ProcessContact(contact);
        }
    }


    private void ProcessContact(BoardContact contact)
    {
        if (StartCollider.OverlapPoint(contact.screenPosition))
        {
            // if (contact.phase == BoardContactPhase.Began)
            // {
                Debug.Log("start clicked");
                button_StartGame();
                // TestText.text = "start clicked";
            // }
        }
        else if (ExitCollider.OverlapPoint(contact.screenPosition))
        {
            // if (contact.phase == BoardContactPhase.Began)
            // {
                Debug.Log("exit clicked");
                button_End();
                // TestText.text = "exit clicked";
            // }
        }
    }

    public void button_StartGame()
    {
        Debug.Log("start clicked");
        // TestText.text = "Start";
        // SceneManager.LoadScene("GameScene");
    }

    public void button_End()
    {
        Application.Quit();
        Debug.Log("exit clicked");
        // TestText.text = "Exit";
    }
}*/
