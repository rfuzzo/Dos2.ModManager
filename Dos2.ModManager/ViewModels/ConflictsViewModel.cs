using Dos2.ModManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dos2.ModManager.ViewModels
{
    public class ConflictsViewModel : DockableViewModel
    {
        #region Properties

        private ObservableCollection<Dos2Conflict> _conflictsList;
        /// <summary>
        /// Holds the global active Conflicts.
        /// </summary>
        public ObservableCollection<Dos2Conflict> ConflictsList
        {
            get
            {

                return GetConflicts();
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

        private ObservableCollection<Dos2Conflict> GetConflicts()
        {
            //List<string> HigherConflicts = new List<string>();
            //List<string> LowerConflicts = new List<string>();

            ObservableCollection<Dos2Conflict> ret = new ObservableCollection<Dos2Conflict>();

            Dos2Mod mod = ParentViewModel.ActiveProperty;
            if (mod == null)
                return ret;
            if (!mod.IsEnabled)
                return ret;

            List<Dos2Mod> activeMods = ParentViewModel.ModsList.Where(x => x.IsEnabled).ToList();
            List<Dos2Mod> othermods = activeMods.Where(x => x.UUID != mod.UUID).ToList();
            //List<string> allotherfiles = new List<string>();


            foreach (Dos2Mod mod2compare in othermods)
            {
                foreach (string file in mod.Files)
                {
                    string[] hidden = new string[] { "Assets", "Content" };

                    string first = file.Split('/').First();
                    if (hidden.Contains(first))
                    {
                        continue;
                    }


                    if (mod2compare.Files.Contains(file))
                    {
                        string type = "";
                        if (mod2compare.LoadOrder > mod.LoadOrder)
                            type = "Higher Conflicts";
                        else
                            type = "Lower Conflicts";

                        Dos2Conflict conf = new Dos2Conflict
                        {
                            Name = file,
                            ModID = mod2compare.Name,
                            Type = type,
                        };
                        ret.Add(conf);
                    }
                }

            }
            return ret;
        }

        private ObservableCollection<Dos2Mod> _selectedMods;
        /// <summary>
        /// Holds the global active Conflicts.
        /// </summary>
        public ObservableCollection<Dos2Mod> SelectedMods
        {
            get
            {
                return _selectedMods;
            }
            set
            {
                if (_selectedMods != value)
                {
                    _selectedMods = value;
                    InvokePropertyChanged();
                }
            }
        }


        #endregion





        public ConflictsViewModel(MainViewModel vm)
        {
            ParentViewModel = vm;

            //ConflictsList = new ObservableCollection<Dos2Conflict>();
            SelectedMods = new ObservableCollection<Dos2Mod>();

            //populate conflicts list
            //RegenerateConflictsList();
            



        }

        public void RegenerateConflictsList()
        {
            ConflictsList = GetConflicts();
            
            
            /*ConflictsList.Clear();
            var modlist = Properties.Settings.Default.ModList;

            List<Dos2Mod> activeMods = modlist.Where(x => x.IsEnabled).ToList();
            foreach (Dos2Mod mod in activeMods)
            {
                List<Dos2Mod> othermods = activeMods.Where(x => x.UUID != mod.UUID).ToList();
                List<string> allotherfiles = new List<string>();
                foreach (var item in othermods)
                {
                    allotherfiles.AddRange(item.Files);
                    allotherfiles = allotherfiles.Distinct().ToList();
                }

                foreach (string file in mod.Files)
                {

                    if (allotherfiles.Contains(file))
                    {
                        Dos2Conflict conf = new Dos2Conflict
                        {
                            Name = file,
                            ModID = mod.Name
                        };
                        ConflictsList.Add(conf);
                    }
                }

            }*/
        }
    }
}
