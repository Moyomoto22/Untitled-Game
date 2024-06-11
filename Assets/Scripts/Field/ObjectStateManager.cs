using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ドアなどオブジェクトの状態がシーンをまたいでも変わらないよう管理する
/// </summary>
public class ObjectStateManager : MonoBehaviour
{
    public static ObjectStateManager Instance { get; private set; }

    private Dictionary<string, bool> objStates = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetState(string key, bool isOpened)
    {
        objStates[key] = isOpened;
    }

    public bool GetDoorState(string key)
    {
        return objStates.ContainsKey(key) && objStates[key];
    }

    public void ResetDoorStates()
    {
        objStates.Clear();
    }
}
