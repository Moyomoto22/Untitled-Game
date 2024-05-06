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

                // ScrollViewの上端または下端にはみ出しているか判定
                float margin = 0.1f; // マージンの設定（必要に応じて調整）
                float objectYPosition = selectedObject.GetComponent<RectTransform>().position.y;
                float viewportTopY = viewportRect.position.y + viewportRect.rect.yMax + margin;
                float viewportBottomY = viewportRect.position.y + viewportRect.rect.yMin - margin;

                bool isAboveViewport = objectYPosition > viewportTopY;
                bool isBelowViewport = objectYPosition < viewportBottomY;

                lastSelectedButton = selectedObject;

                if (isAboveViewport)
                {
                    // 上にスクロール
                    Scroll(true);
                }
                else if (isBelowViewport)
                {
                    // 下にスクロール
                    Scroll(false);
                }
            }
        }
    }

    private void Scroll(bool isUp)
    {
        // コンテンツ全体の高さ
        float contentHeight = content.rect.height + content.GetComponent<GridLayoutGroup>().spacing.y;

        float cellHeight = content.GetComponent<GridLayoutGroup>().cellSize.y + content.GetComponent<GridLayoutGroup>().spacing.y;

        int numberOfRows = MathUtilities.RoundDivide(content.transform.childCount, 2);
        int overRows = numberOfRows - numberOfInnerRows;
        float scrollHeight = 1.0f / (float)overRows;

        float hosei = overRows * 0.00022815f;

        scrollHeight = scrollHeight - hosei;// 0.00365f;

        // スクロール位置を取得
        float verticalScrollPosition = scrollRect.verticalNormalizedPosition;

        Debug.Log("numberOfRows: " + numberOfRows + " / overRows: " + overRows + " / numberOfInnerRows: " + numberOfInnerRows);
        Debug.Log("Before Scroll: " + scrollRect.verticalNormalizedPosition);
        // ScrollViewを指定の高さまでスクロール
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
        // 要素の高さと間隔を取得
        float cellSize = content.GetComponent<GridLayoutGroup>().cellSize.y;
        float spacing = content.GetComponent<GridLayoutGroup>().spacing.y;

        // コンテンツの高さを計算
        float scrollHeight = (cellSize + spacing);

        return scrollHeight;
        // コンテンツの高さを設定
        //content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
    }

    private void ScrollToTop()
    {
        // スクロール位置が上端になるように設定
        scrollRect.verticalNormalizedPosition = 1.0f;

        // レイアウトの再構築を待つ
        StartCoroutine(DelayedLayoutRebuild());
    }

    private void ScrollToBottom()
    {
        // スクロール位置が下端になるように設定
        scrollRect.verticalNormalizedPosition = -1.0f;

        // レイアウトの再構築を待つ
        StartCoroutine(DelayedLayoutRebuild());
    }

    private IEnumerator DelayedLayoutRebuild()
    {
        // レイアウトの再構築を待つ
        yield return null;
    }

    /// <summary>
    /// 選択されたScrollView内のオブジェクトが表示されているか判定
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
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

    /// <summary>
    /// ScrollViewのスクロール
    /// </summary>
    /// <param name="scrollAmount"></param>
    private void ScrollScrollView(float scrollAmount)
    {
        // スクロールバーを無効化する
        scrollView.verticalScrollbar = null;

        scrollView.verticalNormalizedPosition += scrollAmount * Time.deltaTime;
        scrollView.verticalNormalizedPosition = Mathf.Clamp01(scrollView.verticalNormalizedPosition);
    }
}