using UnityEngine;
using UnityEngine.SceneManagement;
using Board.Input;
using UnityEngine.UI;
using TMPro;

public class EndCanvasUI : MonoBehaviour
{
    public static EndCanvasUI instance;

    [SerializeField] private GameObject[] coins;

    [SerializeField] private GameObject canvasFolder;


    [Header("Percentage text")]
    [SerializeField] private TextMeshProUGUI percentageText;

    [Header("Ghost sprites")]
    [SerializeField] private GameObject winSprite;
    [SerializeField] private GameObject loseSprite;

    [Header("Audio")]
    [SerializeField] private AudioClip loseAudio;
    [SerializeField] private AudioClip partialWinAudio;
    [SerializeField] private AudioClip winAudio;

    private void Awake()
    {
        if(instance==null){instance=this;}
        else{Destroy(gameObject);}
    }

    private void Start()
    {
        winSprite.SetActive(false);
        loseSprite.SetActive(false);
        canvasFolder.SetActive(false);
    }


    public void ShowEndCanvas(float playerScore, float treshold1, float treshold2, float treshold3)
    {
        canvasFolder.SetActive(true);

        int coinsGotten = 0;

        if (playerScore < treshold1)
        {
            // no coins
            loseSprite.SetActive(true);

            coins[0].SetActive(false);
            coins[1].SetActive(false);
            coins[2].SetActive(false);

            AudioManager.instance.PlayAudioClip(loseAudio);
        }
        else if (playerScore >= treshold1 && playerScore < treshold2)
        {
            // coin1.sprite = activeCoinSprite;
            loseSprite.SetActive(true);

            coinsGotten = 1;
            coins[0].SetActive(true);
            coins[1].SetActive(false);
            coins[2].SetActive(false);

            AudioManager.instance.PlayAudioClip(loseAudio);
        }
        else if (playerScore >= treshold2 && playerScore < treshold3)
        {
            // coin1.sprite = activeCoinSprite;
            // coin2.sprite = activeCoinSprite;
            winSprite.SetActive(true);

            coinsGotten = 2;
            coins[0].SetActive(true);
            coins[1].SetActive(true);
            coins[2].SetActive(false);


            AudioManager.instance.PlayAudioClip(partialWinAudio);
        }
        else if (playerScore >= treshold3)
        {
            // coin1.sprite = activeCoinSprite;
            // coin2.sprite = activeCoinSprite;
            // coin3.sprite = activeCoinSprite;
            winSprite.SetActive(true);

            coinsGotten = 3;
            coins[0].SetActive(true);
            coins[1].SetActive(true);
            coins[2].SetActive(true);

            AudioManager.instance.PlayAudioClip(winAudio);
        }

        percentageText.text = $"{(int)playerScore}%";

        StatsManager statsManager = FindFirstObjectByType<StatsManager>();
        if(statsManager != null)
        {
            statsManager.AddCoins(coinsGotten);
        }
        else
        {
            Debug.Log("Start from StartScene to save coins");
        }


    }
}
