using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.Sql
{
    internal class SqlBindingUtilities
    {
        /// <summary>
        /// Builds a connection using the connection string and authentication information specified in "attribute". 
        /// Only builds a new connection if "connection" is null. Otherwise just returns "connection" 
        /// </summary>
        /// <param name="connection">Used to determine whether or not a new connection must be built. The function 
        /// simply returns "connection" if it is non-null </param>
        /// <param name="attribute">Contains the connection string and authentication information for the user's database</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the connection string in "attribute" is null
        /// </exception>
        /// <returns>The built connection </returns>
        internal static SqlConnection BuildConnection(SqlAttribute attribute, IConfiguration configuration)
        {
            if (attribute.ConnectionStringSetting == null)
            {
                throw new InvalidOperationException("Must specify a ConnectionStringSetting, which refers to the name of an app setting which contains" +
                    "the SQL connection string, to connect to your SQL server instance.");
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            return new SqlConnection(configuration.GetConnectionStringOrSetting(attribute.ConnectionStringSetting));
        }

        /// <summary>
        /// Parses the parameter string into a list of parameters, where each parameter is separted by "," and has the form 
        /// "@param1=param2". "@param1" is the parameter name to be used in the query or stored procedure, and param1 is the 
        /// parameter value. Parameter name and parameter value are separated by "=". Parameter names/values cannot contain ',' or '='. 
        /// A valid parameter string would be "@param1=param1,@param2=param2". Attaches each parsed parameter to command if
        /// command is provided. 
        /// </summary>
        /// <param name="parameters">The parameter string to be parsed</param>
        /// <param name="command">The SqlCommand to which the parsed parameters will be added to, if specified</param>
        /// <returns>A DynamicParameters object containing all param pairs to be used with Dapper queries</returns>
        internal static DynamicParameters ParseParameters(string parameters, SqlCommand command =  null)
        {
            var dapperParams = new DynamicParameters();
            // If parameters is null, user did not specify any parameters in their function so nothing to parse
            if (parameters != null)
            {
                // Because we remove empty entries, we will ignore any commas that appear at the beginning/end of the parameter list.
                // I.e., ",,@param1=param1,@param2=param2,,," will be parsed just like "@param1=param1,@param2=param2" is.
                // Do we want this? 
                string[] paramPairs = parameters.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var pair in paramPairs)
                {
                    // Note that we don't throw away empty entries here, so a parameter pair that looks like "=@param1=param1"
                    // or "@param2=param2=" is considered malformed
                    string[] items = pair.Split('=');
                    if (items.Length != 2)
                    {
                        throw new ArgumentException("Parameters must be separated by \",\" and parameter name and parameter value must be separated by \"=\", " +
                            "i.e. \"@param1=param1,@param2=param2\"");
                    }
                    if (!items[0].StartsWith("@"))
                    {
                        throw new ArgumentException("Parameter name must start with \"@\", i.e. \"@param1=param1,@param2=param2\"");
                    }
                    if (command != null)
                    {
                        command.Parameters.Add(new SqlParameter(items[0], items[1]));
                    }
                    else
                    {
                        dapperParams.Add(items[0], items[1]);
                    }
                }
            }
            return dapperParams;
        }

        /// <summary>
        /// Builds a SqlCommand using the query/stored procedure and parameters specifed in attribute.
        /// </summary>
        /// <param name="attribute">The SqlAttribute with the parameter, command type, and command text</param>
        /// <param name="connection">The connection to attach to the SqlCommand</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the CommandType specified in attribute is neither StoredProcedure nor Text. We only support
        /// commands that refer to the name of a StoredProcedure (the StoredProcedure CommandType) or are themselves 
        /// raw queries (the Text CommandType).
        /// </exception>
        /// <returns>The build SqlCommand</returns>
        /// <summary>
        /// Builds a SqlCommand using the query/stored procedure and parameters specifed in attribute.
        /// </summary>
        /// <param name="attribute">The SqlAttribute with the parameter, command type, and command text</param>
        /// <param name="connection">The connection to attach to the SqlCommand</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the CommandType specified in attribute is neither StoredProcedure nor Text. We only support
        /// commands that refer to the name of a StoredProcedure (the StoredProcedure CommandType) or are themselves 
        /// raw queries (the Text CommandType).
        /// </exception>
        /// <returns>The build SqlCommand</returns>
        internal static SqlCommand BuildCommand(SqlAttribute attribute, SqlConnection connection)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = attribute.CommandText;
            if (attribute.CommandType == CommandType.StoredProcedure)
            {
                command.CommandType = CommandType.StoredProcedure;
            }
            else if (attribute.CommandType != CommandType.Text)
            {
                throw new ArgumentException("The Type of the SQL attribute for an input binding must be either CommandType.Text for a plain text" +
                    "SQL query, or CommandType.StoredProcedure for a stored procedure.");
            }
            SqlBindingUtilities.ParseParameters(attribute.Parameters, command);
            return command;
        }

        /// <summary>
        /// Executes the query in attribute using the parameters specified in attribute and the provided connection. 
        /// If <see cref="SqlAttribute.Buffered"/> is set to true, then the query is executed synchronously, and a buffered
        /// IEnumerable is returned. This IEnumerable only reads a row from the database when MoveNext is called.
        /// Otherwise, the query is executed asychronously and all rows are returned at once.
        /// </summary>
        /// <typeparam name="T">The type that the SQL row represents</typeparam>
        /// <param name="attribute">Contains the query to execute and parameters</param>
        /// <param name="connection">The connection used to execute the query</param>
        /// <returns>The list of rows</returns>
        internal static async Task<IEnumerable<T>> ExecuteQuery<T>(SqlAttribute attribute, SqlConnection connection)
        {
            var parameters = ParseParameters(attribute.Parameters);
            if (attribute.CommandType == CommandType.StoredProcedure)
            {
                if (attribute.Buffered)
                {
                    return connection.Query<T>(attribute.CommandText, parameters, commandType: CommandType.StoredProcedure, buffered: attribute.Buffered);
                }
                return await connection.QueryAsync<T>(attribute.CommandText, parameters, commandType: CommandType.StoredProcedure);
            }
            else if (attribute.CommandType == CommandType.Text)
            {
                if (attribute.Buffered)
                {
                    return connection.Query<T>(attribute.CommandText, parameters, buffered: attribute.Buffered);
                }
                return await connection.QueryAsync<T>(attribute.CommandText, parameters);
            } 
            else
            {
                throw new ArgumentException("The Type of the SQL attribute for an input binding must be either CommandType.Text for a plain text" +
                    "SQL query, or CommandType.StoredProcedure for a stored procedure.");
            }
        }
    }
}
