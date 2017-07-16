using Data.Models.Interfaces;
using System;

namespace Data.Events
{
    public class DatabaseEntityEventArgs : EventArgs
    {
        public string ModelTypeName { get; set; }
        public IModelWithId Model { get; set; }
    }
}