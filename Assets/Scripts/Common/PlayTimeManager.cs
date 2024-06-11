using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTimeManager : MonoBehaviour
{
    // シングルトン
    public static PlayTimeManager Instance { get; private set; }

    private double startTime;
    private double playTime;

    private void Awake()
    {
        // シングルトンインスタンスの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いでオブジェクトを保持
            startTime = Time.realtimeSinceStartup;
        }
        else
        {
            Destroy(gameObject); // 重複するインスタンスを破棄
        }
    }

    private void Update()
    {
        // プレイ時間を更新
        if (Instance == this)
        {
            playTime = Time.realtimeSinceStartup - startTime;
        }
    }

    public TimeSpan GetPlayTimeForTimeSpan()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(playTime);
        return timeSpan;
    }

    /// <summary>
    /// セーブデータからプレイ時間を復元
    /// </summary>
    /// <param name="loadedPlayTime"></param>
    public void Resume(TimeSpan loadedPlayTime)
    {
        playTime = loadedPlayTime.TotalSeconds;
        startTime = Time.realtimeSinceStartup - playTime;
    }

    /// <summary>
    /// プレイ時間をhh:mm:ssのフォーマットで返す
    /// </summary>
    /// <returns></returns>
    public string FormatPlayTime()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(playTime);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}
