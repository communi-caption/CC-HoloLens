using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OCRTextRenderer : MonoBehaviour
{
    public RectTransform canvasRect;
    private RectTransform uiRect;

    void Awake()
    {
        uiRect = GetComponent<RectTransform>();
    }

    //void Start()
    //{
    //    Invoke("deneme", 2);
    //}
    //
    //void deneme()
    //{
    //    Render(new Vector2(-5.3f, 0), 2, 3, 90, "asddsa");
    //}

    public void Render(Vector2 center, float width, float height, float rotationZ, string text)
    {
        Debug.Log(">>>>> center: " + center);
        Debug.Log(">>>>> width: " + width);
        Debug.Log(">>>>> height: " + height);

        Vector2 centerTransformed = Camera.main.WorldToViewportPoint(center);
        Vector2 leftTransformed = Camera.main.WorldToViewportPoint(center - Vector2.left * width);
        Vector2 topTransformed = Camera.main.WorldToViewportPoint(center + Vector2.up * height);

        Vector2 centerNormalized = new Vector2(
        ((centerTransformed.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
        ((centerTransformed.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

        Vector2 leftNormalized = new Vector2(
        ((leftTransformed.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
        ((leftTransformed.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

        Vector2 topNormalized = new Vector2(
        ((topTransformed.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
        ((topTransformed.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

        float widthTransformed = (leftNormalized - centerNormalized).magnitude;
        float heightTransformed = (topNormalized - centerNormalized).magnitude;

        uiRect.anchoredPosition = centerNormalized;
        uiRect.sizeDelta = new Vector2(widthTransformed, heightTransformed);
        uiRect.Rotate(0, 0, rotationZ);

        GetComponentInChildren<Text>().text = text;

        Debug.Log(">>>>> centerNormalized: " + centerNormalized);
        Debug.Log(">>>>> widthTransformed: " + widthTransformed);
        Debug.Log(">>>>> heightTransformed: " + heightTransformed);
    }
}
