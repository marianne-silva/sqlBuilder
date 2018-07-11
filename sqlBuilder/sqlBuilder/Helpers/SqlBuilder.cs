using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace sqlBuilder.Helpers
{
    public static class SqlBuilder
    {
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

        public static SqlCommand BuildInsertCommand(DataRow dtRow)
        {
            DataTable dt = dtRow.Table;
            string sql = BuildInsert(dt);
            SqlCommand sqlCommand = new SqlCommand(sql);
            sqlCommand.CommandType = CommandType.Text;

            foreach (DataColumn dtColumn in dt.Columns)
            {
                if (!dtColumn.AutoIncrement)
                {
                    string parameterName = "@" + dtColumn.ColumnName;
                    InsertParameter(sqlCommand, parameterName, dtColumn.ColumnName, dtRow[dtColumn.ColumnName]);
                }
            }
            return sqlCommand;
        }

        public static object InsertRow(DataRow row, string connectionString)
        {
            SqlCommand sqlCommand = BuildInsertCommand(row);
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                connection.Open();
                return sqlCommand.ExecuteScalar();
            }
        }

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
