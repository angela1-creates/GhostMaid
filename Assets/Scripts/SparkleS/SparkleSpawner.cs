using Board.Input;
using UnityEngine;

public class SparkleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject sparkleSprite;

    // only onw spawner
    public static SparkleSpawner instance;

    private void Awake()
    {
        if(instance==null){instance=this;}
        else{Destroy(gameObject);}
    }

    public void SpawnSparkle(BoardContact contact)
    {
        Vector2 location = contact.screenPosition;
        Vector3 worldLocation = Camera.main.ScreenToWorldPoint(new Vector3(location.x, location.y, 10f));
        GameObject spawnedSparkle = Instantiate(sparkleSprite, worldLocation, Quaternion.identity);
        Debug.Log("spawning sparkle in SparkleSpawner.cs");
    }
}
