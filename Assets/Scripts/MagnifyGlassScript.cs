using System;
using Board.Input;
using UnityEngine;

public class MagnifyGlassScript : MonoBehaviour
{
    // [SerializeField] private InteractableObjectSO interactable;
    [SerializeField] private GameObject panelScrub;
    [SerializeField] private GameObject panelBag;
    [SerializeField] private GameObject panelDry;
    void Start()
    {
        panelBag.SetActive(false);
        panelScrub.SetActive(false);
        panelDry.SetActive(false);
    }

    // private void ProssessContact(BoardContact contact)
    // {
    //     if (contact.glyphId!=4) return;
    //     switch (contact.phase)
    //     {
    //         case BoardContactPhase.Began:
    //             CheckClean(contact);
    //             Debug.Log("2");
    //             break;
    //         case BoardContactPhase.Moved:
    //             break;
    //         case BoardContactPhase.Stationary:
    //             break;
    //         case BoardContactPhase.Ended:
    //         case BoardContactPhase.Canceled:
    //             break;
    //     }
    // }
    public void MagnifyUI(string componentType)
    {
        if (componentType == "scrub")
        {
            panelScrub.SetActive(true);
        }
        else if (componentType == "trash")
        {
            panelBag.SetActive(true);
        }
        else if (componentType == "dry")
        {
            panelDry.SetActive(true);
        }
    }

    public void DisableMagnifyUI()
    {
        panelScrub.SetActive(false);
        panelBag.SetActive(false);
        panelDry.SetActive(false);
    }

    // private bool CheckCollision(BoardContact contact)
    // {
    //     Vector2 location = contact.screenPosition;
    //     Vector3 worldLocation = Camera.main.ScreenToWorldPoint(new Vector3(location.x, location.y, 10f));
    //     worldLocation.z = Camera.main.transform.position.z;

    //     Debug.DrawRay(worldLocation, Vector3.forward * 20f, Color.red);
        
    //     RaycastHit hit;
    //     if(Physics.Raycast(worldLocation, Vector3.forward, out hit, 20f))
    //     {
    //         return true;
    //     }
    //     return false;
    // }

    // private void CheckClean(BoardContact contact)
    // {
    //     if(!CheckCollision(contact))
    //     {
    //         return;
    //     }
        
    // }
}
