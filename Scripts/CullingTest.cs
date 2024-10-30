using System.Collections;

using UnityEngine;

public class CullingTest : MonoBehaviour
{
    private RectTransform uiElement;
    private Canvas canvas;
    public RectTransform viewPort;

    private void Awake()
    {
        uiElement = GetComponent<RectTransform>();
        canvas = GetComponent<Canvas>();
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (true)
        {
            if (CalculateVisibility())
            {
                canvas.enabled = true;
            }
            else
            {
                canvas.enabled = false;
            }
            yield return null;
        }
    }

    private bool CalculateVisibility()
    {
        if (uiElement == null || viewPort == null)
            return false;

        var rect = GetWorldRect(uiElement);
        if (rect.width <= 0 || rect.height <= 0)
            return false;

        var viewport = GetWorldRect(viewPort);
        if (viewport.width <= 0 || viewport.height <= 0)
            return false;

        var visible = rect.Overlaps(viewport);
        return visible;
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        
        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];
        
        return new Rect(bottomLeft, topRight - bottomLeft);
    }
}
