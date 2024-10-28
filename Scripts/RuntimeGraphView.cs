using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dunward.GraphView.Runtime
{
    public class RuntimeGraphView : MonoBehaviour
    {
#region Unity Inspector Fields
        [SerializeField]
        protected RectTransform viewTransform;
        [SerializeField]
        protected RectTransform zoomTransform;

        [Header("Prefabs")]
        [SerializeField]
        private GameObject contextMenuPrefab;
        [SerializeField]
        protected GameObject nodePrefab;
#endregion

        private bool isDragViewer = false;
        private Vector2 lastMousePosition;
        private Vector2 lastViewerPosition;

        private float minZoom = 0.25f;
        private float maxZoom = 1f;
        private float zoomStep = 0.15f;

        public void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                isDragViewer = true;
                lastMousePosition = Input.mousePosition;
                lastViewerPosition = viewTransform.transform.localPosition;
            }

            if (Input.GetMouseButtonUp(2))
            {
                isDragViewer = false;
                lastMousePosition = Vector2.zero;
            }

            if (isDragViewer)
            {
                viewTransform.transform.localPosition = lastViewerPosition + (Vector2)Input.mousePosition - lastMousePosition;
            }

            // Scroll condition
            if (Input.mouseScrollDelta.y != 0 && !isDragViewer)
            {
                var scale = zoomTransform.transform.localScale.x;
                scale += Input.mouseScrollDelta.y > 0 ? zoomStep : -zoomStep;
                scale = Mathf.Clamp(scale, minZoom, maxZoom);
                zoomTransform.transform.localScale = new Vector3(scale, scale, 1);
            }

            if (Input.GetMouseButtonDown(1))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform,
                Input.mousePosition,
                Camera.main,
                out Vector2 localPoint);

                var contextMenu = Instantiate(contextMenuPrefab, transform);
                contextMenu.transform.localPosition = localPoint;
            }
        }
    }
}
