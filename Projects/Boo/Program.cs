using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BooExp
{
    static class Program
    {
        [DllImport("user32")]
        public static extern int GetNextWindow(IntPtr hwnd, int wFlag);

        [DllImport("user32")]
        public static extern int SetForegroundWindow(IntPtr hwnd);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //IntPtr hWnd = Process.GetProcessesByName("FirstExrc")[0].MainWindowHandle;
            //SetForegroundWindow(hWnd);
            Application.Run(new Form1());
        }
    }
}
