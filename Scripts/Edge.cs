using UnityEngine;
using UnityEngine.UI;

namespace Dunward.GraphView.Runtime
{
    public class Edge : MaskableGraphic
    {
        public RectTransform test;

        public RectTransform startNode;
        public RectTransform endNode;

        public float defaultSize = 4f;

        private void Update()
        {
            if (!canvas.enabled) return;
            
            UpdateRectTransform();
            SetVerticesDirty();
        }

        private void UpdateRectTransform()
        {
            Vector2 startPos = startNode.TransformPoint(startNode.anchoredPosition);
            Vector2 endPos = endNode.TransformPoint(endNode.anchoredPosition);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(test.transform as RectTransform, startPos, null, out startPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(test.transform as RectTransform, endPos, null, out endPos);

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
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(test.transform as RectTransform, startPos, null, out startPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(test.transform as RectTransform, endPos, null, out endPos);

            DrawEdge(vh, startPos, endPos, 45f, color, defaultSize);
        }

        private void DrawEdge(VertexHelper vh, Vector2 startPos, Vector2 endPos, float minimumLength, Color color, float width)
        {
            var diagonalStart = startPos + new Vector2(minimumLength, 0);
            DrawLine(vh, startPos - rectTransform.anchoredPosition, diagonalStart - rectTransform.anchoredPosition, width, color);
            var diagonalEnd = endPos + new Vector2(-minimumLength, 0);
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