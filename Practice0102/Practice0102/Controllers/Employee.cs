using Azure;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Practice0102.Helper;
using Practice0102.Interfaces;
using Practice0102.Models;
using System.Data;
using System.Text;

namespace Practice0102.Controllers
{
    public class Employee : Controller
    {

        #region Without API Controller call code 
        Api_CommonResponse Response = new Api_CommonResponse();

        private readonly IDbService _db;
        private readonly IWebHostEnvironment _env;

        public Employee(IDbService db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public ActionResult EmployeeDetails()
        {
            ViewBag.EmployeeDetails = GetEmployeeDetails();
            return View();
        }
        
        [HttpPost]
        public IActionResult SaveDetails([FromBody] EmployeeModel Emp)
        {
            try
            {
                SqlParameter[] param = new SqlParameter[8];
                param[0] = new SqlParameter("@sEmpName", Emp.sEmpName);
                param[1] = new SqlParameter("@sEmpCode", Emp.sEmpCode);
                param[2] = new SqlParameter("@dtDojDt", Emp.dtDojDt);
                param[3] = new SqlParameter("@iDeptId", Emp.iDeptId);
                param[4] = new SqlParameter("@iMarriageStatus", Emp.iMarriageStatus);
                param[5] = new SqlParameter("@dSalary", Emp.dSalary);
                param[6] = new SqlParameter("@dTaxAmount", Emp.dTaxAmount);
                param[7] = new SqlParameter("@sAttachment", Emp.sAttachment);

                DataSet ds = _db.ExecuteDataSet("[dbo].[SaveEmployeeDetails]", param);

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

        
        public List<EmployeeModel> GetEmployeeDetails()
        {
            List<EmployeeModel> Route = new List<EmployeeModel>();
            DataSet ds = _db.ExecuteDataSet("[dbo].[GetEmployeeDetailsforView]");
            if (ds != null && ds.Tables != null && ds.Tables[0].Rows.Count > 0)
            {
                Response.message = "Employee List";
                Response.statusCode = 200;
                Response.data = JsonConvert.SerializeObject(ds.Tables[0]);
                Response.responseCode = 1;
                if (Response.data != null)
                    Route = JsonConvert.DeserializeObject<List<EmployeeModel>>(Response.data.ToString());
            }
            else
            {
                Response.message = "No Data Available";
                Response.statusCode = 1;
                Response.responseCode = 1;
            }
            return Route;
        }

        #endregion

        #region With API Controller Call Code

        //private readonly IHttpClientFactory _httpClientFactory;

        //public Employee(IHttpClientFactory httpClientFactory)
        //{
        //    _httpClientFactory = httpClientFactory;
        //}
        //[HttpPost]
        //public async Task<IActionResult> SaveDetails([FromBody] EmployeeModel model)
        //{
        //    var client = _httpClientFactory.CreateClient();

        //    await client.PostAsJsonAsync("https://localhost:7247/api/EmployeeApi/SaveDetails", model);

        //    return RedirectToAction("EmployeeDetails");
        //}



        //public async Task<IActionResult> EmployeeDetails()
        //{
        //    var client = _httpClientFactory.CreateClient();
        //    var response = await client.GetAsync("https://localhost:7247/api/EmployeeApi/GetEmployeeDetails");

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var json = await response.Content.ReadAsStringAsync();
        //        var list = JsonConvert.DeserializeObject<List<EmployeeModel>>(json);
        //        ViewBag.EmployeeDetails = list;
        //    }

        //    return View();
        //}

        #endregion

        #region File Store 
        //-- File Store
        [HttpPost]
        public async Task<IActionResult> EmployeeFileStore(IFormFile FormAttachment, string GUID)
        {
            try
            {
                string uploadPath = Path.Combine(_env.WebRootPath, "Docs", "EmployeeDetails");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                string savedPath = "";

                if (FormAttachment != null && FormAttachment.Length > 0)
                {
                    string fileName = $"{GUID}_{Path.GetFileName(FormAttachment.FileName)}";
                    string fullPath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await FormAttachment.CopyToAsync(stream);
                    }

                    savedPath = fullPath;
                }


                return Json(new { StatusCode = 200, Message = savedPath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message });
            }
        }
        #endregion

        #region Export to Excel


        //-- Export to Excel through SP
        public ActionResult EmployeeExportToExcel()
        {
            try
            {
                //SqlParameter[] param = new SqlParameter[2];
                //param[0] = new SqlParameter("@ToDate", ToDate);
                //param[1] = new SqlParameter("@Format", Format);

                DataSet ds = _db.ExecuteDataSet("[dbo].[USP_ExployeeExportData_Get]");
                DataTable dt = ds.Tables[0];

                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Sheet1");
                    ws.Cell(1, 1).InsertTable(dt);
                    ws.Cells().Style.Protection.Locked = true;
                    ws.Protect("1234567890");

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        stream.Flush();
                        stream.Seek(0, SeekOrigin.Begin);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeExport.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                throw;
            }
        }



        //----- CSV
        public ActionResult EmployeeExportToCsv()
        {
            try
            {
                DataSet ds = _db.ExecuteDataSet("[dbo].[USP_ExployeeExportData_Get]");
                DataTable dt = ds.Tables[0];

                StringBuilder sb = new StringBuilder();

                // Header row
                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>()
                                                .Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));

                // Data rows
                foreach (DataRow row in dt.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field =>
                        "\"" + field.ToString().Replace("\"", "\"\"") + "\""
                    );
                    sb.AppendLine(string.Join(",", fields));
                }

                byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());

                return File(buffer, "text/csv", "EmployeeExport.csv");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                throw;
            }
        }

        #endregion
    }
}