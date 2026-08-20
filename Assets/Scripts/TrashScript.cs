using UnityEngine;
//using Board.Input;

public class TrashScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    public InteractableObjectSO objectData;
    private Collider zone;
    private LevelManager testManager;
    private bool isActive;

    void Awake()
    {
        zone = GetComponent<BoxCollider>();
        testManager = GameObject.FindFirstObjectByType<LevelManager>();
        Disable();
    }

    public void TrashComplete()
    {
        // audio sound
        AudioManager.instance.PlayAudioClip(objectData);
        testManager.JustPickedUpTrash(this);
        Disable();
    }

    public void Enable()
    {
        SetSprite();
        sprite.enabled = true;
        zone.enabled = true;
        isActive = true;
    } 

    private void Disable()
    {
        sprite.enabled = false;
        zone.enabled = false;
        isActive = false;
    }

    private void SetSprite()
    {
        Sprite[] sprites = objectData.trashSprites;
        sprite.sprite = sprites[Random.Range(0, sprites.Length)];
    }

        
}
