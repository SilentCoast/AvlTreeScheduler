﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvlTreeScheduler.Classes
{
    public class MessageEventArgs : EventArgs
    {
        public string Message {  get; set; }
        public string Caption { get; set; }
    }
}
