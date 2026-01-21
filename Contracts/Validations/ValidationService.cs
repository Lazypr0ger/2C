using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Contracts.Validations
{
    public class ValidationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ValidateAsync<T>(T model, CancellationToken ct = default)
        {
            var result = new ValidationResult();


            var validators = _serviceProvider.GetServices<IValidator<T>>();

            foreach (var validator in validators)
            {
                var partial = await validator.ValidateAsync(model, ct);
                if (partial != null)
                {
                    result.AddRange(partial.Errors);
                }
            }

            result.ThrowIfInvalid();
        }
    }
}
