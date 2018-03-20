using System;

using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Forms
{
    public delegate void TemplateEventHandler(object sender, TemplateEventArgs e);
    public delegate void TemplateItemEventHandler(object sender, TemplateItemEventArgs e);

    public class TemplateEventArgs : EventArgs
    {
        private object _commandArg;
        private string _commandName;

        public string CommandName
        {
            get { return _commandName; }
            set { _commandName = value; }
        }

        public object CommandArgument
        {
            get { return _commandArg; }
            set { _commandArg = value; }
        }
    }

    public class TemplateItemEventArgs : TemplateEventArgs
    {
        private ItemData _item;

        public TemplateItemEventArgs(ItemData item)
        {
            _item = item;
        }

        public ItemData Item
        {
            get { return _item; }
        }

        
    }
}
