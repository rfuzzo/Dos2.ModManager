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
using System.Windows;
using System.Diagnostics;

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
        public LsxTools lt { get; set; }
        public PakTools pt { get; set; }
        public bool IsInitialized { get; set; }
        
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
            return IsInitialized;
        }
        public void Save()
        {
            WorkspaceViewModel wvm = (WorkspaceViewModel)AnchorablesSource.First(x => x.ContentId == "mods");

            //var dbg = wvm.ModsCollectionView.CurrentItem;
            //wvm.ModsCollectionView.CommitEdit();
            //wvm.ModsCollectionView.Refresh();

            
            //Logging Start
            Logger.ProgressValue = 0;
            Logger.IsIndeterminate = true;
            Logger.Status = "Saving...";

            Dos2.ModManager.Properties.Settings.Default.Save();

            MessageBoxResult result = MessageBox.Show(
                    "Save current load order? This operation cannot be undone.",
                    "Save Load Order?", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                lt.SaveModSettings(ActiveProfile, ModsList.ToList());

                // refresh
                //FIXME this should be redundant
                bool initProfiles = GetProfileInfo();
                if (initProfiles)
                {
                    
                    //FIXME check for updates
                    wvm.GetModDataAsync();
                }
                Logger.LogString($"Saved Modlist for profile {ActiveProfile.Name}.");

            }
            else
            {
                MessageBoxResult viper = MessageBox.Show("You'll coward don't even smoke crack.");
                // do noting
            }

            //Logging End
            Logger.ProgressValue = 100;
            Logger.IsIndeterminate = false;
            Logger.LogString("Finished Saving.");
            Logger.NotifyStatusChanged();
        }

        

        public bool CanRefresh()
        {
            return IsInitialized;
        }
        public void Refresh()
        {
            //Logging Start
            Logger.ProgressValue = 0;
            Logger.IsIndeterminate = true;
            Logger.Status = "Refreshing...";
            Logger.Log = "";

            IsInitialized = false;
            ModsList.Clear();

            bool initProfiles = GetProfileInfo();
            if (initProfiles)
            {
                WorkspaceViewModel wvm = (WorkspaceViewModel)AnchorablesSource.First(x => x.ContentId == "mods");
                //FIXME check for updates
                wvm.GetModDataAsync();

                Logger.LogString($"Refreshed Modlist.");
            }

            //Logging End
            Logger.ProgressValue = 100;
            Logger.IsIndeterminate = false;
            Logger.Status = "Finished.";
            Logger.NotifyStatusChanged();

        }


        public bool CanRunGame()
        {
            return IsInitialized;
        }
        public void RunGame()
        {
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
                    Dos2.ModManager.Properties.Settings.Default.Save();
                    Process.Start(Dos2.ModManager.Properties.Settings.Default.Dos2);
                }
            }
            else
            {
                Process.Start(Dos2.ModManager.Properties.Settings.Default.Dos2);
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

            #endregion

            #region Relay Commands
            ExitCommand = new RelayCommand(Exit);
            SaveCommand = new RelayCommand(Save, CanSave);
            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            RunGameCommand = new RelayCommand(RunGame, CanRunGame);

            LocateDocumentsCommand = new RelayCommand(LocateDocuments, CanLocateDocuments);
            LocateGameCommand = new RelayCommand(LocateGame, CanLocateGame);
            #endregion

            ModsList = Properties.Settings.Default.ModList;
            Logger = new DMMLogger();
            Profiles = new ObservableCollection<Dos2ModsSettings>();
            lt = new LsxTools();
            pt = new PakTools(this);

            // Get Profile Info
           

            // FIXME check for changes
            bool initProfiles =  GetProfileInfo();
            //check if any profiles were loaded 
            // FIXME get all profiles and check if they were loaded
            // FIXME handle the case where profiles are messed up better
            if (initProfiles)
            {
                // Layout
                // mod list is generated in WorkspaceViewModel
                AnchorablesSource = new ObservableCollection<DockableViewModel>()
                {

                    new WorkspaceViewModel(this)
                    {
                        Title = "Mods",
                        ContentId = "mods",
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
                    new ConflictsViewModel(this)
                    {
                        Title = "Conflicts List",
                        ContentId = "conflicts",
                    },
                };

                
            }
        }

        private void ChangeActiveProfile(Dos2ModsSettings profile)
        {
            // FIXME bad check
            if (profile == null)
            {
                return;
            }
            
            //get old profile
            var id = profile.UUID;
            Profiles.FirstOrDefault(x => x.IsActive).IsActive = false;
            Profiles.FirstOrDefault(x => x.UUID == id).IsActive = true;

            //write to the lsb
            // FIXME I don't want this here

            //go into workspace view model and apply view
            if (AnchorablesSource != null)
            {
                WorkspaceViewModel wvm = (WorkspaceViewModel)AnchorablesSource.First(x => x.ContentId == "mods");
                if (wvm != null)
                {
                    wvm.ApplyModSettings(ActiveProfile); 
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
        private bool GetProfileInfo()
        {
            Logger.Status = "Fetching Profile Data...";
            Logger.LogString("Fetching Profile Data...");

            Profiles.Clear(); //FIXME check for changes

            //get active profile ID
            string profileDir = Path.Combine(Path.GetDirectoryName(Dos2.ModManager.Properties.Settings.Default.Mods), @"PlayerProfiles");
            string fileName = Path.Combine(profileDir, @"playerprofiles.lsb");
            DOS2_UserProfiles playerProfile = lt.GetActiveProfile(fileName);

            //checks
            if (!File.Exists(fileName) || playerProfile == null || String.IsNullOrEmpty(playerProfile.ActiveProfile))
            {
                MessageBoxResult result = MessageBox.Show(
                    "No active profile found. Please check your paths or start the game once.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                Logger.Status = "Finished With Errors.";
                Logger.LogString("No active profile found. Please check your paths or start the game once.");
                return false;
            }
            else if (!Directory.Exists(profileDir) || !Directory.GetDirectories(profileDir).ToList().Any())
            {
                MessageBoxResult result = MessageBox.Show(
                   "No profile data found. Please check your paths or start the game once.",
                   "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                Logger.Status = "Finished With Errors.";
                Logger.LogString("No profile data found. Please check your paths or start the game once.");
                return false;
            }
            else
            {
                //get data from profiles
                try
                {
                    var profileDirList = Directory.GetDirectories(profileDir).ToList();
                    foreach (var dir in profileDirList)
                    {
                        var profile = Directory.GetFiles(dir).FirstOrDefault(x => Path.GetFileName(x).Equals("profile.lsb"));
                        DOS2_PlayerProfile pp = lt.GetProfile(profile);
                        if (pp == null || String.IsNullOrEmpty(pp.PlayerProfileID))
                            continue;

                        //Get profile Info for Mods
                        var settings = new Dos2ModsSettings()
                        {
                            Name = Path.GetFileNameWithoutExtension(dir),
                            UUID = pp.PlayerProfileID,
                            IsActive = (pp.PlayerProfileID == playerProfile.ActiveProfile)
                        };

                        //read modsettings file
                        var modsettings = Directory.GetFiles(dir).FirstOrDefault(x => Path.GetFileName(x).Equals("modsettings.lsx"));
                   
                        using (XmlReader xmlReader = XmlReader.Create(modsettings))
                        {
                            XDocument xml = XDocument.Load(xmlReader);
                            LsxTools lt = new LsxTools();

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
                        Profiles.Add(settings);
                    }

                    //FIXME should be redundant, but isn't in the case of refresh...
                    ActiveProfile = Profiles.FirstOrDefault(x => x.IsActive);

                    Logger.Status = "Finished.";
                    Logger.LogString("Finished loading profile data.");
                    return true;
                }
                catch (Exception e)
                {
                    MessageBoxResult result = MessageBox.Show(
                   "Something went wrong when trying to load profile data. Please check your paths or start the game once.",
                   "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.Status = "Finished With Errors.";
                    Logger.LogString("Something went wrong when trying to load profile data. Please check your paths or start the game once.");
                    Logger.LogString(e.ToString());
                    return false;
                }
            }
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



