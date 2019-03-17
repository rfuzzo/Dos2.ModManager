using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Dos2.ModManager.Commands;
using Dos2.ModManager.Views;

namespace Dos2.ModManager.ViewModels
{
    public class UtilitiesViewModel : ViewModel
    {
        public string URLGithub { get; }  = "https://github.com/rfuzzo/Witcher-3-ModKit-UI";
        public Version Version { get; set; }  = Assembly.GetExecutingAssembly().GetName().Version;

        public ICommand OpenGithubCommand { get; }
        public ICommand OpenAboutCommand { get; }

        private IKernel kernel;

        public UtilitiesViewModel(IKernel kernel)
        {
            this.kernel = kernel;

            OpenGithubCommand = new RelayCommand(() => Process.Start(URLGithub));
            OpenAboutCommand = new RelayCommand(OpenAbout);
        }

        public void OpenAbout()
        {
            var about = kernel.Get<AboutView>();
            about.DataContext = this;
            about.ShowDialog();
        }
    }
}
