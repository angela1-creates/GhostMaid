using UnityEngine;
using System.Collections;

public class GhostShopScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer hatSprite;
    [SerializeField] private SpriteRenderer faceSprite;
    [SerializeField] private SpriteRenderer clothesSprite;

    // remember, this aint the real ghost being changed

    private void Start()
    {
        EquipClothes();
    }

    private void Update()
    {
        
    }

    private void EquipClothes()
    {
        if (StatsManager.Instance.equippedHat)
        {
            hatSprite.sprite = StatsManager.Instance.equippedHat.image;
        }
        if (StatsManager.Instance.equippedFace)
        {
            faceSprite.sprite = StatsManager.Instance.equippedFace.image;
        }
        if (StatsManager.Instance.equippedClothes)
        {
            clothesSprite.sprite = StatsManager.Instance.equippedClothes.image;
        }
    }
}
