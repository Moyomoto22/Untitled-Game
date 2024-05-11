using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

/// <summary>
/// �e�L�X�g�A�j���[�V�����Ǘ��N���X
/// </summary>
public class TextAnimationManager : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public List<GameObject> objects;

    private List<TextMeshProUGUI> TMPros;

    public float duration = 0.2f;                        // �A�j���[�V�����̑�����
    public Vector3 startScale = new Vector3(4f, 4f, 4f); // �J�n���̃X�P�[��
    public float endScale = 1.0f;                        // �I�����̃X�P�[��

    public string text;

    void Start()
    {
        gameObject.SetActive(true);
    }

    private async void Awake()
    {
        TMPros = objects.Select(obj => obj.GetComponent<TextMeshProUGUI>()).ToList();
    }

    /// <summary>
    /// �_���[�W�e�L�X�g�A�j���[�V��������
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damageStrings"></param>
    /// <returns></returns>
    public async UniTask ShowDamageTextAnimation(CharacterStatus target, List<string> damageStrings, SpriteManipulator manip)
    {
        if (damageStrings.Count > 0)
        {
            // �퓬��ʂ̃L�����o�X���擾
            Canvas canvas = GameObject.Find("BattleSceneCanvas").GetComponent<Canvas>();
            // ���̃e�L�X�g�\���܂ł̃f�B���C
            float delay = 0.5f / damageStrings.Count;

            for (int i = 0; i < damageStrings.Count; i++)
            {
                GameObject at = Instantiate(gameObject, canvas.transform);
                RectTransform rectTransform = at.GetComponent<RectTransform>();

                // ���O�Ɏ擾�������W���L�����o�X��ł̍��W�ɕϊ�
                rectTransform.anchoredPosition = target.positionInScreen;
                rectTransform.localRotation = Quaternion.identity;

                if (rectTransform != null)
                {
                    // 2���[�v�ڈȍ~�A�㉺���E�Ƀ����_���ɕ\���ʒu�����炷
                    System.Random random = new System.Random();
                    var randomOffsetX = 0.0f;
                    var randomOffsetY = 0.0f;

                    if (i > 0)
                    {
                        randomOffsetX = (float)random.NextDouble() * 100.0f - 50.0f;
                        randomOffsetY = (float)random.NextDouble() * 100.0f - 50.0f;
                    }

                    // RectTransform�̈ʒu��ݒ�
                    rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + randomOffsetX, rectTransform.anchoredPosition.y + randomOffsetY);
                }
                // �A�j���[�V�����J�n
                _ = UniTask.WhenAll(
                    at.GetComponent<TextAnimationManager>().RevealDamageTextAnimation(damageStrings[i]),
                    manip.Shake(0.2f, 0.1f),
                    manip.Flash(0.2f, Color.red));

                // �����[�v�܂Ńf�B���C
                await UniTask.Delay(TimeSpan.FromSeconds(delay));
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        }
    }

    /// <summary>
    /// �񕜃e�L�X�g�A�j���[�V��������
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damageStrings"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public async UniTask ShowHealTextAnimation(CharacterStatus target, List<string> damageStrings, Color color)
    {
        GameObject canvasObject = GameObject.Find("BattleSceneCanvas");  
        if (canvasObject != null)
        {
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            if (damageStrings.Count > 0 && canvas != null)
            {
                float delay = 0.1f / damageStrings.Count;

                for (int i = 0; i < damageStrings.Count; i++)
                {
                    GameObject at = Instantiate(gameObject, canvas.transform);
                    RectTransform rectTransform = at.GetComponent<RectTransform>();

                    // ���O�Ɏ擾�������W���L�����o�X��ł̍��W�ɕϊ�
                    rectTransform.anchoredPosition = target.positionInScreen;
                    rectTransform.localRotation = Quaternion.identity;

                    if (rectTransform != null)
                    {
                        // �㉺���E�Ƀ����_���ɕ\���ʒu�����炷
                        System.Random random = new System.Random();
                        var randomOffsetX = 0.0f;
                        var randomOffsetY = 0.0f;

                        if (i > 0)
                        {
                            randomOffsetX = (float)random.NextDouble() * 100.0f - 50.0f;
                            randomOffsetY = (float)random.NextDouble() * 100.0f - 50.0f;
                        }
                        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + randomOffsetX, rectTransform.anchoredPosition.y + randomOffsetY);
                    }
                    var ta = at.GetComponent<TextAnimationManager>();
                    if (ta != null)
                    {
                        await ta.RevealHealTextAnimation(damageStrings[i], color);
                    }
                    await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                }
            }
        }
    }

    /// <summary>
    /// �e�L�X�g�A�j���[�V�����\���J�n
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public async UniTask RevealDamageTextAnimation(string text)
    {
        System.Random random = new System.Random();
        var array = TextToArray(text);

        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(true); // �I�u�W�F�N�g���A�N�e�B�u��

            var textMesh = objects[i].GetComponent<TextMeshProUGUI>();
            textMesh.text = array[i];
            var rectTransform = objects[i].GetComponent<RectTransform>();

            // �����_���ȉ�]�A�I�t�Z�b�g�A�X�P�[���𐶐�
            var randomRotationZ = (float)random.NextDouble() * 20.0f - 10.0f;
            var randomOffsetX = (float)random.NextDouble() * 10.0f - 5.0f;
            var randomOffsetY = (float)random.NextDouble() * 10.0f - 5.0f;
            var randomScale = (float)random.NextDouble() * 1.5f + 1.0f;

            if (textMesh != null)
            {
                if (rectTransform != null)
                {
                    // �����_���Ƀe�L�X�g�ό`
                    rectTransform.rotation = Quaternion.Euler(0, 0, randomRotationZ);
                    rectTransform.position = new Vector3(rectTransform.position.x + randomOffsetX, rectTransform.position.y + randomOffsetY, 0);
                    rectTransform.localScale = new Vector3(randomScale, randomScale, 1);
                }

                textMesh.transform.localScale = startScale; // �����X�P�[���ݒ�
                await textMesh.DOFade(1f, 0.05f).From(0f); // �t�F�[�h�C��
                await textMesh.transform.DOScale(Vector3.one * randomScale, 0.05f); // �X�P�[���̃A�j���[�V����
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        }
        // �t�F�[�h�A�E�g
        await FadeOutAllTexts();
        Destroy(gameObject);
    }

    /// <summary>
    /// �e�L�X�g�A�j���[�V�����\���J�n
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public async UniTask RevealHealTextAnimation(string text, Color color)
    {
        var array = TextToArray(text);

        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(true); // �I�u�W�F�N�g���A�N�e�B�u��

            var textMesh = objects[i].GetComponent<TextMeshProUGUI>();
            textMesh.text = array[i];
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1.5f, 1.5f, 1);

            if (textMesh != null)
            {
                textMesh.color = color;  // �e�L�X�g�̐F��ύX
                _ = textMesh.DOFade(1f, 0.05f).From(0f); // �t�F�[�h�C��
                _ = textMesh.rectTransform.DOAnchorPosY(-20f, 0.05f).SetEase(Ease.InOutQuad); // �ォ�痎��
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        await FadeOutAllTexts();
    }

    /// <summary>
    /// ��������ꕶ���Â̔z��ɕϊ�����
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private string[] TextToArray(string text)
    {
        string[] array = new string[5];

        // ������̊e������z��Ɋi�[
        for (int i = 0; i < text.Length; i++)
        {
            array[i + 1] = text[i].ToString();
        }

        // �z��̖��g�p�������󕶎��Ŗ��߂�
        for (int i = text.Length + 1; i < array.Length; i++)
        {
            array[i] = "";
        }
        return array;
    }

    /// <summary>
    /// �e�L�X�g��S�ăt�F�[�h�A�E�g������
    /// </summary>
    /// <returns></returns>
    private async UniTask FadeOutAllTexts()
    {
        // �e�e�L�X�g�R���|�[�l���g��DOFade��K�p���A���ꂼ���UniTask�����X�g�Ɋi�[
        var fadingTasks = new UniTask[TMPros.Count];
        for (int i = 0; i < TMPros.Count; i++)
        {
            var text = TMPros[i];
            fadingTasks[i] = FadeText(text);
        }

        await UniTask.WhenAll(fadingTasks);
    }

    /// <summary>
    /// �e�L�X�g���t�F�[�h�A�E�g������
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    UniTask FadeText(TextMeshProUGUI text)
    {
        // DOTween�̃A�j���[�V������UniTask�Ƃ��ĕԂ�
        return text.DOFade(0, 0.2f).ToUniTask();
    }
}
