using System;
using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private GameManager _gameManager;

    public int index;
    [HideInInspector] public int colorIndex ;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    public void Move()
    {
        transform.position = _gameManager.path.GetPointAtDistance(++index);
    }
}