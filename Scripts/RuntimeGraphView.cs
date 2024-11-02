using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Dunward.GraphView.Runtime
{
    public class RuntimeGraphView : MonoBehaviour, IScrollHandler, IPointerClickHandler, IDragHandler, IPointerUpHandler
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
        [SerializeField]
        private GameObject selectionBoxPrefab;
#endregion

        protected List<IContextMenuElement> menu = new List<IContextMenuElement>()
        {
            new ContextMenuItem("Copy", () => Debug.Log("Copy")),
            new ContextMenuItem("Paste", () => Debug.Log("Paste")),
            new ContextMenuSeparator(),
            new ContextMenuItem("Delete", () => Debug.Log("Delete"))
        };

        private float minZoom = 0.25f;
        private float maxZoom = 1f;
        private float zoomStep = 0.15f;

        private GameObject _selectionBox;

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (_selectionBox == null)
                {
                    _selectionBox = Instantiate(selectionBoxPrefab, transform);
                }

                var width = eventData.position.x - eventData.pressPosition.x;
                var height = eventData.position.y - eventData.pressPosition.y;

                var x = eventData.pressPosition.x;
                var y = eventData.pressPosition.y;

                if (width < 0)
                {
                    x = eventData.position.x;
                    width = Mathf.Abs(width);
                }

                if (height < 0)
                {
                    y = eventData.position.y;
                    height = Mathf.Abs(height);
                }

                var rect = _selectionBox.transform as RectTransform;
                
                rect.anchoredPosition = new Vector2(x, y);
                rect.sizeDelta = new Vector2(width, height);
            }

            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                viewTransform.anchoredPosition += eventData.delta;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    transform as RectTransform,
                    eventData.position,
                    Camera.main,
                    out Vector2 localPoint);
                
                var contextMenu = Instantiate(contextMenuPrefab, transform).GetComponent<ContextMenu>();
                menu.ForEach(element => contextMenu.AddContextMenuElement(element));
                contextMenu.transform.localPosition = localPoint;
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (_selectionBox != null)
                {
                    Destroy(_selectionBox);
                    _selectionBox = null;
                }
            }
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
    }
}
