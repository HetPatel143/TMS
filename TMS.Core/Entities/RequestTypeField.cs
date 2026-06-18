namespace TMS.Core.Entities
{
    public class RequestTypeField
    {
        public int FieldId { get; set; }
        public int RequestTypeId { get; set; }
        public string FieldName { get; set; } = null!;
        public string FieldLabel { get; set; } = null!;
        public string FieldType { get; set; } = null!;
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
         public string? Options { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual RequestType RequestType { get; set; } = null!;
        public virtual ICollection<RequestFieldValue> FieldValues { get; set; } = new List<RequestFieldValue>();
    }
}
