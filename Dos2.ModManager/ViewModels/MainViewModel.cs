using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;
using Microsoft.Win32;
using Ninject;
using Ninject.Infrastructure;
using Dos2.ModManager.Commands;
using Dos2.ModManager.Models;

namespace Dos2.ModManager.ViewModels
{
    

    /// <summary>
    /// Represents the currently open workspace and project information.
    /// </summary>
    public class MainViewModel : ViewModel, IHaveKernel
    {
        #region Documents / Content / Anchorables
        private object _activeContent;
        /// <summary>
        /// Holds the currently active content in the window.
        /// </summary>
        public object ActiveContent
        {
            get
            {
                return _activeContent;
            }
            set
            {
                if (_activeContent != value)
                {
                    _activeContent = value;
                    InvokePropertyChanged();
                }
            }
        }

        private ICollection<WorkspaceViewModel> _documentsSource;
        /// <summary>
        /// Holds all the currently open documents in the project.
        /// </summary>
        public ICollection<WorkspaceViewModel> DocumentsSource
        {
            get
            {
                return _documentsSource;
            }
            set
            {
                if (_documentsSource != value)
                {
                    _documentsSource = value;
                    InvokePropertyChanged();
                }
            }
        }


        private ICollection<DockableViewModel> _anchorablesSource;
        /// <summary>
        /// Holds the anchorable panes for controls.
        /// </summary>
        public ICollection<DockableViewModel> AnchorablesSource
        {
            get
            {
                return _anchorablesSource;
            }
            set
            {
                if (_anchorablesSource != value)
                {
                    _anchorablesSource = value;
                    InvokePropertyChanged();
                }
            }
        }
        #endregion

        #region Properties
        private object _activeProperty;
        /// <summary>
        /// Holds the currently active Wcc Lite Command in the window.
        /// </summary>
        public object ActiveProperty
        {
            get
            {
                return _activeProperty;
            }
            set
            {
                if (_activeProperty != value)
                {
                    _activeProperty = value;
                    InvokePropertyChanged();
                }
            }
        }

        private string _log;
        /// <summary>
        /// Holds the Logger Class from the Wcc Task Handler.
        /// </summary>
        public string Log
        {
            get
            {
                return _log;
            }
            set
            {
                if (_log != value)
                {
                    _log = value;
                    InvokePropertyChanged();
                }
            }
        }

        private ObservableCollection<Dos2Conflict> _conflictsList;
        /// <summary>
        /// Holds the global active Conflicts.
        /// </summary>
        public ObservableCollection<Dos2Conflict> ConflictsList
        {
            get
            {
                return _conflictsList;
            }
            set
            {
                if (_conflictsList != value)
                {
                    _conflictsList = value;
                    InvokePropertyChanged();
                }
            }
        }

        private ObservableCollection<Dos2Mod> _modsList;
        /// <summary>
        /// Holds the ModList stored in the Settings.
        /// </summary>
        public ObservableCollection<Dos2Mod> ModsList
        {
            get
            {
                return _modsList;
            }
            set
            {
                if (_modsList != value)
                {
                    _modsList = value;
                    InvokePropertyChanged();
                }
            }
        }


        private IViewModel _utilities;
        public IViewModel Utilities
        {
            get
            {
                return _utilities;
            }
            set
            {
                if (_utilities != value)
                {
                    _utilities = value;
                    InvokePropertyChanged();
                }
            }
        }

        //public DivineTaskHandler DivineTaskHandler { get; set; }
        //public QBMSTaskHandler QBMSTaskHandler { get; set; }
        #endregion

        #region Commands
        public ICommand ExitCommand { get; }
        public ICommand SaveCommand { get; }


        public ICommand LocateDivineCommand { get; }
        public ICommand LocateQuickBMSCommand { get; }
        public ICommand LocateGameCommand { get; }



        #region Command Implementation
        private void Exit()
        {
            Kernel.Get<MainWindow>().Close();
        }
        public bool CanSave()
        {
            return true;
        }
        public void Save()
        {
            Dos2.ModManager.Properties.Settings.Default.Save();
        }


        public bool CanLocateDivine()
        {
            return true;
        }
        public void LocateDivine()
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
        public bool CanLocateQuickBMS()
        {
            return true;
        }
        public void LocateQuickBMS()
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
        public bool CanLocateGame()
        {
            return true;
        }
        public void LocateGame()
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





        #endregion
        #endregion

        public IKernel Kernel { get; }

        public MainViewModel(IKernel kernel)
        {
            Kernel = kernel;

            #region ViewModels
            Utilities = kernel.Get<UtilitiesViewModel>();

            #endregion

            #region Relay Commands
            ExitCommand = new RelayCommand(Exit);
            SaveCommand = new RelayCommand(Save, CanSave);

            LocateDivineCommand = new RelayCommand(LocateDivine, CanLocateDivine);
            LocateQuickBMSCommand = new RelayCommand(LocateQuickBMS, CanLocateQuickBMS);
            LocateGameCommand = new RelayCommand(LocateGame, CanLocateGame);
            #endregion

            // Layout
            AnchorablesSource = new ObservableCollection<DockableViewModel>()
            {
               
                new ConflictsViewModel()
                {
                    Title = "Conflicts List",
                    ContentId = "conflictsList",
                    ParentViewModel = this,
                },
                new LogViewModel()
                {
                    Title = "Log",
                    ContentId = "log",
                    ParentViewModel = this,
                },
                new PropertiesViewModel()
                {
                    Title = "Properties",
                    ContentId = "properties",
                    ParentViewModel = this,
                },
                 new WorkspaceViewModel()
                {
                    Title = "Mods",
                    ContentId = "mods",
                    ParentViewModel = this,
                },

            };
            DocumentsSource = new ObservableCollection<WorkspaceViewModel>
            {
               
                

            };


            // core logic
            //DivineTaskHandler = new DivineTaskHandler(Properties.Settings.Default.Divine);
            //QBMSTaskHandler = new QBMSTaskHandler(Properties.Settings.Default.QuickBMS);
            ModsList = new ObservableCollection<Dos2Mod>();
            ConflictsList = new ObservableCollection<Dos2Conflict>();
            Log = "test";

        }


    }
}