using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArduinoInterface
{
    internal static class Program
    {
        /// Главная точка входа для приложения.

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 childForm = new Form1();
            Application.Run(childForm);
        }
    }
}
