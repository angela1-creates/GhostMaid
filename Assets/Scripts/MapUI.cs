using UnityEngine;
using TMPro;
using Board.Input;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class MapUI : MonoBehaviour
{
    public static MapUI instance;

    [SerializeField] private GameObject mapBackground;
    [SerializeField] private GameObject mapAnimation;
    private Animator mapAnimator;

    [Header("Animations")]
    [SerializeField] private AnimationClip mapOpeningClip;
    [SerializeField] private AnimationClip mapClosingClip;

    private bool isMapOpen = false;

    [Header("Camera changing stuff")]
    [SerializeField] private Camera cam;
    [SerializeField] private float roomOffset = 50f;
    [SerializeField] private LevelManager levelManager;  

    [Header("Audio")]
    [SerializeField] private AudioClip mapOpen;
    [SerializeField] private AudioClip mapClose;
    [SerializeField] private AudioClip clickSfx;

    [Header("Room buttons")]
    [SerializeField] private GameObject[] allButtons;
    [SerializeField] private GameObject bedroomButton;
    [SerializeField] private GameObject bathroomButton;
    [SerializeField] private GameObject livingRoomButton;
    [SerializeField] private GameObject kitchenButton;

    [Header("Scaling of buttons")]
    [SerializeField] private float inactiveScale = 2f;
    [SerializeField] private float activeScale = 2.5f;

    [Header("Exit / Restart")]
    [SerializeField] private GameObject warningPopup;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private GameObject menuButtons;
    private bool warningOpen = false;
    private bool isExiting = false;
    private bool isRestarting = false;

    [Header("Dirty Item Count")]
    public int bedroomDirtyNum = 0;
    public int bathroomDirtyNum = 0;
    public int livingRoomDirtyNum = 0;
    public int kitchenDirtyNum = 0;

    [Header("Dirty map images")]
    [SerializeField] private float denominator;
    [SerializeField] private Image bedroomDirtyMap;
    [SerializeField] private Image bathroomDirtyMap;
    [SerializeField] private Image livingRoomDirtyMap;
    [SerializeField] private Image kitchenDirtyMap;


    private void Awake()
    {
        if(instance==null){instance=this;}
        else{Destroy(gameObject);}
    }

    private void Start()
    {
        mapBackground.SetActive(false);
        updateRoomButton();
        mapAnimator = mapAnimation.GetComponent<Animator>();
        mapAnimation.SetActive(false);

        bedroomDirtyMap.color = new Color(1.0f, 1.0f, 1.0f, 0f);
        bathroomDirtyMap.color = new Color(1.0f, 1.0f, 1.0f, 0f);
        kitchenDirtyMap.color = new Color(1.0f, 1.0f, 1.0f, 0f);
        livingRoomDirtyMap.color = new Color(1.0f, 1.0f, 1.0f, 0f);

        warningPopup.SetActive(false);
    }
        
    private IEnumerator StartAnimation(bool y)
    {
        mapAnimation.SetActive(true);
        if (y)  // opening map
        {
            mapAnimator.SetBool("isOpen", true);
            mapAnimator.SetBool("isClose", false);
            yield return new WaitForSeconds(mapOpeningClip.length);
            mapBackground.SetActive(true);
        }
        else if (!y) // closing map
        {
            mapBackground.SetActive(false);
            mapAnimator.SetBool("isOpen", false);
            mapAnimator.SetBool("isClose", true);
            yield return new WaitForSeconds(mapClosingClip.length);
        }
        isMapOpen = y;
        mapAnimation.SetActive(false);
    }

    public void button_OpenMap()
    {
        Vector3 worldLocation = Camera.main.ScreenToWorldPoint(new Vector3(cam.transform.position.x, cam.transform.position.y, 1f));
        if (!isMapOpen && levelManager.currentState == GameState.Playing) 
        {
            Debug.Log("map is opening");
            AudioManager.instance.PlayAudioClip(mapOpen);
            levelManager.currentMapState = MapState.MapIsOpen;

            // animation stuff
            StartCoroutine("StartAnimation", true);
        }
        else if (isMapOpen || levelManager.currentState == GameState.PostRound)
        {            
            Debug.Log("map is closing");
            AudioManager.instance.PlayAudioClip(mapClose);
            levelManager.currentMapState = MapState.MapIsClosed;
            resetWarning();

            // animation stuff
            StartCoroutine("StartAnimation", false);
        }
    }

    public void button_GoToBedroom()
    {
        button_OpenMap();
        levelManager.DisableMagnifyUI();
        cam.transform.position = new Vector3(0,cam.transform.position.y, cam.transform.position.z);
        GameManager.instance.SetPlayerLocation("Bedroom");
    
        updateRoomButton();
    }

    public void button_GoToBathroom()
    {
        button_OpenMap();
        levelManager.DisableMagnifyUI();
        cam.transform.position = new Vector3(roomOffset, cam.transform.position.y, cam.transform.position.z);
        GameManager.instance.SetPlayerLocation("Bathroom");

        updateRoomButton();
    }

    public void button_GoToLivingRoom()
    {
        button_OpenMap();
        levelManager.DisableMagnifyUI();
        cam.transform.position = new Vector3(roomOffset * 2, cam.transform.position.y, cam.transform.position.z);
        GameManager.instance.SetPlayerLocation("LivingRoom");

        updateRoomButton();
    }

    public void button_GoToKitchen()
    {
        button_OpenMap();
        levelManager.DisableMagnifyUI();
        cam.transform.position = new Vector3(roomOffset * 3, cam.transform.position.y, cam.transform.position.z);
        GameManager.instance.SetPlayerLocation("Kitchen");

        updateRoomButton();
    }

    private void updateRoomButton()
    {
        Debug.Log("Changing map room scale");
        levelManager.currentMapState = MapState.MapIsClosed;
        if (GameManager.instance.currentPlayerLocation == "Bedroom")
        {
            ScaleButton(bedroomButton);
        }
        else if (GameManager.instance.currentPlayerLocation == "Bathroom")
        {
            ScaleButton(bathroomButton);
        }
        else if (GameManager.instance.currentPlayerLocation == "LivingRoom")
        {
            ScaleButton(livingRoomButton);
        }
        else if (GameManager.instance.currentPlayerLocation == "Kitchen")
        {
            ScaleButton(kitchenButton);
        }
        else
        {
            System.Diagnostics.Debug.Write("scale button not set");
        }
    }

    private void ScaleButton(GameObject button)
    {
        for(int i = 0; i < allButtons.Length; i++)
        {
            allButtons[i].transform.localScale = new Vector3(inactiveScale, inactiveScale, inactiveScale);
        } 
        button.transform.localScale = new Vector3(activeScale, activeScale, activeScale);
    }

    public void UpdateMapDirtyNum() // gotta change func name to smth else in levelmanager + this script
    {
        bedroomDirtyMap.color = new Color(1.0f, 1.0f, 1.0f, ((float)bedroomDirtyNum)/denominator);
        bathroomDirtyMap.color = new Color(1.0f, 1.0f, 1.0f, ((float)bathroomDirtyNum)/denominator);
        kitchenDirtyMap.color = new Color(1.0f, 1.0f, 1.0f, ((float)kitchenDirtyNum)/denominator);
        livingRoomDirtyMap.color = new Color(1.0f, 1.0f, 1.0f, ((float)livingRoomDirtyNum)/denominator);
    }
    
    public void button_Restart()
    {
        if (warningOpen) {return;}
        isRestarting = true;
        warningText.text += " restart?";
        OpenWarning();
    }

    public void button_Exit()
    {
        if (warningOpen) {return;}
        isExiting = true;
        warningText.text += " exit?";
        OpenWarning();
    }

    private void OpenWarning()
    {
        if(warningOpen)
        {
            return;
        }
        AudioManager.instance.PlayAudioClip(clickSfx);
        foreach (GameObject button in allButtons)
        {
            button.SetActive(false);
        }
        menuButtons.SetActive(false);
        warningPopup.SetActive(true);
        warningOpen = true;
        AudioManager.instance.PauseBGM(true);
        Time.timeScale = 0f;
    }

    public void button_BackToGame()
    {
        AudioManager.instance.PlayAudioClip(clickSfx);
        resetWarning();
    }

    public void button_Confirm()
    {
        //resetWarning();
        AudioManager.instance.PlayAudioClip(clickSfx);
        if (isRestarting)
        {
            Debug.Log("Restarting");
            //reset game val, prolly in lvl manager
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene");
            this.enabled = false;
        }
        else if (isExiting)
        {
            Debug.Log("Exiting");
            //go back to main menu
            Time.timeScale = 1f;
            SceneManager.LoadScene("StartScene");
            this.enabled = false;
        }
    }

    private void resetWarning()
    {
        if(!warningOpen)
        {
            return;
        }
        foreach (GameObject button in allButtons)
        {
            button.SetActive(true);
        }

        AudioManager.instance.PauseBGM(false);
        Time.timeScale = 1f;

        menuButtons.SetActive(true);
        warningPopup.SetActive(false);
        warningOpen = false;
        isExiting = false;
        isRestarting = false;
        warningText.text = "Are you sure you would like to";
    }
}
