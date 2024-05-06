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

                // ボタンがViewport外にあるかどうかを判定
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

                        // ボタンの高さ分だけScrollViewをスクロール
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
        // スクロールバーを無効化する
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
        // ターゲットオブジェクトのコーナーをワールド座標で取得
        Vector3[] objectCorners = new Vector3[4];
        target.GetWorldCorners(objectCorners);

        // オブジェクトのコーナーをビューポートのローカル座標に変換
        Vector2[] viewportLocalCorners = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            viewportLocalCorners[i] = RectTransformUtility.WorldToScreenPoint(null, objectCorners[i]);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewportRect, viewportLocalCorners[i], null, out viewportLocalCorners[i]);
        }

        // オブジェクトのどれかのコーナーがビューポートの矩形内にあるかチェック
        for (int i = 0; i < 4; i++)
        {
            if (!viewportRect.rect.Contains(viewportLocalCorners[i]))
            {
                return false; // 少なくとも1つのコーナーが表示されていない
            }
        }
        return true; // どのコーナーも表示されている
    }
}
