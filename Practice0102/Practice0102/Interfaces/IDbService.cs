using Microsoft.Data.SqlClient;
using System.Data;

namespace Practice0102.Interfaces
{
    public interface IDbService
    {
        public DataSet ExecuteDataSet(string sp, SqlParameter[] param = null);
        public DataTable ExecuteDataTable(string sp, SqlParameter[] param = null);
    }
}

