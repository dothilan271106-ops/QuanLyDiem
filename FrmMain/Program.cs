using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                using (var login = new FrmDangNhap())
                {
                    if (login.ShowDialog() != DialogResult.OK)
                    {
                        break;
                    }
                }

                var main = new FrmMain();
                Application.Run(main);

                if (!main.DangXuat)
                {
                    break;
                }
            }
        }
    }
}
