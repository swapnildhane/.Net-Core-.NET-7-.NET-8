using Practice2901.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Practice2901.Services
{
    public class DbServices : IDbService
    {
        private readonly string _conn;

        public DbServices(IConfiguration config)
        {
            _conn = config.GetConnectionString("DBCS");

            if (string.IsNullOrEmpty(_conn))
                throw new Exception("Connection string is NULL or NOT LOADED");
        }

        public DataSet ExecuteDataSet(string sp, SqlParameter[] param = null)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sp, con);
            cmd.CommandType = CommandType.StoredProcedure;

            if (param != null)
            {
                foreach (var p in param)
                {
                    cmd.Parameters.Add(p);
                }
            }

            using var adpt = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            adpt.Fill(ds);
            return ds;
        }

        public DataTable ExecuteDataTable(string sp, SqlParameter[] param = null)
        {
            var ds = ExecuteDataSet(sp, param);
            return ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }
    }
}
