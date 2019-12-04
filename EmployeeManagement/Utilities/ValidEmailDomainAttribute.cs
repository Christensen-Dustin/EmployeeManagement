using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Utilities
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string allowedDomain;

        public ValidEmailDomainAttribute(string allowedDomain)
        {
            this.allowedDomain = allowedDomain;
        }

        public override bool IsValid(object value)
        {
            // The domain name, by its self
            string[] strings = value.ToString().Split("@");

            // convert to upper to eliminate case sensitivity and compare
            return strings[1].ToUpper() == allowedDomain.ToUpper();
        }
    }
}
