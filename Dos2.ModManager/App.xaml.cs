using Dos2.ModManager.ViewModels;
using Microsoft.Win32;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using System;
using System.IO;
using System.Windows;

namespace Dos2.ModManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IKernel Kernel { get; }

        public App() : base()
        {
            Kernel = new StandardKernel();
            Kernel.Load<StandardModule>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            InitAppSetting();

            base.OnStartup(e);
            MainWindow = Kernel.Get<MainWindow>();
            MainWindow.Show();

        }

        private void InitAppSetting()
        {

            //Divine Path
            if (Dos2.ModManager.Properties.Settings.Default.Divine == null || !File.Exists(Dos2.ModManager.Properties.Settings.Default.Divine))
            {
                var fd = new OpenFileDialog
                {
                    Title = "Select divine.exe.",
                    FileName = Dos2.ModManager.Properties.Settings.Default.Divine,
                    Filter = "divine.exe|divine.exe"
                };
                if (fd.ShowDialog() == true && fd.CheckFileExists)
                {
                    Dos2.ModManager.Properties.Settings.Default.Divine = fd.FileName;
                }
            }

            //QuickBMS Path
            if (Dos2.ModManager.Properties.Settings.Default.QuickBMS == null || !File.Exists(Dos2.ModManager.Properties.Settings.Default.QuickBMS))
            {
                var fd = new OpenFileDialog
                {
                    Title = "Select quickbms.exe.",
                    FileName = Dos2.ModManager.Properties.Settings.Default.QuickBMS,
                    Filter = "quickbms.exe|quickbms.exe"
                };
                if (fd.ShowDialog() == true && fd.CheckFileExists)
                {
                    Dos2.ModManager.Properties.Settings.Default.QuickBMS = fd.FileName;
                }
            }

            //Game Path
            if (Dos2.ModManager.Properties.Settings.Default.Dos2 == null || !File.Exists(Dos2.ModManager.Properties.Settings.Default.Dos2))
            {
                var fd = new OpenFileDialog
                {
                    Title = "Select EoCApp.exe.",
                    FileName = Dos2.ModManager.Properties.Settings.Default.Dos2,
                    Filter = "EoCApp.exe|EoCApp.exe"
                };
                if (fd.ShowDialog() == true && fd.CheckFileExists)
                {
                    Dos2.ModManager.Properties.Settings.Default.Dos2 = fd.FileName;
                }
            }

           

            Dos2.ModManager.Properties.Settings.Default.Save();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Dos2.ModManager.Properties.Settings.Default.Save();
        }
    }


    /// <summary>
    /// The standard module used for the app.
    /// </summary>
    public class StandardModule : NinjectModule
    {
        public override void Load()
        {
            Bind<MainViewModel>().ToSelf().InSingletonScope();
            Bind<MainWindow>().ToSelf().InSingletonScope();
            Bind<IViewModel>().To<MainViewModel>().WhenInjectedInto<MainWindow>();

           

        }
    }

}
