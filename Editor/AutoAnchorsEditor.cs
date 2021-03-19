using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;

public class AutoAnchorsEditor : Editor
{
    private static void Anchor(RectTransform rectTrans)
    {
        RectTransform parentRectTrans = null;
        if (rectTrans.transform.parent != null)
            parentRectTrans = rectTrans.transform.parent.GetComponent<RectTransform>();

        if (parentRectTrans == null)
            return;
        else if (IsDriven(parentRectTrans))
            return;

        Undo.RecordObject(rectTrans, "Auto Anchor Object");
        Rect pRect = parentRectTrans.rect;
        if (rectTrans.localScale != Vector3.one)
            Debug.LogError($"The Scale of the object cannot be different from {Vector3.one} Tip: Do not scale UI objects, use the \"Rect Tool\" instead of the \"Scale Tool\"");
        else
        {
            float offsetMinX = rectTrans.offsetMin.x, offsetMaxX = rectTrans.offsetMax.x;
            float offsetMinY = rectTrans.offsetMin.y, offsetMaxY = rectTrans.offsetMax.y;
            Vector2 min = new Vector2(rectTrans.anchorMin.x + (offsetMinX / pRect.width), rectTrans.anchorMin.y + (offsetMinY / pRect.height));
            Vector2 max = new Vector2(rectTrans.anchorMax.x + (offsetMaxX / pRect.width), rectTrans.anchorMax.y + (offsetMaxY / pRect.height));
            rectTrans.anchorMin = min;
            rectTrans.anchorMax = max;
            ResetOffsetAndPivot(rectTrans);
        }
    }

    private static void ResetOffsetAndPivot(RectTransform rectTrans)
    {
        Vector2 centerPivot = new Vector2(0.5f, 0.5f);
        rectTrans.offsetMin = Vector2.zero;
        rectTrans.offsetMax = Vector2.zero;
        rectTrans.pivot = centerPivot;
        rectTrans.pivot = centerPivot;
    }

    private static bool IsDriven(RectTransform transform)
    {
        var gridLayoutGroup = transform.GetComponent<GridLayoutGroup>();
        var verticalLayoutGroup = transform.GetComponent<VerticalLayoutGroup>();
        var horizontalLayoutGroup = transform.GetComponent<HorizontalLayoutGroup>();
        return (gridLayoutGroup != null || verticalLayoutGroup != null || horizontalLayoutGroup != null);
    }

    [MenuItem("Neutron/UI Tools/Auto Anchors On Selected Game Objects _F1")]
    private static void AnchorSelectedObjects()
    {
        RectTransform[] rectTransforms = Selection.gameObjects.Select(x => x.GetComponent<RectTransform>()).ToArray();
        for (int i = 0; i < rectTransforms.Length; i++)
        {
            RectTransform rectTrans = rectTransforms[i];
            if (rectTrans != null)
                Anchor(rectTrans);
        }
    }

    [MenuItem("Neutron/UI Tools/Auto Anchors On All Game Objects _F2")]
    private static void AnchorAll()
    {
        RectTransform[] rectTransforms = GameObject.FindObjectsOfType<RectTransform>();
        for (int i = 0; i < rectTransforms.Length; i++)
        {
            RectTransform rectTrans = rectTransforms[i];
            if (rectTrans != null)
                Anchor(rectTrans);
        }
    }

    [MenuItem("Neutron/UI Tools/Auto Anchors On Selected Game Objects And Match _F3")]
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