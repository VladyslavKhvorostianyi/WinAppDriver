using System;
using System.Diagnostics;
using OpenQA.Selenium.Remote;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using System.Threading;

namespace CalculatorTest
{
    [TestClass]
    public class CalcutorOperationTest
    {
        [TestMethod]
        public void TestMethod1()
        {

        }

        // startup server and connect to him before runing test
        public CalcutorOperationTest()
        {
            serverProcess = StartServer();
            calcProcess = StartCalculator();
            session = StartSession();
        }


        // release resources 
        ~CalcutorOperationTest()
        {
            serverProcess.Dispose();
            if (!serverProcess.HasExited)
                serverProcess.Kill();
            calcProcess.Dispose();
            if (!calcProcess.HasExited)
                calcProcess.Kill();
            session.Dispose();
        }

        private Process serverProcess;
        public Process ServerProcess
        {
            get
            {
                if (serverProcess == null)
                {
                    serverProcess = StartServer();
                }
                return serverProcess;
            }
        }

        private Process StartServer()
        {
            // get path to server exe
            string debugPath = Path.GetDirectoryName(new Uri(
                Assembly.GetAssembly(typeof(CalcutorOperationTest)).CodeBase).LocalPath);
            DirectoryInfo solutionPath = new DirectoryInfo(debugPath).Parent.Parent.Parent;
            string serverExePath = Path.Combine(solutionPath.FullName, "WinAppDriver.Server", "bin",
                "Debug", "WinAppDriver.Server.exe");

            return Process.Start(serverExePath);

        }

        private RemoteWebDriver session;
        public RemoteWebDriver Session
        {
            get
            {
                if (session == null)
                {
                    session = StartSession();
                }
                return session;
            }
        }

        private RemoteWebDriver StartSession()
        {
            // set capabilities for calculator
            DesiredCapabilities appCapabilities = new DesiredCapabilities();
            appCapabilities.SetCapability("processName", "calc");
            appCapabilities.SetCapability("mode", "attach");
            
            return new RemoteWebDriver(new Uri("http://127.0.0.1:12345"), appCapabilities);
        }

        private Process calcProcess;
        public Process CalcProcess
        {
            get
            {
                if (calcProcess == null)
                    calcProcess = StartCalculator();
                return calcProcess;
            }
        }

        private Process StartCalculator()
        {
            Process process = Process.Start("calc.exe");
            int counter = 0;
            while (process.MainWindowHandle.ToInt32() == 0 && !process.HasExited)
            {
                Thread.Sleep(50);
                counter += 1;
                if (counter > 40)
                    throw new Exception("Calculator window doesn't appear more then 2 second.");
            }
            return process;
        }


    }
}
