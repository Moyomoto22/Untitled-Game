using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SaveDataSlotController : MonoBehaviour
{
    public int slot;
    public bool isSaving = true;
    
    public List<Image> faceImages;

    public List<TextMeshProUGUI> classes;
    public List<TextMeshProUGUI> levels;

    public TextMeshProUGUI dateTime;
    public TextMeshProUGUI playTime;
    public TextMeshProUGUI location;

    SaveDataSlotController(int slot)
    {
        this.slot = slot;
    }

    private void Awake()
    {
        SetInfo();
    }

    private void SetInfo()
    {
        print(Application.persistentDataPath);

        PlayerData playerData = SaveManager.Instance.GetPlayerData(slot);

        if (playerData != null)
        {
            for (int i = 0; i < 4; i++)
            {
                faceImages[i].enabled = true;
                faceImages[i].sprite = ClassManager.Instance.AllClasses[i].imagesC[i];

                Class cl = ClassManager.Instance.GetClassByID(playerData.partyMembers[i].classID);
                int classIndex = ClassManager.Instance.AllClasses.IndexOf(cl);

                classes[i].text = cl.classAbbreviation;
                levels[i].text = "Lv" + playerData.partyMembers[i].classLevels[classIndex].ToString();
            }
            location.text = playerData.location;
            dateTime.text = playerData.saveDateTime.ToString("yyyy/MM/dd HH:mm:ss"); ;
            playTime.text = FormatPlayTime(playerData.playTime);
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                faceImages[i].enabled = false;
                classes[i].text = "";
                levels[i].text = "";
            }
            location.text = "-";
            dateTime.text = "-";
            playTime.text = "-";
        }
    }

    /// <summary>
    /// プレイ時間をhh:mm:ssのフォーマットで返す
    /// </summary>
    /// <returns></returns>
    private string FormatPlayTime(TimeSpan timeSpan)
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }

    public async void OnPressSaveSlotButton()
    {
        if (isSaving)
        {
            SaveManager.Instance.SaveGame(slot);
            SetInfo();
        }
        else 
        {
            SaveManager.Instance.LoadGame(slot);
        }
    }
}
