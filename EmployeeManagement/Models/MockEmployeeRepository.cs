using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private readonly List<Employee> _employeeList;

        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()
            {
                new Employee() { Id = 1, Name = "Dustin", Department = Dept.SE, Email = "DustinSC1977@gmail.com" },
                new Employee() { Id = 2, Name = "Garrison", Department = Dept.IT, Email = "G-Man08@gmail.com" },
                new Employee() { Id = 3, Name = "Faith", Department = Dept.HR, Email = "Crazy4Faith@gmail.com" }
            };
        }

        public Employee Add(Employee employee)
        {
            employee.Id = _employeeList.Max(e => e.Id) + 1;
            _employeeList.Add(employee);

            return employee;
        }

        public Employee Delete(int id)
        {
            // Finds the employee
            Employee employee = _employeeList.FirstOrDefault(e => e.Id == id);

            // checks to see if the information of the employee is present, not a NULL
            if (employee != null)
            {
                // Removes the employee
                _employeeList.Remove(employee);
            }

            // RETURNS the employee that we have deleted (the location without the information)
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employeeList;
        }

        public Employee GetEmployee(int Id)
        {
            return _employeeList.FirstOrDefault(e => e.Id == Id);
        }

        public Employee Update(Employee employeeChanges)
        {
            // Finds the employee
            Employee employee = _employeeList.FirstOrDefault(e => e.Id == employeeChanges.Id);

            // checks to see if the information of the employee is present, not a NULL
            if (employee != null)
            {
                // Changes the employee information
                employee.Name = employeeChanges.Name;
                employee.Email = employeeChanges.Email;
                employee.Department = employeeChanges.Department;
            }

            // RETURNS the changes in to the employee
            return employee;
        }
    }
}
