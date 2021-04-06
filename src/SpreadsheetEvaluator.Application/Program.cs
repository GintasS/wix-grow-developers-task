using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpreadsheetEvaluator.Domain;
using SpreadsheetEvaluator.Domain.Interfaces;
using SpreadsheetEvaluator.Domain.Services;
using System;

namespace SpreadsheetEvaluator.Application
{
    class Program
    {


        static void Main(string[] args)
        {
            var serviceProvider = Startup.InitServiceProvider();

            var service = serviceProvider.GetService<IHubService>();

            Console.WriteLine("Hello World!");
            var t = service.GetJobs();

        }
    }
}
