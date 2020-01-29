using System;
using System.Windows;

using Diploma.UI.Auxiliary.Common;

namespace Diploma.UI
{

    public partial class App : Application
    {

        App()
        {
            InitializeComponent();
        }

        [STAThread]
        static void Main()
        {
            new App().Run(ViewsFactory.CreateMainView());
        }

    }

}