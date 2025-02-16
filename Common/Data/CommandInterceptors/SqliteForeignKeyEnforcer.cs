using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace BlueBellDolls.Common.Data.CommandInterceptors
{
    public class SqliteForeignKeyEnforcer : DbCommandInterceptor
    {
        public override InterceptionResult<int> NonQueryExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            if (command.CommandText.StartsWith("PRAGMA foreign_keys", StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }

            command.CommandText = "PRAGMA foreign_keys = ON;\n" + command.CommandText;
            return result;
        }
    }
}
