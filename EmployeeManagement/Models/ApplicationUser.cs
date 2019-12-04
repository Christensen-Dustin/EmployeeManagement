using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    // Extending the IdentityUser Class by adding City.
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
    }
}
