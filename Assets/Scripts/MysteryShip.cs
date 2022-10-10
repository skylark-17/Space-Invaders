using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Camera;
using Random = UnityEngine.Random;

public class MysteryShip : MonoBehaviour
{
    public float speed = 5;

    public float cooldown = 20;

    public static int Score => Random.Range(30, 50) * 10;

    public bool active = true; 
    
    private Vector3 _direction = Vector3.left;
    
    private Camera _camera;

    private Vector3 _leftEdge;
    private Vector3 _rightEdge;

    public Action WhenKilled;
    
    private void Awake()
    {
        gameObject.SetActive(false);
        
        _camera = main;
        
        _leftEdge = _camera.ViewportToWorldPoint(Vector3.zero);
        _rightEdge = _camera.ViewportToWorldPoint(Vector3.right);
        
        Start();
    }

    private void Appear()
    {
        var position = transform.position;
        position.x = _rightEdge.x + 5.0f;
        transform.position = position;
 
        gameObject.SetActive(true);
    }

    public void Stop()
    {
        CancelInvoke(nameof(Appear));
    }

    public void Start()
    {
        InvokeRepeating(nameof(Appear), cooldown, cooldown);
    }

    void Update()
    {
        if (!active) return;
        
        transform.position += _direction * (speed * Time.deltaTime);
        
        if (transform.position.x < _leftEdge.x - 5.0f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            WhenKilled?.Invoke();
            
            gameObject.SetActive(false);
        }
    }
}
