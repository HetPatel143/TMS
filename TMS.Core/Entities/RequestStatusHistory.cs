namespace TMS.Core.Entities
{
    public class RequestStatusHistory
    {
        public int StatusHistoryId { get; set; }
        public int RequestId { get; set; }
        public string OldStatus { get; set; } = null!;
        public string NewStatus { get; set; } = null!;
        public int ChangedByUserId { get; set; }
        public string? ChangeReason { get; set; }
        public DateTime ChangedAt { get; set; }

        public virtual Request Request { get; set; } = null!;
        public virtual User ChangedByUser { get; set; } = null!;
    }
}
