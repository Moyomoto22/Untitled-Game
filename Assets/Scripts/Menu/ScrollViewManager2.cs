using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewManager2 : MonoBehaviour
{
    public ScrollRect scrollView;
    public RectTransform content;
    public int numberOfColumns;
    public int numberOfVisibleRows;
    public float scrollSpeed = 1f;

    private GameObject lastSelectedObject;
    private EventSystem eventSystem;
    private RectTransform viewportRect;

    private void Start()
    {
        eventSystem = EventSystem.current;
        viewportRect = scrollView.viewport;
    }

    private void Update()
    {
        GameObject selectedObject = eventSystem.currentSelectedGameObject;
        if (selectedObject != null)
        {
            GameObject parent = selectedObject.transform.parent.gameObject;
            var index = parent.transform.GetSiblingIndex();

            if (selectedObject != null && selectedObject != lastSelectedObject)
            {
                RectTransform selectedRect = selectedObject.GetComponent<RectTransform>();

                // �{�^����Viewport�O�ɂ��邩�ǂ����𔻒�
                if (!IsObjectVisible(selectedRect))
                {
                    lastSelectedObject = selectedObject;

                    if (index == 0)
                    {
                        ScrollToTop();
                    }
                    else if (index + 1 == content.transform.childCount)
                    {
                        ScrollToBottom();
                    }
                    else
                    {
                        float objectYPosition = selectedObject.GetComponent<RectTransform>().position.y;
                        float viewportTopY = viewportRect.position.y + viewportRect.rect.yMax;
                        float viewportBottomY = viewportRect.position.y + viewportRect.rect.yMin;

                        bool isAboveViewport = objectYPosition > viewportTopY;
                        bool isBelowViewport = objectYPosition < viewportBottomY;

                        int numberOfRows = MathUtilities.RoundDivide(content.transform.childCount, numberOfColumns);
                        int overRows = numberOfRows - numberOfVisibleRows;

                        if (overRows <= 0)
                        {
                            return;
                        }

                        float scrollHeight = 1.0f / (float)overRows;

                        // �{�^���̍���������ScrollView���X�N���[��
                        float buttonHeight = selectedRect.rect.height;
                        float normalizedScrollAmount = buttonHeight / content.rect.height;

                        if (isBelowViewport)
                        {
                            scrollHeight = 0 - scrollHeight;
                        }

                        ScrollScrollView(scrollHeight);
                    }
                }
            }
        }
    }

    public void ScrollScrollView(float scrollAmount)
    {
        // �X�N���[���o�[�𖳌�������
        scrollView.verticalScrollbar = null;

        scrollView.verticalNormalizedPosition += scrollAmount;// * Time.deltaTime;
        scrollView.verticalNormalizedPosition = Mathf.Clamp01(scrollView.verticalNormalizedPosition);
    }

    public void ScrollToTop()
    {
        scrollView.verticalNormalizedPosition = 1f;
    }

    public void ScrollToBottom()
    {
        scrollView.verticalNormalizedPosition = 0f;
    }

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
}
