using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Services.Helpers
{
    public class ValidationHelper
    {
        internal static void ModelValidation (object model)
        {
            ValidationContext validationContext = new ValidationContext(model);
            List<ValidationResult> errors = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(model, validationContext, errors, true);
            if (!isValid)
            {
                throw new ArgumentException(errors.FirstOrDefault()?.ErrorMessage);
            }
        }
    }
}
