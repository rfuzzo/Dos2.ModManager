using Dos2.ModManager.ViewModels;
using Microsoft.Win32;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            //debug
            Dos2.ModManager.Properties.Settings.Default.Divine = @"E:\moddingdir_dos2\TOOLS\divine\divine.exe";
            Dos2.ModManager.Properties.Settings.Default.Mods = @"E:\moddingdir_dos2\Larian Studios\Divinity Original Sin 2 Definitive Edition\graphicSettings.lsx";
            Dos2.ModManager.Properties.Settings.Default.Dos2 = @"C:\Steam\steamapps\common\Divinity Original Sin 2\DefEd\bin\EoCApp.exe";

            //debug


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

            //Documents Path
            if (Dos2.ModManager.Properties.Settings.Default.Mods == null || !File.Exists(Dos2.ModManager.Properties.Settings.Default.Mods))
            {
                var fd = new OpenFileDialog
                {
                    Title = "Select graphicSettings.lsx.",
                    FileName = Dos2.ModManager.Properties.Settings.Default.Mods,
                    Filter = "graphicSettings.lsx|graphicSettings.lsx"
                };
                if (fd.ShowDialog() == true && fd.CheckFileExists)
                {
                    Dos2.ModManager.Properties.Settings.Default.Mods = fd.FileName;
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

            //Working Directory Path
            if (!Directory.Exists(Dos2.ModManager.Properties.Settings.Default.WorkingDir))
            {
                string wd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"wd");
                try
                {
                    Directory.CreateDirectory(wd);
                    Dos2.ModManager.Properties.Settings.Default.WorkingDir = wd;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            
            //Saved ModList
            if (Dos2.ModManager.Properties.Settings.Default.ModList == null)
            {
                Dos2.ModManager.Properties.Settings.Default.ModList = new ObservableCollection<Models.Dos2Mod>();
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
