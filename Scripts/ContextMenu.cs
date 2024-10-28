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

        public void Initialize()
        {
            AddMenu("Copy", () => Debug.Log("Copy"));
            AddMenu("Paste", () => Debug.Log("Paste"));
            AddSeparator();
            AddMenu("Delete", () => Debug.Log("Delete"));
        }

        public void AddMenu(string title, System.Action action)
        {
            var menu = Instantiate(menuPrefab, transform);
            menu.GetComponentInChildren<Text>().text = title;
            menu.GetComponent<Button>().onClick.AddListener(() => action());
        }

        public void AddSeparator()
        {
            Instantiate(separatorPrefab, transform);
        }
    }
}