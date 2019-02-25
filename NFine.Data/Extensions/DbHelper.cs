/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
 

namespace NFine.Data.Extensions
{
    public class DbHelper
    {
        public static string connstring = ConfigurationManager.ConnectionStrings["NFineDbContext"].ConnectionString;
        public static int ExecuteSqlCommand(string cmdText)
        {
            using (DbConnection conn = new SqlConnection(connstring))
            {
                DbCommand cmd = new SqlCommand();
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, null);
                return cmd.ExecuteNonQuery();
            }
        }
        private static void PrepareCommand(DbCommand cmd, DbConnection conn, DbTransaction isOpenTrans, CommandType cmdType, string cmdText, DbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (isOpenTrans != null)
                cmd.Transaction = isOpenTrans;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                cmd.Parameters.AddRange(cmdParms);
            }
        }

        public static DataTable QueryDB(string sql)
        {
            string constr = connstring;
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(constr))
            {
                connection.Open();

                using (SqlDataAdapter command = new SqlDataAdapter(sql, connection))
                {
                    connection.Close();
                    command.SelectCommand.CommandTimeout = 0;
                    command.Fill(dt);

                    return dt;
                }
            }
        }





    }
}
