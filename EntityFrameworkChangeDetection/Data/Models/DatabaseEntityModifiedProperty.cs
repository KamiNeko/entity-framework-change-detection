namespace Data.Models
{
    public class DatabaseEntityModifiedProperty
    {
        public string PropertyName { get; set; }
        public string TypeName { get; set; }
        public object ValueBeforeChange { get; set; }
        public object ValueAfterChange { get; set; }
    }
}
