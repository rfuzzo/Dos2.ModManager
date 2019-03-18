using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dos2.ModManager.Models
{
    public class DivineTaskHandler
    {
        public DivineTaskHandler(string path)
        {
            Divine_path = path;
            Logger = new DMMLogger();
            Output = "";
        }



        #region Properties
        private string _divine_path;
        public string Divine_path
        {
            get { return _divine_path; }
            set { _divine_path = value; }
        }

        public DMMLogger Logger { get; set; }
        public string Output { get; set; }

        #endregion



        #region Public Accessors
        /// <summary>
        /// Async run Task. 
        /// </summary>
        public async Task RunArgs(string args)
        {
            try
            {
                //Logging Start
                //Logger.ProgressValue = 0;
                //Logger.IsIndeterminate = true;
                Logger.LogString("----------------------------------------------------------------");
                Logger.LogString($"--- {args} ---");
                Logger.LogString("----------------------------------------------------------------");

                DMMLogger innerlog = await Task.Run(() => Divine_RunProcess(args));

                //Logging End
                Logger.LogStrings(innerlog.RawLog);
                //Logger.IsIndeterminate = false;
                //Logger.ProgressValue = 100;
                //Logger.NotifyStatusChanged();
            }
            catch (Exception e) // Fixme: Catch specific exceptions
            {
                Logger.LogString(e.ToString());
            }
        }



        #endregion







        /// <summary>
        /// runs divine.exe as process with given argumentsa and logs the output to the logger. 
        /// </summary>
        private async Task<DMMLogger> Divine_RunProcess(string args)
        {
            var proc = new ProcessStartInfo(Divine_path) { WorkingDirectory = Path.GetDirectoryName(Divine_path) };
            DMMLogger inlogger = new DMMLogger();

            try
            {
                Output = "";

                proc.Arguments = args;
                proc.UseShellExecute = false;
                proc.RedirectStandardOutput = true;
                proc.WindowStyle = ProcessWindowStyle.Hidden;
                proc.CreateNoWindow = true;

                using (var process = Process.Start(proc))
                {
                    using (var reader = process.StandardOutput)
                    {
                        while (true)
                        {
                            string result = await reader.ReadLineAsync();

                            //inlogger.LogString(result);
                            Output += result + ";";

                            if (reader.EndOfStream)
                                break;
                        }
                    }
                    process.WaitForExit();
                    
                }

                

                return inlogger;
            }
            catch (Exception ex)
            {
                inlogger.LogString(ex.ToString());
                throw ex;
            }
        }




    }
}
