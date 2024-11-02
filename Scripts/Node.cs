using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dunward.GraphView.Runtime
{
    public class Node : MonoBehaviour, IGraphElement, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private GameObject hoverIndicator;
        [SerializeField]
        public GameObject selectionIndicator;

        public RectTransform rectTransform
        {
            get => transform as RectTransform;
        }

        private RuntimeGraphView graphView;

        public void Initialize(RuntimeGraphView graphView)
        {
            this.graphView = graphView;
            GetComponent<CullingTest>().viewPort = graphView.transform as RectTransform;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            rectTransform.anchoredPosition += eventData.delta;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverIndicator.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverIndicator.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Create Node Context Menu
            }
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