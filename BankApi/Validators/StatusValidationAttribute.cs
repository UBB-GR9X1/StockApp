namespace BankApi.Validators
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using BankApi.Models;

    /// <summary>
    /// Validates that a given value matches a valid <see cref="Status"/> enumeration value.
    /// </summary>
    public class StatusValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Determines whether the specified value is valid.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns><c>true</c> if the value is a valid <see cref="Status"/>; otherwise, <c>false</c>.</returns>
        public override bool IsValid(object? value)
        {
            if (value is string statusString)
            {
                return Enum.GetNames<Status>().Contains(statusString);
            }

            return false;
        }
    }
}
