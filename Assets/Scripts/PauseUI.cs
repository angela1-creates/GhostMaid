using UnityEngine;
using TMPro;
using Board.Input;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class PauseUI : MonoBehaviour
{
    public static PauseUI instance;

    [SerializeField] private GameObject mapBackground;

    private bool isPauseOpen = false;

    private void Awake()
    {
        if(instance==null){instance=this;}
        else{Destroy(gameObject);}
    }

    private void Start()
    {
        mapBackground.SetActive(false);
    }


    public void button_OpenPause()
    {
        if(isPauseOpen)
        {
            return;
        }
        AudioManager.instance.PauseBGM(true);
        Time.timeScale = 0f;
        isPauseOpen = true;
        mapBackground.SetActive(true);
    }

    public void button_Resume()
    {
        if(!isPauseOpen)
        {
            return;
        }
        AudioManager.instance.PauseBGM(false);
        Time.timeScale = 1f;
        isPauseOpen = false;
        mapBackground.SetActive(false);
    }

    public void button_Restart()
    {
        if(!isPauseOpen)
        {
            return;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void button_Menu()
    {
        if(!isPauseOpen)
        {
            return;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }
}