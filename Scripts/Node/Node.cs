using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dunward.GraphView.Runtime
{
    public class Node : MonoBehaviour, IGraphElement, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
#region Unity Inspector Fields
        [SerializeField]
        private GameObject hoverIndicator;
        [SerializeField]
        public GameObject selectionIndicator;

        [SerializeField]
        private Transform inputPortContainer;
        [SerializeField]
        private Transform outputPortContainer;

        [Header("Prefabs")]
        [SerializeField]
        private GameObject inputPortPrefab;
        [SerializeField]
        private GameObject outputPortPrefab;
#endregion

        private RuntimeGraphView graphView;
        private NodeModel model;

        public RectTransform rectTransform
        {
            get => transform as RectTransform;
        }

        private List<Port> inputPorts = new List<Port>();
        private List<Port> outputPorts = new List<Port>();

        public void Initialize<T>(RuntimeGraphView graphView) where T : NodeModel
        {
            this.graphView = graphView;
            model = Activator.CreateInstance<T>();
            model.Initialize(this);
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

        public void AddPort(Port.Direction direction, Port.Capacity capacity)
        {
            if (direction == Port.Direction.Input)
            {
                var port = Instantiate(inputPortPrefab, inputPortContainer).GetComponentInChildren<Port>();
                port.Initialize(direction, capacity);
                inputPorts.Add(port);
            }
            else
            {
                var port = Instantiate(outputPortPrefab, outputPortContainer).GetComponentInChildren<Port>();
                port.Initialize(direction, capacity);
                outputPorts.Add(port);
            }
        }
    }
}