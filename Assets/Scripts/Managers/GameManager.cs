using UnityEngine;
using Board.Input;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public string currentPlayerLocation; // "Bedroom" or "Bathroom"
    [SerializeField] private string startingRoom;

    
    private void Awake()
    {
        if(instance==null){instance=this;}
        else{Destroy(gameObject);}
    }

    private void Start()
    {
        currentPlayerLocation = startingRoom;
        AudioManager.instance.PlayRoomAmbience();
    }


    public void SetPlayerLocation(string room)
    {
        if(room == "Bedroom")
        {
            currentPlayerLocation = "Bedroom";
        }
        else if (room == "Bathroom")
        {
            currentPlayerLocation = "Bathroom";
        }
        else if (room == "LivingRoom")
        {
            currentPlayerLocation = "LivingRoom";
        }
        else if (room == "Kitchen")
        {
            currentPlayerLocation = "Kitchen";
        }
        else
        {
            Debug.Log("Room error, likely syntax error in MapUI.cs script");
        }
        AudioManager.instance.PlayRoomAmbience();
    }





}
