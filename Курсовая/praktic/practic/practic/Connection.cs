using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace practic
{
    internal static class Connection
    {
        private static string _connStr = $"Server=localhost;" +
            $"Uid=root;" +
            $"Pwd=;" +
            $"Database=toy_shop;";
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connStr);
        }
    }
}
