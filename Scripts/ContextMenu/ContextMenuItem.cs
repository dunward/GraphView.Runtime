using System;

namespace Dunward.GraphView.Runtime
{
    public class ContextMenuItem : IContextMenuElement
    {
        public string actionName;
        public Action action;
        public Func<bool> predicate;

        public ContextMenuItem(string actionName, System.Action action, Func<bool> predicate)
        {
            this.actionName = actionName;
            this.action = action;
            this.predicate = predicate;
        }
    }
}