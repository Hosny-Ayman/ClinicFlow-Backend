using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicFlow.Application.Common.Errors
{
    public static class ValidationMapping
    {

        public static List<Error> ToErrors(this FluentValidation.Results.ValidationResult result)
        {
            return result.Errors
                .Select(x => new Error("ValidationError", x.ErrorMessage))
                .ToList();
        }



    }
}
