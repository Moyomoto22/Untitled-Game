using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using HighlightPlus;

public class SpriteManipulator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Image image;

    private HighlightEffect hightlightEffect;
    private bool isGlowing = false;

    private void Awake()
    {
        hightlightEffect = gameObject.GetComponent<HighlightEffect>();
    }

    private void OnDestroy()
    {
        isGlowing = false; // �Q�[���I�u�W�F�N�g���j�������ۂɃ��[�v���~
    }

    private void OnDisable()
    {
        isGlowing = false; // �X�N���v�g�������ɂȂ�ۂɃ��[�v���~
    }

    public async UniTask Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            // Wait until next frame
            await UniTask.Yield();
            elapsed += Time.deltaTime;
        }

        transform.localPosition = originalPosition;
    }

    /// <summary>
    /// duration�̊ԃX�v���C�g�����E�ɉ���������
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="magnitude"></param>
    /// <returns></returns>
    public async UniTask ShakeHorizon(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float halfDuration = duration / 2;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            // ���Ɉړ�
            await transform.DOLocalMoveX(originalPosition.x - magnitude, halfDuration).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();

            // �E�Ɉړ�
            await transform.DOLocalMoveX(originalPosition.x + magnitude, halfDuration).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();

            elapsed += duration; // ���[�v�̏I�����m���ɂ��邽�߂ɁA�o�ߎ��Ԃ��X�V
        }

        // ���̈ʒu�ɖ߂�
        transform.localPosition = originalPosition;
    }

    public async UniTask Flash(float duration, Color color)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // �o�ߎ��ԂɊ�Â��ĐF���Ԃ���
            float t = elapsed / duration;  // 0����1�̒l
            Color currentColor = Color.Lerp(color, Color.white, t);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = currentColor;
            }
            else if (image != null)
            {
                image.color = currentColor;
            }

            await UniTask.Yield(PlayerLoopTiming.Update); // �t���[���̍X�V���ɑҋ@
            elapsed += Time.deltaTime;
        }

        // ���[�v�I����A�F�����ɖ߂�
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        else if (image != null)
        {
            image.color = Color.white;
        }
    }

    public async UniTask FlashWithHighlightPlus(float duration, Color color)
    {
        float initialGlow = 0.001f;
        var effect = gameObject.GetComponent<HighlightEffect>();
        if (effect != null)
        {
            float elapsed = initialGlow;
            effect.glowMaskMode = MaskMode.IgnoreMask;

            while (elapsed < duration)
            {
                // �o�ߎ��ԂɊ�Â��ċP�x���Ԃ���
                float t = elapsed / duration;  // 0.001����1�̒l

                effect.glow = t;

                await UniTask.Yield(PlayerLoopTiming.Update); // �t���[���̍X�V���ɑҋ@
                elapsed += Time.deltaTime;
            }
            effect.glow = initialGlow;           
        }
    }

    public async UniTask FadeIn(float duration)
    {
        image = gameObject.GetComponent<Image>();

        // Image�R���|�[�l���g�����݂��邱�Ƃ��m�F
        if (image == null)
        {
            Debug.LogError("Image component not found!");
            return;
        }

        // ���������x��0�ɐݒ�
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

        // �����x��0����1�֕ω�������
        await image.DOFade(1.0f, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();

    }

    public async UniTask FadeOut(float duration)
    {
        image = gameObject.GetComponent<Image>();

        // Image�R���|�[�l���g�����݂��邱�Ƃ��m�F
        if (image == null)
        {
            Debug.LogError("Image component not found!");
            return;
        }

        // ���������x��0�ɐݒ�
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

        // �����x��0����1�֕ω�������
        await image.DOFade(0f, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();

    }

    public async UniTask AnimateColor(Color targetColor, float duration = 0.3f)
    {
        await image.DOColor(targetColor, duration);
    }

    /// <summary>
    /// �yHighlightPlus�g�p�z�X�v���C�g�_�ŊJ�n
    /// </summary>
    /// <param name="minBrightness">�ŏ����x</param>
    /// <param name="maxBrightness">�ő喾�x</param>
    /// <param name="totalDuration">�_�ŊԊu</param>
    /// <param name="cancellationToken">�L�����Z���g�[�N��</param>
    /// <returns></returns>
    public async void StartGlowingEffect(float minBrightness, float maxBrightness, float totalDuration)
    { 
        if (hightlightEffect != null)
        {
            isGlowing = true;
            float startTime = Time.time;

            while (isGlowing)
            {
                float elapsed = Time.time - startTime;
                float cycleTime = elapsed % totalDuration; // totalDuration���ƂɃ��Z�b�g
                float value = Mathf.PingPong(cycleTime / totalDuration * 2 * (maxBrightness - minBrightness), maxBrightness - minBrightness) + minBrightness;
                hightlightEffect.glow = value;

                await UniTask.Yield(PlayerLoopTiming.Update); // ���̃t���[���܂őҋ@
            }
        }     
    }

    /// <summary>
    /// �X�v���C�g�_�Œ�~
    /// </summary>
    /// <param name="index">�_�Œ�~������X�v���C�g�̓G�p�[�e�B���ł̃C���f�b�N�X</param>
    /// <returns></returns>
    public void StopGlowingEffect()
    {
        isGlowing = false;
        if (hightlightEffect != null)
        {
            hightlightEffect.glow = 0.001f;
        }
    }

    /// <summary>
    /// �X�v���C�g�A�E�g���C���_�ŊJ�n
    /// </summary>
    /// <param name="effect">HighlightPlus�R���|�[�l���g</param>
    /// <param name="minBrightness">�ŏ����x</param>
    /// <param name="maxBrightness">�ő喾�x</param>
    /// <param name="totalDuration">�_�ŊԊu</param>
    /// <param name="cancellationToken">�L�����Z���g�[�N��</param>
    /// <returns></returns>
    private async UniTask AnimateGlowBrightness(HighlightEffect effect, float minBrightness, float maxBrightness, float totalDuration, Color color, CancellationToken cancellationToken)
    {
        float startTime = Time.time;
        effect.outlineColor = color;
        while (!cancellationToken.IsCancellationRequested)
        {
            float elapsed = Time.time - startTime;
            float cycleTime = elapsed % totalDuration; // totalDuration���ƂɃ��Z�b�g
            float value = Mathf.PingPong(cycleTime / totalDuration * 2 * (maxBrightness - minBrightness), maxBrightness - minBrightness) + minBrightness;
            effect.outline = value;

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken); // ���̃t���[���܂őҋ@
        }
        effect.outline = 0.001f;
    }

    /// <summary>
    /// �X�v���C�g�A�E�g���C���_�Œ�~
    /// </summary>
    /// <param name="index">�_�Œ�~������X�v���C�g�̓G�p�[�e�B���ł̃C���f�b�N�X</param>
    /// <returns></returns>
    public void StopFlashingGlowEffect(int index, CancellationTokenSource cancellationTokenSource)
    {
        GameObject obj = EnemyManager.Instance.GetEnemyIns(index);
        if (obj != null)
        {
            HighlightEffect effect = obj.GetComponentInChildren<HighlightEffect>();
            if (effect != null)
            {
                effect.outline = 0.001f;
            }
            cancellationTokenSource.Cancel();
        }
    }
}