using System;
using System.Threading;
using System.Windows.Forms;

namespace WindowsShade
{
    static class Program
    {
        private const string SingleInstanceMutexName = @"Local\WindowsShade.SingleInstance";
        private const string ShowMainWindowEventName = @"Local\WindowsShade.ShowMainWindow";

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew;
            using (var instanceMutex = new Mutex(true, SingleInstanceMutexName, out createdNew))
            using (var showMainWindowEvent = new EventWaitHandle(false, EventResetMode.AutoReset, ShowMainWindowEventName))
            {
                if (!createdNew)
                {
                    showMainWindowEvent.Set();
                    return;
                }

                RunApplication(instanceMutex, showMainWindowEvent);
            }
        }

        private static void RunApplication(Mutex instanceMutex, EventWaitHandle showMainWindowEvent)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = new FormMain();
            _ = mainForm.Handle;
            RegisteredWaitHandle showMainWindowWaitHandle = null;
            showMainWindowWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                showMainWindowEvent,
                (state, timedOut) =>
                {
                    if (timedOut || mainForm.IsDisposed)
                        return;

                    try
                    {
                        mainForm.BeginInvoke(new Action(mainForm.ShowMainWindow));
                    }
                    catch (InvalidOperationException)
                    {
                    }
                },
                null,
                Timeout.Infinite,
                false);

            try
            {
                Application.Run(mainForm);
            }
            finally
            {
                showMainWindowWaitHandle?.Unregister(null);
                instanceMutex.ReleaseMutex();
            }
        }
    }
}
