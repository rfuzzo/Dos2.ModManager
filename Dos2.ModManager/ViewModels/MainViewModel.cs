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
using System.Xml.Linq;
using System.Xml;

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
                    InvokePropertyChanged("ModsList");
                    InvokePropertyChanged("ModsCollectionView");

                }
            }
        }

        private Dos2Mod _activeProperty;
        /// <summary>
        /// Holds the currently active Wcc Lite Command in the window.
        /// </summary>
        public Dos2Mod ActiveProperty
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
                    NotifyModChanged();
                    InvokePropertyChanged("ActiveProperty");
                }
            }
        }

        private DMMLogger _logger;
        /// <summary>
        /// Holds the Logger Class from the Divine Task Handler.
        /// </summary>
        public DMMLogger Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                if (_logger != value)
                {
                    _logger = value;
                    InvokePropertyChanged();
                }
            }
        }

        private ObservableCollection<Dos2ModsSettings> _profiles;
        /// <summary>
        /// Holds the ModList stored in the Settings.
        /// </summary>
        public ObservableCollection<Dos2ModsSettings> Profiles
        {
            get
            {
                return _profiles;
            }
            set
            {
                if (_profiles != value)
                {
                    _profiles = value;
                    InvokePropertyChanged();
                }
            }
        }
        private Dos2ModsSettings _activeProfile;
        /// <summary>
        /// Holds the ModList stored in the Settings.
        /// </summary>
        public Dos2ModsSettings ActiveProfile
        {
            get
            {
                return Profiles.FirstOrDefault(x => x.IsActive);
            }
            set
            {
                if (_activeProfile != value)
                {
                    _activeProfile = value;
                    InvokePropertyChanged();
                    ChangeActiveProfile(value);
                }
               
            }
        }
        public DivineTaskHandler DivineTaskHandler { get; set; }
        #endregion

        #region ViewModels

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

        private IViewModel _workspace;
        public IViewModel Workspace
        {
            get
            {
                return _workspace;
            }
            set
            {
                if (_workspace != value)
                {
                    _workspace = value;
                    InvokePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands
        public ICommand ExitCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand RunGameCommand { get; }


        public ICommand LocateDivineCommand { get; }
        public ICommand LocateDocumentsCommand { get; }
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
        public bool CanRefresh()
        {
            return true;
        }
        public void Refresh()
        {
            throw new NotImplementedException();
        }
        public bool CanRunGame()
        {
            return true;
        }
        public void RunGame()
        {
            throw new NotImplementedException();
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
        public bool CanLocateDocuments()
        {
            return true;
        }
        public void LocateDocuments()
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
            //Workspace = kernel.Get<WorkspaceViewModel>();

            #endregion

            #region Relay Commands
            ExitCommand = new RelayCommand(Exit);
            SaveCommand = new RelayCommand(Save, CanSave);
            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            RunGameCommand = new RelayCommand(RunGame, CanRunGame);

            LocateDivineCommand = new RelayCommand(LocateDivine, CanLocateDivine);
            LocateDocumentsCommand = new RelayCommand(LocateDocuments, CanLocateDocuments);
            LocateGameCommand = new RelayCommand(LocateGame, CanLocateGame);
            #endregion

            // core logic
            ModsList = Properties.Settings.Default.ModList;
            DivineTaskHandler = new DivineTaskHandler(Properties.Settings.Default.Divine);
            Logger = DivineTaskHandler.Logger;
            Profiles = new ObservableCollection<Dos2ModsSettings>();

            //Get Profile Info
            // FIXME check for changes
            GetProfileInfo();


            // Layout
            AnchorablesSource = new ObservableCollection<DockableViewModel>()
            {

                new WorkspaceViewModel(this)
                {
                    Title = "Mods",
                    ContentId = "mods",
                },
                new ConflictsViewModel(this)
                {
                    Title = "Conflicts List",
                    ContentId = "conflicts",
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
               

            };
            DocumentsSource = new ObservableCollection<WorkspaceViewModel>
            {
               
                

            };
        }

        private void ChangeActiveProfile(Dos2ModsSettings profile)
        {
            //get old profile
            var id = profile.UUID;
            Profiles.FirstOrDefault(x => x.IsActive).IsActive = false;
            Profiles.FirstOrDefault(x => x.UUID == id).IsActive = true;

            //write to the lsb
            // FIXME

            //go into workspace view model and apply view
            if (AnchorablesSource != null)
            {
                WorkspaceViewModel wvm = (WorkspaceViewModel)AnchorablesSource.First(x => x.ContentId == "mods");
                if (wvm != null)
                {
                    wvm.ApplyModSettings(); 
                }
            }
        }


        /// <summary>
        /// Gets Data from profile lsbs and lsx
        /// </summary>
        #region GetProfileData
        /// <summary>
        /// Gets Data from profile lsbs and lsx and returns the UUID of the active profile.
        /// </summary>
        /// <param name="activeProfileID"></param>
        /// <returns></returns>
        private void GetProfileInfo()
        {
           

            Profiles.Clear(); //FIXME check for changes

            //get active profile ID
            string profileDir = Path.Combine(Path.GetDirectoryName(Dos2.ModManager.Properties.Settings.Default.Mods), @"PlayerProfiles");
            string fileName = Path.Combine(profileDir, @"playerprofiles.lsb");
            string activeProfileID = hack_ExtractSingleUUID(fileName);

            //get data from profiles
            var profileDirList = Directory.GetDirectories(profileDir).ToList();
            foreach (var dir in profileDirList)
            {
                var profile = Directory.GetFiles(dir).FirstOrDefault(x => Path.GetFileName(x).Equals("profile.lsb"));
                string id = hack_ExtractSingleUUID(profile);

                //Get profile Info for Mods
                var settings = new Dos2ModsSettings()
                {
                    Name = Path.GetFileNameWithoutExtension(dir),
                    UUID = id,
                    IsActive = (id == activeProfileID)
                };

                //read modsettings file
                var modsettings = Directory.GetFiles(dir).FirstOrDefault(x => Path.GetFileName(x).Equals("modsettings.lsx"));
                try
                {
                    using (XmlReader xmlReader = XmlReader.Create(modsettings))
                    {
                        XDocument xml = XDocument.Load(xmlReader);
                        LsxTools lt = new LsxTools(xml);

                        //Mod Order
                        XElement modOrder = xml.Descendants("node").FirstOrDefault(x => x.Attribute("id").Value == "ModOrder");
                        if (modOrder.HasElements)
                        {
                            var children = modOrder.Elements().First().Elements().ToList();
                            foreach (var module in children)
                            {
                                var atts = module.Elements().ToList();
                                string modID = lt.GetAttributeByName(atts, "UUID");
                                settings.ModLoadOrder.Add(modID);
                            }
                        }

                        //Active Mods
                        var activeMods = xml.Descendants("node").Where(x => x.Attribute("id").Value == "ModuleShortDesc").ToList();
                        if (activeMods.Any())
                        {
                            foreach (var item in activeMods)
                            {
                                var atts = item.Elements().ToList();
                                string modID = lt.GetAttributeByName(atts, "UUID");
                                settings.ActiveMods.Add(modID);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw;
                }

                Profiles.Add(settings);
            }
        }
        /// <summary>
        /// a hack to quickly extract one UUID from an lsb without converting to lsx.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string hack_ExtractSingleUUID(string file)
        {
            string uuid = "";

            if (File.Exists(file))
            {
                using (BinaryReader br = new BinaryReader(File.Open(file, FileMode.Open)))
                {
                    var bytes = br.ReadBytes((int)br.BaseStream.Length);

                    string utf8 = System.Text.Encoding.UTF8.GetString(bytes);
                    var dashes = utf8.Count(f => f == '-');
                    if (dashes == 4)
                    {
                        uuid = utf8.Substring(utf8.IndexOf('-') - 8, 36);
                    }
                    else if (dashes < 4)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
            return uuid;
        }
        #endregion


        public void NotifyModChanged()
        {
            if (AnchorablesSource != null)
            {
                ConflictsViewModel cvm = (ConflictsViewModel)AnchorablesSource.First(x => x.ContentId == "conflicts");
                if (cvm != null)
                {
                    cvm.RegenerateConflictsList();
                }
            }
            
            
        }
    }
}



