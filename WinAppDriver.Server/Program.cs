﻿using WinAppDriver.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClientApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            var commandDispatcher = new DriverHost("http://localhost:12345");
            commandDispatcher.Start();
            Console.ReadLine();
        }
    }
}
