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

      

        private ICollectionView _modsCollectionView;
        /// <summary>
        /// Holds the ModList stored in the Settings.
        /// </summary>
        public ICollectionView ModsCollectionView
        {
            get
            {
                _modsCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(ModsList);
                if (_modsCollectionView != null)
                    _modsCollectionView.SortDescriptions.Add(new SortDescription("LoadOrder", ListSortDirection.Ascending));
                return _modsCollectionView;
            }
            set
            {
                if (_modsCollectionView != value)
                {
                    _modsCollectionView = value;
                    InvokePropertyChanged();
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
            //_modsList.CollectionChanged += mods_CollectionChanged;

            //MOD DATA
            //Get Mod Info
            GetModDataAsync();

            //apply profile info to list (enable/disable mod and change load order)
            //ApplyModSettings();

            



        }

        /*private void mods_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
            }
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ParentViewModel.NotifyModChanged();
        }*/

        /// <summary>
        /// Applies Profile Settings to ModList
        /// </summary>
        #region Appply Mod Settings
        /// <summary>
        /// Change Load Order and enables mods depending on active profile
        /// </summary>
        public void ApplyModSettings()
        {
            Dos2ModsSettings activeSettings = ParentViewModel.Profiles.FirstOrDefault(x => x.IsActive);
            foreach (var mod in ModsList)
            {
                mod.LoadOrder = activeSettings.ModLoadOrder.FindIndex(x => x == mod.UUID);
                mod.IsEnabled = activeSettings.ActiveMods.Contains(mod.UUID);
            }

            SortCollectionByProperty("LoadOrder");
        }

        private void SortCollectionByProperty(string prop)
        {
            ModsCollectionView.SortDescriptions.Clear();
            ModsCollectionView.SortDescriptions.Add(new SortDescription(prop, ListSortDirection.Ascending));
        }

        #endregion



        /// <summary>
        /// Gets Moddata from paks if not already in ModList
        /// </summary>
        #region GetModData
        /// <summary>
        /// Gets Moddata from paks if not already in ModList
        /// </summary>
        private async void GetModDataAsync()
        {
            string modDir = Path.Combine(Path.GetDirectoryName(Dos2.ModManager.Properties.Settings.Default.Mods), @"Mods");
            try
            {
                //reduntant checks FIXME
                #region checks
                if (!Directory.Exists(Dos2.ModManager.Properties.Settings.Default.WorkingDir))
                    return;
                if (!Directory.Exists(modDir))
                    return;
                if (!Directory.GetFiles(modDir).ToList().Any())
                    return;
                #endregion  

                List<string> modFiles = Directory.GetFiles(modDir).ToList();

                foreach (var modPath in modFiles)
                {
                    //get UUID
                    string uuid = Path.GetFileNameWithoutExtension(modPath).Split('_').Last();
                    string meta = "";

                    //check if mod is already in ModList
                    if (!ModsList.Where(x => x.UUID.Equals(uuid)).Any())
                    {
                        meta = await Task.Run(() => ExtractModMeta(modPath));
                        if (String.IsNullOrEmpty(meta))
                            continue;

                        //interpret meta.lsx
                        Dos2Mod mod = InterpretModMeta(meta);
                        //Dos2Mod mod = await Task.Run(() => InterpretModMeta(meta));

                        //get file list
                        var rawoutput = await Task.Run(() => GetFileListForPak(modPath));
                        var fileList = new List<string>();
                        foreach (var item in rawoutput)
                        {
                            var f = item.Split('\t').First();
                            f = f.Substring(f.IndexOf('/') + 1);
                            f = f.Substring(f.IndexOf('/') + 1);
                            if (!(f.Contains("meta.lsx") || String.IsNullOrEmpty(f) ))
                                fileList.Add(f);
                        }

                        mod.Files = fileList;

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

                }

                ApplyModSettings();
            }
            catch (Exception)
            {
                throw;
            }
        }

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
        /// waits for file to be readable and reads the lsx (xml)
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
                    LsxTools lt = new LsxTools(xml);

                    //Mod Info
                    XElement moduleInfo = xml.Descendants("node").FirstOrDefault(x => x.Attribute("id").Value == "ModuleInfo");
                    List<XElement> els = moduleInfo.Elements().ToList();

                    modData.Name = lt.GetAttributeByName(els, "Name");
                    modData.UUID = lt.GetAttributeByName(els, "UUID");
                    modData.Author = lt.GetAttributeByName(els, "Author");
                    modData.Description = lt.GetAttributeByName(els, "Description");
                    modData.Folder = lt.GetAttributeByName(els, "Folder");
                    //modData.Tags = lt.GetAttributeByName(els, "Tags");
                    modData.Type = lt.GetAttributeByName(els, "Type");
                    modData.Version = lt.GetAttributeByName(els, "Version");

                    //Target Modes
                    XElement targetModes = xml.Descendants("node").FirstOrDefault(x => x.Attribute("id").Value == "TargetModes");
                    var modes = targetModes.Elements().First().Elements().ToList();
                    foreach (var item in modes)
                    {
                        var atts = item.Elements().ToList();
                        string target = lt.GetAttributeByName(atts, "Object");
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
                            string uuid = lt.GetAttributeByName(atts, "UUID");
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




        #region Drag and Drop Implementation
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            Dos2Mod sourceItem = dropInfo.Data as Dos2Mod;
            Dos2Mod targetItem = dropInfo.TargetItem as Dos2Mod;

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
