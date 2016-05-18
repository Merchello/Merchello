namespace Merchello.Web.Store.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Resources;
    using System.Web.Mvc;

    using Umbraco.Core;

    /// <summary>
    /// Validates an expiration date is in the format mm/yy and that the date has not already past.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class ValidateExpiresDateAttribute : ValidationAttribute, IClientValidatable
    {
        public ValidateExpiresDateAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateExpiresDateAttribute"/> class.
        /// </summary>
        /// <param name="errorMessage">
        /// The error Message.
        /// </param>
        public ValidateExpiresDateAttribute(string errorMessage)
            : base(errorMessage)
        {
        }


        /// <summary>
        /// Gets the client site validation rules.
        /// </summary>
        /// <param name="metadata">
        /// The metadata.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ModelClientValidationRule}"/>.
        /// </returns>=
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            // Construct the value for the JQuery validation adapter
            var validExpiresDate = new ModelClientValidationRule
                {
                    ErrorMessage = this.ErrorMessageString,
                    ValidationType = "validateexpiresdate"   // NOTE: This must be all lower case per spec
                };
            
            yield return validExpiresDate;
        }

        /// <summary>
        /// Does the actual validation.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="validationContext">
        /// The validation context.
        /// </param>
        /// <returns>
        /// The <see cref="ValidationResult"/>.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var result = ValidationResult.Success;

            var expStr = value.ToString();

            // validate the format of the string
            if (expStr.IsNullOrWhiteSpace() || 
                expStr.Length != 5 ||
                !expStr.Contains("/") ||
                expStr.IndexOf('/') != 2) return new ValidationResult(this.ErrorMessageString);

            // validate the pairs can be converted to ints
            var split = expStr.Split('/');
            int mm;
            int yy;
            if (!int.TryParse(split[0], out mm) || !int.TryParse(split[1], out yy))
            {
                return new ValidationResult(this.ErrorMessageString);
            }

            // validate the values
            var yyyy = 2000 + yy;
            if ((mm <= 0 || mm > 12) || 
                (yyyy < DateTime.Today.Year) ||
                (mm < DateTime.Today.Month && yyyy == DateTime.Today.Year))
            {
                return new ValidationResult(this.ErrorMessageString);
            }

            return result;
        }
    }
}