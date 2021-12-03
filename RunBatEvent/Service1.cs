using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;

namespace RunBatEvent
{
    public partial class Service1 : ServiceBase
    {
        string FilePath = ConfigurationManager.AppSettings["FilePath"];
        string FileName = ConfigurationManager.AppSettings["FileName"];
        string Logpath = ConfigurationManager.AppSettings["LogPath"];
        string LogFileName = ConfigurationManager.AppSettings["LogFileName"];

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                DeleteFile();
                WriteToFile("RunBatEvent is started at " + DateTime.Now);
                this.RunScript(@FileName);
            }
            catch(Exception ex)
            {
                WriteToFile("OnStart error: " + ex);
            }
        }

        protected override void OnStop()
        {
            try
            {
                WriteToFile("RunBatEvent is stopped at " + DateTime.Now);
            }
            catch(Exception ex)
            {
                WriteToFile("OnStop error: " + ex);
            }

        }

        public void WriteToFile(string Message)
        {
            
            if (!Directory.Exists(Logpath))
            {
                Directory.CreateDirectory(Logpath);
            }
            string filepath = Path.Combine(@Logpath,LogFileName);
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }


        private void RunScript(string processFileName)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C " + Path.Combine(@FilePath, processFileName),
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                var process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                StreamReader so = process.StandardOutput;

                string line;
                while ((line = so.ReadLine()) != null)
                {
                    WriteToFile("BatFileOutput: " + line);
                }
            }
            catch (Exception ex)
            {
                WriteToFile("RunScript error: " + ex);
            }

        }

        public void DeleteFile()
        {
            string filepath = Path.Combine(@Logpath, LogFileName);
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

        }
    }
}
