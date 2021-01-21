using System;
using System.Diagnostics;
using System.Threading;

using Xlfdll.Diagnostics;
using Xlfdll.Windows.Presentation;

namespace BarefootVideoHelper
{
    public class LogViewModel : ViewModelBase
    {
        public LogViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;
            this.AutoScroll = true;
        }

        public MainViewModel MainViewModel { get; }

        private Boolean _autoScroll;

        public Boolean AutoScroll
        {
            get => _autoScroll;
            set => SetField(ref _autoScroll, value);
        }

        public String Log
        {
            get
            {
                return App.Log;
            }
            set
            {
                App.Log = value;

                OnPropertyChanged(nameof(this.Log));
            }
        }

        public RelayCommand<Object> ClearCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    this.Log = String.Empty;
                },
                delegate
                {
                    return !this.MainViewModel.IsBusy;
                }
            );

        private RedirectedProcess _redirectedProcess;

        public RedirectedProcess RedirectedProcess
        {
            get
            {
                return _redirectedProcess;
            }
            set
            {
                if (_redirectedProcess != null)
                {
                    _redirectedProcess.Started -= RedirectedProcess_Started;
                    _redirectedProcess.OutputDataReceived -= RedirectedProcess_DataReceived;
                    _redirectedProcess.ErrorDataReceived -= RedirectedProcess_DataReceived;
                    _redirectedProcess.Exited -= RedirectedProcess_Exited;
                }

                _redirectedProcess = value;

                if (_redirectedProcess != null)
                {
                    _redirectedProcess.Started += RedirectedProcess_Started;
                    _redirectedProcess.OutputDataReceived += RedirectedProcess_DataReceived;
                    _redirectedProcess.ErrorDataReceived += RedirectedProcess_DataReceived;
                    _redirectedProcess.Exited += RedirectedProcess_Exited;
                }
            }
        }

        private void RedirectedProcess_Started(object sender, EventArgs e)
        {
            this.Log += Environment.NewLine;
            this.Log += $"[Process {this.RedirectedProcess.Name} started]";
            this.Log += Environment.NewLine;
            this.Log += $"[Arguments: {this.RedirectedProcess.StartInfo.Arguments}]";
            this.Log += Environment.NewLine;

            this.Log += Environment.NewLine;
        }

        private void RedirectedProcess_DataReceived(object sender, DataReceivedEventArgs e)
        {
            // Create mutex object with a name on the fly, so that different threads get the same one for use
            Mutex mutex = new Mutex(false, "BarefootVideoHelper_LogMutex");

            if (mutex.WaitOne())
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    this.Log += e.Data;
                    this.Log += Environment.NewLine;
                }

                mutex.ReleaseMutex();
            }
        }

        private void RedirectedProcess_Exited(object sender, EventArgs e)
        {
            this.Log += Environment.NewLine;
            this.Log += $"[Process {this.RedirectedProcess.Name} exited]";
            this.Log += Environment.NewLine;

            this.Log += Environment.NewLine;

            this.RedirectedProcess = null;
        }
    }
}