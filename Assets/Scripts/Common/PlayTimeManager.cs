using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTimeManager : MonoBehaviour
{
    // �V���O���g��
    public static PlayTimeManager Instance { get; private set; }

    private double startTime;
    private double playTime;

    private void Awake()
    {
        // �V���O���g���C���X�^���X�̐ݒ�
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �V�[�����ׂ��ŃI�u�W�F�N�g��ێ�
            startTime = Time.realtimeSinceStartup;
        }
        else
        {
            Destroy(gameObject); // �d������C���X�^���X��j��
        }
    }

    private void Update()
    {
        // �v���C���Ԃ��X�V
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
    /// �Z�[�u�f�[�^����v���C���Ԃ𕜌�
    /// </summary>
    /// <param name="loadedPlayTime"></param>
    public void Resume(TimeSpan loadedPlayTime)
    {
        playTime = loadedPlayTime.TotalSeconds;
        startTime = Time.realtimeSinceStartup - playTime;
    }

    /// <summary>
    /// �v���C���Ԃ�hh:mm:ss�̃t�H�[�}�b�g�ŕԂ�
    /// </summary>
    /// <returns></returns>
    public string FormatPlayTime()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(playTime);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}
