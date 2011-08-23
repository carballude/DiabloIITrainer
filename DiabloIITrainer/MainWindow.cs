using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DiabloIITrainer
{
    public partial class MainWindow : Form
    {

        #region DllImports

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern Int32 WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, uint[] lpBuffer, UInt32 nSize, IntPtr lpNumberOfBytesWritten);

        #endregion

        /// <summary>
        /// Gets Diablo II process or null if it's not loaded
        /// </summary>
        Func<Process> GetDiablo = () => Process.GetProcessesByName("Game").SingleOrDefault();

        /// <summary>
        /// Writes a value to a specific memory address
        /// </summary>
        /// <param name="address">Memory address in which we want to write</param>
        /// <param name="value">Desired value for the address</param>
        private void WriteAddress(IntPtr address, uint value)
        {
            var diablo = GetDiablo(); //Look for Diablo
            var pHandle = OpenProcess(0x1F0FFF, 0, (UInt32)diablo.Id); //Open it
            WriteProcessMemory(pHandle, address, new uint[] { value }, 4, (IntPtr)0); //Write the new value
            CloseHandle(pHandle); //Close
        }

        /// <summary>
        /// Sets the health.
        /// </summary>
        /// <param name="value">Desired health. Be aware that X health in game is actually X*256 here.</param>
        private void SetHealth(uint value)
        {
            WriteAddress((IntPtr)0x01E42D24, value);
            WriteAddress((IntPtr)0x04C43024, value);
            WriteAddress((IntPtr)0x053B3124, value);
            WriteAddress((IntPtr)0x05964624, value);
            WriteAddress((IntPtr)0x6FBCA794, value);
            WriteAddress((IntPtr)0x6FBCA79C, value);
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btSetHealth_Click(object sender, EventArgs e)
        {
            SetHealth((uint)numericUpDown1.Value*256);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SetHealth((uint)numericUpDown1.Value * 256);
        }

        private void chkBoxAutoHealth_CheckedChanged(object sender, EventArgs e)
        {            
            if (timer1.Enabled) timer1.Stop();
            else timer1.Start();
        }

    }

}
