using System;

namespace AvlTreeScheduler.Classes
{
    public class MessageEventArgs : EventArgs
    {
        public string Message {  get; set; }
        public string Caption { get; set; }
    }
}
