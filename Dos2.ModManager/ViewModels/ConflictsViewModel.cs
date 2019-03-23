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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<Dos2Conflict> GetConflicts()
        {

            ObservableCollection<Dos2Conflict> ret = new ObservableCollection<Dos2Conflict>();

            //checks
            Dos2Mod mod = ParentViewModel.ActiveProperty;
            if (mod == null)
                return ret;
            if (!mod.IsEnabled)
                return ret;
            if (!ParentViewModel.IsInitialized)
                return ret;

            try
            {
                List<Dos2Mod> activeMods = ParentViewModel.ModsList.Where(x => x.IsEnabled).ToList();
                List<Dos2Mod> othermods = activeMods.Where(x => x.UUID != mod.UUID).ToList();


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
            }
            catch (Exception)
            {

                throw;
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

            SelectedMods = new ObservableCollection<Dos2Mod>();

        }

        public void RegenerateConflictsList()
        {
            ConflictsList = GetConflicts();
        }
    }
}
