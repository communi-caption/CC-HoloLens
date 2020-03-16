using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OCRTextRenderer : MonoBehaviour
{
    Text sentence;
    Image rect;

    public RectTransform CanvasRect;
    private RectTransform UI_Element;

    void Awake()
    {
        UI_Element = GetComponent<RectTransform>();
    }

    void Start()
    {
        Invoke("deneme", 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void deneme()
    {
        Render(new Vector2(-5.3f, 0),  2, 3, "asddsa");
    }

    public void Render(Vector2 center, float width, float height, string text)
    {
        Vector2 centerTransformed = Camera.main.WorldToViewportPoint(center);
        Vector2 leftTransformed = Camera.main.WorldToViewportPoint(center - Vector2.left * width);
        Vector2 topTransformed = Camera.main.WorldToViewportPoint(center + Vector2.up * height);

        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((centerTransformed.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((centerTransformed.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        Vector2 WorldObject_ScreenPosition2 = new Vector2(
        ((leftTransformed.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((leftTransformed.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        Vector2 WorldObject_ScreenPosition3 = new Vector2(
        ((topTransformed.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((topTransformed.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        float widthTransformed = (WorldObject_ScreenPosition2 - WorldObject_ScreenPosition).magnitude;
        float heightTransformed = (WorldObject_ScreenPosition3 - WorldObject_ScreenPosition).magnitude;

        UI_Element.anchoredPosition = WorldObject_ScreenPosition;
        UI_Element.sizeDelta = new Vector2(widthTransformed, heightTransformed);
    }
}
