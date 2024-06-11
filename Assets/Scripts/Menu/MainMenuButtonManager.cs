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

    // �{�^���I���������̃A�j���[�V�����𐧌�
    public void OnDeselect()
    {
        fillImage.DOKill(); // �A�j���[�V�������L�����Z��
        fillImage.fillAmount = 0;
    }

    // �{�^���I�����̃A�j���[�V�����𐧌�
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
