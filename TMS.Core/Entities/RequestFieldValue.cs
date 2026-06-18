namespace TMS.Core.Entities
{
    public class RequestFieldValue
    {
        public int FieldValueId { get; set; }
        public int RequestId { get; set; }
        public int FieldId { get; set; }
        public string FieldValue { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public virtual Request Request { get; set; } = null!;
        public virtual RequestTypeField Field { get; set; } = null!;
    }
}
