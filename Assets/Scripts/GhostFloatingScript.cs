using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostFloatingScript : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float height;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * height;
        transform.position = new Vector3(transform.position.x, newY, 0);
    }
}
