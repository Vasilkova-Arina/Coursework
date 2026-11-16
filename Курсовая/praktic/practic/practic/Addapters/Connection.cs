using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace practic
{
    public static class Connection
    {
        public static string _connStr = $"Server=localhost;" +
            $"Uid=root;" +
            $"Pwd=root;" +
            $"Database=toy_shop;";
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connStr);
        }
    }
}
