using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �Q�[�W�ފǗ��N���X
/// </summary>
public class GaugeManager : MonoBehaviour
{
    //private int maxValue;
    //private int currentValue;
    public bool isUpdateByText = false;

    public TextMeshProUGUI maxValueText;
    public TextMeshProUGUI currentValueText;

    // �Q�[�W�X�v���C�g(UI)
    public Image gaugeSprite;
    // �Q�[�W(�X�v���C�g�}�e���A�� - �G�p)
    public SpriteRenderer renderer;

    // Start is called before the first frame update
    private void Start()
    {
        //gaugeImage = gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        if (isUpdateByText)
        {
            updateGaugeByText();
        }
    }

    private void Awake()
    {
        updateGaugeByText();
    }

    public void updateGauge(int maxValue, int currentValue)
    {
        float per = (float)currentValue / (float)maxValue;

        if (gaugeSprite != null)
        {
            gaugeSprite.fillAmount = per;
        }
    }

    public void updateGaugeByText()
    {
        if (maxValueText != null && currentValueText != null)
        {
            int maxValue = int.Parse(maxValueText.text);
            int currentValue = int.Parse(currentValueText.text);
            float per = (float)currentValue / (float)maxValue;

            if (gaugeSprite != null)
            {
                gaugeSprite.fillAmount = per;
            }
        }
    }

    /// <summary>
    /// �e�L�X�g�ƃQ�[�W�𓯎��ɃA�j���[�V����������
    /// </summary>
    /// <param name="targetValue"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask AnimateTextAndGauge(int targetValue, float duration = 1.0f)
    {
        // �L���X�g���s�A�[�����Z��
        if (!int.TryParse(maxValueText.text, out var maxValue) || maxValue <= 0)
        {
            return;
        }

        float targetFillAmount = (float)targetValue / maxValue;

        await UniTask.WhenAll(
            CountText(targetValue, duration),
            AnimateGauge(targetFillAmount, duration)
            );
    }

    /// <summary>
    /// �e�L�X�g���w��̒l�܂ŃJ�E���g�A�b�v�E�_�E��������
    /// </summary>
    /// <param name="targetValue">�J�E���g������ڕW�l</param>
    /// <param name="duration">�J�E���g�����鎞�ԊԊu</param>
    /// <returns></returns>
    public async UniTask CountText(int targetValue, float duration = 1.0f)
    {
        if (!int.TryParse(currentValueText.text, out var currentValue))
        {
            return;
        }
        await DOTween.To(
            () => currentValue,                        // �J�n�l
            x => currentValueText.text = x.ToString(), // �e�L�X�g�X�V�̃A�N�V����
            targetValue,                            �@ // �ڕW�l
            duration                                �@ // �p������
        ).SetEase(Ease.OutQuad).SetUpdate(true);                       // �A�j���[�V�����̃C�[�W���O�ݒ�
    }

    /// <summary>
    /// �Q�[�W�摜��FillAmount���w��̒l�܂ŃA�j���[�V����������
    /// </summary>
    /// <param name="targetFillAmount">�ڕWFillAmount</param>
    /// <param name="duration">���ԊԊu</param>
    /// <returns></returns>
    public async UniTask AnimateGauge(float targetFillAmount, float duration = 1.0f)
    {
        await gaugeSprite.DOFillAmount(targetFillAmount, duration).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    /// <summary>
    /// �G�p�Q�[�W���A�j���[�V����������
    /// </summary>
    /// <param name="targetFillAmount"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask AnimateGaugeRenderer(float targetFillAmount, float duration = 1.0f)
    {
        Material material = renderer.material;

        if (material != null)
        {
            float currentAmount = material.GetFloat("_FillAmount");
            await DOTween.To(() => material.GetFloat("_FillAmount"), x => material.SetFloat("_FillAmount", x), targetFillAmount, duration);
        }
    }
}
