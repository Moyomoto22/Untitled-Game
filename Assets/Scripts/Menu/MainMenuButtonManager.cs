using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButtonManager : MonoBehaviour
{
    [SerializeField]
    private Image fillImage;
    [SerializeField]
    private EventSystem eventSystem;

    private const float duration = 0.2f;

    public bool shouldPlaySound = true;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    // ボタン選択解除時のアニメーションを制御
    public void OnDeselect()
    {
        fillImage.DOKill(); // アニメーションをキャンセル
        fillImage.fillAmount = 0;
    }

    // ボタン選択時のアニメーションを制御
    public void OnSelect()
    {
        if (shouldPlaySound)
        {
            SoundManager.Instance.PlaySelect(0.5f, 1.0f);
        }
        fillImage.DOFillAmount(1, duration).SetUpdate(true);
        Debug.Log(this.name);

        shouldPlaySound = true;
    }
}
