using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Practice2901.Interfaces;

namespace Practice2901.Controllers
{
    public class Employee : Controller
    {
        private readonly IDbService _db;
        public Employee(IDbService db)
        {
            _db = db;
        }

        public IActionResult Login_User()
        {
            return View();
        }
        public IActionResult EmployeeDetails()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(string Username, string Password)
        {
            

            SqlParameter[] param = new SqlParameter[2];
            param[0] = new SqlParameter("@username", Username);
            param[1] = new SqlParameter("@password", Password);

            var ds = _db.ExecuteDataSet("CheckEmployeeLogin", param);

            if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return Json(ds);
            }

            return Json(ds);
        }

    }
}
