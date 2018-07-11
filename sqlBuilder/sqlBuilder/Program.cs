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
using System.Diagnostics;
using sqlBuilder.Data.Models.PersonsTableAdapters;
using static sqlBuilder.Data.Models.Persons;

namespace sqlBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            //Log Instance
            Logger Log = new Logger();
            Log.WriteEntry("Iniciando o SQL BUILDER", EventLogEntryType.Information);
            //DataSet Instance
            PersonsTableAdapter dsPersons = new PersonsTableAdapter();

            #region DataBase Test
            //Getting DataTable
            PersonsDataTable dt = null;
            dsPersons.Fill(dt);
            //Generate the insert command from an excel file.
            SqlBuilder.BuildInsertCommand(dt);

            FileManager.WriteText(SqlBuilder.GeneratedSQL.ToString(), AppSettings.SCRIPT_PATH);
            #endregion

            #region Excel Test
            DataTable dataTable = null;
            //Reading Excel and Generate a DataTable
            dataTable = ExcelReader.ReadRows();
            //Setting a name to the Data Table.
            dt.TableName = "dbo.Persons";
            #endregion

            Console.WriteLine("Scripts e Log gerados com sucesso. Aperte uma tecla para finalizar.");
            Console.ReadLine();
        }
    }
}
