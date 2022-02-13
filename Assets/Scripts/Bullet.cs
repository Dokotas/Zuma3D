using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameManager _gameManager;

    [HideInInspector] public int colorIndex;
    private bool _hit;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ball") && !_hit)
        {
            var ball = other.collider.GetComponent<Ball>();
            if(_gameManager.balls[ball.index+1])
                _gameManager.PopBall(ball.index, colorIndex);
            else 
                _gameManager.InsertBall(ball.index+1, colorIndex);
            Destroy(gameObject);
            _hit = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
            Destroy(gameObject);
    }
}