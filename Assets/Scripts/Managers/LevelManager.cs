using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Board.Input;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using System.Collections;
using System.Threading;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;

public enum GameState {Playing, PostRound} 
public enum MapState {MapIsOpen, MapIsClosed}

public class LevelManager : MonoBehaviour
{ 
    public GameState currentState = GameState.Playing;
    public MapState currentMapState = MapState.MapIsClosed;

    private bool playerCanInteract = true;

    [Header("Hierarchy Folders")]
    [SerializeField] private Transform stainableFolder;
    [SerializeField] private Transform trashableFolder;
    [SerializeField] private Transform wetableFolder;

    [Header("Smoke Prefab")]
    [SerializeField] private GameObject smokePrefab;
    private float smokeZPosition = 81f;

    [Header("Ghost")]
    [SerializeField] private GameObject ghost;
    private float ghostZPosition = 80.5f;
    private float difficultyMultiplyer = 1f;

    [Header("Percentage for coin")]
    [SerializeField] private float treshold1 = 33.33f;
    [SerializeField] private float treshold2 = 66.66f;
    [SerializeField] private float treshold3 = 90f;

    private List<ScrubScript> stainableList;
    private List<ScrubScript> alreadyStainedList;

    private List<TrashScript> trashableList;
    private List<TrashScript> alreadyTrashedList;

    private List<BlowDryerScript> wetableList;
    private List<BlowDryerScript> alreadyWetList;

    private int totalStainableObjects;
    private int totalTrashableObjects;

    private int totalDirtied = 0; //total things messed up by the ghost
    private int totalCleaned = 0; //total cleaned by player

    private int totalStained = 0; //total objects stained
    private int totalStainedCleaned = 0; //total stained objects scrubbed

    private int totalTrashed = 0; //total trash left behind by ghost
    private int totalTrashedPickedUp = 0; //total trash bagged

    private int totalWet = 0; //total water left behind by ghost
    private int totalWetDried = 0; //total water dried

    private int totalSmoked = 0;

    private float cycleTime;
    private float currentTime;


    [Header("Break Times")]
    [SerializeField] private float startBreak; //how much time the player is given before timer starts
    [SerializeField] private float endBreak; //how much time the player is given before timer ends
    
    [Header("Phase Times")]
    [SerializeField] private float secondPhase;
    [SerializeField] private float thirdPhase;

    [Header("Dirty Numbers - Phase 0")]
    [SerializeField] private int startAmountDirty;
    private int totalStainedPhase0;
    private int totalTrashedPhase0;
    private int totalWetPhase0;

    [Header("Dirty Numbers - Phase 1")]
    [SerializeField] private int willStainPhase1;
    [SerializeField] private int willTrashPhase1;
    [SerializeField] private int willWetPhase1;
    [SerializeField] private int ghostSmokePhase1;

    [Header("Dirty Numbers - Phase 2")]
    [SerializeField] private int willStainPhase2;
    [SerializeField] private int willTrashPhase2;
    [SerializeField] private int willWetPhase2;
    [SerializeField] private int ghostSmokePhase2;

    [Header("Dirty Numbers - Phase 3")]
    [SerializeField] private int willStainPhase3;
    [SerializeField] private int willTrashPhase3;
    [SerializeField] private int willWetPhase3;
    [SerializeField] private int ghostSmokePhase3;


    private int willStain;
    private int willTrash;
    private int willWet;

    private int currentPhase = 0;
    //0 = everything starting out dirty at the beginning

    private bool canDirty = false;
    //can the ghost dirty things

    private float maxTime;
    [Header("Public Time")]
    public float time;
    [SerializeField] private ClockScript clock;

    private float roomLeftBound = -6.5f;
    private float roomRightBound = 6.5f;

    private float roomDownBound = -3.5f;
    private float roomUpBound = 3.5f;

    private float roomOffset = 50f;



    private ScrubScript lastScrub;

    [Header("Aiming Tools")]
    [SerializeField] private GameObject nozzleRaycast;
    [SerializeField] private GameObject nozzleAim;
    [SerializeField] private GameObject wandRaycast;
    [SerializeField] private GameObject wandAim;

    private string componentType;

    [Header("Tool Panels")]
    [SerializeField] private GameObject panelScrub;
    [SerializeField] private GameObject panelBag;
    [SerializeField] private GameObject panelDry;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem scrubParticles;
    [SerializeField] private ParticleSystem bagParticles;
    [SerializeField] private ParticleSystem dryParticles;

    [Header("Audio")]
    [SerializeField] private AudioClip cleanItem;

    [Header("Progress Bar")]

    private bool isGhostStunned = false;

    [SerializeField] private Image progressBar;
    [SerializeField] private Image progressBarGhost;
    [SerializeField] private Sprite happyGhost;
    [SerializeField] private Sprite neutralGhost;
    [SerializeField] private Sprite stunnedGhost;
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

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxTime = time;

        if (StatsManager.Instance.ReturnNumPlayers() != 1)
        {
            difficultyMultiplyer = 1.5f;
        }

        willStainPhase1 = (int)(willStainPhase1 * difficultyMultiplyer);
        willTrashPhase1 = (int)(willTrashPhase1 * difficultyMultiplyer);
        willWetPhase1 = (int)(willWetPhase1 * difficultyMultiplyer);
        ghostSmokePhase1 = (int)(ghostSmokePhase1 * difficultyMultiplyer);

        willStainPhase2 = (int)(willStainPhase2 * difficultyMultiplyer);
        willTrashPhase2 = (int)(willTrashPhase2 * difficultyMultiplyer);
        willWetPhase2 = (int)(willWetPhase2 * difficultyMultiplyer);
        ghostSmokePhase2 = (int)(ghostSmokePhase2 * difficultyMultiplyer);
        
        willStainPhase3 = (int)(willStainPhase3 * difficultyMultiplyer);
        willTrashPhase3 = (int)(willTrashPhase3 * difficultyMultiplyer);
        willWetPhase3 = (int)(willWetPhase3 * difficultyMultiplyer);
        ghostSmokePhase3 = (int)(ghostSmokePhase3 * difficultyMultiplyer);

        progressBarColor = progressBar.color;

        currentTime = cycleTime;
        stainableList = new List<ScrubScript>();
        alreadyStainedList = new List<ScrubScript>();

        trashableList = new List<TrashScript>();
        alreadyTrashedList = new List<TrashScript>();

        wetableList = new List<BlowDryerScript>();
        alreadyWetList = new List<BlowDryerScript>();

        foreach(Transform obj in stainableFolder)
        {
            stainableList.Add(obj.GetComponent<ScrubScript>());
        }

        foreach(Transform obj in trashableFolder)
        {
            trashableList.Add(obj.GetComponent<TrashScript>());
        }

        foreach(Transform obj in wetableFolder)
        {
            wetableList.Add(obj.GetComponent<BlowDryerScript>());
        }

        for(int i = 0; i < startAmountDirty; i++)
        {
            DirtyRandom();
        }

        Debug.Log($"Starting with {totalDirtied} dirtied");

        //setting up phase 1
        currentPhase = 1;

        cycleTime = (secondPhase - startBreak) / (float)((willStainPhase1 + willTrashPhase1 + willWetPhase1 + ghostSmokePhase1));
        ResetGhostSpeed();

        willStain = willStainPhase1 + willStainPhase2 + willStainPhase3;
        willTrash = willTrashPhase1 + willTrashPhase2 + willTrashPhase3;
        willWet = willWetPhase1 + willWetPhase2 + willWetPhase3;


        totalStainableObjects = stainableList.Count;
        totalTrashableObjects = trashableList.Count;

        panelBag.SetActive(false);
        panelScrub.SetActive(false);
        panelDry.SetActive(false);

        AudioManager.instance.PlayBGM();

        Invoke("ToggleCanDirty", startBreak);
        Invoke("SecondPhase", secondPhase);
        Invoke("ThirdPhase", thirdPhase);

        scrubParticles.Stop();
        bagParticles.Stop();
        dryParticles.Stop();

        

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

    private void SecondPhase()
    {
        Debug.Log("Ghost entering 2nd phase");
        currentPhase = 2;

        willStainPhase2 += (willStainPhase1 - totalStained + totalStainedPhase0);
        willTrashPhase2 += (willTrashPhase1 - totalTrashed + totalTrashedPhase0);
        willWetPhase2 += (willWetPhase1 - totalWet + totalWetPhase0);
        ghostSmokePhase2 += (ghostSmokePhase1 - totalSmoked);

        cycleTime = (thirdPhase - secondPhase) / (float)((willStainPhase2 + willTrashPhase2 + willWetPhase2 + ghostSmokePhase2));
        ResetGhostSpeed();

        clock.SetColor(Color.yellow);
    }

    private void ThirdPhase()
    {
        Debug.Log("Ghost entering 3nd phase");
        currentPhase = 3;

        willStainPhase3 += (willStainPhase2 + willStainPhase1 - totalStained + totalStainedPhase0);
        willTrashPhase3 += (willTrashPhase2 + willTrashPhase1 - totalTrashed + totalTrashedPhase0);
        willWetPhase3 += (willWetPhase2 + willWetPhase1 - totalWet + totalWetPhase0);
        ghostSmokePhase3 += (ghostSmokePhase2 + ghostSmokePhase1 - totalSmoked);

        cycleTime = ((maxTime - thirdPhase - endBreak) / (float)(willStainPhase3 + willTrashPhase3 + willWetPhase3 + ghostSmokePhase3));
        ResetGhostSpeed();

        clock.SetColor(Color.red);
    }

    private void ResetGhostSpeed()
    {
        ghost.GetComponent<GhostScript>().SetGhostSpeed(cycleTime / 2f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!playerCanInteract)
        {
            return;
        }
        time -= Time.deltaTime;
        if(time <= 0)
        {
            playerCanInteract = false;
            Invoke("End", 1f);
        }
        if(!canDirty)
        {
            return;
        }
        if(currentTime <= 0f)
        {
            Debug.Log("Dirtied an item!");
            DirtyRandom();
            currentTime = cycleTime;
        }
        else
        {
            currentTime -= Time.deltaTime;
        }
    }

    void Update()
    {
        if(!playerCanInteract)
        {
            return;
        }
        if(currentState == GameState.Playing || currentMapState == MapState.MapIsClosed)
        {
            var contacts = BoardInput.GetActiveContacts();

            foreach(var contact in contacts)
            {
                ProcessContact(contact);
            }   
        }
        // Debug.Log(totalCleaned / totalDirtied);
    }

    private void ProcessContact(BoardContact contact)
    {
        // if(contact.type != BoardContactType.Glyph)
        // {
        //     return;
        // }
        if(contact.glyphId == 1) //bag
        {
            ProcessBag(contact);
        }
        else if(contact.glyphId == 2)//dryer
        {
            ProcessDry(contact);
        }
        else if(contact.glyphId == 3) //scrub + finger(temp)
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
        nozzleAim.transform.position=new Vector3(worldLocation.x,worldLocation.y,80f);
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
        wandAim.transform.position=new Vector3(worldLocation.x,worldLocation.y,80f);
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
        if(obj == null || obj.GetComponent<TrashScript>() == null)
        {
            return;
        }

        TrashScript script = obj.GetComponent<TrashScript>();

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

        if(obj != null && obj.GetComponent<GhostScript>() != null)
        {
            Debug.Log(contact.orientation);
            obj.GetComponent<GhostScript>().StunGhost(new Vector3(Mathf.Cos(contact.orientation),Mathf.Sin(contact.orientation),0f));
            StunGhost();
            ChangeGhostBarSprite(stunnedGhost);
            return;
        }
        
        if(obj == null || obj.GetComponent<BlowDryerScript>() == null)
        {
            AudioManager.instance.EndDryer();
            dryParticles.Stop();
            return;
        }
        BlowDryerScript script = obj.GetComponent<BlowDryerScript>();

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

    private void ProcessMagnify(BoardContact contact)
    {
        GameObject obj = CheckCollisionMagnify(contact);
        if(obj == null)
        {
            DisableMagnifyUI();
            return;
        }

        if (obj.GetComponent<ScrubScript>() != null)
        {
            componentType = "scrub";
        }
        else if (obj.GetComponent<TrashScript>() != null)
        {
            componentType = "trash";
        }
        else if (obj.GetComponent<BlowDryerScript>() != null)
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
        if(obj == null || obj.GetComponent<ScrubScript>() == null)
        {
            AudioManager.instance.EndScrub();
            scrubParticles.Stop();

            if(lastScrub != null)
            {
                lastScrub.ToggleProgressBar(false);
            }

            return;
        }

        ScrubScript script = obj.GetComponent<ScrubScript>();

        lastScrub = script;

        script.ToggleProgressBar(true);

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
                scrubParticles.Stop();
                script.ToggleProgressBar(false);
                break;
            case BoardContactPhase.Canceled:
                scrubParticles.Stop();
                script.ToggleProgressBar(false);
                break;
        }
    }

    private void ToggleCanDirty()
    {
        canDirty = true;
        AudioManager.instance.ActivateGhostSounds(true);
        Debug.Log("Ghost activated!");
    }

    private void DirtyRandom()
    {
        if(isGhostStunned)
        {
            isGhostStunned = false;
            ChangeGhostBarSprite(neutralGhost);
            ResetBarAccessories(true);
            Debug.Log("Ghost Unstunned!");
            return;
        }

        int willStainCurrent = 0;
        int willTrashCurrent = 0;
        int willWetCurrent = 0;
        int ghostSmokeCurrent = 0;

        switch(currentPhase)
        {
            case 0:
                willStainCurrent = 99999;
                willTrashCurrent = 99999;
                willWetCurrent = 99999;
                ghostSmokeCurrent = 99999;
                break;
            case 1:
                willStainCurrent = willStainPhase1;
                willTrashCurrent = willTrashPhase1;
                willWetCurrent = willWetPhase1;
                ghostSmokeCurrent = ghostSmokePhase1;
                break;
            case 2:
                willStainCurrent = willStainPhase1 + willStainPhase2;
                willTrashCurrent = willTrashPhase1 + willTrashPhase2;
                willWetCurrent = willWetPhase1 + willWetPhase2;
                ghostSmokeCurrent = ghostSmokePhase1 + ghostSmokePhase2;
                break;
            case 3:
                willStainCurrent = willStainPhase1 + willStainPhase2 + willStainPhase3;
                willTrashCurrent = willTrashPhase1 + willTrashPhase2 + willTrashPhase3;
                willWetCurrent = willWetPhase1 + willWetPhase2 + willWetPhase3;
                ghostSmokeCurrent = ghostSmokePhase1 + ghostSmokePhase2 + ghostSmokePhase3;
                break;
        }

        int totalStainedCurrent = totalStained - totalStainedPhase0;
        int totalTrashedCurrent = totalTrashed - totalTrashedPhase0;
        int totalWetCurrent = totalWet - totalWetPhase0;

        int randomNum = Random.Range(0,10);

        if(randomNum <= 3)
        {
            if(totalStainedCurrent < willStainCurrent && stainableList.Count > 0) //stain
            {
                StainObject();
            }
            else if(totalTrashedCurrent < willTrashCurrent && trashableList.Count > 0) //spawn trash
            {
                TrashObject();
            }
            else if(totalWetCurrent < willWetCurrent && wetableList.Count > 0) //spawn water
            {
                WetObject();
            }
            else if(totalSmoked < ghostSmokeCurrent)
            {
                CreateSmoke();
            }
            else
            {
                Debug.Log("theres nothing to dirty anymore lolll");
            }
        }
        else if(randomNum <= 6)
        {
            if(totalTrashedCurrent < willTrashCurrent && trashableList.Count > 0) //spawn trash
            {
                TrashObject();
            }
            else if(totalWetCurrent < willWetCurrent && wetableList.Count > 0) //spawn water
            {
                WetObject();
            }
            else if(totalStainedCurrent < willStainCurrent && stainableList.Count > 0) //stain
            {
                StainObject();
            }
            else if(totalSmoked < ghostSmokeCurrent)
            {
                CreateSmoke();
            }
            else
            {
                Debug.Log("theres nothing to dirty anymore lolll");
            }
        }
        else if(randomNum <= 8 || currentPhase == 0)
        {
            if(totalWetCurrent < willWetCurrent && wetableList.Count > 0) //spawn water
            {
                WetObject();
            }
            else if(totalStainedCurrent < willStainCurrent && stainableList.Count > 0) //stain
            {
                StainObject();
            }
            else if(totalTrashedCurrent < willTrashCurrent && trashableList.Count > 0) //spawn trash
            {
                TrashObject();
            }
            else if(totalSmoked < ghostSmokeCurrent)
            {
                CreateSmoke();
            }
            else
            {
                Debug.Log("theres nothing to dirty anymore lolll");
            }
        }
        else
        {
            Debug.Log("SMOKE PICKED");
            if(totalSmoked < ghostSmokeCurrent)
            {
                CreateSmoke();
            }
            else if(totalStainedCurrent < willStainCurrent && stainableList.Count > 0) //stain
            {
                StainObject();
            }
            else if(totalTrashedCurrent < willTrashCurrent && trashableList.Count > 0) //spawn trash
            {
                TrashObject();
            }
            else if(totalWetCurrent < willWetCurrent && wetableList.Count > 0) //spawn water
            {
                WetObject();
            }
            else
            {
                Debug.Log("theres nothing to dirty anymore lolll");
            }
        }

        StopTrashingCheck();
    }

    private void StainObject()
    {
        if(currentPhase == 0)
        {
            totalStainedPhase0++;
        }
        ScrubScript randomObject = stainableList[Random.Range(0, stainableList.Count)];
        stainableList.Remove(randomObject);
        alreadyStainedList.Add(randomObject);
        randomObject.ChangeClean(false);
        totalDirtied++;
        totalStained++;
        // progressBar.fillAmount = totalCleaned / totalDirtied;
        UpdateProgressBar(false);
        MoveGhost(randomObject.gameObject, false);
        AddDirtyMapNum(randomObject.gameObject);
    }

    private void TrashObject()
    {
        if(currentPhase == 0)
        {
            totalTrashedPhase0++;
        }
        TrashScript randomObject = trashableList[Random.Range(0, trashableList.Count)];
        trashableList.Remove(randomObject);
        alreadyTrashedList.Add(randomObject);
        randomObject.Enable();
        totalDirtied++;
        totalTrashed++;
        // progressBar.fillAmount = totalCleaned / totalDirtied;
        UpdateProgressBar(false);
        MoveGhost(randomObject.gameObject, false);
        AddDirtyMapNum(randomObject.gameObject);
    }

    private void WetObject()
    {
        if(currentPhase == 0)
        {
            totalWetPhase0++;
        }
        BlowDryerScript randomObject = wetableList[Random.Range(0, wetableList.Count)];
        wetableList.Remove(randomObject);
        alreadyWetList.Add(randomObject);
        randomObject.ChangeDry(false);
        totalDirtied++;
        totalWet++;
        // progressBar.fillAmount = totalCleaned / totalDirtied;
        UpdateProgressBar(false);
        MoveGhost(randomObject.gameObject, false);
        AddDirtyMapNum(randomObject.gameObject);
    }

    private void CreateSmoke()
    {
        Debug.Log("Smoke Spawned!!!");
        int randomNum = Random.Range(0,4); //0 or 1
        Vector3 smokePosition;

        smokePosition = new Vector3(Random.Range(roomLeftBound + (randomNum * roomOffset), roomRightBound + (randomNum * roomOffset)),
                                    Random.Range(roomDownBound, roomUpBound),
                                    smokeZPosition);

        
        GameObject smoke = Instantiate(smokePrefab, smokePosition, Quaternion.identity);

        smoke.transform.localScale = new Vector3(Random.Range(4f,6f),
                                            Random.Range(1.5f,3f),
                                            1f);

        BlowDryerScript obj = smoke.GetComponent<BlowDryerScript>();
        obj.ChangeDry(false);

        totalSmoked++;
        MoveGhost(smoke, true);
    }

    private void MoveGhost(GameObject obj, bool spawingSmoke)
    {
        Vector3 ghostPosition = obj.GetComponent<BoxCollider>().bounds.center;
        ghostPosition.z = ghostZPosition;

        ghost.GetComponent<GhostScript>().SpawnGhost(ghostPosition, spawingSmoke);
    }

    public void AddDirtyMapNum(GameObject obj) 
    {
        if(obj.transform.position.x <= 25f)  // bedroom
        {
            MapUI.instance.bedroomDirtyNum++;
        }
        else if(obj.transform.position.x <= 75f) // bathroom
        {
            MapUI.instance.bathroomDirtyNum++; 
        }
        else if(obj.transform.position.x <= 125f)  // living room
        {
            MapUI.instance.livingRoomDirtyNum++; 
        }
        else  //kitchen
        {
            MapUI.instance.kitchenDirtyNum++; 
        } 
        
        MapUI.instance.UpdateMapDirtyNum();
    }

    public void SubtractDirtyMapNum()
    {
        if(GameManager.instance.currentPlayerLocation == "Bedroom")  // bedroom
        {
            MapUI.instance.bedroomDirtyNum--;
        }
        else if(GameManager.instance.currentPlayerLocation == "Bathroom") // bathroom
        {
            MapUI.instance.bathroomDirtyNum--; 
        }
        else if(GameManager.instance.currentPlayerLocation == "LivingRoom")  // living room
        {
            MapUI.instance.livingRoomDirtyNum--; 
        }
        else  //kitchen
        {
            MapUI.instance.kitchenDirtyNum--; 
        } 

        MapUI.instance.UpdateMapDirtyNum();
    }

    private void StopTrashingCheck()
    {
        if(totalTrashed >= willTrash && totalStained >= willStain)
        {
            canDirty = false;
            Debug.Log("Ghost decativated!");
        }
    }

    public void JustScrubbed(ScrubScript obj)
    {
        scrubParticles.Stop();
        alreadyStainedList.Remove(obj);
        stainableList.Add(obj);
        totalCleaned++;
        totalStainedCleaned++;
        // progressBar.fillAmount = totalCleaned / totalDirtied;
        AudioManager.instance.PlayAudioClip(cleanItem);
        UpdateProgressBar(true);
        Debug.Log("Cleaned: " + totalCleaned + " / " + totalDirtied);
        SubtractDirtyMapNum();
    }

    public void JustPickedUpTrash(TrashScript obj)
    {
        StartCoroutine("Wait");
        alreadyTrashedList.Remove(obj);
        trashableList.Add(obj);
        totalCleaned++;
        totalTrashedPickedUp++;
        // progressBar.fillAmount = totalCleaned / totalDirtied;
        AudioManager.instance.PlayAudioClip(cleanItem);
        UpdateProgressBar(true);
        Debug.Log("Cleaned: " + totalCleaned + " / " + totalDirtied);
        SubtractDirtyMapNum();
    }

    IEnumerable Wait() //wait 4 particles to play cause ts annoying af and this is the only solution i had
    {
        yield return new WaitForSeconds(.5f);
        bagParticles.Stop();
    }

    public void JustDried(BlowDryerScript obj)
    {
        dryParticles.Stop();
        AudioManager.instance.PlayAudioClip(cleanItem);
        if(obj.countsForScore)
        {
            alreadyWetList.Remove(obj);
            wetableList.Add(obj);
            totalCleaned++;
            totalWetDried++;
            // progressBar.fillAmount = totalCleaned / totalDirtied;
            UpdateProgressBar(true);
            Debug.Log("Cleaned: " + totalCleaned + " / " + totalDirtied);
            SubtractDirtyMapNum();
        }
        else
        {
            Debug.Log("Smoke cleared!");
        }
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
    }

    private void UpdateProgressBar(bool c)
    {
        if(barCoroutine != null)
        {
            StopCoroutine(barCoroutine);
        }
        if(barFxCoroutine != null)
        {
            StopCoroutine(barFxCoroutine);
        }
        barCoroutine = StartCoroutine("ChangeBarSize");
        if(c)
        {
            barFxCoroutine = StartCoroutine("FlashBar");
        }
        else
        {
            barFxCoroutine = StartCoroutine("BarGhost");
        }
    }

    private IEnumerator ChangeBarSize()
    {
        float targetLength = (float)totalCleaned / (float)totalDirtied;
        float currentBarTime = 0f;
        while(currentBarTime <= barSizeTime)
        {
            progressBar.rectTransform.sizeDelta = new Vector2(progressBar.rectTransform.sizeDelta.x,
                                                    Mathf.Lerp(progressBar.rectTransform.sizeDelta.y, targetLength * 100f, currentBarTime / barSizeTime));
            currentBarTime += Time.deltaTime;
            yield return null;
        }
        
        progressBar.rectTransform.sizeDelta = new Vector2(progressBar.rectTransform.sizeDelta.x,
                                                    targetLength * 100f);

    }

    private IEnumerator BarGhost()
    {
        Color start = new Color(1f, 1f, 1f, 1f);
        Color end = new Color(1f, 1f, 1f, 0.2f);

        ChangeGhostBarSprite(happyGhost);

        progressBarGhost.color = start;
        barHatSprite.color = start;
        barFaceSprite.color = start;
        barClothesSprite.color = start;
        
        progressBarGhost.rectTransform.sizeDelta = new Vector2(ghostMaxSize, ghostMaxSize / 10f);
        barAccessories.sizeDelta = new Vector2(ghostMaxSize, ghostMaxSize / 10f);
            

        progressBar.color = progressBarColor;

        yield return new WaitForSeconds(barSizeTime * 3f);

        ChangeGhostBarSprite(neutralGhost);

        
        float currentBarTime = 0f;
        while(currentBarTime <= barSizeTime)
        {
            progressBarGhost.color = Color.Lerp(start, end, currentBarTime / barSizeTime);
            barHatSprite.color = Color.Lerp(start, end, currentBarTime / barSizeTime);
            barFaceSprite.color = Color.Lerp(start, end, currentBarTime / barSizeTime);
            barClothesSprite.color = Color.Lerp(start, end, currentBarTime / barSizeTime);

            progressBarGhost.rectTransform.sizeDelta = Vector2.Lerp(new Vector2(ghostMaxSize, ghostMaxSize / 10f), new Vector2(ghostMinSize, ghostMinSize / 10f), currentBarTime / barSizeTime);
            barAccessories.sizeDelta = Vector2.Lerp(new Vector2(ghostMaxSize, ghostMaxSize / 10f), new Vector2(ghostMinSize, ghostMinSize / 10f), currentBarTime / barSizeTime);
            
            currentBarTime += Time.deltaTime;
            yield return null;
        }

        progressBarGhost.color = end;
        barHatSprite.color = end;
        barFaceSprite.color = end;
        barClothesSprite.color = end;

        progressBarGhost.rectTransform.sizeDelta = new Vector2(ghostMinSize, ghostMinSize / 10f);
        barAccessories.sizeDelta = new Vector2(ghostMinSize, ghostMinSize / 10f);
    }

    private IEnumerator FlashBar()
    {
        Color start = new Color(1f, 1f, 1f, 1f);


        ChangeGhostBarSprite(neutralGhost);
        progressBarGhost.color = new Color(1f,1f,1f,0.2f);
        barHatSprite.color = new Color(1f,1f,1f,0.2f);
        barFaceSprite.color = new Color(1f,1f,1f,0.2f);
        barClothesSprite.color = new Color(1f,1f,1f,0.2f);
        progressBarGhost.rectTransform.sizeDelta = new Vector2(ghostMinSize, ghostMinSize / 10f);
        barAccessories.sizeDelta = new Vector2(ghostMinSize, ghostMinSize / 10f);

        progressBar.color = start;
        
        float currentBarTime = 0f;
        while(currentBarTime <= barSizeTime)
        {
            progressBar.color = Color.Lerp(start, progressBarColor, currentBarTime / barSizeTime);
            currentBarTime += Time.deltaTime;
            yield return null;
        }

        progressBar.color = progressBarColor;
    }

    private void ChangeGhostBarSprite(Sprite ghostSprite)
    {
        progressBarGhost.sprite = ghostSprite;
    }

    public void StunGhost()
    {
        if(isGhostStunned)
        {
            return;
        }
        ResetBarAccessories(false);
        isGhostStunned = true;
        switch(currentPhase)
        {
            case 0:
                Debug.Log("Nothing should happen.");
                break;
            case 1:
                if(willStainPhase1 > 0)
                {
                    willStainPhase1--;
                }
                else if(willTrashPhase1 > 0)
                {
                    willTrashPhase1--;
                }
                else if(willWetPhase1 > 0)
                {
                    willWetPhase1--;
                }
                else if(ghostSmokePhase1 > 0)
                {
                    ghostSmokePhase1--;
                }
                else
                {
                    Debug.Log("No more items to prevent.");
                }
                break;
            case 2:
                if(willStainPhase2 > 0)
                {
                    willStainPhase2--;
                }
                else if(willTrashPhase2 > 0)
                {
                    willTrashPhase2--;
                }
                else if(willWetPhase2 > 0)
                {
                    willWetPhase2--;
                }
                else if(ghostSmokePhase2 > 0)
                {
                    ghostSmokePhase2--;
                }
                else
                {
                    Debug.Log("No more items to prevent.");
                }
                break;
            case 3:
                if(willStainPhase3 > 0)
                {
                    willStainPhase3--;
                }
                else if(willTrashPhase3 > 0)
                {
                    willTrashPhase3--;
                }
                else if(willWetPhase3 > 0)
                {
                    willWetPhase3--;
                }
                else if(ghostSmokePhase3 > 0)
                {
                    ghostSmokePhase3--;
                }
                else
                {
                    Debug.Log("No more items to prevent.");
                }
                break;
        }

    }

    private void ResetBarAccessories(bool c)
    {
        barAccessories.gameObject.SetActive(c);
    }

    private void End()
    {
        /*debugText.text = "Total Cleaned: " + totalCleaned + " / " + totalDirtied + " (" + ((int)(((float)totalCleaned/(float)totalDirtied) * 100f)) + "%)"
                            + "\nTotal Scrubbed: " + totalStainedCleaned + " / " + totalStained
                            + "\nTotal Trash PU'd: " + totalTrashedPickedUp + " / " + totalTrashed;*/
        float playerScore = (float)(totalCleaned/(float)totalDirtied) * 100f;
        EndCanvasUI.instance.ShowEndCanvas(playerScore, treshold1, treshold2, treshold3);
        currentState = GameState.PostRound;
        this.enabled = false;
    }
}
