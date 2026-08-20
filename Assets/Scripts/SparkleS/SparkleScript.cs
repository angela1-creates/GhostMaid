using System.Numerics;
using UnityEngine;

public class SparkleScript : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        Destroy(this.gameObject, 1);  // destroy this gameobject sparkle after 1 second
    }
}
