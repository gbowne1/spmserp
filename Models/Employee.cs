using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace spmserp.Models
{
    public class Employee
    {
        public int id { get; set; }
        private string name { get; set; }
        private double _salary;
        Employee(int id, string name, int age, double sal)
        {
            Console.WriteLine("Constructor for Employee called");
            _empId = id;
            _empName = name;
            _age = age;
            _salary = sal;
        }

        ~Employee()
        {
            Console.WriteLine("Destructor for Employee called");
        }

        static void Main(string[] args)
        {
            Employee objEmp = new Employee(1,"John",45, 3500);
        }
    }
}