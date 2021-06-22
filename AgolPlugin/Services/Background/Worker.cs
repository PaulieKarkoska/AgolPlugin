using System;
using System.ComponentModel;

namespace AgolPlugin.Services.Background
{
    internal static class Worker
    {
        internal static void DoWork(Action<object, DoWorkEventArgs> onWork, Action<object, RunWorkerCompletedEventArgs> onCompleted = null, Action<object, ProgressChangedEventArgs> onProgress = null, object argument = null)
        {
            var bg = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true,
            };

            bg.DoWork += new DoWorkEventHandler(onWork);
            if (onProgress != null) bg.ProgressChanged += new ProgressChangedEventHandler(onProgress);
            if (onCompleted != null) bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(onCompleted);

            bg.RunWorkerAsync(argument);
        }
    }
}