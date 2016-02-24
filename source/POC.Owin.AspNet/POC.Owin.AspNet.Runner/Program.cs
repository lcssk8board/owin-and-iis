using Microsoft.Owin.Hosting;
using System;
using System.Diagnostics;

namespace POC.Owin.AspNet.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>("http://*:3000"))
            {
                Console.WriteLine($"Server listening on: http://*:3000");
                Process.Start("http://127.0.0.1:3000");
                Console.ReadLine();
            }
        }
    }
}
