using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject invPanel;

    [SerializeField] private GameObject shopButton;
    [SerializeField] private GameObject invButton;

    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI actionButtonText;

    [Header("Testing stuff")]

    [SerializeField] private GameObject buttonPrefab;

    [SerializeField] private List<PurchasableObjectSO> hatList;
    [SerializeField] private List<PurchasableObjectSO> faceList;
    [SerializeField] private List<PurchasableObjectSO> clothesList;

    [SerializeField] private Transform hatsScroll;
    [SerializeField] private Transform faceScroll;
    [SerializeField] private Transform clothesScroll;

    [SerializeField] private Transform hatsScrollInv;
    [SerializeField] private Transform faceScrollInv;
    [SerializeField] private Transform clothesScrollInv;

    // [SerializeField] private Image hatPreview;
    // [SerializeField] private Image facePreview;
    // [SerializeField] private Image clothesPreview;

    [SerializeField] private SpriteRenderer hatPreview;
    [SerializeField] private SpriteRenderer facePreview;
    [SerializeField] private SpriteRenderer clothesPreview;

    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemPrice;

    [SerializeField] private TextMeshProUGUI coinsDisplay;

    [SerializeField] private Sprite brokeButtonSprite;
    [SerializeField] private Sprite buyButtonSprite;
    [SerializeField] private Sprite equipButtonSprite;
    [SerializeField] private Sprite equippedButtonSprite;
    [SerializeField] private Sprite ownedButtonSprite;

    [SerializeField] private AudioClip clickSfx;
    [SerializeField] private AudioClip buySfx;
    [SerializeField] private AudioClip equipSfx;


    private PurchasableObjectSO currentPreview;

    private StatsManager statsManager;

    private bool isShopOpen = true;

    private int startMenuScene = 0;

    //[SerializeField] private List<GameObject> catigoriesList;

    void Start()
    {
        HideAllPreview();
        statsManager = FindFirstObjectByType<StatsManager>();
        
        UpdateDisplays();
    }

    private void UpdateDisplays()
    {
        ResetButton();

        HideAllPreview();

        hatPreview.sprite = null;
        facePreview.sprite = null;
        clothesPreview.sprite = null;

        coinsDisplay.text = $"Coins: {statsManager.coins.ToString()}";

        foreach(Transform child in hatsScroll)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform child in faceScroll)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform child in clothesScroll)
        {
            Destroy(child.gameObject);
        }

        foreach(PurchasableObjectSO item in hatList)
        {
            GameObject add = Instantiate(buttonPrefab, hatsScroll);

            Image image = add.GetComponent<Image>();
            image.sprite = item.image; //changing sprite

            TextMeshProUGUI costText = add.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            costText.text = item.cost.ToString(); //blue cost text

            Button addButton = add.GetComponent<Button>();
            addButton.onClick.AddListener(() => button_PreviewItem(item));

            Image border = add.transform.GetChild(1).GetComponent<Image>();
            border.enabled = false; //gets inv border (pink) and hides it

            if(ContainsItem(item))
            {
                image.color = Color.gray;
                costText.color = Color.gray;
                border.color = Color.gray;
            }
        }
        foreach(PurchasableObjectSO item in faceList)
        {
            GameObject add = Instantiate(buttonPrefab, faceScroll);

            Image image = add.GetComponent<Image>();
            image.sprite = item.image; //changing sprite

            TextMeshProUGUI costText = add.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            costText.text = item.cost.ToString(); //blue cost text

            Button addButton = add.GetComponent<Button>();
            addButton.onClick.AddListener(() => button_PreviewItem(item));

            Image border = add.transform.GetChild(1).GetComponent<Image>();
            border.enabled = false; //gets inv border (pink) and hides it

            if(ContainsItem(item))
            {
                image.color = Color.gray;
                costText.color = Color.gray;
                border.color = Color.gray;
            }
        }
        foreach(PurchasableObjectSO item in clothesList)
        {
            GameObject add = Instantiate(buttonPrefab, clothesScroll);

            Image image = add.GetComponent<Image>();
            image.sprite = item.image; //changing sprite

            TextMeshProUGUI costText = add.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            costText.text = item.cost.ToString(); //blue cost text

            Button addButton = add.GetComponent<Button>();
            addButton.onClick.AddListener(() => button_PreviewItem(item));

            Image border = add.transform.GetChild(1).GetComponent<Image>();
            border.enabled = false; //gets inv border (pink) and hides it

            if(ContainsItem(item))
            {
                image.color = Color.gray;
                costText.color = Color.gray;
                border.color = Color.gray;
            }
        }
    }

    private void UpdateDisplaysInv()
    {
        ResetButton();

        HideAllPreview();

        if(statsManager.equippedHat != null)
        {
            hatPreview.enabled = true;
            hatPreview.sprite = statsManager.equippedHat.image;
        }
        else
        {
            hatPreview.sprite = null;
        }

        if(statsManager.equippedFace != null)
        {
            facePreview.enabled = true;
            facePreview.sprite = statsManager.equippedFace.image;
        }
        else
        {
            facePreview.sprite = null;
        }

        if(statsManager.equippedClothes != null)
        {
            clothesPreview.enabled = true;
            clothesPreview.sprite = statsManager.equippedClothes.image;
        }
        else
        {
            clothesPreview.sprite = null;
        }

        foreach(Transform child in hatsScrollInv)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform child in faceScrollInv)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform child in clothesScrollInv)
        {
            Destroy(child.gameObject);
        }

        foreach(PurchasableObjectSO item in hatList)
        {
            if(!ContainsItem(item))
            {
                continue;
            }
            GameObject add = Instantiate(buttonPrefab, hatsScrollInv);

            Image image = add.GetComponent<Image>();
            image.sprite = item.image; //changing sprite

            TextMeshProUGUI costText = add.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Destroy(costText);

            Button addButton = add.GetComponent<Button>();
            addButton.onClick.AddListener(() => button_PreviewItem(item));

            Image border = add.transform.GetChild(2).GetComponent<Image>();
            border.enabled = false; //gets shop border (blue) and hides it

        }
        foreach(PurchasableObjectSO item in faceList)
        {
            if(!ContainsItem(item))
            {
                continue;
            }
            GameObject add = Instantiate(buttonPrefab, faceScrollInv);

            Image image = add.GetComponent<Image>();
            image.sprite = item.image; //changing sprite

            TextMeshProUGUI costText = add.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Destroy(costText);

            Button addButton = add.GetComponent<Button>();
            addButton.onClick.AddListener(() => button_PreviewItem(item));

            Image border = add.transform.GetChild(2).GetComponent<Image>();
            border.enabled = false; //gets shop border (blue) and hides it
        }
        foreach(PurchasableObjectSO item in clothesList)
        {
            if(!ContainsItem(item))
            {
                continue;
            }
            GameObject add = Instantiate(buttonPrefab, clothesScrollInv);

            Image image = add.GetComponent<Image>();
            image.sprite = item.image; //changing sprite

            TextMeshProUGUI costText = add.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Destroy(costText);

            Button addButton = add.GetComponent<Button>();
            addButton.onClick.AddListener(() => button_PreviewItem(item));

            Image border = add.transform.GetChild(2).GetComponent<Image>();
            border.enabled = false; //gets shop border (blue) and hides it
        }
    }

    public void button_OpenInventory()
    {
        if(!isShopOpen)
        {
            return;
        }
        AudioManager.instance.PlayAudioClip(clickSfx);
        shopPanel.SetActive(false);
        invPanel.SetActive(true);
        UpdateDisplaysInv();
        isShopOpen = false;
    }

    public void button_OpenShop()
    {
        if(isShopOpen)
        {
            return;
        }
        AudioManager.instance.PlayAudioClip(clickSfx);
        shopPanel.SetActive(true);
        invPanel.SetActive(false);
        UpdateDisplays();
        isShopOpen = true;
    }

    public void button_PreviewItem(PurchasableObjectSO item)
    {
        AudioManager.instance.PlayAudioClip(clickSfx);

        actionButton.onClick.RemoveAllListeners();

        SpritePreviewItem(item);

        if(isShopOpen)
        {
            SpritePreviewItem(item);
            if(ContainsItem(item))
            {
                SetBuyButton(ownedButtonSprite);
            }
            else if(statsManager.coins >= item.cost)
            {
                SetBuyButton(buyButtonSprite);
                actionButton.onClick.AddListener(() => button_PurchaseItem(item));
            }
            else
            {
                SetBuyButton(brokeButtonSprite);
            }
        }
        else
        {
            PurchasableObjectSO objToCompare = null;

            switch(item.itemType)
            {
                case "Hat":
                    objToCompare = statsManager.equippedHat;
                    break;
                case "Face":
                    objToCompare = statsManager.equippedFace;
                    break;
                case "Clothes":
                    objToCompare = statsManager.equippedClothes;
                    break;
            }

            if(item == objToCompare)
            {
                SetBuyButton(equippedButtonSprite);
                actionButton.onClick.AddListener(() => button_UnequipItem(item));
            }
            else
            {
                SetBuyButton(equipButtonSprite);
                actionButton.onClick.AddListener(() => button_EquipItem(item));
            }
        }


        

    }

    private void SpritePreviewItem(PurchasableObjectSO item)
    {
        currentPreview = item;
        HideAllPreview();

        itemName.text = item.name;
        if(isShopOpen)
        {
            itemPrice.text = item.cost.ToString();
        }

        switch(item.itemType)
        {
            case "Hat":
                hatPreview.enabled = true;
                hatPreview.sprite = item.image;
                if(isShopOpen)
                {
                    facePreview.sprite = null;
                    clothesPreview.sprite = null;
                }
                else
                {
                    if(statsManager.equippedFace != null)
                    {
                        facePreview.enabled = true;
                        facePreview.sprite = statsManager.equippedFace.image;
                    }
                    if(statsManager.equippedClothes != null)
                    {
                        clothesPreview.enabled = true;
                        clothesPreview.sprite = statsManager.equippedClothes.image;
                    }
                }
                break;
            case "Face":
                facePreview.enabled = true;
                facePreview.sprite = item.image;
                if(isShopOpen)
                {
                    hatPreview.sprite = null;
                    clothesPreview.sprite = null;
                }
                else
                {
                    if(statsManager.equippedHat != null)
                    {
                        hatPreview.enabled = true;
                        hatPreview.sprite = statsManager.equippedHat.image;
                    }
                    if(statsManager.equippedClothes != null)
                    {
                        clothesPreview.enabled = true;
                        clothesPreview.sprite = statsManager.equippedClothes.image;
                    }
                }
                break;
            case "Clothes":
                clothesPreview.enabled = true;
                clothesPreview.sprite = item.image;
                if(isShopOpen)
                {
                    hatPreview.sprite = null;
                    facePreview.sprite = null;
                }
                else
                {
                    if(statsManager.equippedHat != null)
                    {
                        hatPreview.enabled = true;
                        hatPreview.sprite = statsManager.equippedHat.image;
                    }
                    if(statsManager.equippedFace != null)
                    {
                        facePreview.enabled = true;
                        facePreview.sprite = statsManager.equippedFace.image;
                    }
                }
                break;
        }
    }

    private void HideAllPreview()
    {
        hatPreview.enabled = false;
        facePreview.enabled = false;
        clothesPreview.enabled = false;

        // hatPreview = null;
        // facePreview = null;
        // clothesPreview = null;
    }

    public void button_PurchaseItem(PurchasableObjectSO item)
    {
        if(statsManager.ownedItems.Contains(item))
        {
            //this shouldnt happen but in case
            return;
        }
        if(item.cost > statsManager.coins)
        {
            //this shouldnt happen but in case
            //SetBuyButton(brokeButtonSprite);
            //uncomment this if you want the broke button to appear after clicking
            return;
        }
        AudioManager.instance.PlayAudioClip(buySfx);
        statsManager.BuyItem(item);
        UpdateDisplays();
        SpritePreviewItem(item);

        
        SetBuyButton(ownedButtonSprite);
    }

    public void button_EquipItem(PurchasableObjectSO item)
    {
        UpdateDisplaysInv();
        SpritePreviewItem(item);

        AudioManager.instance.PlayAudioClip(equipSfx);

        switch(item.itemType)
        {
            case "Hat":
                statsManager.equippedHat = item;
                break;
            case "Face":
                statsManager.equippedFace = item;
                break;
            case "Clothes":
                statsManager.equippedClothes = item;
                break;
        }

        
        SetBuyButton(equippedButtonSprite);

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => button_UnequipItem(item));
    }

    public void button_UnequipItem(PurchasableObjectSO item)
    {
        UpdateDisplaysInv();
        SpritePreviewItem(item);

        AudioManager.instance.PlayAudioClip(equipSfx);

        switch(item.itemType)
        {
            case "Hat":
                statsManager.equippedHat = null;
                hatPreview.sprite = null;
                hatPreview.enabled = false;
                break;
            case "Face":
                statsManager.equippedFace = null;
                facePreview.sprite = null;
                facePreview.enabled = false;
                break;
            case "Clothes":
                statsManager.equippedClothes = null;
                clothesPreview.sprite = null;
                clothesPreview.enabled = false;
                break;
        }

        
        SetBuyButton(equipButtonSprite);
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => button_EquipItem(item));
    }

    private void ResetButton()
    {
        actionButtonText.text = "";
        SetBuyButton(null);
        itemName.text = "";
        itemPrice.text = "";
        actionButton.onClick.RemoveAllListeners();
    }

    private bool ContainsItem(PurchasableObjectSO item)
    {
        return statsManager.ownedItems.Contains(item);
    }

    public void button_GoTostartMenu()
    {
        AudioManager.instance.PlayAudioClip(clickSfx);
        SceneManager.LoadScene(startMenuScene);
    }

    private void SetBuyButton(Sprite buttonSprite)
    {
        if(buttonSprite == null)
        {
            actionButton.gameObject.SetActive(false);
            return;
        }
        actionButton.gameObject.SetActive(true);
        actionButton.gameObject.GetComponent<Image>().sprite = buttonSprite;
    }

    // //get game obj from collision on touch ? 
    // public void testMethod4ShopItems(GameObject chosenItem)
    // {
    //     GameObject displayedItem = chosenItem.transform.GetChild(0).gameObject;
    //     displayedItem.SetActive(true);
    //     // Debug.Log(displayedItem);
    // }
}
