using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Common;

namespace SqlFileizer.Data
{
    public class SqlFileizerDbContext
    {
        public static IEnumerable<Dictionary<string, object>> GetData(string connectionString, string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        var rowValues = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var dataTypeName = reader.GetDataTypeName(i);

                            if (reader.IsDBNull(i))
                            {
                                rowValues.Add(reader.GetName(i), null);
                            }
                            else
                            {
                                rowValues.Add(reader.GetName(i), reader.GetValue(i));
                            }
                        }
                        yield return rowValues;
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Return all procs in a DB. Columns will be "Schema", "ProcName", and "ProcValue".
        /// </summary>
        /// <param name="connectionString">The connection string for the DB you want to retrieve procs from.</param>
        public static IEnumerable<Dictionary<string,object>> GetAllStoredProcsFromDB(string connectionString)
        {
            var sql = @"
                select ROUTINE_SCHEMA [Schema], ROUTINE_NAME [ProcName], ROUTINE_DEFINITION [ProcValue]
                from INFORMATION_SCHEMA.ROUTINES
                where ROUTINE_TYPE = 'PROCEDURE'
                order by ROUTINE_NAME asc";
            return GetData(connectionString, sql);
        }

    }
}