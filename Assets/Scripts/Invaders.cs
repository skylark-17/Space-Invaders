using System;
using UnityEngine;
using static UnityEngine.Camera;
using Random = UnityEngine.Random;

public class Invaders : MonoBehaviour
{
    public int rows = 5;
    public int columns = 11;
    
    public Invader[] prefabs;
    
    public float distanceBetweenInvaders = 2;
    
    public Projectile missilePrefab;
    
    public float missileAttackTime = 1;
    
    public AnimationCurve speed;

    public Action<Invader> Killed;

    public Action TouchedBottom;

    public bool active = true;

    private Vector3 _initialPosition;
    
    private Vector3 _direction = Vector3.right;
    
    private Camera _camera;
    
    private Vector3 _leftEdge;
    private Vector3 _rightEdge;

    private int TotalInvaders => rows * columns; 
    
    private int _amountKilled = 0;
    
    private float PercentKilled => (float)_amountKilled / (float)TotalInvaders;
    
    public int AmountAlive => TotalInvaders - _amountKilled;
    
    private void Awake()
    {
        _initialPosition = transform.position;
        
        _camera = main;
        
        _leftEdge = _camera.ViewportToWorldPoint(Vector3.zero);
        _rightEdge = _camera.ViewportToWorldPoint(Vector3.right);
        
        for (var row = 0; row < rows; row++)
        {
            var width = distanceBetweenInvaders * (columns - 1.0f);
            var height = distanceBetweenInvaders * (rows - 1.0f);
            
            var rowPosition = new Vector3(0, row * distanceBetweenInvaders, 0);
            var centering = new Vector3(-width / 2.0f, -height / 2.0f, 0);  
            
            for (var column = 0; column < columns; column++)
            {
                var invader = Instantiate(prefabs[row], transform);
                
                invader.WhenKilled += OnInvaderKilled;
                invader.InvaderTouchedBottom += OnInvaderTouchedBottom;
                
                var position = rowPosition + centering;
                position.x += column * distanceBetweenInvaders;
                invader.transform.localPosition = position;
            }
        }
    }

    private void OnInvaderTouchedBottom()
    {
        TouchedBottom.Invoke();
    }

    private void OnInvaderKilled(Invader invader)
    {
        invader.gameObject.SetActive(false);
        
        _amountKilled++;
        
        Killed(invader);
    }
    
    private void Start()
    {
        InvokeRepeating(nameof(MissileAttack), missileAttackTime, missileAttackTime);
    }

    private void MissileAttack()
    {
        if (!active) return;
        
        foreach (Transform invader in transform)
        {
            if (!invader.gameObject.activeInHierarchy) continue;

            if (Random.value < (1.0f / AmountAlive))
            {
                Instantiate(missilePrefab, invader.position, Quaternion.identity);
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!active) return;
        
        transform.position += _direction * (speed.Evaluate(PercentKilled) * Time.deltaTime);
        
        foreach (Transform invader in transform)
        {
            if (!invader.gameObject.activeInHierarchy) continue;

            if (_direction == Vector3.right && invader.position.x >= _rightEdge.x - 1.0f)
            {
                AdvanceRow();
                break;
            }
            if (_direction == Vector3.left && invader.position.x <= _leftEdge.x + 1.0f)
            {
                AdvanceRow();
                break;
            }
        }
    }
    
    private void AdvanceRow()
    {
        _direction = _direction == Vector3.right ? Vector3.left : Vector3.right;
        
        transform.position += Vector3.down;
    }

    public void ResetInvaders()
    {
        _amountKilled = 0;
        
        _direction = Vector3.right;
        
        transform.position = _initialPosition;

        foreach (Transform invader in transform) {
            invader.gameObject.SetActive(true);
        }
    }
}
