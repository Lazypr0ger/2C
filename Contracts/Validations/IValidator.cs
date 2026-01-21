using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Validations
{
    public interface IValidator<T>
    {
        Task<ValidationResult> ValidateAsync(T model, CancellationToken ct = default);
    }
}
