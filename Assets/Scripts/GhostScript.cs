using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GhostScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer smokeSprite;
    [SerializeField] private SpriteRenderer stunnedSprite;
    [SerializeField] private float ghostDelay;
    [SerializeField] private AudioClip[] laughs;
    [SerializeField] private AudioClip[] screams;

    [SerializeField] private SpriteRenderer hatSprite;
    [SerializeField] private SpriteRenderer faceSprite;
    [SerializeField] private SpriteRenderer clothesSprite;

    [SerializeField] private AudioClip[] smokeSfx;

    private Vector3 ghostDefaultPosition;

    private Vector3 startHatPosition;
    private Vector3 startFacePosition;
    private Vector3 startClothesPosition;

    private Vector3 nextPoint;

    private float ghostZPosition = 80.5f;

    private float ghostYMove = 0.25f;

    private Coroutine fadeCoroutine;

    private bool isStunned = false;

    public static GhostScript instance; // only one ghost


    private void Awake()
    {
        if(instance==null){instance=this;}
        else{Destroy(gameObject);}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if (StatsManager.Instance.equippedHat)
        // {
        //     hatSprite.sprite = StatsManager.Instance.equippedHat.image;
        // }
        // if (StatsManager.Instance.equippedFace)
        // {
        //     faceSprite.sprite = StatsManager.Instance.equippedFace.image;
        // }
        // if (StatsManager.Instance.equippedTie)
        // {
        //     clothesSprite.sprite = StatsManager.Instance.equippedTie.image;
        // }

        StatsManager statsManager = GameObject.FindFirstObjectByType<StatsManager>();

        if(statsManager.equippedHat != null)
        {
            hatSprite.sprite = statsManager.equippedHat.image;
        }

        if(statsManager.equippedFace != null)
        {
            faceSprite.sprite = statsManager.equippedFace.image;
        }

        if(statsManager.equippedClothes != null)
        {
            clothesSprite.sprite = statsManager.equippedClothes.image;
        }

        startHatPosition = hatSprite.transform.localPosition;
        startFacePosition = faceSprite.transform.localPosition;
        startClothesPosition = clothesSprite.transform.localPosition;

        ghostDefaultPosition = new Vector3(15, 0, 80.5f);

        gameObject.transform.position = ghostDefaultPosition;
        gameObject.SetActive(false);
    }

    private void ShowGhost(SpriteRenderer ghostSprite)
    {
        if(fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(GhostDelay(ghostSprite));
    }

    private IEnumerator GhostDelay(SpriteRenderer ghostSprite)
    {
        Color start = new Color(1f, 1f, 1f, 1f);;
        Color end = new Color(1f, 1f, 1f, 0f);
        
        
        ghostSprite.color = start;
        hatSprite.color = start;
        faceSprite.color = start;
        clothesSprite.color = start;

        yield return new WaitForSeconds(ghostDelay / 2f);

        float currentTime = 0f;

        while(currentTime <= ghostDelay)
        {
            ghostSprite.color = Color.Lerp(start, end, currentTime / ghostDelay);
            hatSprite.color = Color.Lerp(start, end, currentTime / ghostDelay);
            faceSprite.color = Color.Lerp(start, end, currentTime / ghostDelay);
            clothesSprite.color = Color.Lerp(start, end, currentTime / ghostDelay);
            
            if(!isStunned)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + ghostYMove, transform.position.z), currentTime / ghostDelay);
            }
            
            currentTime += Time.deltaTime;
            yield return null;
        }

        ghostSprite.color = end;
        hatSprite.color = end;
        faceSprite.color = end;
        clothesSprite.color = end;

        gameObject.SetActive(false);
    }

    public void SpawnGhost(Vector3 targetPos, bool spawingSmoke)
    {
        ResetAccessories();
        isStunned = false;
        smokeSprite.enabled = false;
        stunnedSprite.enabled = false;
        sprite.enabled = false;
        AudioManager.instance.GhostLaugh(laughs);
        this.transform.position = targetPos;
        ghostZPosition = targetPos.z;
        gameObject.SetActive(true);
        if (spawingSmoke)
        {
            Debug.Log("SMOKE GHOST");

            smokeSprite.enabled = true;

            AudioManager.instance.PlayAudioClip(smokeSfx[Random.Range(0, smokeSfx.Length)]);

            ShowGhost(smokeSprite);
        }
        else
        {
            sprite.enabled = true;

            ShowGhost(sprite);
        }
    }

    public void SetGhostSpeed(float num)
    {
        if(num < ghostDelay)
        {
            ghostDelay = num;
        }
    }

    public void StunGhost(Vector3 direction)
    {
        if(isStunned)
        {
            return;
        }
        isStunned = true;

        AudioManager.instance.GhostLaugh(screams);

        Rigidbody hatRb = hatSprite.gameObject.AddComponent<Rigidbody>();
        Rigidbody faceRb = faceSprite.gameObject.AddComponent<Rigidbody>();
        Rigidbody clothesRb = clothesSprite.gameObject.AddComponent<Rigidbody>();

        Rigidbody ghostRb = stunnedSprite.gameObject.AddComponent<Rigidbody>();
        ghostRb.useGravity = false;

        Debug.Log("STUNNED GHOST");

        stunnedSprite.enabled = true;
        stunnedSprite.GetComponent<Animator>().Play("Ghost_blow_anim",0, 0f);
        smokeSprite.enabled = false;
        sprite.enabled = false;
        ShowGhost(stunnedSprite);

        hatRb.linearVelocity = direction * Random.Range(1f,10f);
        faceRb.linearVelocity = direction * Random.Range(1f,10f);
        clothesRb.linearVelocity = direction * Random.Range(1f,10f);

        ghostRb.linearVelocity = direction * Random.Range(0.75f,2.5f);

        hatRb.angularVelocity = new Vector3(0f,0f, Random.Range(-20f, 20f));
        faceRb.angularVelocity = new Vector3(0f,0f, Random.Range(-20f, 20f));
        clothesRb.angularVelocity = new Vector3(0f,0f, Random.Range(-20f, 20f));

        ghostRb.angularVelocity = new Vector3(0f,0f, Random.Range(-5f, 5f));
    }

    private void ResetAccessories()
    {
        if(hatSprite.gameObject.GetComponent<Rigidbody>() != null)
        {
            Destroy(hatSprite.gameObject.GetComponent<Rigidbody>());
        }
        if(faceSprite.gameObject.GetComponent<Rigidbody>() != null)
        {
            Destroy(faceSprite.gameObject.GetComponent<Rigidbody>());
        }
        if(clothesSprite.gameObject.GetComponent<Rigidbody>() != null)
        {
            Destroy(clothesSprite.gameObject.GetComponent<Rigidbody>());
        }

        if(stunnedSprite.gameObject.GetComponent<Rigidbody>() != null)
        {
            Destroy(stunnedSprite.gameObject.GetComponent<Rigidbody>());
        }

        hatSprite.transform.localPosition = startHatPosition;
        faceSprite.transform.localPosition = startFacePosition;
        clothesSprite.transform.localPosition = startClothesPosition;

        stunnedSprite.transform.localPosition = Vector3.zero;

        hatSprite.transform.rotation = Quaternion.identity;
        faceSprite.transform.rotation = Quaternion.identity;
        clothesSprite.transform.rotation = Quaternion.identity;

        stunnedSprite.transform.rotation = Quaternion.identity;


    }

}
