using UnityEngine;
using Board.Input;
using System.Collections;

public class TutorialScrubScript : MonoBehaviour
{
    [SerializeField] private GameObject testObject;
    [SerializeField] private float scrubTime;

    public InteractableObjectSO objectData;

    private float currentScrubTime;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer flashSprite;
    [SerializeField] private float flashTime;
    private Collider zone;
    private Vector2 lastFramePosition;
    private TutorialManager testManager;
    public bool isClean;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        zone = GetComponent<BoxCollider>();
        testManager = GameObject.FindFirstObjectByType<TutorialManager>();
        ChangeClean(false);
        flashSprite.sprite = objectData.whiteVer;
    }
    

    public void UpdateScrub(BoardContact contact)
    {
        //this is not running
        if(isClean)
        {
            return;
        }
        if(lastFramePosition == new Vector2(-99f, -99f))
        {
            lastFramePosition = contact.screenPosition;
            return;
        }

         // audio sound
        AudioManager.instance.PlayScrub(objectData);

        Debug.Log("scrubbing scrubbing");

        float distanceFromLastFrame = Vector2.Distance(contact.screenPosition, lastFramePosition);
        Debug.Log($"distance: {distanceFromLastFrame}");
        currentScrubTime -= Time.deltaTime * distanceFromLastFrame;
        if(currentScrubTime <= 0)
        {
            ScrubComplete(contact);
        }
        lastFramePosition = contact.screenPosition;

    }

    private void ResetScrubTimer()
    {
        currentScrubTime = scrubTime;
        lastFramePosition = new Vector2(-99f,-99f); // -99, -99 is just used kind of like null just to make let the computer know that there was no "previous" frame
    }

    private void ScrubComplete(BoardContact contact)
    {
        testManager.JustScrubbed(this);
        AudioManager.instance.EndScrub();
        SparkleSpawner.instance.SpawnSparkle(contact);
        ChangeClean(true);
        Flash();
    }

    public void ChangeClean(bool c)
    {
        isClean = c;
        if(c)
        {
            sprite.sprite = objectData.clean;  
            zone.enabled = false;
        }
        else
        {
            ResetScrubTimer();
            sprite.sprite = objectData.dirty;    
            zone.enabled = true;
        }
    }

    private void Flash()
    {
        StartCoroutine("FlashCoroutine");
    }

    private IEnumerator FlashCoroutine()
    {
        flashSprite.gameObject.SetActive(true);

        Color start = new Color(1f, 1f, 1f, 1f);;
        Color end = new Color(1f, 1f, 1f, 0f);

        flashSprite.color = start;

        float currentTime = 0f;

        while(currentTime <= flashTime)
        {
            flashSprite.color = Color.Lerp(start, end, currentTime / flashTime);
            currentTime += Time.deltaTime;
            yield return null;
        }

        flashSprite.color = end;

        flashSprite.gameObject.SetActive(false);
    }
        
}
