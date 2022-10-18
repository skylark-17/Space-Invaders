using System;
using UnityEngine;
using static UnityEngine.Camera;

public class Player : MonoBehaviour
{
    public float speed = 5;

    public Projectile laserPrefab;

    public Action HitInvader;
    public Action HitMissile;
    public Action PicUpPowerUp;
    public Action MysteryShipAppear;

    public bool active = true;

    public int MysteryShipCoolDown = 20;

    private int _shotsDone = 0;
    
    private Camera _camera;

    private Vector3 _leftEdge;
    private Vector3 _rightEdge;

    private bool _readyToShoot = true;

    private void Awake()
    {
        _camera = main;
        
        _leftEdge = _camera.ViewportToWorldPoint(Vector3.zero);
        _rightEdge = _camera.ViewportToWorldPoint(Vector3.right);
    }

    private void FixedUpdate()
    {
        if (!active) return;
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (transform.position.x > _leftEdge.x + 1.0f)
            {
                transform.position += Vector3.left * (speed * Time.deltaTime);
            }
        }
        
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (transform.position.x < _rightEdge.x - 1.0f)
            {
                transform.position += Vector3.right * (speed * Time.deltaTime);
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Shoot();
        }

    }

    private void Shoot()
    {
        if (!_readyToShoot) return;

        ++_shotsDone;
        if (_shotsDone == MysteryShipCoolDown)
        {
            MysteryShipAppear.Invoke();
            _shotsDone = 0;
        }
        
        var projectile = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        projectile.WhenDestroyed += OnLaserDestroyed; 
        
        _readyToShoot = false;
    }

    private void OnLaserDestroyed()
    {
        _readyToShoot = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!active) return;
        
        if (col.gameObject.layer == LayerMask.NameToLayer("Invader")) 
        {
            HitInvader?.Invoke();
        }
        
        if (col.gameObject.layer == LayerMask.NameToLayer("Missile"))
        {
            HitMissile?.Invoke();
        }
        
        if (col.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
        {
            PicUpPowerUp?.Invoke();
        }

    }
}
