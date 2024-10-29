using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dunward.GraphView.Runtime
{
    public class Node : MonoBehaviour, IDragHandler
    {
        private RectTransform rectTransform
        {
            get => transform as RectTransform;
        }

        public void Initialize(RectTransform viewTransform)
        {
            GetComponent<CullingTest>().viewPort = viewTransform;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            rectTransform.anchoredPosition += eventData.delta;
        }

        public void SetPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;
        }

        public void Test(BaseEventData eventData)
        {
            var pointerData = eventData as PointerEventData;
            Debug.Log($"Test {pointerData.button}");
        }
    }
}