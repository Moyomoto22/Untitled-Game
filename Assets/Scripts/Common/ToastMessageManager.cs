using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class ToastMessageManager : MonoBehaviour
{
    public static ToastMessageManager Instance { get; private set; }

    public GameObject prefab; // Prefab�����ꂽUITextMeshPro�I�u�W�F�N�g
    private GameObject canvasObject;
    private Canvas uiCanvas; // ���݂̃V�[������Canvas

    private void Awake()
    {
        // �V���O���g��
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
        // �V�[������Canvas������
        FindCanvas();
    }

    private void OnEnable()
    {
        // �V�[���ύX����Canvas���Č���
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // �V�[�����ύX���ꂽ�Ƃ���Canvas���Č���
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

        // Dialog�̃C���X�^���X���쐬
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

        // ���b�Z�[�W��ݒ�
        textMeshPro.text = message;

        // UniTask�ŃA�j���[�V���������s
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

        // �t�F�[�h�C���Ə�ɃX���C�h
        float duration = 0.5f;
        Vector2 originalPosition = rectTransform.anchoredPosition;
        Vector2 targetPosition = originalPosition + new Vector2(0, 20);

        canvasGroup.alpha = 0;
        rectTransform.anchoredPosition = originalPosition;

        // DOTween���g�p���ăA�j���[�V����
        await DOTween.Sequence()
            .Append(canvasGroup.DOFade(1, duration))
            .Join(rectTransform.DOAnchorPos(targetPosition, duration))
            .SetEase(Ease.OutCubic)
            .SetUpdate(true)
            .AsyncWaitForCompletion();

        // ��莞�ԑҋ@
        await UniTask.DelayFrame(180);

        // �t�F�[�h�A�E�g�Ƃ���ɏ�ɃX���C�h
        Vector2 fadeOutTargetPosition = targetPosition + new Vector2(0, 10);

        await DOTween.Sequence()
            .Append(canvasGroup.DOFade(0, duration))
            .Join(rectTransform.DOAnchorPos(fadeOutTargetPosition, duration))
            .SetEase(Ease.InCubic)
            .SetUpdate(true)
            .AsyncWaitForCompletion();

        // �_�C�A���O���폜
        Destroy(dialogInstance);
    }
}
