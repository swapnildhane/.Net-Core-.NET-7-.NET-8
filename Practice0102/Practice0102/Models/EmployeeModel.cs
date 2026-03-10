using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Practice0102.Models
{
    public class EmployeeModel
    {
        public string? sEmpName { get; set; }
        public string? sEmpCode { get; set; }
        public string? dtDojDt { get; set; }
        public string? Dept { get; set; }
        public string? MarriageStatus { get; set; }
        public int iDeptId { get; set; }
        public int iMarriageStatus { get; set; }
        public decimal dSalary { get; set; }
        public decimal dTaxAmount { get; set; }
        public string? sAttachment { get; set; }
    }
}