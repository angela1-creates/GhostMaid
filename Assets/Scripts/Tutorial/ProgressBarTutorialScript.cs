using UnityEngine;
using UnityEngine.UI;

public class ProgressBarTutorialScript : MonoBehaviour
{

    [SerializeField] private Image progressBar;
    [SerializeField] private Image progressBarGhost;
    [SerializeField] private Sprite happyGhostSprite;
    [SerializeField] private Sprite neutralGhostSprite;
    [SerializeField] private RectTransform barAccessories;
    [SerializeField] private Image barHatSprite;
    [SerializeField] private Image barFaceSprite;
    [SerializeField] private Image barClothesSprite;
    private Coroutine barCoroutine;
    private Coroutine barFxCoroutine;
    private float ghostMaxSize = 180f;
    private float ghostMinSize = 130f;
    private Color progressBarColor;
    private float barSizeTime = 0.25f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressBarColor = progressBar.color;

        if(StatsManager.Instance.equippedHat != null)
        {
            barHatSprite.gameObject.SetActive(true);
            barHatSprite.sprite = StatsManager.Instance.equippedHat.image;
        }
        if(StatsManager.Instance.equippedFace != null)
        {
            barFaceSprite.gameObject.SetActive(true);
            barFaceSprite.sprite = StatsManager.Instance.equippedFace.image;
        }
        if(StatsManager.Instance.equippedClothes != null)
        {
            barClothesSprite.gameObject.SetActive(true);
            barClothesSprite.sprite = StatsManager.Instance.equippedClothes.image;
        }

        progressBar.rectTransform.sizeDelta = new Vector2(progressBar.rectTransform.sizeDelta.x,
                                                    0f);
    }

    public void ProgressBarMove()
    {
        float targetLength = Random.Range(0.0f,1.0f);
        Debug.Log(targetLength);
        float currentBarTime = 0f;
        while(currentBarTime <= barSizeTime)
        {
            progressBar.rectTransform.sizeDelta = new Vector2(progressBar.rectTransform.sizeDelta.x,
                                                    Mathf.Lerp(progressBar.rectTransform.sizeDelta.y, targetLength * 100f, currentBarTime / barSizeTime));
            currentBarTime += Time.deltaTime;
        }
        
        progressBar.rectTransform.sizeDelta = new Vector2(progressBar.rectTransform.sizeDelta.x,
                                                    targetLength * 100f);
    }
}
