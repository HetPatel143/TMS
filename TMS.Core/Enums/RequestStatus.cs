namespace TMS.Core.Enums
{
    /// <summary>
    /// Defines the possible statuses for a request throughout its lifecycle.
    /// </summary>
    public enum RequestStatus
    {
        Draft = 1,
        Submitted = 2,
        InApproval = 3,
        Approved = 4,
        Rejected = 5,
        Cancelled = 6,
        Closed = 7
    }
}
