using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class FadeUniTask : MonoBehaviour
{
    IFade fade;

    void Start()
    {
        Init();
        fade.Range = cutoutRange;
    }

    public float cutoutRange;

    void Init()
    {
        fade = GetComponent<IFade>();
    }

    void OnValidate()
    {
        Init();
        fade.Range = cutoutRange;
    }

    async UniTask FadeoutAsync(float time, Action action, string textureName, CancellationToken cancellationToken)
    {
        float endTime = time + Time.time;
        if (fade == null)
        {
            fade = GetComponent<IFade>();
        }

        //var image = fade as FadeImage;
        //image.maskTexture = await GetFadeTexture(textureName);

        while (Time.time <= endTime)
        {
            cutoutRange = (endTime - Time.time) / time;
            fade.Range = cutoutRange;
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        cutoutRange = 0;
        fade.Range = cutoutRange;

        action?.Invoke();
    }

    async UniTask FadeinAsync(float time, Action action, string textureName, CancellationToken cancellationToken)
    {
        float endTime = Time.timeSinceLevelLoad + time * (1 - cutoutRange);
        if (fade == null)
        {
            fade = GetComponent<IFade>();
        }

        //var image = fade as FadeImage;
        //image.maskTexture = await GetFadeTexture(textureName);

        while (Time.timeSinceLevelLoad <= endTime)
        {
            cutoutRange = 1 - ((endTime - Time.timeSinceLevelLoad) / time);
            fade.Range = cutoutRange;
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        cutoutRange = 1;
        fade.Range = cutoutRange;

        action?.Invoke();
    }

    async UniTask FadeinAsync2(float time, Action action, CancellationToken cancellationToken)
    {
        float endTime = Time.timeSinceLevelLoad + time * (1 - cutoutRange);
        if (fade == null)
        {
            fade = GetComponent<IFade>();
        }
        while (Time.timeSinceLevelLoad <= endTime)
        {
            cutoutRange = 1 - ((endTime - Time.timeSinceLevelLoad) / time);
            fade.Range = cutoutRange;
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        cutoutRange = 1;
        fade.Range = cutoutRange;

        action?.Invoke();
    }

    public async UniTask FadeOut(float time, string textureName, Action action, CancellationToken cancellationToken = default)
    {
        // 前のタスクをキャンセルするためにCancellationTokenを使用
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await FadeoutAsync(time, action, textureName, cancellationTokenSource.Token);
    }

    public async UniTask FadeOut(float time, string textureName, CancellationToken cancellationToken = default)
    {
        await FadeOut(time, textureName, null, cancellationToken);
    }

    public async UniTask FadeIn(float time, string textureName, Action action, CancellationToken cancellationToken = default)
    {
        // 前のタスクをキャンセルするためにCancellationTokenを使用
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await FadeinAsync(time, action, textureName, cancellationTokenSource.Token);
    }

    public async UniTask FadeIn2(float time, Action action, CancellationToken cancellationToken = default)
    {
        // 前のタスクをキャンセルするためにCancellationTokenを使用
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await FadeinAsync2(time, action, cancellationTokenSource.Token);
    }

    public async UniTask FadeIn(float time, string textureName, CancellationToken cancellationToken = default)
    {
        await FadeIn(time, textureName, null, cancellationToken);
    }

    public async UniTask<Texture> GetFadeTexture(string textureName)
    {
        var texture = Resources.Load<Texture2D>("Fade/" + textureName);
        //await UniTask.DelayFrame(10);
        
        return texture;
    }

    private CancellationTokenSource cancellationTokenSource;
}
