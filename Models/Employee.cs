using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace visitor_MVC.Models
{
    public class Employee
    {
        [Column("Department")]
        public string Department { get; set; }
        [Column("DepartmentNo")]
        public string DepartmentNo { get; set; }
        [Column("EmployeeID")]
        public string EmployeeID { get; set; }
        [Column("EmployeeName")]
        public string EmployeeName { get; set; }
        [Column("ExtensionNo")]
        public string ExtensionNo { get; set; }
        [Column("Email")]
        public string Email { get; set; }
        [Column("Permissions")]
        public string Permissions { get; set; }
    }
}