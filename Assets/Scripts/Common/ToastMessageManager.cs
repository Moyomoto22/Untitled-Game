using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class ToastMessageManager : MonoBehaviour
{
    public static ToastMessageManager Instance { get; private set; }

    public GameObject prefab; // Prefab化されたUITextMeshProオブジェクト
    private GameObject canvasObject;
    private Canvas uiCanvas; // 現在のシーン内のCanvas

    private void Awake()
    {
        // シングルトン
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

    private void Start()
    {
        // シーン内のCanvasを検索
        FindCanvas();
    }

    private void OnEnable()
    {
        // シーン変更時にCanvasを再検索
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // シーンが変更されたときにCanvasを再検索
        FindCanvas();
    }

    private void FindCanvas()
    {
        canvasObject = GameObject.FindWithTag("MainCanvas");
        if (canvasObject != null)
        {
            uiCanvas = canvasObject.GetComponent<Canvas>();
            if (uiCanvas == null)
            {
                Debug.LogError("Canvas not found in the scene.");
            }
        }
    }

    public void ShowToastMessage(string message, Sprite sprite = null)
    {
        if (prefab == null)
        {
            Debug.LogError("Dialog prefab is not assigned.");
            return;
        }

        if (uiCanvas == null)
        {
            Debug.LogError("Canvas not found in the scene.");
            return;
        }

        // Dialogのインスタンスを作成
        GameObject dialogInstance = Instantiate(prefab, uiCanvas.transform);

        TextMeshProUGUI textMeshPro = dialogInstance.GetComponentInChildren<TextMeshProUGUI>();
        Image icon = dialogInstance.GetComponentInChildren<Image>();

        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshPro component not found in the dialog prefab.");
            Destroy(dialogInstance);
            return;
        }

        if (icon != null && sprite != null)
        {
            icon.enabled = true;
            icon.sprite = sprite;
        }

        // メッセージを設定
        textMeshPro.text = message;

        // UniTaskでアニメーションを実行
        AnimateToast(dialogInstance).Forget();
    }

    private async UniTaskVoid AnimateToast(GameObject dialogInstance)
    {
        CanvasGroup canvasGroup = dialogInstance.GetComponent<CanvasGroup>();
        RectTransform rectTransform = dialogInstance.GetComponent<RectTransform>();

        if (canvasGroup == null || rectTransform == null)
        {
            Debug.LogError("CanvasGroup or RectTransform component not found in the dialog prefab.");
            Destroy(dialogInstance);
            return;
        }

        // フェードインと上にスライド
        float duration = 0.5f;
        Vector2 originalPosition = rectTransform.anchoredPosition;
        Vector2 targetPosition = originalPosition + new Vector2(0, 20);

        canvasGroup.alpha = 0;
        rectTransform.anchoredPosition = originalPosition;

        // DOTweenを使用してアニメーション
        await DOTween.Sequence()
            .Append(canvasGroup.DOFade(1, duration))
            .Join(rectTransform.DOAnchorPos(targetPosition, duration))
            .SetEase(Ease.OutCubic)
            .SetUpdate(true)
            .AsyncWaitForCompletion();

        // 一定時間待機
        await UniTask.DelayFrame(180);

        // フェードアウトとさらに上にスライド
        Vector2 fadeOutTargetPosition = targetPosition + new Vector2(0, 10);

        await DOTween.Sequence()
            .Append(canvasGroup.DOFade(0, duration))
            .Join(rectTransform.DOAnchorPos(fadeOutTargetPosition, duration))
            .SetEase(Ease.InCubic)
            .SetUpdate(true)
            .AsyncWaitForCompletion();

        // ダイアログを削除
        Destroy(dialogInstance);
    }
}
