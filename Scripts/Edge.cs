using UnityEngine;
using UnityEngine.UI;

namespace Dunward.GraphView.Runtime
{
    public class Edge : MaskableGraphic
    {
        public RectTransform view;
        public RectTransform test;

        public RectTransform startNode;
        public RectTransform endNode;

        private float defaultSize = 4f;
        private float hoverSize = 7f;
        private float hoverThreshold = 30f;

        private float baseLineLength = 45f;
        private bool isHovered;

        private void Update()
        {
            if (!canvas.enabled) return;
            
            DetectHover();
            UpdateRectTransform();
            SetVerticesDirty();
        }

        private void DetectHover()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(test, Input.mousePosition, Camera.main, out var mousePosition);

            Vector2 startPos = startNode.TransformPoint(startNode.anchoredPosition);
            Vector2 endPos = endNode.TransformPoint(endNode.anchoredPosition);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(test, startPos, null, out startPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(test, endPos, null, out endPos);

            startPos += new Vector2(baseLineLength, 0);
            endPos -= new Vector2(baseLineLength, 0);
            
            Debug.Log($"before diagonalStart {startPos} diagonalEnd {endPos}");

            float distance = DistancePointToLine(mousePosition, startPos, endPos);
            isHovered = distance <= hoverThreshold;
            Debug.Log($"diagonalStart {startPos} diagonalEnd {endPos} - mouse : {mousePosition} - distance {distance}");
        }

        private float DistancePointToLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            var line = lineEnd - lineStart;
            var pointToStart = point - lineStart;
            float lineLengthSquared = line.sqrMagnitude;
            float projection = Vector2.Dot(pointToStart, line) / lineLengthSquared;

            if (projection < 0)
                return Vector2.Distance(point, lineStart);
            else if (projection > 1)
                return Vector2.Distance(point, lineEnd);

            var closestPoint = lineStart + projection * line;
            return Vector2.Distance(point, closestPoint);
        }

        private void UpdateRectTransform()
        {
            Vector2 startPos = startNode.TransformPoint(startNode.anchoredPosition);
            Vector2 endPos = endNode.TransformPoint(endNode.anchoredPosition);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(test, startPos, null, out startPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(test, endPos, null, out endPos);

            Vector2 min = Vector2.Min(startPos, endPos);
            Vector2 max = Vector2.Max(startPos, endPos);

            Vector2 size = max - min;

            Vector2 center = (startPos + endPos) * 0.5f;

            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = center;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Vector2 startPos = startNode.TransformPoint(startNode.anchoredPosition);
            Vector2 endPos = endNode.TransformPoint(endNode.anchoredPosition);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(test, startPos, null, out startPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(test, endPos, null, out endPos);

            DrawEdge(vh, startPos, endPos, color, isHovered ? hoverSize : defaultSize);
        }

        private void DrawEdge(VertexHelper vh, Vector2 startPos, Vector2 endPos, Color color, float width)
        {
            var diagonalStart = startPos + new Vector2(baseLineLength, 0);
            DrawLine(vh, startPos - rectTransform.anchoredPosition, diagonalStart - rectTransform.anchoredPosition, width, color);
            var diagonalEnd = endPos + new Vector2(-baseLineLength, 0);
            DrawLine(vh, endPos - rectTransform.anchoredPosition, diagonalEnd - rectTransform.anchoredPosition, width, color);
            DrawLine(vh, diagonalStart - rectTransform.anchoredPosition, diagonalEnd - rectTransform.anchoredPosition, width, color);
        }

        private void DrawLine(VertexHelper vh, Vector2 start, Vector2 end, float thickness, Color color)
        {
            Vector2 direction = (end - start).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x) * thickness * 0.5f;

            UIVertex[] verts = new UIVertex[4];
            verts[0].position = start - perpendicular;
            verts[1].position = start + perpendicular;
            verts[2].position = end + perpendicular;
            verts[3].position = end - perpendicular;

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i].color = color;
            }

            vh.AddUIVertexQuad(verts);
        }
    }
}