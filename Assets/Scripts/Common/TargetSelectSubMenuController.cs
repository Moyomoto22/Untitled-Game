using SpriteGlow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// �퓬��� - �ΏۑI���T�u���j���[�R���g���[��
/// </summary>
public class TargetSelectSubMenuController : MonoBehaviour
{
    public GameObject frame;
    public GameObject header;
    public Image image;
    public TextMeshProUGUI skillName;
    public GameObject buttonPrefab;

    public int numberOfButtons;
    public float spacing;

    private EventSystem eventSystems;

    public List<GameObject> createdButtons;

    // Start is called before the first frame update
    void Start()
    {
        //CreateSubMenuSingleRow();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        eventSystems = FindObjectOfType<EventSystem>();
    }

    /// <summary>
    /// 
    /// </summary>
    public void CreateSubMenuSingleRow(int numberOfButtons)
    {
        RectTransform frameRect = frame.GetComponent<RectTransform>();
        RectTransform headerRect = header.GetComponent<RectTransform>();

        float offset = 20.0f;
        float buttonHeight = buttonPrefab.GetComponent<RectTransform>().sizeDelta.y;
        float frameHeight = buttonHeight * numberOfButtons + spacing * (numberOfButtons - 1) + offset * 2; // �X�y�[�X��ǉ�
        frameRect.sizeDelta = new Vector2(frame.GetComponent<RectTransform>().sizeDelta.x, frameHeight);

        // �ŏ��̃{�^����Y���W���v�Z�B�t���[���̒��S����̑��Έʒu�Ƃ��Đݒ�B
        float firstButtonY = frameHeight / 2.0f - buttonHeight / 2.0f - offset;

        createdButtons = new List<GameObject>();

        for (int i = 0; i < numberOfButtons; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, frame.transform, false);
            // �e�{�^����Y���W���v�Z�B�ŏ��̃{�^�����珇�ɃX�y�[�X���J���Ĕz�u�B
            float buttonY = firstButtonY - i * (buttonHeight + spacing);
            Vector3 newPosition = new Vector3(0, buttonY, 0);
            newButton.GetComponent<RectTransform>().anchoredPosition = newPosition;

            createdButtons.Add(newButton);
        }

        float frameTop = frameRect.anchoredPosition.y + frameRect.rect.height / 2;
        float headerHeight = headerRect.rect.height;

        // objectA�̒�ӂ�objectB�̏�ӂɃX�y�[�X���󂯂Ĕz�u
        headerRect.anchoredPosition = new Vector2(headerRect.anchoredPosition.x, frameTop + headerHeight / 2 + 1.0f);
    }

    public void SetHeader(Skill skill, Item item)
    {
        var icon = skill != null ? skill.icon : item.iconImage;
        var name = skill != null ? skill.skillName : item.itemName;
        
        image.sprite = icon;
        skillName.text = name;
    }

    public void SetButtonTitles(List<string> titles)
    {
        if (titles.Count == createdButtons.Count)
        {
            for (int i = 0; i < titles.Count; i++)
            {
                createdButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = titles[i];
            }
        }
    }

    public void SelectButton(int number = 0)
    {
        if (eventSystems != null && createdButtons.Count > 0)
        {
            var buttonToSelect = createdButtons[number].transform.GetChild(0).gameObject;
            Debug.Log(eventSystems.name);
            eventSystems.SetSelectedGameObject(buttonToSelect);
            Debug.Log("select " + createdButtons[number].GetComponentInChildren<TextMeshProUGUI>().text);
        }
    }

    public void AddSelectOrDeselectActionToButtonsSelectEnemy(bool isOnSelect)
    {
        BattleController battleController = FindObjectOfType<BattleController>();
        if (battleController != null)
        {
            for (int i = 0; i < createdButtons.Count; i++)
            {
                // �퓬�s�\�łȂ��G�l�~�[�̃��X�g���擾
                var aliveEnemies = EnemyManager.Instance.GetEnemiesInsExceptKnockedOut();
                // �퓬�s�\�łȂ��G�l�~�[�̃��X�g�̃o�g���������p�C���f�b�N�X���擾
                var index = aliveEnemies[i].GetComponent<EnemyBehaviour>().indexInBattle;

                GameObject button = createdButtons[i].transform.GetChild(0).gameObject;
                EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.AddComponent<EventTrigger>();

                EventTrigger.Entry entry = new EventTrigger.Entry();

                if (isOnSelect)
                {
                    entry.eventID = EventTriggerType.Select; // Select�C�x���g�����b�X��
                    entry.callback.AddListener((data) =>
                    {
                        // ��Ŏ擾�����C���f�b�N�X�������ɓn��
                        battleController.StartFlashingGlowEffect(index).Forget();
                    });
                }
                else
                {
                    entry.eventID = EventTriggerType.Deselect; // Deselect�C�x���g�����b�X��
                    entry.callback.AddListener((data) =>
                    {
                        // ��Ŏ擾�����C���f�b�N�X�������ɓn��
                        battleController.StopFlashingGlowEffect(index);
                    });
                }
                // �G���g�����g���K�[���X�g�ɒǉ�
                trigger.triggers.Add(entry);
            }
        }
    }
}
