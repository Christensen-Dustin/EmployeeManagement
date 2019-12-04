using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext context;
        private readonly ILogger<SQLEmployeeRepository> logger;

        // Constructor to include, inject AppDbContext Class
        public SQLEmployeeRepository(AppDbContext context,
                                     ILogger<SQLEmployeeRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public Employee Add(Employee employee)
        {
            // Adds new employee to Employee Context
            context.Employees.Add(employee);

            // Saves changes to Employee Context
            context.SaveChanges();

            // RETURN add employee object
            return employee;
        }

        public Employee Delete(int id)
        {
            // Locate the Employee (via ID) to be deleted
            Employee employee = context.Employees.Find(id);

            // Verify that employee information is present
            if(employee != null)
            {
                // Remove the employee information
                context.Employees.Remove(employee);
                // Save changes made to the context
                context.SaveChanges();
            }

            // RETURN deleted employee object
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            // RETURN all employees
            return context.Employees;
        }

        public Employee GetEmployee(int Id)
        {
            // Logging types
            logger.LogTrace("Trace Log");
            logger.LogDebug("Debug Log");
            logger.LogInformation("Information Log");
            logger.LogWarning("Warning Log");
            logger.LogError("Error Log");
            logger.LogCritical("Critical Log");

            // Find and Return the indicated employee (ID)
            return context.Employees.Find(Id);
        }

        public Employee Update(Employee employeeChanges)
        {
            // Save changes to a variable
            var employee = context.Employees.Attach(employeeChanges);

            // Indicate that employee information has been modified
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            // Save changes to the Employee Context
            context.SaveChanges();

            // Return the updated employee object
            return employeeChanges;
        }
    }
}
