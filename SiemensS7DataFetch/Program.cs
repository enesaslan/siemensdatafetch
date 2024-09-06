using System;
using System.Windows.Forms;

namespace SiemensS7DataFetch // SiemensPLCApp yerine doğru namespace kullanılmalı
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Form1 sınıfının doğru namespace içinde olduğundan emin olun
        }
    }
}
