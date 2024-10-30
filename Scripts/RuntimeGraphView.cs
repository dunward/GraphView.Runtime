using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Dunward.GraphView.Runtime
{
    public class RuntimeGraphView : MonoBehaviour, IScrollHandler, IPointerClickHandler, IDragHandler
    {
#region Unity Inspector Fields
        [SerializeField]
        private RectTransform viewTransform;
        [SerializeField]
        private RectTransform zoomTransform;

        [Header("Prefabs")]
        [SerializeField]
        private GameObject contextMenuPrefab;
        [SerializeField]
        private GameObject nodePrefab;
#endregion

        private List<IContextMenuElement> contextElements = new List<IContextMenuElement>();

        private float minZoom = 0.25f;
        private float maxZoom = 1f;
        private float zoomStep = 0.15f;

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Middle) return;
            viewTransform.anchoredPosition += eventData.delta;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform,
                eventData.position,
                Camera.main,
                out Vector2 localPoint);
            
            var contextMenu = Instantiate(contextMenuPrefab, transform).GetComponent<ContextMenu>();
            contextMenu.Initialize();
            contextElements.ForEach(element => contextMenu.AddContextMenuElement(element));
            contextMenu.transform.localPosition = localPoint;
        }

        public void OnScroll(PointerEventData eventData)
        {
            var scale = zoomTransform.transform.localScale.x;
            scale += eventData.scrollDelta.y > 0 ? zoomStep : -zoomStep;
            scale = Mathf.Clamp(scale, minZoom, maxZoom);
            zoomTransform.transform.localScale = new Vector3(scale, scale, 1);
        }

        public void SetViewPosition(Vector2 position)
        {
            viewTransform.anchoredPosition = position;
        }

        public void SetViewScale(float scale)
        {
            zoomTransform.localScale = new Vector3(scale, scale, 1);
        }

        public Node AddNode()
        {
            var node = Instantiate(nodePrefab, viewTransform).GetComponent<Node>();
            node.Initialize(viewTransform.parent as RectTransform);
            return node;
        }

        public void AddContextMenuElement(IContextMenuElement element)
        {
            contextElements.Add(element);
        }
    }
}
