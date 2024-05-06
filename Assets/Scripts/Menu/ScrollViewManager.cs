using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ScrollViewManager : MonoBehaviour
{
    public ScrollRect scrollView;
    public RectTransform content;
    public int numberOfInnerRows;
    public float scrollSpeed = 1f;

    private EventSystem eventSystem;
    private ScrollRect scrollRect;
    private RectTransform viewportRect;
    private bool isScrolling;
    private GameObject lastSelectedButton;

    public RectTransform prefab;
    public int numberOfColumns;

    private float scrollHeight;

    private void Start()
    {
        eventSystem = EventSystem.current;
        scrollRect = scrollView.GetComponent<ScrollRect>();
        viewportRect = scrollView.viewport;
    }

    private void Update()
    {
        GameObject selectedObject = eventSystem.currentSelectedGameObject;

        if (selectedObject != null && selectedObject != lastSelectedButton)
        {
            if (IsObjectVisible(selectedObject.GetComponent<RectTransform>()))
            {
                Debug.Log("Selected Object is visible!");
            }
            else
            {
                Debug.Log("Selected Object is not visible!");

                // ScrollView�̏�[�܂��͉��[�ɂ͂ݏo���Ă��邩����
                float margin = 0.1f; // �}�[�W���̐ݒ�i�K�v�ɉ����Ē����j
                float objectYPosition = selectedObject.GetComponent<RectTransform>().position.y;
                float viewportTopY = viewportRect.position.y + viewportRect.rect.yMax + margin;
                float viewportBottomY = viewportRect.position.y + viewportRect.rect.yMin - margin;

                bool isAboveViewport = objectYPosition > viewportTopY;
                bool isBelowViewport = objectYPosition < viewportBottomY;

                lastSelectedButton = selectedObject;

                if (isAboveViewport)
                {
                    // ��ɃX�N���[��
                    Scroll(true);
                }
                else if (isBelowViewport)
                {
                    // ���ɃX�N���[��
                    Scroll(false);
                }
            }
        }
    }

    private void Scroll(bool isUp)
    {
        // �R���e���c�S�̂̍���
        float contentHeight = content.rect.height + content.GetComponent<GridLayoutGroup>().spacing.y;

        float cellHeight = content.GetComponent<GridLayoutGroup>().cellSize.y + content.GetComponent<GridLayoutGroup>().spacing.y;

        int numberOfRows = MathUtilities.RoundDivide(content.transform.childCount, 2);
        int overRows = numberOfRows - numberOfInnerRows;
        float scrollHeight = 1.0f / (float)overRows;

        float hosei = overRows * 0.00022815f;

        scrollHeight = scrollHeight - hosei;// 0.00365f;

        // �X�N���[���ʒu���擾
        float verticalScrollPosition = scrollRect.verticalNormalizedPosition;

        Debug.Log("numberOfRows: " + numberOfRows + " / overRows: " + overRows + " / numberOfInnerRows: " + numberOfInnerRows);
        Debug.Log("Before Scroll: " + scrollRect.verticalNormalizedPosition);
        // ScrollView���w��̍����܂ŃX�N���[��
        if (isUp)
        {
            //scrollRect.verticalNormalizedPosition = verticalScrollPosition + scrollHeight;
            scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition + scrollHeight, 0f, 1f);
        }
        else
        {
            //scrollRect.verticalNormalizedPosition = verticalScrollPosition - scrollHeight;
            scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition - scrollHeight, 0f, 1f);
        }

        float newVerticalScrollPosition = scrollRect.verticalNormalizedPosition;
        Debug.Log("After Scroll: " + newVerticalScrollPosition);
        //Debug.Log(contentHeight + " / " + newVerticalScrollPosition + " / " + scrollHeight);
    }

    private float CalcScrollHeight()
    {
        // �v�f�̍����ƊԊu���擾
        float cellSize = content.GetComponent<GridLayoutGroup>().cellSize.y;
        float spacing = content.GetComponent<GridLayoutGroup>().spacing.y;

        // �R���e���c�̍������v�Z
        float scrollHeight = (cellSize + spacing);

        return scrollHeight;
        // �R���e���c�̍�����ݒ�
        //content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
    }

    private void ScrollToTop()
    {
        // �X�N���[���ʒu����[�ɂȂ�悤�ɐݒ�
        scrollRect.verticalNormalizedPosition = 1.0f;

        // ���C�A�E�g�̍č\�z��҂�
        StartCoroutine(DelayedLayoutRebuild());
    }

    private void ScrollToBottom()
    {
        // �X�N���[���ʒu�����[�ɂȂ�悤�ɐݒ�
        scrollRect.verticalNormalizedPosition = -1.0f;

        // ���C�A�E�g�̍č\�z��҂�
        StartCoroutine(DelayedLayoutRebuild());
    }

    private IEnumerator DelayedLayoutRebuild()
    {
        // ���C�A�E�g�̍č\�z��҂�
        yield return null;
    }

    /// <summary>
    /// �I�����ꂽScrollView���̃I�u�W�F�N�g���\������Ă��邩����
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool IsObjectVisible(RectTransform target)
    {
        // �^�[�Q�b�g�I�u�W�F�N�g�̃R�[�i�[�����[���h���W�Ŏ擾
        Vector3[] objectCorners = new Vector3[4];
        target.GetWorldCorners(objectCorners);

        // �I�u�W�F�N�g�̃R�[�i�[���r���[�|�[�g�̃��[�J�����W�ɕϊ�
        Vector2[] viewportLocalCorners = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            viewportLocalCorners[i] = RectTransformUtility.WorldToScreenPoint(null, objectCorners[i]);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewportRect, viewportLocalCorners[i], null, out viewportLocalCorners[i]);
        }

        // �I�u�W�F�N�g�̂ǂꂩ�̃R�[�i�[���r���[�|�[�g�̋�`���ɂ��邩�`�F�b�N
        for (int i = 0; i < 4; i++)
        {
            if (!viewportRect.rect.Contains(viewportLocalCorners[i]))
            {
                return false; // ���Ȃ��Ƃ�1�̃R�[�i�[���\������Ă��Ȃ�
            }
        }

        return true; // �ǂ̃R�[�i�[���\������Ă���
    }

    /// <summary>
    /// ScrollView�̃X�N���[��
    /// </summary>
    /// <param name="scrollAmount"></param>
    private void ScrollScrollView(float scrollAmount)
    {
        // �X�N���[���o�[�𖳌�������
        scrollView.verticalScrollbar = null;

        scrollView.verticalNormalizedPosition += scrollAmount * Time.deltaTime;
        scrollView.verticalNormalizedPosition = Mathf.Clamp01(scrollView.verticalNormalizedPosition);
    }
}