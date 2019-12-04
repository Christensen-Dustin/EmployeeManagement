using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Jean",
                    Department = Dept.HR,
                    Email = "jyyChristensen@gmail.com"
                },
                new Employee
                {
                    Id = 2,
                    Name = "Mark",
                    Department = Dept.Facilities,
                    Email = "markjc@gmail.com"
                },
                new Employee
                {
                    Id = 3,
                    Name = "Dustin",
                    Department = Dept.SE,
                    Email = "dustinsc1977@gmail.com"
                }
             );
        }
    }
}
