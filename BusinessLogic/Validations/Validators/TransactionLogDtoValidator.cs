using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts.DTO;                    // TransactionLogDto
using Contracts.Exceptions;             // ValidationException
using Contracts.Validations;
using Contracts.Validations.Extensions; // GuardExtensions

namespace BusinessLogic.Validations.Validators
{
    /// <summary>
    /// Валидация проводки/строки журнала проводок.
    /// Цель: не допустить отрицательных сумм и "кривых" связей.
    /// </summary>
    public class TransactionLogDtoValidator : IValidator<TransactionLogDto>
    {
        public Task<ValidationResult> ValidateAsync(TransactionLogDto model, CancellationToken ct = default)
        {
            var r = new ValidationResult();

            if (model == null)
            {
                r.Add("TransactionLog", "null", "TransactionLog is null");
                return Task.FromResult(r);
            }

            // Id (если у тебя при создании генерируется на сервере — можно не требовать,
            // но на update обычно нужен)
            Try(() =>
            {
                if (!string.IsNullOrWhiteSpace(model.Id))
                    model.Id.RequireGuid(nameof(model.Id));
            }, r, nameof(model.Id), "invalid_id");

            // DateOperation
            Try(() =>
            {
                if (model.DateOperation == default(DateTime))
                    throw new ValidationException("DateOperation is empty");
            }, r, nameof(model.DateOperation), "invalid_date");

            // ChartOfAccountDebId / ChartOfAccountCredId
            Try(() => model.ChartOfAccountDebId.RequireGuid(nameof(model.ChartOfAccountDebId)), r, nameof(model.ChartOfAccountDebId), "invalid_account");
            Try(() => model.ChartOfAccountCredId.RequireGuid(nameof(model.ChartOfAccountCredId)), r, nameof(model.ChartOfAccountCredId), "invalid_account");

            // Amounts (1C rule)
            Try(() =>
            {

                model.DebitAmount.RequireNonNegative(nameof(model.DebitAmount));
                model.CreditAmount.RequireNonNegative(nameof(model.CreditAmount));

                if (model.DebitAmount == 0m && model.CreditAmount == 0m)
                    throw new ValidationException("Both DebitAmount and CreditAmount cannot be 0");

                if (model.DebitAmount != model.CreditAmount)
                    throw new ValidationException("DebitAmount must equal CreditAmount for a standard posting");
            }, r, "Amounts", "invalid_amounts");

            // Count
            Try(() =>
            {
                // В журнале проводок count обычно >= 0 (иногда 0 для отклонений)
                model.Count.RequireNonNegative(nameof(model.Count));
            }, r, nameof(model.Count), "invalid_count");

            // Comment length
            Try(() =>
            {
                model.Comment.RequireMaxLength(500, nameof(model.Comment));
            }, r, nameof(model.Comment), "max_length");

            // Subconto lengths (если используешь строки)
            Try(() => model.Subconto1Deb.RequireMaxLength(100, nameof(model.Subconto1Deb)), r, nameof(model.Subconto1Deb), "max_length");
            Try(() => model.Subconto2Deb.RequireMaxLength(100, nameof(model.Subconto2Deb)), r, nameof(model.Subconto2Deb), "max_length");
            Try(() => model.Subconto1Cred.RequireMaxLength(100, nameof(model.Subconto1Cred)), r, nameof(model.Subconto1Cred), "max_length");
            Try(() => model.Subconto2Cred.RequireMaxLength(100, nameof(model.Subconto2Cred)), r, nameof(model.Subconto2Cred), "max_length");

            // OperationId (если есть)
            Try(() =>
            {
                if (!string.IsNullOrWhiteSpace(model.OperationId))
                    model.OperationId.RequireGuid(nameof(model.OperationId));
            }, r, nameof(model.OperationId), "invalid_operation_id");

            return Task.FromResult(r);
        }

        private static void Try(Action action, ValidationResult r, string field, string code)
        {
            try
            {
                action();
            }
            catch (ValidationException ex)
            {
                r.Add(field, code, ex.Message);
            }
        }
    }
}
