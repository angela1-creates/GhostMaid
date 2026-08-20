using Board.Input;
using UnityEngine;
using UnityEngine.TextCore;

public class BlowDryerScript : MonoBehaviour
{

    //[SerializeField] private GameObject tempObj;

    //[SerializeField] private GameObject raycastPosition;
    [SerializeField] private SpriteRenderer sprite;
    public InteractableObjectSO objectData;
    public bool countsForScore;

    private Collider zone;
    private LevelManager testManager;

    [SerializeField] private float dryTime;
    private float currentDryTime;
    private bool isDry;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        zone = GetComponent<BoxCollider>();
        testManager = GameObject.FindFirstObjectByType<LevelManager>();
        currentDryTime = dryTime;
        ChangeDry(true);
    }

    /*
    // Update is called once per frame
    void Update()
    {
        var contacts = BoardInput.GetActiveContacts(BoardContactType.Glyph);

        foreach (var contact in contacts)
        {
            ProssessContact(contact);
        }
    }

    private void ProssessContact(BoardContact contact)
    {
        if (contact.glyphId!=2) return;
        switch (contact.phase)
        {
            case BoardContactPhase.Began:
            case BoardContactPhase.Moved:
            case BoardContactPhase.Stationary:
                // LevelManager.ProcessDryer(contact);
                break;
            case BoardContactPhase.Ended:
            case BoardContactPhase.Canceled:
                //end animation
                break;
        }
    }*/

    public void UpdateDryer(BoardContact contact)
    {
        //particle affect ?
        //check orientation

        /*
        Vector2 location = contact.screenPosition;
        Vector3 worldLocation = Camera.main.ScreenToWorldPoint(new Vector3(location.x, location.y, 10f));
        worldLocation.z = Camera.main.transform.position.z;

        float rotation = contact.orientation;
        //for animation particles, bring particles to glyph
        tempObj.transform.position=new Vector3(worldLocation.x,worldLocation.y,0);
        tempObj.transform.rotation=Quaternion.Euler(0, 0,rotation * Mathf.Rad2Deg);

        Debug.DrawRay(raycastPosition.transform.position, Vector3.forward*20f, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(raycastPosition.transform.position, Vector3.forward, out hit, 20f))
        {*/
            if (isDry) return;
            currentDryTime-=Time.deltaTime;
            // Debug.Log(dryTime);

            AudioManager.instance.PlayDryer(objectData);

            sprite.color = new Color(1f,1f,1f,currentDryTime / dryTime);

            if (currentDryTime <= 0)
            {
                isDry=true;
                //Destroy(gameObject);
                DryComplete();
            }
        //}
    }

    public void ChangeDry(bool c)
    {
        isDry = c;
        if(c)
        {
            sprite.enabled = false;
            zone.enabled = false;
        }
        else
        {
            ResetTimer();
            sprite.sprite = objectData.trashSprites[Random.Range(0, objectData.trashSprites.Length)]; 
            sprite.enabled = true;
            zone.enabled = true;
        }
    }

    private void ResetTimer()
    {
        currentDryTime = dryTime;
        sprite.color = new Color(1f,1f,1f,1f);
    }

    private void DryComplete()
    {
        testManager.JustDried(this);
        AudioManager.instance.EndDryer();
        if(countsForScore)
        {
            ChangeDry(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
