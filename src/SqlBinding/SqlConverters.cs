// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Threading;

namespace Microsoft.Azure.WebJobs.Extensions.Sql
{
    internal class SqlConverters
    {
        internal class SqlConverter : IConverter<SqlAttribute, SqlCommand>
        {
            private IConfiguration _configuration;

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlConverter/>"/> class.
            /// </summary>
            /// <param name="configuration"></param>
            /// <exception cref="ArgumentNullException">
            /// Thrown if the configuration is null
            /// </exception>
            public SqlConverter(IConfiguration configuration)
            {
                _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            }

            /// <summary>
            /// Creates a SqlCommand containing a SQL connection and the SQL query and parameters specified in attribute.
            /// The user can open the connection in the SqlCommand and use it to read in the results of the query themselves. 
            /// </summary>
            /// <param name="attribute">
            /// Contains the SQL query and parameters as well as the information necessary to build the SQL Connection
            /// </param>
            /// <returns>The SqlCommand</returns>
            public SqlCommand Convert(SqlAttribute attribute)
            {
                return SqlBindingUtilities.BuildCommand(attribute, SqlBindingUtilities.BuildConnection(attribute, _configuration));
            }

        }

        internal class SqlGenericsConverter<T> : IAsyncConverter<SqlAttribute, IEnumerable<T>>, IConverter<SqlAttribute, IAsyncEnumerable<T>>,
            IAsyncConverter<SqlAttribute, string>
        {
            private IConfiguration _configuration;

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlGenericsConverter<typeparamref name="T"/>"/> class.
            /// </summary>
            /// <param name="configuration"></param>
            /// <exception cref="ArgumentNullException">
            /// Thrown if the configuration is null
            /// </exception>
            public SqlGenericsConverter(IConfiguration configuration) 
            {
                _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            }

            /// <summary>
            /// Opens a SqlConnection, reads in the data from the user's database, and returns it as a list of POCOs.
            /// </summary>
            /// <param name="attribute">
            /// Contains the information necessary to establish a SqlConnection, and the query to be executed on the database
            /// </param>
            /// <returns>An IEnumerable containing the rows read from the user's database in the form of the user-defined POCO</returns>
            public async Task<IEnumerable<T>> ConvertAsync(SqlAttribute attribute, CancellationToken cancellationToken)
            {
                using (var connection = SqlBindingUtilities.BuildConnection(attribute, _configuration))
                {
                    return await SqlBindingUtilities.ExecuteQuery<T>(attribute, connection);
                }
            }

            /// <summary>
            /// Opens a SqlConnection, reads in the data from the user's database, and returns it as a JSON-formatted string.
            /// </summary>
            /// <param name="attribute">
            /// Contains the information necessary to establish a SqlConnection, and the query to be executed on the database
            /// </param>
            /// <returns>
            /// The JSON string. I.e., if the result has two rows from a table with schema ProductID: int, Name: varchar, Cost: int, 
            /// then the returned JSON string could look like
            /// [{"productID":3,"name":"Bottle","cost":90},{"productID":5,"name":"Cup","cost":100}]
            /// </returns>
            async Task<string> IAsyncConverter<SqlAttribute, string>.ConvertAsync(SqlAttribute attribute, CancellationToken cancellationToken)
            {
                using (var connection = SqlBindingUtilities.BuildConnection(attribute, _configuration))
                {
                    var rows = await SqlBindingUtilities.ExecuteQuery<dynamic>(attribute, connection);
                    return JsonConvert.SerializeObject(rows);
                }
            }

            IAsyncEnumerable<T> IConverter<SqlAttribute, IAsyncEnumerable<T>>.Convert(SqlAttribute attribute)
            {
                return new SqlAsyncEnumerable<T>(SqlBindingUtilities.BuildConnection(attribute, _configuration), attribute);
            }
        }
    }
}
