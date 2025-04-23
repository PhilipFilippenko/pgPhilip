using UnityEngine;

public class UICursorFollow : MonoBehaviour
{
    private RectTransform rect;
    private Vector3 originalScale;

    internal float pulseSpeed = 5f;
    internal float pulseAmount = 0.2f;

    void Start()
    {
        Cursor.visible = false;
        rect = GetComponent<RectTransform>();
        originalScale = rect.localScale;
    }

    void Update()
    {
        rect.position = Input.mousePosition;

        float scale = 1 + Mathf.Sin(Time.time * pulseSpeed + Mathf.PI / 2) * pulseAmount;
        rect.localScale = originalScale * scale;
    }
}