using System.Collections;
using System.Collections.Generic;
using PathCreation;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Material[] ballsColors;
    public Slider sensitivitySlider;
    [SerializeField] private GameObject ballPrefab, endMenu, endLabel;
    [HideInInspector] public GameObject[] balls;
    [HideInInspector] public VertexPath path;

    [SerializeField] private int countOfStartBalls, maxAmountOfBalls;
    [SerializeField] private float secondsPerFrame;
    [HideInInspector] public int length;
    [HideInInspector] public bool pause;
    int amountOfBalls;


    private void Start()
    {
        path = FindObjectOfType<PathCreator>().path;
        length = (int) path.length;
        balls = new GameObject[length];

        StartCoroutine(Game());
    }

    private int StartOfSequence()
    {
        int i;
        for (i = 0; i < length; i++)
        {
            if (balls[i])
                return i;
        }

        return 0;
    }

    private int EndOfSequence(int startIndex)
    {
        int i;
        for (i = startIndex; i < length; i++)
        {
            if (!balls[i])
                return i;
        }

        return 0;
    }

    private void CreateBall(int index, int colorIndex)
    {
        balls[index] = Instantiate(ballPrefab, path.GetPointAtDistance(index), Quaternion.identity);
        balls[index].GetComponent<Renderer>().material = ballsColors[colorIndex];
        var newBall = balls[index].GetComponent<Ball>();
        newBall.index = index;
        newBall.colorIndex = colorIndex;
        amountOfBalls++;
    }

    public void RemoveSameBalls(int index)
    {
        var sameBalls = new List<GameObject>();
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
            {
                balls[ball.GetComponent<Ball>().index] = null;
                Destroy(ball);
            }
        }

        if (!AnyBalls())
            EndGame("You Win");
    }

    public void PopBall(int index, int colorIndex)
    {
        MoveBalls(index, EndOfSequence(index));
        CreateBall(index, colorIndex);
        RemoveSameBalls(index);
    }

    public void InsertBall(int index, int colorIndex)
    {
        CreateBall(index, colorIndex);
        RemoveSameBalls(index);
    }

    private void MoveBalls(int startIndex, int endIndex)
    {
        for (int i = endIndex; i > startIndex; i--)
        {
            balls[i] = balls[i - 1];
            balls[i].GetComponent<Ball>().Move();
        }

        balls[startIndex] = null;
    }

    private bool AnyBalls()
    {
        for (int i = 0; i < length; i++)
        {
            if (balls[i])
                return true;
        }

        return false;
    }

    private IEnumerator Game()
    {
        var spawnLag = .1f;
        for (int i = 0; i < countOfStartBalls; i++)
        {
            CreateBall(0, Random.Range(0, ballsColors.Length));
            yield return new WaitForSeconds(.1f * .009f);
            MoveBalls(0, EndOfSequence(0));
            yield return new WaitForSeconds(.001f);
        }

        while (!balls[length - 2])
        {
            if (amountOfBalls < maxAmountOfBalls)
                CreateBall(0, Random.Range(0, ballsColors.Length));

            yield return new WaitForSeconds(spawnLag / 2);

            var startIndex = StartOfSequence();
            var endIndex = EndOfSequence(startIndex);

            MoveBalls(startIndex, endIndex);
            if (balls[endIndex + 1])
                if (balls[endIndex].GetComponent<Ball>().colorIndex ==
                    balls[endIndex + 1].GetComponent<Ball>().colorIndex)
                    RemoveSameBalls(endIndex);

            yield return new WaitForSeconds(spawnLag / 2);


            yield return new WaitForSeconds(secondsPerFrame - spawnLag);
        }

        EndGame("You lose");
    }

    private void EndGame(string endText)
    {
        SetPause(true);
        
        endLabel.GetComponent<TextMeshProUGUI>().text = endText;
        endMenu.SetActive(true);
    }

    public void RestartGame()
    {
        SetPause(false);
        
        SceneManager.LoadScene("Game");
    }

    public void SetPause(bool isPause)
    {
        pause = isPause;
        Time.timeScale = pause ? 0f : 1f;
    }
}