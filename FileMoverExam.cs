using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using log4net;
using log4net.Config;
using NLog;

namespace FileMoverExam
{
    public partial class FileMoverExam : ServiceBase
    {
        private FileSystemWatcher watcher;
        private static readonly ILog log = log4net.LogManager.GetLogger(typeof(FileMoverExam));

        public FileMoverExam()
        {
            InitializeComponent();
            ConfigureLog4Net();
        }

        protected override void OnStart(string[] args)
        {
            log.Info("Service started.");
            SetUpWatcher();
        }

        protected override void OnStop()
        {
            log.Info("Service stopped.");
            watcher.Dispose();
        }

        private void SetUpWatcher()
        {
            watcher = new FileSystemWatcher
            {
                Path = @"C:\Folder1",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,
                Filter = "*.*"
            };

            watcher.Created += OnCreated;
            watcher.EnableRaisingEvents = true;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                string targetPath = Path.Combine(@"C:\Folder2", Path.GetFileName(e.FullPath));
                File.Move(e.FullPath, targetPath);
                log.Info($"File moved: {e.FullPath} to {targetPath}");
            }
            catch (Exception ex)
            {
                log.Error($"Error moving file: {ex.Message}");
            }
        }

        private void ConfigureLog4Net()
        {
            XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));
        }
    }
}
