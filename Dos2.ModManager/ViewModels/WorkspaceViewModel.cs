using Dos2.ModManager.Commands;
using Dos2.ModManager.Models;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;

namespace Dos2.ModManager.ViewModels
{
    /// <summary>
    /// SUMMARY: viewmodel for wcc lite workflows
    /// TODO serialize as xmls
    /// 
    /// </summary>
    public class WorkspaceViewModel : DockableViewModel, IDropTarget
    {
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

      

        private ListCollectionView _modsCollectionView;
        /// <summary>
        /// Holds the ModList stored in the Settings.
        /// </summary>
        public ListCollectionView ModsCollectionView
        {
            get
            {
                _modsCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(ModsList);
                return _modsCollectionView;
            }
            set
            {
                if (_modsCollectionView != value)
                {
                    _modsCollectionView = value;
                    InvokePropertyChanged();

                    ModsCollectionView.CommitEdit();
                }
            }
        }

        /// <summary>
        /// File path to the to-serialize xml
        /// </summary>
        #region FilePath
        private string _filePath;
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    InvokePropertyChanged();
                    InvokePropertyChanged(nameof(Title));
                }
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        #region Title
        public override string Title
        {
            get
            {
                if (String.IsNullOrEmpty(FilePath))
                {
                    return base.Title;
                }
                if (HasUnsavedChanges)
                {
                    return String.Format("{0}*", Path.GetFileName(FilePath));
                }
                return Path.GetFileName(FilePath);
            }
        }
       
        #endregion

        #region HasUnsavedChanges
        private bool _hasUnsavedChanges;
        public bool HasUnsavedChanges
        {
            get
            {
                return _hasUnsavedChanges;
            }
            set
            {
                if (_hasUnsavedChanges != value)
                {
                    _hasUnsavedChanges = value;
                    InvokePropertyChanged();
                    InvokePropertyChanged(nameof(Title));
                }
            }
        }
        #endregion

        #region IsSelected
        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    InvokePropertyChanged();
                }
            }
        }
        #endregion

        #endregion

       

        public WorkspaceViewModel(MainViewModel vm)
        {
            ParentViewModel = vm;

            ModsList = vm.ModsList;

            //MOD DATA
            //Get Mod Info
            GetModDataAsync();

        }


        /// <summary>
        /// Gets Moddata from paks if not already in ModList
        /// </summary>
        #region GetModData
        /// <summary>
        /// Gets Moddata from paks if not already in ModList
        /// </summary>
        public async void GetModDataAsync()
        {
            string modDir = Path.Combine(Path.GetDirectoryName(Dos2.ModManager.Properties.Settings.Default.Mods), @"Mods");
            // checks
            #region checks
            if (!Directory.Exists(Dos2.ModManager.Properties.Settings.Default.WorkingDir) ||
                !Directory.Exists(modDir))
            {
                MessageBoxResult result = MessageBox.Show(
                "No mod directory found. Please check your paths.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Directory.GetFiles(modDir).ToList().Any())
                return;
            #endregion

            try
            {
                List<string> modFiles = Directory.GetFiles(modDir).ToList();

                //Logging Start
                ParentViewModel.Logger.ProgressValue = 0;
                ParentViewModel.Logger.IsIndeterminate = false;

                for (int i = 0; i < modFiles.Count; i++)
                {
                    string modPath = modFiles[i];

                    //get UUID
                    string uuid = Path.GetFileNameWithoutExtension(modPath).Split('_').Last();
                    string meta = "";

                    //check if mod is already in ModList
                    // FIXME check for updates
                    if (!ModsList.Where(x => x.UUID.Equals(uuid)).Any())
                    {
                        meta = await Task.Run(() => ExtractModMeta(modPath));
                        if (String.IsNullOrEmpty(meta))
                            continue;

                        //interpret meta.lsx
                        Dos2Mod mod = InterpretModMeta(meta);
                        mod.PakPath = modPath;

                        //add to modslist
                        ModsList.Add(mod);
                    }
                    else
                    {
                        //is updated
                        Dos2Mod oldmod = ModsList.FirstOrDefault(x => x.UUID.Equals(uuid));
                        if (oldmod.IsUpdated)
                        {
                            //extract meta.lsx and interpret

                            //replace existing mod
                            throw new NotImplementedException();
                        }
                        else
                        {
                            //do nothing.
                        }
                    }

                    //Logging Progress
                    int prg = Convert.ToInt32(100 / (modFiles.Count - 1) * i);
                    ParentViewModel.Logger.ProgressValue = prg;
                }

                //Logging End
                ParentViewModel.Logger.IsIndeterminate = false;
                ParentViewModel.Logger.NotifyStatusChanged();

                // populate files list for mods
                // FIXME blocking?
                GetModFilesAsync();


                ApplyModSettings(ParentViewModel.ActiveProfile);
            }
            catch (Exception)
            {
                MessageBoxResult result = MessageBox.Show(
                   "Something went wrong when trying to load mod data. Please check your paths.",
                   "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
                //throw;
            }
        }

        /// <summary>
        /// populate mod files list
        /// </summary>
        private async void GetModFilesAsync()
        {
            //Logging Start
            ParentViewModel.Logger.ProgressValue = 0;
            ParentViewModel.Logger.IsIndeterminate = false;

            for (int i = 0; i < ModsList.Count; i++)
            {
                Dos2Mod mod = (Dos2Mod)ModsList[i];

                if (mod.Files != null && mod.Files.Any())
                    continue;

                //get file list
                var rawoutput = await Task.Run(() => GetFileListForPak(mod.PakPath));
                var fileList = new List<string>();
                foreach (var item in rawoutput)
                {
                    var f = item.Split('\t').First();
                    f = f.Substring(f.IndexOf('/') + 1);
                    f = f.Substring(f.IndexOf('/') + 1);

                    string[] hidden = new string[] { "goals.raw", "story.div", "goals.div", "meta.lsx" };

                    if (!(hidden.Any(f.Contains) || String.IsNullOrEmpty(f)))
                        fileList.Add(f);
                }

                mod.Files = fileList;

                //Logging Progress
                int prg = Convert.ToInt32(100 / (ModsList.Count - 1 )* i);
                ParentViewModel.Logger.ProgressValue = prg;
            }

            //Logging End
            ParentViewModel.Logger.ProgressValue = 100;
            ParentViewModel.Logger.IsIndeterminate = false;
            ParentViewModel.Logger.NotifyStatusChanged();

            // set ready and save data
            ParentViewModel.IsInitialized = true;
            Dos2.ModManager.Properties.Settings.Default.Save();
        }

        /// <summary>
        /// calls divine.exe TaskHandler to list files for a .pak
        /// </summary>
        /// <param name="pak"></param>
        /// <returns></returns>
        private async Task<List<string>> GetFileListForPak(string pak)
        {

            List<string> files = new List<string>();

            string argument = "list-package";
            string source = pak;

            string arg = $"-a {argument} -s \"{source}\"";
            await ParentViewModel.DivineTaskHandler.RunArgs(arg);
            files = ParentViewModel.DivineTaskHandler.Output.Split(';').ToList();

            return files;
        }

        /// <summary>
        /// calls divine.exe TaskHandler to extract meta.lsx
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        private async Task<string> ExtractModMeta(string mod)
        {

            string wd = Dos2.ModManager.Properties.Settings.Default.WorkingDir;
            string modName = Path.GetFileNameWithoutExtension(mod);

            string meta = Path.Combine(wd, $"{modName}.lsx");

            //extract meta.lsx into a working dir location
            //FIXME move into Divine-Command class
            string argument = "extract-single-file";
            string file = "meta.lsx";
            string source = mod;
            string destination = meta;


            //check if paths exist

            string arg = $"-a {argument} -s \"{source}\" -d \"{destination}\" -f {file}";
            await ParentViewModel.DivineTaskHandler.RunArgs(arg);



            //check if outputfile was created //this should never trigger
            if (!File.Exists(meta))
                return "";


            return meta;
        }

        /// <summary>
        /// reads the lsx (xml)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private Dos2Mod InterpretModMeta(string file)
        {
            Dos2Mod modData = new Dos2Mod();

            try
            {
                using (XmlReader xmlReader = XmlReader.Create(file))
                {
                    XDocument xml = XDocument.Load(xmlReader);
                    LsxTools ls = ParentViewModel.ls;

                    //Mod Info
                    XElement moduleInfo = xml.Descendants("node").FirstOrDefault(x => x.Attribute("id").Value == "ModuleInfo");
                    List<XElement> els = moduleInfo.Elements().ToList();

                    modData.Name = ls.GetAttributeByName(els, "Name");
                    modData.UUID = ls.GetAttributeByName(els, "UUID");
                    modData.Author = ls.GetAttributeByName(els, "Author");
                    modData.Description = ls.GetAttributeByName(els, "Description");
                    modData.Folder = ls.GetAttributeByName(els, "Folder");
                    modData.Tags = ls.GetAttributeByName(els, "Tags");
                    modData.MD5 = ls.GetAttributeByName(els, "MD5");
                    modData.Type = ls.GetAttributeByName(els, "Type");
                    modData.Version = ls.GetAttributeByName(els, "Version");

                    //Target Modes
                    XElement targetModes = xml.Descendants("node").FirstOrDefault(x => x.Attribute("id").Value == "TargetModes");
                    var modes = targetModes.Elements().First().Elements().ToList();
                    foreach (var item in modes)
                    {
                        var atts = item.Elements().ToList();
                        string target = ls.GetAttributeByName(atts, "Object");
                        modData.TargetModes.Add(target);
                    }

                    //Dependencies
                    XElement dependencies = xml.Descendants("node").FirstOrDefault(x => x.Attribute("id").Value == "Dependencies");
                    if (dependencies.HasElements)
                    {
                        var deps = dependencies.Elements().First().Elements().ToList();
                        foreach (var item in deps)
                        {
                            var atts = item.Elements().ToList();
                            string uuid = ls.GetAttributeByName(atts, "UUID");
                            modData.Dependencies.Add(uuid);
                        }
                    }


                }
            }
            catch (Exception e)
            {

                throw;
            }
            return modData;
        }


        #endregion

        /// <summary>
        /// Applies Profile Settings to ModList
        /// </summary>
        #region Appply Mod Settings
        /// <summary>
        /// Change Load Order and enables mods depending on active profile
        /// </summary>
        public void ApplyModSettings(Dos2ModsSettings activeSettings)
        {
            //Dos2ModsSettings activeSettings = ParentViewModel.Profiles.FirstOrDefault(x => x.IsActive);
            foreach (var mod in ModsList)
            {
                mod.IsEnabled = activeSettings.ActiveMods.Contains(mod.UUID);
                //find index of mod by UUID
                mod.LoadOrder = activeSettings.ModLoadOrder.FindIndex(x => x == mod.UUID);
            }
            var uncategorized = ModsList.Where(x => x.LoadOrder < 0).ToList();
            foreach (var item in uncategorized)
            {
                // give highest load order index 
                int maxLoadOrder = ModsList.Max(x => x.LoadOrder);
                item.LoadOrder = maxLoadOrder + 1;
            }

            SortCollectionByProperty("LoadOrder");
        }

        private void SortCollectionByProperty(string prop)
        {
            ModsCollectionView.CommitEdit();

            ModsCollectionView.SortDescriptions.Clear();
            ModsCollectionView.SortDescriptions.Add(new SortDescription(prop, ListSortDirection.Ascending));
        }

        #endregion

        /// <summary>
        /// Drag and drop inside mods list
        /// </summary>
        #region Drag and Drop Implementation
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            Dos2Mod sourceItem = dropInfo.Data as Dos2Mod;
            Dos2Mod targetItem = dropInfo.TargetItem as Dos2Mod;

            if (!ParentViewModel.IsInitialized)
                return;

            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {

            Dos2Mod sourceItem = dropInfo.Data as Dos2Mod;
            Dos2Mod targetItem = dropInfo.TargetItem as Dos2Mod;

            var insertIndex = dropInfo.InsertIndex;
            var sourceIndex = dropInfo.DragInfo.SourceIndex;
            insertIndex = Math.Min(insertIndex, ModsList.Count - 1);

            if (!ParentViewModel.IsInitialized)
                return;

            if (insertIndex < sourceIndex) //move up
            {
                List<Dos2Mod> higher = ModsList.Where(x => x.LoadOrder >= insertIndex && x.LoadOrder < sourceIndex).ToList();
                foreach (var item in higher)
                {
                    item.LoadOrder += 1;
                }
                sourceItem.LoadOrder = insertIndex;
            }
            else //move down
            {
                List<Dos2Mod> lower = ModsList.Where(x => x.LoadOrder <= insertIndex && x.LoadOrder > sourceIndex).ToList();
                foreach (var item in lower)
                {
                    item.LoadOrder -= 1;
                }
                sourceItem.LoadOrder = insertIndex;
            }
           

            SortCollectionByProperty("LoadOrder");

        }
        #endregion

    }
}
