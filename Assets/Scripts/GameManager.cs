using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text livesText;

    public TMP_Text gameOverScreen;

    public int score = 0;
    public int lives = 3;
    
    private Player _player;
    private Invaders _invaders;
    private MysteryShip _mysteryShip;
    private Bunker[] _bunkers;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _invaders = FindObjectOfType<Invaders>();
        _mysteryShip = FindObjectOfType<MysteryShip>();
        _bunkers = FindObjectsOfType<Bunker>();
    }
    
    private void Start()
    {
        gameOverScreen.gameObject.SetActive(false);
        
        _player.HitInvader += GameOver;
        _player.HitMissile += OnPlayerHitMissile;
        _invaders.Killed += OnInvaderKilled;
        _invaders.TouchedBottom += GameOver;
        _mysteryShip.WhenKilled += OnMysteryShipKilled;
        _player.PicUpPowerUp += OnPlayerPickUpPowerUp;

        NewGame();
    }

    public void Update()
    {
        if (lives <= 0 && Input.GetKey(KeyCode.Return))
        {
            NewGame();
        }
    }

    private void IncreaceLives(int x)
    {
        lives = Math.Max(lives + x, 0); 
        livesText.text = lives.ToString();
    }

    private void ResetLives(int lives = 0)
    {
        this.lives = lives;
        livesText.text = this.lives.ToString();
    }
    
    private void AddScore(int score)
    {
        this.score += score;
        scoreText.text = this.score.ToString();
    }

    private void ResetScore()
    {
        score = 0;
        scoreText.text = score.ToString();
    }
    
    private void ResetPlayer()
    {
        var position = _player.transform.position;
        position.x = 0f;
        _player.transform.position = position;
        
        _player.gameObject.SetActive(true);
        _invaders.gameObject.SetActive(true);
    }

    private void OnMysteryShipKilled()
    {
        AddScore(MysteryShip.Score);
    }

    private void OnInvaderKilled(Invader invader)
    {
        AddScore(invader.score);

        if (_invaders.AmountAlive > 0) return;
        
        _invaders.gameObject.SetActive(false);
        _player.gameObject.SetActive(false);
        
        Invoke(nameof(NewRound), 2);
    }
    
    private void OnPlayerHitMissile()
    {
        IncreaceLives(-1);
        
        if (lives <= 0)
        {
            GameOver();
            return;
        }
        
        _player.gameObject.SetActive(false);
        _invaders.gameObject.SetActive(false);
        
        Invoke(nameof(ResetPlayer), 2);
    }

    private void OnPlayerPickUpPowerUp()
    {
        IncreaceLives(1);
    }

    private void NewRound()
    {
        _invaders.ResetInvaders();
        _invaders.gameObject.SetActive(true);

        foreach (var bunker in _bunkers)
        {
            bunker.gameObject.SetActive(true);
        }
        _mysteryShip.Stop();
        _mysteryShip.Start();
        
        ResetPlayer();
    }
    
    private void GameOver()
    {
        ResetLives();
        
        gameOverScreen.gameObject.SetActive(true);
        _invaders.gameObject.SetActive(false);
        _player.gameObject.SetActive(false);
        
        _mysteryShip.Stop();
        _mysteryShip.gameObject.SetActive(false);
    }
    
    private void NewGame()
    {
        gameOverScreen.gameObject.SetActive(false);
        
        ResetScore();
        ResetLives(3);
        
        NewRound();
    }
}
