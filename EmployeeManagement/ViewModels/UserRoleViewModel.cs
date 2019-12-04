using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    // This class is to connect the role class and the employee class
    public class UserRoleViewModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public bool IsSelected { get; set; }
    }
}
