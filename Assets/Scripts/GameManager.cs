using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Material[] ballsColors;
    [SerializeField] private GameObject ballPrefab;
    [HideInInspector] public GameObject[] balls;
    [HideInInspector] public VertexPath path;

    [SerializeField] private int countOfStartBalls;
    [SerializeField] private float spawnBallDeltaTime;
    [HideInInspector] public int length;


    private void Start()
    {
        path = FindObjectOfType<PathCreator>().path;
        length = (int) path.length;
        balls = new GameObject[length];

        StartCoroutine(Game());
    }


    private int EndOfSequence(int startIndex)
    {
        int end = 0;

        for (int i = startIndex; i < length; i++)
        {
            if (!balls[i])
            {
                end = i;
                break;
            }
        }

        return end;
    }

    private void CreateBall(int index, int colorIndex)
    {
        balls[index] = Instantiate(ballPrefab, path.GetPointAtDistance(index), Quaternion.identity);
        balls[index].GetComponent<Renderer>().material = ballsColors[colorIndex];
        var newBall = balls[index].GetComponent<Ball>();
        newBall.index = index;
        newBall.colorIndex = colorIndex;
    }

    public void RemoveSameBalls(int index)
    {
        List<GameObject> sameBalls = new List<GameObject>();
        int colorIndex = balls[index].GetComponent<Ball>().colorIndex;

        for (int i = index; i >= 0; i--)
        {
            if (!balls[i])
                break;

            if (balls[i].GetComponent<Ball>().colorIndex != colorIndex)
            {
                break;
            }

            sameBalls.Add(balls[i]);
        }

        for (int i = index + 1; i < length; i++)
        {
            if (!balls[i])
                break;

            if (balls[i].GetComponent<Ball>().colorIndex != colorIndex)
                break;

            sameBalls.Add(balls[i]);
        }

        if (sameBalls.Count > 2)
        {
            foreach (var ball in sameBalls)
                Destroy(ball);
        }
    }

    public void PopBall(int index, int colorIndex)
    {
        MoveBalls(index, EndOfSequence(index));
        CreateBall(index, colorIndex);
        RemoveSameBalls(index);
    }

    private void MoveBalls(int startIndex, int endIndex)
    {
        var t = balls[endIndex];
        for (int i = endIndex; i > startIndex; i--)
        {
            balls[i] = balls[i - 1];

            if (balls[i])
            {
                balls[i].GetComponent<Ball>().Move();
            }
        }
    }

    private IEnumerator Game()
    {
        var spawnLag = .1f;
        for (int i = 0; i < countOfStartBalls; i++)
        {
            MoveBalls(0, EndOfSequence(0));
            yield return new WaitForSeconds(.001f);
            CreateBall(0, Random.Range(0, ballsColors.Length));
            yield return new WaitForSeconds(.1f * .009f);
        }

        while (true)
        {
            if (balls[length - 1])
            {
                Lose();
                break;
            }

            var endIndex = EndOfSequence(0);

            MoveBalls(0, endIndex);
            yield return new WaitForSeconds(.1f);
            if (balls[endIndex+1])
                RemoveSameBalls(endIndex);
            
            yield return new WaitForSeconds(spawnLag);
            CreateBall(0, Random.Range(0, ballsColors.Length));
            yield return new WaitForSeconds(spawnBallDeltaTime - spawnLag);
        }
    }

    private void Lose()
    {
        Debug.Log("You lose");
        //Time.timeScale = 0;
    }
}