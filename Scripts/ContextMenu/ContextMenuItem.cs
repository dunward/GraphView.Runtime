namespace Dunward.GraphView.Runtime
{
    public class ContextMenuItem : IContextMenuElement
    {
        public string actionName;
        public System.Action action;

        public ContextMenuItem(string actionName, System.Action action)
        {
            this.actionName = actionName;
            this.action = action;
        }
    }
}