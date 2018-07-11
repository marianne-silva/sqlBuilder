using sqlBuilder.Data.Constants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace sqlBuilder.Excel
{
    public static class ExcelReader
    {
        private static OleDbCommand cmd = null;
        private static OleDbConnection connection = null;
        private static OleDbDataAdapter dtAdapter = null;
        private static string GenerateConnection(string fileExtension)
        {
            string connection = string.Empty;

            switch (fileExtension.ToUpper())
            {
                case ".XLS":
                    connection = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=
                                        " + AppSettings.FILE_PATH + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
                    break;
                case ".XLSX":
                    connection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=
                                        " + AppSettings.FILE_PATH + ";Extended Properties=\"Excel 12.0;HDR=YES;\"";
                    break;
                default:
                    connection = string.Empty;
                    break;
            }

            return connection;
        }

        public static DataTable ReadRows()
        {
            DataSet dts = new DataSet();

            string connectionString = GenerateConnection(Path.GetExtension(AppSettings.FILE_PATH));
            DataTable dt = null;
            try
            {
                connection = new OleDbConnection(connectionString);
                connection.Open();
                dtAdapter = new OleDbDataAdapter("select * from [Sheet1$]", connection);
                dt = new DataTable();
                dtAdapter.Fill(dt);
            }
            //TODO Fazer tratativa da exception em um sistema de log.
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dtAdapter != null)
                {
                    dtAdapter.Dispose();
                    dtAdapter = null;
                }

                if(connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
            }

            return dt;
        }
    }
}
