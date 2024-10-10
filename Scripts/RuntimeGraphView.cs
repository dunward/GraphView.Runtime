using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dunward.GraphView.Runtime
{
    public class RuntimeGraphView : MonoBehaviour
    {
#region Unity Inspector Fields
        [SerializeField]
        private GameObject viewer;
#endregion

        private bool isDragViewer = false;
        private Vector2 lastMousePosition;
        private Vector2 lastViewerPosition;

        public void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                isDragViewer = true;
                lastMousePosition = Input.mousePosition;
                lastViewerPosition = viewer.transform.localPosition;
            }

            if (Input.GetMouseButtonUp(2))
            {
                isDragViewer = false;
                lastMousePosition = Vector2.zero;
            }

            if (isDragViewer)
            {
                viewer.transform.localPosition = lastViewerPosition + (Vector2)Input.mousePosition - lastMousePosition;
            }
        }
    }
}
