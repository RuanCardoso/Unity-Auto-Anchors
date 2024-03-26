/*===========================================================
    Author: Ruan Cardoso
    -
    Country: Brazil(Brasil)
    -
    Contact: cardoso.ruan050322@gmail.com
    -
    Support: neutron050322@gmail.com
    -
    Unity Minor Version: 2021.3 LTS
    -
    License: Open Source (MIT)
    ===========================================================*/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;

public class AutoAnchorsEditor : Editor
{
    private static void Anchor(RectTransform rect)
    {
        if (rect.transform.parent != null)
        {
            if (rect.transform.parent.TryGetComponent(out RectTransform pRectTransform))
            {
                if (IsDriven(pRectTransform))
                    return;

                Undo.RecordObject(rect, "Auto Anchor Editor");
                if (rect.localScale != Vector3.one)
                {
                    float wScale = rect.rect.width * rect.localScale.x;
                    float yScale = rect.rect.height * rect.localScale.y;
                    AutoScale(rect, wScale, yScale);
                    // Fix the aspect ratio.
                    rect.localScale = Vector3.one;
                }

                Rect pRect = pRectTransform.rect;
                float offsetMinX = rect.offsetMin.x, offsetMaxX = rect.offsetMax.x;
                float offsetMinY = rect.offsetMin.y, offsetMaxY = rect.offsetMax.y;

                Vector2 min = new Vector2(rect.anchorMin.x + (offsetMinX / pRect.width), rect.anchorMin.y + (offsetMinY / pRect.height));
                Vector2 max = new Vector2(rect.anchorMax.x + (offsetMaxX / pRect.width), rect.anchorMax.y + (offsetMaxY / pRect.height));

                rect.anchorMin = min;
                rect.anchorMax = max;
                ResetOffsetAndPivot(rect);
            }
        }
    }

    private static void AutoScale(RectTransform rect, float widthScale, float heightScale)
    {
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, widthScale);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightScale);
    }

    private static void ResetOffsetAndPivot(RectTransform rectTrans)
    {
        rectTrans.pivot = new Vector2(0.5f, 0.5f);
        rectTrans.offsetMin = Vector2.zero;
        rectTrans.offsetMax = Vector2.zero;
    }

    private static bool IsDriven(RectTransform transform)
    {
        GridLayoutGroup gridLayoutGroup = transform.GetComponent<GridLayoutGroup>();
        VerticalLayoutGroup verticalLayoutGroup = transform.GetComponent<VerticalLayoutGroup>();
        HorizontalLayoutGroup horizontalLayoutGroup = transform.GetComponent<HorizontalLayoutGroup>();
        return gridLayoutGroup != null || verticalLayoutGroup != null || horizontalLayoutGroup != null;
    }

    [MenuItem("UI Tools/Auto Anchors On Selected Game Objects _F1")]
    private static void AnchorSelectedObjects()
    {
        RectTransform[] rectTransforms = Selection.gameObjects.Select(x => x.GetComponent<RectTransform>()).ToArray();
        for (int i = 0; i < rectTransforms.Length; i++)
        {
            RectTransform rectTrans = rectTransforms[i];
            if (rectTrans != null) Anchor(rectTrans);
        }
    }

    [MenuItem("UI Tools/Auto Anchors On All Game Objects _F2")]
    private static void AnchorAll()
    {
        RectTransform[] rectTransforms = FindObjectsByType<RectTransform>(FindObjectsSortMode.None);
        for (int i = 0; i < rectTransforms.Length; i++)
        {
            RectTransform rectTrans = rectTransforms[i];
            if (rectTrans != null) Anchor(rectTrans);
        }
    }

    [MenuItem("UI Tools/Auto Anchors On Selected Game Objects And Match _F3")]
    private static void Match()
    {
        RectTransform[] rectTransforms = Selection.gameObjects.Select(x => x.GetComponent<RectTransform>()).ToArray();
        for (int i = 0; i < rectTransforms.Length; i++)
        {
            RectTransform rectTrans = rectTransforms[i];
            if (rectTrans != null)
            {
                rectTrans.anchorMin = Vector2.zero;
                rectTrans.anchorMax = Vector2.one;
                rectTrans.anchoredPosition = Vector2.zero;
                rectTrans.sizeDelta = Vector3.zero;
                Anchor(rectTrans);
            }
        }
    }
}
#endif
