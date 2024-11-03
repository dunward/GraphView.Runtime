using UnityEngine;
using UnityEngine.EventSystems;

namespace Dunward.GraphView.Runtime
{
    public class Port : MonoBehaviour, IPointerClickHandler, IDragHandler
    {
        public enum Direction
        {
            Input,
            Output
        }

        public enum Capacity
        {
            Single,
            Multi
        }

        public Direction direction { get; private set; }
        public Capacity capacity { get; private set; }

        public void Initialize(Direction direction, Capacity capacity)
        {
            this.direction = direction;
            this.capacity = capacity;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.LogError($"Port Click");
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.LogError($"Port Drag");
        }
    }
}