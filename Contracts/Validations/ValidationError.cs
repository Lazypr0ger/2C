namespace Contracts.Validations
{
    public class ValidationError
    {
        public string Field { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }

        public ValidationError(string field, string code, string message)
        {
            Field = field;
            Code = code;
            Message = message;
        }
    }
}
