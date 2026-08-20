using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PurchaseObjectData", menuName = "ScriptableObjects/PurchaseObjectData")]
public class PurchasableObjectSO : ScriptableObject
{
    public Sprite image;
    public bool isPurchased; // false for not purchaed, true for purchased
    public int cost;
    public bool isEquipped;  // on ghost?
    public string itemType;
}
