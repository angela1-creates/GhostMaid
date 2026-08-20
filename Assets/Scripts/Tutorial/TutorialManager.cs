using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Board.Input;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.UI;


// acts like levelmanager

public class TutorialManager : MonoBehaviour
{ 
   
    [Header("Tool Panels")]
    [SerializeField] private GameObject panelScrub;
    [SerializeField] private GameObject panelBag;
    [SerializeField] private GameObject panelDry;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem scrubParticles;
    [SerializeField] private ParticleSystem bagParticles;
    [SerializeField] private ParticleSystem dryParticles;

    
    [Header("Aiming Tools")]
    [SerializeField] private GameObject nozzleRaycast;
    [SerializeField] private GameObject nozzleAim;
    [SerializeField] private GameObject wandRaycast;
    [SerializeField] private GameObject wandAim;

    [Header("Audio")]
    [SerializeField] private AudioClip cleanItem;
    [SerializeField] private AudioClip clickSfx;

    [Header("Tutorial Canvas")]
    [SerializeField] private GameObject[] tutorials;
    [SerializeField] private string[] tutorialType; // bag, scrub, dryer
    [SerializeField] private int tutorialIndex = 0; 

    [Header("When we're done")]
    [SerializeField] private GameObject nextButton;
    [SerializeField] private Animator animator;

    [Header("Ghost")]
    [SerializeField] private GameObject sadGhost;
    [SerializeField] private GameObject happyGhost;
    [SerializeField] private AudioClip[] ghostLaughs;
    // [SerializeField] private AnimationClip happyGhostAnim;

    [Header("Congrats text")]
    [SerializeField] private TextMeshProUGUI congratsText;
    [SerializeField] private string[] congratsTexts;

    
    private bool canClickNext = true;
    private string componentType;
    private int nextTutorialIndex;

    [Header("Button modifications")]
    [SerializeField] private GameObject button_BackToStartMenu;


    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextButton.SetActive(true);

        scrubParticles.Stop();
        bagParticles.Stop();
        dryParticles.Stop();
        for(int i = 0; i <= tutorials.Length - 1; i++)
        {
            tutorials[i].SetActive(false);
        }
        tutorials[tutorialIndex].SetActive(true);
        IsGhostHappy(false);
        ActivateCongratsText(false);
        // StartCoroutine("ProgBarLoop");
        if (tutorialType[tutorialIndex] == "ProgressBar")
        {
            StartCoroutine("ProgBarLoop");
        }
    }

    void Update()
    {
        var contacts = BoardInput.GetActiveContacts();

        foreach(var contact in contacts)
        {
            ProcessContact(contact);
        }   
    }

    private void IsGhostHappy(bool y)
    {
        if (y)
        {
            if(componentType != "magnify")
            {
                happyGhost.SetActive(true);
                sadGhost.SetActive(false);   
            }

        }
        else
        {
            sadGhost.SetActive(true);
            happyGhost.SetActive(false);
        }
    }

    private void ActivateCongratsText(bool y)
    {
        if (y)
        {
            congratsText.text = congratsTexts[Random.Range(0, congratsTexts.Length)];
        }
        else
        {
            congratsText.text = "";
        }
    }


    // buttons
    public void button_Next()
    {
        if (canClickNext)
        {
            canClickNext = false;
            StartCoroutine(ButtonDelay());
        }
        else
        {
            // pass
        }

    }

    private IEnumerator ButtonDelay()
    {
        nextTutorialIndex = tutorialIndex + 1;
        if(nextTutorialIndex >= tutorials.Length)
        {
            EndOfTutorial();
        }
        else
        {
            NextTutorial();
        }
        yield return new WaitForSeconds(0.4f);
        canClickNext = true;
    }

    private void EndOfTutorial()
    {
        nextButton.SetActive(false);
        animator.SetBool("isActive", true);
    }

    private void NextTutorial()
    {
        if (tutorialType[nextTutorialIndex] != "map" || tutorialType[nextTutorialIndex] != "ProgressBar")
        {
            IsGhostHappy(false);
        }
        else
        {
            IsGhostHappy(true);
        }

        tutorials[tutorialIndex].SetActive(false);
        tutorials[nextTutorialIndex].SetActive(true);
        if (tutorialType[nextTutorialIndex] == "ProgressBar")
        {
            StartCoroutine(ProgBarLoop());
        }
        tutorialIndex++;
    }


    private IEnumerator ProgBarLoop()
    {
        ProgressBarTutorialScript script = tutorials[tutorialIndex].GetComponent<ProgressBarTutorialScript>(); 
        while (tutorialType[tutorialIndex] == "ProgressBar")
        {
            script.ProgressBarMove();
            yield return new WaitForSeconds(1.5f);
        }
    }

    private void ProcessContact(BoardContact contact)
    {
        if(contact.type != BoardContactType.Glyph)
        {
            return;
        }
        if(contact.glyphId == 1) //bag
        {
            ProcessBag(contact);
        }
        else if(contact.glyphId == 2)//dryer
        {
            ProcessDry(contact);
        }
        else if(contact.glyphId == 3) //scrub
        {
            ProcessScrub(contact);
        }
        else if(contact.glyphId == 4) //magnify
        {
            ProcessMagnify(contact);
        }
    }

    private GameObject CheckCollision(BoardContact contact)
    {
        Vector2 location = contact.screenPosition;
        Vector3 worldLocation = Camera.main.ScreenToWorldPoint(new Vector3(location.x, location.y, 10f));
        worldLocation.z = Camera.main.transform.position.z;

        Debug.DrawRay(worldLocation, Vector3.forward * 20f, Color.red);
    
        RaycastHit hit;
        if(Physics.Raycast(worldLocation, Vector3.forward, out hit, 50f))
        {
            if (contact.glyphId == 3)
            {
                scrubParticles.transform.position= new Vector3(worldLocation.x, worldLocation.y, worldLocation.z + 1f);
            }
            else if (contact.glyphId == 1)
            {
                bagParticles.transform.position= new Vector3(worldLocation.x, worldLocation.y, worldLocation.z + 1f);
            }
            return hit.collider.gameObject;
        }
        return null;   
    }

    private GameObject CheckCollisionDryer(BoardContact contact)
    {
        Vector2 location = contact.screenPosition;
        Vector3 worldLocation = Camera.main.ScreenToWorldPoint(new Vector3(location.x, location.y, 10f));
        worldLocation.z = Camera.main.transform.position.z;

        float rotation = contact.orientation;
        nozzleAim.transform.position=new Vector3(worldLocation.x,worldLocation.y,worldLocation.z);
        nozzleAim.transform.rotation=Quaternion.Euler(0, 0,rotation * Mathf.Rad2Deg);

        Debug.DrawRay(nozzleRaycast.transform.position, Vector3.forward*20f, Color.red);
        
        RaycastHit hit;
        Vector3 rayOrigin = BoardInput.IsPointerFallback ? worldLocation : nozzleRaycast.transform.position;
        if(Physics.Raycast(rayOrigin, Vector3.forward, out hit, 50f))
        {
            dryParticles.transform.position = new Vector3(worldLocation.x,worldLocation.y, worldLocation.z +1f);
            dryParticles.transform.rotation=Quaternion.Euler(0, 0, (rotation - 0.52f) * Mathf.Rad2Deg);
            return hit.collider.gameObject;
        }
        return null;
    }

    private GameObject CheckCollisionMagnify(BoardContact contact)
    {
        Vector2 location = contact.screenPosition;
        Vector3 worldLocation = Camera.main.ScreenToWorldPoint(new Vector3(location.x, location.y, 10f));
        worldLocation.z = Camera.main.transform.position.z;

        float rotation = contact.orientation;
        wandAim.transform.position=new Vector3(worldLocation.x,worldLocation.y, worldLocation.z);
        wandAim.transform.rotation=Quaternion.Euler(0, 0,rotation * Mathf.Rad2Deg - 90f);

        Debug.DrawRay(wandRaycast.transform.position, Vector3.forward*20f, Color.red);
        
        RaycastHit hit;
        Vector3 rayOrigin = BoardInput.IsPointerFallback ? worldLocation : wandRaycast.transform.position;
        if(Physics.Raycast(rayOrigin, Vector3.forward, out hit, 50f))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private void ProcessBag(BoardContact contact)
    {
        GameObject obj = CheckCollision(contact);
        if(obj == null || obj.GetComponent<TutorialTrashScript>() == null)
        {
            return;
        }

        TutorialTrashScript script = obj.GetComponent<TutorialTrashScript>();

        switch(contact.phase)
        {
            case BoardContactPhase.Began:
                bagParticles.Play();
                script.TrashComplete();
                break;
        }
    } 

    private void ProcessDry(BoardContact contact)
    {
        GameObject obj = CheckCollisionDryer(contact);
        // Debug.Log("dryer!");

        if(obj != null && obj.GetComponent<GhostTutorialScript>() != null)
        {
            // we got ghost!
            obj.GetComponent<GhostTutorialScript>().StunGhost(new Vector3(Mathf.Cos(contact.orientation),Mathf.Sin(contact.orientation),0f));
            return;
        }

        if (obj != null && obj.GetComponent<BlowingGhostAwayScript>() != null)
        {
            Debug.Log("we've collided with happy ghost");
            HappyGhostBlownAway();
            return;
        }

        if(obj == null || obj.GetComponent<TutorialBlowDryerScript>() == null)
        {
            // AudioManager.instance.EndDryer();
            dryParticles.Stop();
            return;
        }
        TutorialBlowDryerScript script = obj.GetComponent<TutorialBlowDryerScript>();

        switch(contact.phase)
        {
            case BoardContactPhase.Began:
                dryParticles.Play();
                break;
            case BoardContactPhase.Moved:
            case BoardContactPhase.Stationary:
                if (dryParticles.isStopped)
                    {
                        dryParticles.Play();
                    }
                script.UpdateDryer(contact);
                break;
            case BoardContactPhase.Ended:
                dryParticles.Stop();
                break;
            case BoardContactPhase.Canceled:
                dryParticles.Stop();
                break;
        }
    }

    public void HappyGhostBlownAway()
    {
        StartCoroutine(BlowingGhostAwayScript.instance.PlayAnimation());
    } 

    private void ProcessMagnify(BoardContact contact)
    {
        GameObject obj = CheckCollisionMagnify(contact);
        if(obj == null)
        {
            DisableMagnifyUI();
            return;
        }

        if (obj.GetComponent<TutorialScrubScript>() != null)
        {
            componentType = "scrub";
        }
        else if (obj.GetComponent<TutorialTrashScript>() != null)
        {
            componentType = "trash";
        }
        else if (obj.GetComponent<TutorialBlowDryerScript>() != null)
        {
            componentType = "dry";
        }
        else
        {
            DisableMagnifyUI();
        }

        //MagnifyGlassScript script = obj.GetComponent<MagnifyGlassScript>();

        switch(contact.phase)
        {
            case BoardContactPhase.Began:
            case BoardContactPhase.Moved:
                MagnifyUI(componentType);
                break;
            case BoardContactPhase.Stationary:
                MagnifyUI(componentType);
                break;
            case BoardContactPhase.Ended:
                DisableMagnifyUI();
                break;
            case BoardContactPhase.Canceled:
                DisableMagnifyUI();
                break;
        }
    } 

    private void ProcessScrub(BoardContact contact)
    {
        GameObject obj = CheckCollision(contact);
        if(obj == null || obj.GetComponent<TutorialScrubScript>() == null)
        {
            //AudioManager.instance.EndSfx();
            return;
        }

        TutorialScrubScript script = obj.GetComponent<TutorialScrubScript>();

        switch(contact.phase)
        {
            case BoardContactPhase.Began:
                scrubParticles.Play();
                break;
            case BoardContactPhase.Moved:
                if (scrubParticles.isStopped)
                        {
                            scrubParticles.Play();
                        }
                script.UpdateScrub(contact);
                break;
            case BoardContactPhase.Ended:
            case BoardContactPhase.Canceled:
                break;
        }
    }

    private void TurnOffAnimation()
    {
        try
        {
            GameObject curAnimation = tutorials[tutorialIndex].transform.Find("Animation").gameObject;
            curAnimation.SetActive(false);
        }
        catch
        {
            UnityEngine.Debug.Log("no curAnimation");
        }

    }

    public void GhostBlownAway()
    {
        AudioManager.instance.PlayAudioClip(ghostLaughs[Random.Range(0, ghostLaughs.Length)]);
        IsGhostHappy(true);
        ActivateCongratsText(true);
        // TurnOffAnimation();
    }

    // private void ChangeGhostBarSprite(Sprite ghostSprite)
    // {
    //     progressBarGhost.sprite = ghostSprite;
    // }

    // private IEnumerator BarGhost()
    // {
    //     Color start = new Color(1f, 1f, 1f, 1f);
    //     Color end = new Color(1f, 1f, 1f, 0.2f);

    //     ChangeGhostBarSprite(happyGhostSprite);

    //     progressBarGhost.color = start;
    //     barHatSprite.color = start;
    //     barFaceSprite.color = start;
    //     barClothesSprite.color = start;
        
    //     progressBarGhost.rectTransform.sizeDelta = new Vector2(ghostMaxSize, ghostMaxSize / 10f);
    //     barAccessories.sizeDelta = new Vector2(ghostMaxSize, ghostMaxSize / 10f);
            

    //     progressBar.color = progressBarColor;

    //     yield return new WaitForSeconds(barSizeTime * 3f);

    //     ChangeGhostBarSprite(neutralGhostSprite);

        
    //     float currentBarTime = 0f;
    //     while(currentBarTime <= barSizeTime)
    //     {
    //         progressBarGhost.color = Color.Lerp(start, end, currentBarTime / barSizeTime);
    //         barHatSprite.color = Color.Lerp(start, end, currentBarTime / barSizeTime);
    //         barFaceSprite.color = Color.Lerp(start, end, currentBarTime / barSizeTime);
    //         barClothesSprite.color = Color.Lerp(start, end, currentBarTime / barSizeTime);

    //         progressBarGhost.rectTransform.sizeDelta = Vector2.Lerp(new Vector2(ghostMaxSize, ghostMaxSize / 10f), new Vector2(ghostMinSize, ghostMinSize / 10f), currentBarTime / barSizeTime);
    //         barAccessories.sizeDelta = Vector2.Lerp(new Vector2(ghostMaxSize, ghostMaxSize / 10f), new Vector2(ghostMinSize, ghostMinSize / 10f), currentBarTime / barSizeTime);
            
    //         currentBarTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     progressBarGhost.color = end;
    //     barHatSprite.color = end;
    //     barFaceSprite.color = end;
    //     barClothesSprite.color = end;

    //     progressBarGhost.rectTransform.sizeDelta = new Vector2(ghostMinSize, ghostMinSize / 10f);
    //     barAccessories.sizeDelta = new Vector2(ghostMinSize, ghostMinSize / 10f);
    // }

    // private IEnumerator ChangeBarSize()
    // {
    //     float targetLength = 0.73f;
    //     float currentBarTime = 0f;
    //     while(currentBarTime <= barSizeTime)
    //     {
    //         progressBar.rectTransform.sizeDelta = new Vector2(progressBar.rectTransform.sizeDelta.x,
    //                                                 Mathf.Lerp(progressBar.rectTransform.sizeDelta.y, targetLength * 100f, currentBarTime / barSizeTime));
    //         currentBarTime += Time.deltaTime;
    //         yield return null;
    //     }
        
    //     progressBar.rectTransform.sizeDelta = new Vector2(progressBar.rectTransform.sizeDelta.x,
    //                                                 targetLength * 100f);
    // }

    public void JustScrubbed(TutorialScrubScript obj)
    {
        scrubParticles.Stop();
        AudioManager.instance.PlayAudioClip(cleanItem);
        IsGhostHappy(true);
        ActivateCongratsText(true);
        TurnOffAnimation();
    }

    public void JustPickedUpTrash(TutorialTrashScript obj)
    {
        StartCoroutine("Wait");
        AudioManager.instance.PlayAudioClip(cleanItem);
        IsGhostHappy(true);
        ActivateCongratsText(true);
        TurnOffAnimation();
    }

    IEnumerable Wait() //wait 4 particles to play cause ts annoying af and this is the only solution i had
    {
        yield return new WaitForSeconds(.5f);
        bagParticles.Stop();
    }

    public void JustDried(TutorialBlowDryerScript obj)
    {
        dryParticles.Stop();
        AudioManager.instance.PlayAudioClip(cleanItem);
        IsGhostHappy(true);
        ActivateCongratsText(true);
        TurnOffAnimation();
    }

    private void MagnifyUI(string componentType)
    {
        if (componentType == "scrub")
        {
            panelScrub.SetActive(true);
            panelBag.SetActive(false);
            panelDry.SetActive(false);
        }
        else if (componentType == "trash")
        {
            panelScrub.SetActive(false);
            panelBag.SetActive(true);
            panelDry.SetActive(false);
        }
        else if (componentType == "dry")
        {
            panelScrub.SetActive(false);
            panelBag.SetActive(false);
            panelDry.SetActive(true);
        }
    }

    public void DisableMagnifyUI()
    {
        panelScrub.SetActive(false);
        panelBag.SetActive(false);
        panelDry.SetActive(false);
        IsGhostHappy(true);
        TurnOffAnimation();
    }

}
