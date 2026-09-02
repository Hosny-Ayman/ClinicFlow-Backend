using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicFlow.Application.Common.Errors
{
    public class SystemErrors
    {

        public static Error Unexpected() => new ("ServerError","An unexpected error occurred.");

    }
}
