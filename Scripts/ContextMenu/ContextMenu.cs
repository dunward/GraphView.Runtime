using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dunward.GraphView.Runtime
{
    public class ContextMenu : MonoBehaviour
    {
#region Unity Inspector Fields
        [Header("Prefabs")]
        [SerializeField]
        private GameObject menuPrefab;
        [SerializeField]
        private GameObject separatorPrefab;
#endregion

        public void AddMenuItem(string title, System.Action action, bool interactable)
        {
            var menu = Instantiate(menuPrefab, transform);
            menu.GetComponentInChildren<Text>().text = title;
            menu.GetComponentInChildren<Text>().color = interactable ? Color.black : Color.gray;
            menu.GetComponent<Button>().onClick.AddListener(() => action());
            menu.GetComponent<Button>().interactable = interactable;
        }

        public void AddSeparator()
        {
            Instantiate(separatorPrefab, transform);
        }

        public void AddContextMenuElement(IContextMenuElement element)
        {
            if (element is ContextMenuItem item)
            {
                AddMenuItem(item.actionName, item.action, item.predicate.Invoke());
            }
            else if (element is ContextMenuSeparator)
            {
                AddSeparator();
            }
        }

        private void LateUpdate()
        {
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
                Destroy(gameObject);

            if (Input.GetMouseButtonDown(1) && !IsPointerOverUIObject())
                Destroy(gameObject);
        }

        private bool IsPointerOverUIObject()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var result in results)
            {
                if (result.gameObject == gameObject || result.gameObject.transform.IsChildOf(transform))
                {
                    return true;
                }
            }
            return false;
        }
    }
}