using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Exceptions;

namespace Contracts.Validations
{
    public class ValidationResult
    {
        public List<ValidationError> Errors { get; } = new List<ValidationError>();

        public bool IsValid
        {
            get { return Errors.Count == 0; }
        }

        public void Add(string field, string code, string message)
        {
            Errors.Add(new ValidationError(field, code, message));
        }

        public void AddRange(IEnumerable<ValidationError> errors)
        {
            if (errors == null) return;
            Errors.AddRange(errors);
        }

        public void ThrowIfInvalid()
        {
            if (IsValid) return;

            // Сообщение компактное, но информативное
            var msg = string.Join("; ", Errors.Select(e => e.Field + ": " + e.Message));
            throw new ValidationException(msg);
        }
    }
}
