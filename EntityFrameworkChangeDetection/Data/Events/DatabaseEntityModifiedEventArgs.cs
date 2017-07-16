using Data.Models;
using System.Collections.Generic;

namespace Data.Events
{
    public class DatabaseEntityModifiedEventArgs : DatabaseEntityEventArgs
    {
        public DatabaseEntityModifiedEventArgs()
        {
            ChangedProperties = new HashSet<DatabaseEntityModifiedProperty>();
        }

        public ICollection<DatabaseEntityModifiedProperty> ChangedProperties { get; set; }
    }
}
