namespace TMS.Core.DTOs.RequestTypes
{
    public class CreateFieldDto
    {
        public string FieldName { get; set; } = null!;
        public string FieldLabel { get; set; } = null!;
        public string FieldType { get; set; } = null!;
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
         public string? Options { get; set; }
    }
}
