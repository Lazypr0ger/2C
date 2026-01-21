using System;
using Contracts.Exceptions;

namespace Contracts.Validations.Extensions
{
    public static class GuardExtensions
    {
        public static void RequireNotNull<T>(this T? value, string fieldName)
            where T : class
        {
            if (value == null)
                throw new ValidationException(fieldName + " is null");
        }

        public static void RequireNotEmpty(this string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(fieldName + " is empty");
        }

        public static void RequireGuid(this string? value, string fieldName)
        {
            value.RequireNotEmpty(fieldName);

            if (!Guid.TryParse(value, out _))
                throw new ValidationException(fieldName + " must be a GUID");
        }

        public static void RequireMaxLength(this string? value, int maxLen, string fieldName)
        {
            if (value == null) return;

            if (value.Length > maxLen)
                throw new ValidationException(fieldName + " length must be <= " + maxLen);
        }

        public static void RequirePositive(this decimal value, string fieldName)
        {
            if (value <= 0m)
                throw new ValidationException(fieldName + " must be > 0");
        }

        public static void RequireNonNegative(this decimal value, string fieldName)
        {
            if (value < 0m)
                throw new ValidationException(fieldName + " must be >= 0");
        }

        public static void RequirePositive(this int value, string fieldName)
        {
            if (value <= 0)
                throw new ValidationException(fieldName + " must be > 0");
        }

        public static void RequireNonNegative(this int value, string fieldName)
        {
            if (value < 0)
                throw new ValidationException(fieldName + " must be >= 0");
        }

        public static void RequireInRange(this DateTime value, DateTime min, DateTime max, string fieldName)
        {
            if (value < min || value > max)
                throw new ValidationException(fieldName + " must be between " + min + " and " + max);
        }
    }
}
