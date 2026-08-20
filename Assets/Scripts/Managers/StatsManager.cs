using UnityEngine;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance {get; private set;}

    public int coins;

    public List<PurchasableObjectSO> ownedItems = new List<PurchasableObjectSO>();
    public PurchasableObjectSO equippedHat;
    public PurchasableObjectSO equippedFace;
    public PurchasableObjectSO equippedClothes;

    private int playerNumber;

    public bool activateCutscene;  // default yes for cutscene

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);

            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SetPlayerNum(int players)
    {
        playerNumber = players;
    }

    public int ReturnNumPlayers()
    {
        return playerNumber;
    }
    

    public void AddCoins(int num)
    {
        coins += num;
        Debug.Log($"Coins: {coins}");
    }

    public void BuyItem(PurchasableObjectSO data)
    {
        coins -= data.cost;
        Debug.Log($"Coins: {coins}");
        ownedItems.Add(data);
    }

    public void SetActivateCutscene(bool y)
    {
        activateCutscene = y;
    }
}
