using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Dunward.GraphView.Runtime
{
    public class RuntimeGraphView : MonoBehaviour, IScrollHandler, IPointerClickHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
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

        private List<Node> _nodes = new List<Node>();
        private List<Edge> _edges = new List<Edge>();

        public IReadOnlyList<Node> nodes
        {
            get => _nodes;
        }

        public IReadOnlyList<Edge> edges
        {
            get => _edges;
        }

        protected List<Node> selection = new List<Node>();

        protected List<IContextMenuElement> menu;

        private float minZoom = 0.25f;
        private float maxZoom = 1f;
        private float zoomStep = 0.15f;

        private GameObject _selectionBox;

        protected virtual void Awake()
        {
            menu = new List<IContextMenuElement>()
            {
                new ContextMenuItem("Copy", () => Debug.Log("Copy"), () => IsCopyAvailable()),
                new ContextMenuItem("Paste", () => Debug.Log("Paste"), () => IsPasteAvailable()),
                new ContextMenuSeparator(),
                new ContextMenuItem("Delete", () => Debug.Log("Delete"), () => IsDeleteAvailable())
            };
        }

#region Inputs
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var width = eventData.position.x - eventData.pressPosition.x;
                var height = eventData.pressPosition.y - eventData.position.y;

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
        {if (eventData.button == PointerEventData.InputButton.Right)
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
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _selectionBox = Instantiate(selectionBoxPrefab, transform);
                
                if (!Input.GetKey(KeyCode.LeftControl))
                {
                    selection.ForEach(node => node.selectionIndicator.SetActive(false));
                    selection.Clear();
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (_selectionBox != null)
                {
                    var rect = _selectionBox.transform as RectTransform;

                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        viewTransform,
                        rect.position,
                        null,
                        out Vector2 localPoint);

                    var selectionBoxLeftTop = localPoint;
                    var selectionBoxRightTop = localPoint + new Vector2(rect.sizeDelta.x, 0);
                    var selectionBoxLeftBottom = localPoint - new Vector2(0, rect.sizeDelta.y);
                    var selectionBoxRightBottom = localPoint + new Vector2(rect.sizeDelta.x, -rect.sizeDelta.y);

                    var selectionArea = (selectionBoxLeftTop, selectionBoxRightTop, selectionBoxLeftBottom, selectionBoxRightBottom);
                    
                    foreach (var node in _nodes)
                    {
                        var nodeLeftTop = node.rectTransform.anchoredPosition;
                        var nodeRightTop = node.rectTransform.anchoredPosition + new Vector2(node.rectTransform.sizeDelta.x, 0);
                        var nodeLeftBottom = node.rectTransform.anchoredPosition - new Vector2(0, node.rectTransform.sizeDelta.y);
                        var nodeRightBottom = node.rectTransform.anchoredPosition + new Vector2(node.rectTransform.sizeDelta.x, -node.rectTransform.sizeDelta.y);

                        if (IsPointInArea(selectionArea, nodeLeftTop) ||
                            IsPointInArea(selectionArea, nodeRightTop) ||
                            IsPointInArea(selectionArea, nodeLeftBottom) ||
                            IsPointInArea(selectionArea, nodeRightBottom))
                        {
                            node.selectionIndicator.SetActive(true);
                            selection.Add(node);
                        }
                    }

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
#endregion

        public void SetViewPosition(Vector2 position)
        {
            viewTransform.anchoredPosition = position;
        }

        public void SetViewScale(float scale)
        {
            zoomTransform.localScale = new Vector3(scale, scale, 1);
        }

        public Node AddNode<T>() where T : NodeModel
        {
            var node = Instantiate(nodePrefab, viewTransform).GetComponent<Node>();
            node.Initialize<T>(this);
            _nodes.Add(node);
            return node;
        }

        private bool IsCopyAvailable()
        {
            return selection.Count > 0;
        }

        private bool IsDeleteAvailable()
        {
            return selection.Count > 0;
        }

        protected virtual bool IsPasteAvailable()
        {
            return GUIUtility.systemCopyBuffer.StartsWith("application/vnd.unity.graphview.elements");
        }

        private bool IsPointInArea((Vector2 leftTop, Vector2 rightTop, Vector2 leftBottom, Vector2 rightBottom)area, Vector2 position)
        {
            return area.leftTop.x < position.x && area.leftTop.y > position.y &&
                   area.rightBottom.x > position.x && area.rightBottom.y < position.y;
        }
    }
}
