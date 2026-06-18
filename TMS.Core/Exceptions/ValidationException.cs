namespace TMS.Core.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> ErrorList { get; }
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(string message) : base(message)
        {
            ErrorList = new List<string> { message };
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(List<string> errors)
            : base(string.Join("; ", errors))
        {
            ErrorList = errors;
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message, IDictionary<string, string[]> errors) : base(message)
        {
            ErrorList = errors.SelectMany(e => e.Value).ToList();
            Errors = errors;
        }
    }
}
