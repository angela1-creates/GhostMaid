using Board.Input;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TextCore;

public class TutorialBlowDryerScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    public InteractableObjectSO objectData;

    private Collider zone;
    private TutorialManager testManager;

    [SerializeField] private float dryTime;
    private float currentDryTime;  // read only
    private bool isDry;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        zone = GetComponent<BoxCollider>();
        testManager = GameObject.FindFirstObjectByType<TutorialManager>();
        currentDryTime = dryTime;
        ChangeDry(false);
    }


    public void UpdateDryer(BoardContact contact)
    {
        if (isDry) return;
        currentDryTime-=Time.deltaTime;
        Debug.Log(dryTime);

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
            // sprite.sprite = objectData.trashSprites[Random.Range(0, objectData.trashSprites.Length)]; 
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
        ChangeDry(true);
    }
}
