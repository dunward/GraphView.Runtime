using UnityEngine;

namespace Dunward.GraphView.Runtime
{
    public class Port : MonoBehaviour
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
    }
}