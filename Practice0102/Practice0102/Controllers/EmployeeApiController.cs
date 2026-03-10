using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Practice0102.Helper;
using Practice0102.Interfaces;
using Practice0102.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Practice0102.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeApiController : ControllerBase
    {
        Api_CommonResponse Response = new Api_CommonResponse();
        private readonly IDbService _db;

        public EmployeeApiController(IDbService db)
        {
            _db = db;
        }

        // 🔹 GET: api/EmployeeApi/GetEmployeeDetails
        [HttpGet("GetEmployeeDetails")]
        public IActionResult GetEmployeeDetails()
        {
            List<EmployeeModel> list = new List<EmployeeModel>();
            DataSet ds = _db.ExecuteDataSet("[dbo].[GetEmployeeDetailsforView]");

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                list = JsonConvert.DeserializeObject<List<EmployeeModel>>(
                    JsonConvert.SerializeObject(ds.Tables[0])
                );
            }

            return Ok(list);
        }

        // 🔹 POST: api/EmployeeApi/SaveDetails
        [HttpPost("SaveDetails")]
        public IActionResult SaveDetails([FromBody] EmployeeModel Emp)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@sEmpName", Emp.sEmpName),
                new SqlParameter("@sEmpCode", Emp.sEmpCode),
                new SqlParameter("@dtDojDt", Emp.dtDojDt),
                new SqlParameter("@iDeptId", Emp.iDeptId),
                new SqlParameter("@iMarriageStatus", Emp.iMarriageStatus),
                new SqlParameter("@dSalary", Emp.dSalary),
                new SqlParameter("@dTaxAmount", Emp.dTaxAmount),
                new SqlParameter("@sAttachment", Emp.sAttachment)
            };

            DataSet ds = _db.ExecuteDataSet("[dbo].[SaveEmployeeDetails]", param);

            return Ok(new
            {
                Message = "Employee Saved Successfully"
            });
        }
    }
}
