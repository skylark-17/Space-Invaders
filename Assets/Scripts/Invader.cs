using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Invader : MonoBehaviour
{
    public Sprite[] animationSprites;

    public int score = 10;
    
    public float animationTime = 1;

    public Projectile powerUpPrefab;

    public float powerUpChance = 0.1f;

    public Action<Invader> WhenKilled;

    public Action InvaderTouchedBottom;

    private SpriteRenderer _spriteRenderer;
    
    private int _animationFrame;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void ChangeAnimationSprite()
    {
        _animationFrame++;

        if (_animationFrame >= animationSprites.Length)
        {
            _animationFrame = 0;
        }

        _spriteRenderer.sprite = animationSprites[_animationFrame];
    }

    private void Start()
    {
        InvokeRepeating(nameof(ChangeAnimationSprite),  animationTime, animationTime); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            if (Random.value < powerUpChance)
            {
                Instantiate(powerUpPrefab, gameObject.transform.position, Quaternion.identity);
            }

            WhenKilled?.Invoke(this);
            
            gameObject.SetActive(false);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Border"))
        {
            InvaderTouchedBottom?.Invoke();
            
            gameObject.SetActive(false);
        }
    }
}
