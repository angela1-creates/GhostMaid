using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ClockScript : MonoBehaviour
{
    [SerializeField] private Image clock;
    private float timeRemaining;
    private float totalTime;
    [SerializeField] private LevelManager levelManager;

    [Header("Text stuff")]
    [SerializeField] private TextMeshProUGUI timerText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalTime = levelManager.time;
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining = levelManager.time;
        CountDown();
    }

    public void CountDown()
    {
        //timeRemaining-=Time.deltaTime;
        timerText.text = ((int)timeRemaining).ToString();
        clock.fillAmount = timeRemaining/totalTime;
    }

    public void SetColor(Color c)
    {
        clock.color = c;
    }
}
