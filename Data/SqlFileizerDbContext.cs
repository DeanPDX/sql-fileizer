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
        public static IEnumerable<Dictionary<string, object>> GetAllStoredProcsFromDB(string connectionString)
        {
            var sql = @"
				SELECT sch.name as [Schema], OBJECT_NAME(sm.object_id) AS ProcName, sm.definition as ProcValue
				FROM sys.sql_modules AS sm  
					JOIN sys.objects AS o ON sm.object_id = o.object_id
					JOIN sys.schemas sch on o.schema_id = sch.schema_id
				WHERE o.type = 'P'
				ORDER BY ProcName asc";
            return GetData(connectionString, sql);
        }

        /// <summary>
        /// Return all views in a DB. Columns will be "Schema", "ViewName", and "ViewValue".
        /// </summary>
        /// <param name="connectionString">The connection string for the DB you want to retrieve procs from.</param>
        public static IEnumerable<Dictionary<string, object>> GetAllViewsFromDB(string connectionString)
        {
            var sql = @"
				SELECT sch.name as [Schema], OBJECT_NAME(sm.object_id) AS ViewName, sm.definition as ViewValue
				FROM sys.sql_modules AS sm  
					JOIN sys.objects AS o ON sm.object_id = o.object_id
					JOIN sys.schemas sch on o.schema_id = sch.schema_id
				WHERE o.type_desc = 'VIEW'
				ORDER BY ViewName asc";
            return GetData(connectionString, sql);
        }

        /// <summary>
        /// Return all functions in a DB. Columns will be "Schema", "FunctionName", and "FunctionValue".
        /// </summary>
        /// <param name="connectionString">The connection string for the DB you want to retrieve procs from.</param>
        public static IEnumerable<Dictionary<string, object>> GetAllFunctionsFromDB(string connectionString)
        {
            var sql = @"
                SELECT sch.name as [Schema], OBJECT_NAME(sm.object_id) AS FunctionName, sm.definition as FunctionValue
				FROM sys.sql_modules AS sm  
					JOIN sys.objects AS o ON sm.object_id = o.object_id
					JOIN sys.schemas sch on o.schema_id = sch.schema_id
				WHERE o.type_desc like '%_FUNCTION'
				order by FunctionName";
            return GetData(connectionString, sql);
        }

        /// <summary>
        /// Return all triggers in a DB. Columns will be "Schema", "TriggerName", and "TriggerValue".
        /// </summary>
        /// <param name="connectionString">The connection string for the DB you want to retrieve procs from.</param>
        public static IEnumerable<Dictionary<string, object>> GetAllTriggersFromDB(string connectionString)
        {
            var sql = @"
                SELECT sch.name as [Schema], OBJECT_NAME(sm.object_id) AS TriggerName, sm.definition as TriggerValue
				FROM sys.sql_modules AS sm  
					JOIN sys.objects AS o ON sm.object_id = o.object_id
					JOIN sys.schemas sch on o.schema_id = sch.schema_id
				WHERE o.type = 'TR'
				order by TriggerName";
            return GetData(connectionString, sql);
        }
    }
}
