using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 direction = Vector3.up;

    public float speed = 5;

    public Action WhenDestroyed;

    private void FixedUpdate()
    {
        transform.position += direction * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        WhenDestroyed?.Invoke();
        
        Destroy(gameObject);
    }
}
