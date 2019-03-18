using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dos2.ModManager
{


    public enum DMMLoggerSingleStatus
    {
        LSS_Idle,
        LSS_Finished,
        LSS_FinishedWithErrors,
        LSS_FinishedWithWarnings
    }


    public class DMMLogger : ObservableObject
    {
        public DMMLogger()
        {
            RawLog = new ObservableCollection<string>();
        }

        #region Properties

        private int _progressValue;
        /// <summary>
        /// Progress Value for Progress Bar.
        /// </summary>
        public int ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    InvokePropertyChanged();
                }
            }
        }
        private bool _isIndeterminate;
        /// <summary>
        /// Indeterminate Status for Progress Bar.
        /// </summary>
        public bool IsIndeterminate
        {
            get
            {
                return _isIndeterminate;
            }
            set
            {
                if (_isIndeterminate != value)
                {
                    _isIndeterminate = value;
                    InvokePropertyChanged();
                }
            }
        }
        private DMMLoggerSingleStatus _status;
        /// <summary>
        /// Overall Status of the Logger.
        /// </summary>
        public DMMLoggerSingleStatus Status
        {
            get
            {

                if (!RawLog.Any())
                    return DMMLoggerSingleStatus.LSS_Idle;
                else
                    return DMMLoggerSingleStatus.LSS_Finished;

                /*if (true)
                    return DMMLoggerSingleStatus.LSS_FinishedWithErrors;
                else if (hasWarning)
                    return DMMLoggerSingleStatus.LSS_FinishedWithWarnings;
                else
                    return DMMLoggerSingleStatus.LSS_Finished;*/
            }

        }

        private string _log;
        /// <summary>
        /// Progress Value for Progress Bar.
        /// </summary>
        public string Log
        {
            get
            {
                return _log;
            }
            set
            {
                if (_log != value)
                {
                    _log = value;
                    InvokePropertyChanged();
                }
            }
        }
        public ObservableCollection<string> RawLog { get; set; }

        #endregion

        public void ClearLog()
        {
            RawLog.Clear();
        }
        public void NotifyStatusChanged()
        {
            InvokePropertyChanged("Status");
        }

        /// <summary>
        /// Log simple strings
        /// </summary>
        public void LogString(string value)
        {
            RawLog.Add(value);
            Log += value + "\r\n";
        }
        internal void LogStrings(ObservableCollection<string> innerlog)
        {
            foreach (string item in innerlog)
            {
                RawLog.Add(item);
                Log += item + "\r\n";
            }
        }

        


        

        
    }
}
