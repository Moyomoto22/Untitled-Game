using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FadeController : MonoBehaviour
{
    public Canvas fadeCanvas;
    private FadeUniTask fadeComponent;

    private const string normal = "001";
    private const string mosaic = "171";

    const float fadeDuration = 0.6f;

    // �V���O���g���p�^�[��
    public static FadeController Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            fadeComponent = fadeCanvas.GetComponent<FadeUniTask>();
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(fadeCanvas);
            DontDestroyOnLoad(fadeComponent);

        }
        else if (Instance != this)
        {
            // Destroy(gameObject);
        }
        //await FadeOutLoaded();
    }

    /// <summary>
    /// �t�F�[�h�C��(�Ó])
    /// </summary>
    /// <returns></returns>
    public async UniTask FadeIn()
    {
        await fadeComponent.FadeIn(fadeDuration, null);
    }

    /// <summary>
    /// �t�F�[�h�A�E�g(�Ó]����)
    /// </summary>
    /// <returns></returns>
    public async UniTask FadeOut()
    {
        await fadeComponent.FadeOut(fadeDuration, null);
    }
}
