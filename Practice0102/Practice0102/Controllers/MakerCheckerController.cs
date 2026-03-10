using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Practice0102.Helper;
using Practice0102.Interfaces;
using Practice0102.Models;
using System.Data;

namespace Practice0102.Controllers
{
    public class MakerCheckerController : Controller
    {
        Api_CommonResponse Response = new Api_CommonResponse();
        private readonly IDbService _db;
        public MakerCheckerController(IDbService db) {
            _db = db;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UserLogin(string Username, string Password)
        {
            SqlParameter[] param = new SqlParameter[2];
            param[0] = new SqlParameter("@Username", Username);
            param[1] = new SqlParameter("@Password", Password);

            var ds = _db.ExecuteDataSet("USP_checkLoginDetails", param);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var statusCode = Convert.ToInt32(ds.Tables[0].Rows[0]["StatusCode"]);
                var role = ds.Tables[0].Rows[0]["Role"].ToString();

                if (statusCode == 200)
                {
                    if (role == "M")
                    {
                        return RedirectToAction("Maker");
                    }
                    else if (role == "C")
                    {
                        return RedirectToAction("Checker");
                    }
                }
            }

            return RedirectToAction("Login");
        }


        public IActionResult Checker()
        {
            ViewBag.CheckerDetails = GetCheckerDetails();
            return View();
        }

        [HttpPost]
        public IActionResult SearchData(string empName, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                SqlParameter[] param = new SqlParameter[3]; 
                param[0] = new SqlParameter("@EmpName", string.IsNullOrEmpty(empName) ? (object)DBNull.Value : empName); 
                param[1] = new SqlParameter("@FromDate", fromDate.HasValue ? fromDate.Value : (object)DBNull.Value); 
                param[2] = new SqlParameter("@ToDate", toDate.HasValue ? toDate.Value : (object)DBNull.Value); 
                DataSet ds = _db.ExecuteDataSet("[dbo].[USP_CheckerSearchData]", param); 
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Response.statusCode = 200;
                    Response.message = "Data fetched successfully";
                    Response.data = JsonConvert.SerializeObject(ds.Tables[0]); 
                    var list = JsonConvert.DeserializeObject<List<MakerCheckerModel>>(Response.data.ToString()); 
                    return PartialView("_CheckerGridRows", list); 
                   
                } else 
                { 
                    return PartialView("_CheckerGridRows", new List<MakerCheckerModel>());
                } 
            } catch (Exception ex) 
            { 
                return StatusCode(500, ex.Message);
            }
        } 
        
        [HttpPost]
        public IActionResult UpdateStatus(int id, int action)
        {
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@Id", id);
                param[1] = new SqlParameter("@Action", action);

                DataSet ds = _db.ExecuteDataSet("[dbo].[USP_CheckerUpdateStatus]", param);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Response.statusCode = Convert.ToInt32(ds.Tables[0].Rows[0]["StatusCode"]);
                    Response.message = ds.Tables[0].Rows[0]["Message"].ToString();

                }
                else
                {
                    Response.statusCode = 500;
                    Response.message = "Unable to update status";
                }
                return Json(new
                {
                    StatusCode = Response.statusCode,
                    Message = Response.message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        StatusCode = 500,
                        Message = ex.Message
                    });
            }
        }
        public IActionResult Maker()
        {
            ViewBag.MakerDetails = GetMakerDetails();
            return View();
        }

        [HttpPost]
        public IActionResult SaveDetails([FromBody] MakerCheckerModel Emp)
        {
            try
            {
                SqlParameter[] param = new SqlParameter[6];
                param[0] = new SqlParameter("@sEmpName", Emp.sEmpName);
                param[1] = new SqlParameter("@sPanNo", Emp.sPanNo);
                param[2] = new SqlParameter("@dMnthlyIncm", Emp.dMnthlyIncm);
                param[3] = new SqlParameter("@sMbleNo", Emp.sMbleNo);
                param[4] = new SqlParameter("@iMrgSts", Emp.iMrgSts);
                param[5] = new SqlParameter("@sAdrs", Emp.sAdrs);
               

                DataSet ds = _db.ExecuteDataSet("[dbo].[USP_SaveMakerDetails]", param);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Response.responseCode = 0;
                    Response.message = ds.Tables[0].Rows[0]["Message"].ToString();
                    Response.statusCode = Convert.ToInt32(ds.Tables[0].Rows[0]["StatusCode"]);
                }
                else
                {
                    Response.responseCode = 1;
                    Response.message = "We are facing server issue at the moment please try again...";
                    Response.statusCode = -1;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return Json(new
            {
                StatusCode = Response.statusCode,
                Data = Response,
                Failure = false,
                Message = Response.message
            });
        }

        public List<MakerCheckerModel> GetMakerDetails()
        {
            List<MakerCheckerModel> MakerList = new List<MakerCheckerModel>();
            DataSet ds = _db.ExecuteDataSet("[dbo].[GetMakerDetailsforView]");
            if (ds != null && ds.Tables != null && ds.Tables[0].Rows.Count > 0)
            {
                Response.message = "Maker List";
                Response.statusCode = 200;
                Response.data = JsonConvert.SerializeObject(ds.Tables[0]);
                Response.responseCode = 1;
                if (Response.data != null)
                    MakerList = JsonConvert.DeserializeObject<List<MakerCheckerModel>>(Response.data.ToString());
            }
            else
            {
                Response.message = "No Data Available";
                Response.statusCode = 1;
                Response.responseCode = 1;
            }
            return MakerList;
        }

        public List<MakerCheckerModel> GetCheckerDetails()
        {
            List<MakerCheckerModel> MakerList = new List<MakerCheckerModel>();
            DataSet ds = _db.ExecuteDataSet("[dbo].[GetCheckerDetailsforView]");
            if (ds != null && ds.Tables != null && ds.Tables[0].Rows.Count > 0)
            {
                Response.message = "Maker List";
                Response.statusCode = 200;
                Response.data = JsonConvert.SerializeObject(ds.Tables[0]);
                Response.responseCode = 1;
                if (Response.data != null)
                    MakerList = JsonConvert.DeserializeObject<List<MakerCheckerModel>>(Response.data.ToString());
            }
            else
            {
                Response.message = "No Data Available";
                Response.statusCode = 1;
                Response.responseCode = 1;
            }
            return MakerList;
        }


    }
}
