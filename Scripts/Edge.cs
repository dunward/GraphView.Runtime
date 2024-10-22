using UnityEngine;
using UnityEngine.UI;

namespace Dunward.GraphView.Runtime
{
    public class Edge : MaskableGraphic
    {
        public RectTransform startNode;
        public RectTransform endNode;
        public float defaultSize = 5f;

        private void Update()
        {
            UpdateRectTransform();
            SetVerticesDirty();
        }

        private void UpdateRectTransform()
        {
            Vector2 startPos = startNode.anchoredPosition;
            Vector2 endPos = endNode.anchoredPosition;

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

            Vector2 startPos = startNode.anchoredPosition;
            Vector2 endPos = endNode.anchoredPosition;

            var (x, y) = ResolveTangents(startPos, endPos, 0.8f);
            DrawBezier(vh, startPos, endPos, x, y, color, defaultSize);
        }

        private void DrawBezier(VertexHelper vh, Vector2 startPos, Vector2 endPos, Vector2 startTangent, Vector2 endTangent, Color color, float width)
        {
            const int segmentCount = 20;
            Vector2 prevPoint = startPos;

            for (int i = 1; i <= segmentCount; i++)
            {
                float t = i / (float)segmentCount;
                Vector2 currentPoint = CalculateBezierPoint(t, startPos, startTangent, endPos, endTangent);
                DrawLine(vh, prevPoint - rectTransform.anchoredPosition, currentPoint - rectTransform.anchoredPosition, width, color);
                prevPoint = currentPoint;
            }
        }

        private Vector2 CalculateBezierPoint(float t, Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            return uu * start + 2 * u * t * startTangent + tt * endTangent + tt * end;
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
    
        public static (Vector2, Vector2) ResolveTangents(Vector2 from, Vector2 to, float rigidMlt)
        {
            var fromRect = new Rect(0, 0, 1, 1);
            var toRect = new Rect(0, 0, 1, 1);
            fromRect.center = from;
            toRect.center = to;

            var tangentX = Mathf.Abs(from.x - to.x) * rigidMlt;
            var tangentY = Mathf.Abs(from.y - to.y) * rigidMlt;

            var x = new Vector2(
                from.x <= fromRect.xMin ? -tangentX : (from.x >= fromRect.xMax ? tangentX : 0),
                from.y <= fromRect.yMin ? -tangentY : (from.y >= fromRect.yMax ? tangentY : 0)
            );

            var y = new Vector2(
                to.x <= toRect.xMin ? -tangentX : (to.x >= toRect.xMax ? tangentX : 0),
                to.y <= toRect.yMin ? -tangentY : (to.y >= toRect.yMax ? tangentY : 0)
            );
            
            return (x, y);
        }
    }
}