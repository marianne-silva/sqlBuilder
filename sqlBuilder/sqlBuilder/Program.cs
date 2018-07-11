using sqlBuilder.Data.Constants;
using sqlBuilder.Data.Models;
using sqlBuilder.Excel;
using sqlBuilder.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using sqlBuilder.Log;

namespace sqlBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            //Log Instance
            Logger log = new Logger();
            log.WriteEntry("Iniciando o SQL BUILDER");
            Persons dsPersons = new Persons();
            List<string> text = new List<string>();
            DataTable dt = ExcelReader.ReadRows();
            dt.TableName = "dbo.Persons";
            string fields = SqlBuilder.GenerateFields(dt);
            //Console.WriteLine(fields);
            string sqlCommand = SqlBuilder.BuildInsert(dt);
            //Console.WriteLine(sqlCommand);
            var s = "Data Source=DESKTOP-7E4JGAN;Initial Catalog=master;Integrated Security=True";
            StringBuilder st = new StringBuilder();
            foreach (DataRow item in dt.Rows)
            {
                SqlCommand v = null;
                var insert = SqlBuilder.BuildInsertCommand(item);
                foreach (DataColumn t in dt.Columns)
                {
                     v = SqlBuilder.InsertParameter(insert, t.ColumnName, t.ColumnName, item[t]);
                }

                foreach (SqlParameter r in v.Parameters)
                {
                   v.CommandText= v.CommandText.Replace("@"+r.ParameterName, "'" + r.Value.ToString() + "'");
                }
                st.Append(v.CommandText);
                text.Add(v.CommandText);

            }
            FileManager.CreateFile(text);
            //Console.WriteLine(st.ToString());
            Console.ReadLine();
        }
    }
}
