using Data.Models.Interfaces;
using System.Collections.Generic;

namespace Data.Models.Models
{
    public class Customer : IModelWithId
    {
        public Customer()
        {
            this.Contracts = new HashSet<Contract>();
        }

        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public ICollection<Contract> Contracts { get; set; }
    }
}
