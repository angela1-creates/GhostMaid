using UnityEngine;
using Board.Input;
using System.Collections;

public class ScrubScript : MonoBehaviour
{
    [SerializeField] private GameObject testObject;
    [SerializeField] private float scrubTime;

    public InteractableObjectSO objectData;

    private float currentScrubTime;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer flashSprite;
    [SerializeField] private float flashTime;
    [SerializeField] private Transform wholeProgressBar;
    [SerializeField] private Transform progressBarSize;
    [SerializeField] private SpriteRenderer progressBarSprite;
    [SerializeField] private SpriteRenderer progressBarFrameSprite;
    private Collider zone;
    private Vector2 lastFramePosition;
    private LevelManager testManager;

    private float barTransparency = 0.5f;

    private bool isStain = false;

    public bool isClean;

    private Coroutine currentBarCoroutine;

    // for ghost spawning stuff
    private Vector3 ghostSpawnPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        zone = GetComponent<BoxCollider>();
        testManager = GameObject.FindFirstObjectByType<LevelManager>();
        wholeProgressBar.position = zone.bounds.center;
        ChangeClean(true);
        flashSprite.sprite = objectData.whiteVer;
        if(objectData.clean == null)
        {
            isStain = true;
        }
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     if(isClean) //dont need to check if already clean
    //     {
    //         return;
    //     }
        
    // }
    

    public void UpdateScrub(BoardContact contact)
    {
        if(isClean)
        {
            return;
        }
        if(lastFramePosition == new Vector2(-99f, -99f))
        {
            lastFramePosition = contact.screenPosition;
            return;
        }

        // spawn ghost
        ghostSpawnPos = this.transform.position;
        //GhostScript.instance.SpawnGhost(ghostSpawnPos);

        // audio sound
        AudioManager.instance.PlayScrub(objectData);

        float distanceFromLastFrame = Vector2.Distance(contact.screenPosition, lastFramePosition);
        currentScrubTime -= Time.deltaTime * distanceFromLastFrame;
        if(currentScrubTime <= 0)
        {
            ScrubComplete(contact);
        }

        if(isStain)
        {
            sprite.color = new Color(1f,1f,1f,currentScrubTime / scrubTime);
        }
        progressBarSize.localScale = new Vector3(currentScrubTime / scrubTime, 1f, 1f);

        lastFramePosition = contact.screenPosition;

    }

    public void ToggleProgressBar(bool c)
    {
        // if(isStain)
        // {
        //     return;
        // }
        if(currentBarCoroutine != null)
        {
            StopCoroutine(currentBarCoroutine);
        }
        if(c)
        {
            currentBarCoroutine = StartCoroutine("ProgressBarFadeIn");
        }
        else
        {
            currentBarCoroutine = StartCoroutine("ProgressBarFadeOut");
        }
    }

    private void ResetScrubTimer()
    {
        currentScrubTime = scrubTime;
        sprite.color = new Color(1f,1f,1f,1f);
        progressBarSize.localScale = new Vector3(1f, 1f, 1f);
        lastFramePosition = new Vector2(-99f,-99f); // -99, -99 is just used kind of like null just to make let the computer know that there was no "previous" frame
    }

    private void ScrubComplete(BoardContact contact)
    {
        testManager.JustScrubbed(this);
        AudioManager.instance.EndScrub();
        SparkleSpawner.instance.SpawnSparkle(contact);
        ChangeClean(true);
        Flash();
        ToggleProgressBar(false);
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

    private IEnumerator ProgressBarFadeIn()
    {
        progressBarSize.gameObject.SetActive(true);
        progressBarFrameSprite.gameObject.SetActive(true);

        while(progressBarSprite.color.a <= barTransparency)
        {
            progressBarSprite.color = new Color(1f,1f,1f,progressBarSprite.color.a + (barTransparency / 10f));
            progressBarFrameSprite.color = new Color(1f,1f,1f,progressBarSprite.color.a + 0.1f);
            yield return null;
        }

        progressBarSprite.color = new Color(1f,1f,1f,barTransparency);
        progressBarFrameSprite.color = new Color(1f,1f,1f,1f);
    }

    private IEnumerator ProgressBarFadeOut()
    {
        while(progressBarSprite.color.a >= 0f)
        {
            progressBarSprite.color = new Color(1f,1f,1f,progressBarSprite.color.a - (barTransparency / 10f));
            progressBarFrameSprite.color = new Color(1f,1f,1f,progressBarSprite.color.a - 0.1f);
            yield return null;
        }
        
        progressBarSprite.color = new Color(1f,1f,1f,0f);
        progressBarFrameSprite.color = new Color(1f,1f,1f,0f);

        progressBarSize.gameObject.SetActive(false);
        progressBarFrameSprite.gameObject.SetActive(false);
    }
        
}
