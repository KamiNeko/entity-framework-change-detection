using Data.Models.Interfaces;

namespace Data.Models.Models
{
    public class Contract : IModelWithId
    {
        public long Id { get; set; }
        public string ContractNumber { get; set; }
        public string Note { get; set; }
        public long CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
