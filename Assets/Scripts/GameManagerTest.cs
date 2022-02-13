using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerTest : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameObject[] balls;

    public int lenth = 32;

    private void Start()
    {
        balls = new GameObject[lenth];
    }

    private int StartOfSequnce()
    {
        for (int i = 0; i < lenth; i++)
        {
            if (balls[i])
                return i;
        }

        return 0;
    }

    private int EndOfSequnce(int start)
    {
        for (int i = start; i < lenth; i++)
        {
            if (!balls[i])
                return i;
        }

        return 0;
    }

    private IEnumerator Game()
    {
        while (!balls[lenth - 2])
        {
            var start = StartOfSequnce();
            var end = EndOfSequnce(start);
            for (int i = end; i > start; i--)
            {
                balls[i] = balls[i - 1];
            }

            Destroy(balls[start]);
            yield return new WaitForSeconds(1f);
        }
    }
}