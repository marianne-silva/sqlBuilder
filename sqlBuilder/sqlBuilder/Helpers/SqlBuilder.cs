using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace sqlBuilder.Helpers
{
    public static class SqlBuilder
    {
        public static StringBuilder GeneratedSQL = new StringBuilder();
        /// <summary>
        /// Method that generates the fields of a datatable based on his columns' name.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static string GenerateFields(DataTable dataTable)
        {
            string fields = string.Empty;

            foreach (DataColumn dtColumn in dataTable.Columns)
            {
                if (fields.Length > 0) { fields += ","; }
                fields += dtColumn.ColumnName;
            }
            return fields;           
        }

        /// <summary>
        /// Method that inserts a parameter into a sql command.
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="pamName"></param>
        /// <param name="srcColumn"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlCommand InsertParameter(SqlCommand sqlCommand, string pamName, string srcColumn, object value)
        {
            SqlParameter sqlParameter = new SqlParameter(pamName, value);
            sqlParameter.Direction = ParameterDirection.Input;
            sqlParameter.ParameterName = pamName;
            sqlParameter.SourceColumn = srcColumn;
            sqlParameter.SourceVersion = DataRowVersion.Current;

            sqlCommand.Parameters.Add(sqlParameter);

            return sqlCommand;
        }


        /// <summary>
        /// Method that return an insert command built in with parameters.
        /// </summary>
        /// <param name="dtRow"></param>
        /// <returns></returns>
        public static void BuildInsertCommand(DataTable dataTable)
        {
            string sql = BuildInsert(dataTable);
            SqlCommand sqlCommand = null;

            foreach (DataRow row in dataTable.Rows)
            {
                sqlCommand = new SqlCommand(sql);
                sqlCommand.CommandType = CommandType.Text;

                foreach (DataColumn dtColumn in dataTable.Columns)
                {
                    sqlCommand = InsertParameter(sqlCommand, dtColumn.ColumnName, dtColumn.ColumnName, row[dtColumn.ColumnName]);
                }

                foreach (SqlParameter parameter in sqlCommand.Parameters)
                {
                    sqlCommand.CommandText = sqlCommand.CommandText.Replace("@" + parameter.ParameterName, "'" + parameter.Value.ToString() + "'");
                }

                GeneratedSQL.Append(sqlCommand.CommandText);
                GeneratedSQL.AppendLine("");
            }

        }   
        /// <summary>
        /// Methot that builds the insert.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static string BuildInsert(DataTable dataTable)
        {
            StringBuilder stBuilder = new StringBuilder("INSERT INTO " + dataTable.TableName + " (");
            StringBuilder values = new StringBuilder("VALUES (");
            bool isFirst = true;
            bool isIdentity = false;
            string identityType = null;

            foreach (DataColumn dtColumn in dataTable.Columns)
            {
                if (dtColumn.AutoIncrement)
                {
                    isIdentity = true;

                    switch (dtColumn.DataType.Name)
                    {
                        case "Int16":
                            identityType = "smallint";
                            break;
                        case "SByte":
                            identityType = "tinyint";
                            break;
                        case "Int64":
                            identityType = "bigint";
                            break;
                        case "Decimal":
                            identityType = "decimal";
                            break;
                        default:
                            identityType = "int";
                            break;
                    }
                }
                else
                {
                    if (isFirst) { isFirst = false; }
                    else
                    {
                        stBuilder.Append(", ");
                        values.Append(", ");
                    }

                    stBuilder.Append(dtColumn.ColumnName);
                    values.Append("@");
                    values.Append(dtColumn.ColumnName);
                }
            }

            stBuilder.Append(") ");
            stBuilder.Append(values.ToString());
            stBuilder.Append(")");

            if (isIdentity)
            {
                stBuilder.Append("; SELECT CAST(scope_identity() AS ");
                stBuilder.Append(identityType);
                stBuilder.Append(")");
            }

            return stBuilder.ToString();
        }

    }
}
