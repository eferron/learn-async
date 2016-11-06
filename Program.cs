using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace async
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmd = String.Empty;
            var pwd = args[0];
            var cmdargs = new string[] { };
            do
            {
                Console.WriteLine("Enter command [new arg1 arg2] or [read]");
                cmd = Console.ReadLine();
                cmdargs = cmd.Split();
                if (cmdargs[0] == "read")
                {
                    ReadRowsAsync(pwd);
                    Console.WriteLine("Completed read operation");
                }
                else if (cmdargs[0] == "new")
                {
                    AddRowAsync(pwd, new string[] {cmdargs[1], cmdargs[2]});
                    Console.WriteLine($"Row {cmdargs[1]} {cmdargs[2]} added to table");
                }
            }while (cmdargs[0] != "end");
            Console.WriteLine("Processing Completed");
        }

        static async void ReadRowsAsync(string pwd)
        {
            DBWorker worker = new DBWorker(pwd);
            var output = await worker.ReadRow();
        }

        static async void AddRowAsync(string pwd,string[] args)
        {
            DBWorker worker = new DBWorker(pwd);
            var output = await worker.AddRow(args);
        }
    }

    public class DBWorker
    {
        private int _lastId = -1;
        private string _password;

        public DBWorker(string password) {
            _password = password;
        }
        public async Task<int> ReadRow()
        {
            var connectString = $"Data Source=ejf.database.windows.net;Initial Catalog=demodb;Integrated Security=False;User ID=edferron;Password={_password};Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var cmdText = $"select top 1 * from demotable order by [id] desc";
            SqlConnection connection = null;
            SqlCommand command = null;
            SqlDataReader reader = null; 

            using (connection = new SqlConnection(connectString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (command = new SqlCommand(cmdText, connection))
                {
                    reader = await command.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        if (reader.GetInt32(0) != _lastId)
                        {
                            Console.WriteLine($"id {reader["id"]} assigned to {reader["firstname"]} {reader["lastname"]}");
                        }
                    }
                }
            }
            return 0;
        }

        public async Task<int> AddRow(string[] args)
        {
            var connectString = $"Data Source=ejf.database.windows.net;Initial Catalog=demodb;Integrated Security=False;User ID=edferron;Password={_password};Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            //var cmdText = $"insert into demotable(firstname, lastname) values('{args[0]}', '{args[1]}')";
            SqlConnection connection = null;
            SqlCommand command = null;
            for (int index = 0; index < 10; index++)
            {
                var cmdText = $"insert into demotable(firstname, lastname) values('{args[0]} - {index}', '{args[1]} - {index}')";
                using (connection = new SqlConnection(connectString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    using (command = new SqlCommand(cmdText, connection))
                    {
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            return 0;
        }
    }

}
