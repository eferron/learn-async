using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace async
{
    class Program
    {
        static void Main(string[] args)
        {
            InvokeDemoFunction();
        }

        static async void InvokeDemoFunction()
        {
            RemoteWork worker = new RemoteWork();
            var input = 10;
            Console.WriteLine($"1 - input {input}");
            var output = await worker.FetchDataAsync(input);
            Console.WriteLine($"2 - output {output}");
            Debugger.Break();

        }
    }

    public class RemoteWork
    {
        public RemoteWork() { }
        public async Task<int> FetchDataAsync(int input)
        {
            Console.WriteLine($"A - inside of FDA");
            Thread.Sleep(2000);
            return await Task.Run(() => {
                return input * 100;
            }); ;
            
        }
    }

}
