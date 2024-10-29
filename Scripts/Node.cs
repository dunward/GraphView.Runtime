using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dunward.GraphView.Runtime
{
    public class Node : MonoBehaviour
    {
        private RectTransform rectTransform
        {
            get => transform as RectTransform;
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