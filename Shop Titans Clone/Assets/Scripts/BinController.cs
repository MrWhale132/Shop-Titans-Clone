using System;
using System.Collections.Generic;
using UnityEngine;

public class BinController : MonoBehaviour
{
    static BinController instance;

    Queue<(Action<int> callback, int newLevel)> setFulnesses = new Queue<(Action<int> callback, int newLevel)>();


    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        while (setFulnesses.Count > 0)
        {
            setFulnesses.Peek().callback(setFulnesses.Dequeue().newLevel);
        }
    }


    public static void EnqueueSetFulness(Action<int> setFulness, int newLevel)
    {
        instance.setFulnesses.Enqueue((setFulness, newLevel));
    }
}