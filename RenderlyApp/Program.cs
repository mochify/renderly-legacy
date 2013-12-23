using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using Renderly.Drivers;
using Renderly.Imaging;
using Renderly.Models;
using Renderly.Views;
using Renderly.Controllers;
using RenderlyApp.Commands;
using ManyConsole;

using Autofac;

namespace RenderlyApp
{
    class Program
    {
        private static ContainerBuilder RegisterAssemblyTypes()
        {
            // TODO do the IoC wiring here, which probably also means
            // using abstract type factories for deep object creation
            var programAssembly = Assembly.GetExecutingAssembly();
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(programAssembly).AsImplementedInterfaces();
            builder.RegisterType<StandaloneImageComparator>().As<IImageComparer>();

            return builder;
        }

        private static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
        }

        static void Main(string[] args)
        {
            //var containerBuilder = RegisterAssemblyTypes();
            var commands = GetCommands();
            ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
       }
    }
}
