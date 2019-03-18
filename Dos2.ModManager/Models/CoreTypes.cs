using Dos2.ModManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dos2.ModManager.Models
{
    public class Dos2ModsSettings
    {
        public Dos2ModsSettings()
        {
            ModLoadOrder = new List<string>();
            ActiveMods = new List<string>();
        }

        //Meta Data
        public bool IsChanged { get; set; }

        //Profile Data
        public string Name { get; set; }
        public string UUID { get; set; }
        public bool IsActive { get; set; }


        //Mod Data
        public List<string> ModLoadOrder { get; set; }
        public List<string> ActiveMods { get; set; }


        public override string ToString()
        {
            return Name;
        }
    }

    public class Dos2Conflict
    {
        public Dos2Conflict()
        {

        }

        public string Name { get; set; }
        public string ModID { get; set; }
        public string Type { get; set; }


        public override string ToString()
        {
            return Name;
        }

    }

    [Serializable]
    public class Dos2Mod : ObservableObject
    {

        public Dos2Mod()
        {
            TargetModes = new List<string>();
            Dependencies = new List<string>();
        }

        //Meta Data
        
        [CategoryAttribute("Meta")]
        //public bool IsEnabled { get; set; }
        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    InvokePropertyChanged();
                }
            }
        }



        [ReadOnlyAttribute(true)]
        [CategoryAttribute("Meta")]
        public int LoadOrder { get; set; }


        [CategoryAttribute("Meta")]
        [BrowsableAttribute(false)]
        public string Category { get; set; }
        [CategoryAttribute("Meta")]
        [BrowsableAttribute(false)]
        public bool IsUpdated { get; set; }
        [CategoryAttribute("Meta")]
        [BrowsableAttribute(false)]
        public bool HasUnsavedChanges { get; set; }
        


        //Mod Data
        [CategoryAttribute("Mod")]
        public string Name { get; set; }
        [CategoryAttribute("Mod")]
        public string UUID { get; set; }
        [CategoryAttribute("Mod")]
        public string Author { get; set; }
        [CategoryAttribute("Mod")]
        public string Description { get; set; }
        [CategoryAttribute("Mod")]
        public string Folder { get; set; }
        //[CategoryAttribute("Mod")]
        //public string Tags { get; set; }
        [CategoryAttribute("Mod")]
        public string Type { get; set; }
        [CategoryAttribute("Mod")]

        public string Version { get; set; }

        //public string CharacterCreationLevelName { get; set; }
        //public string LobbyLevelName { get; set; }
        //public string MenuLevelName { get; set; }
        //public string StartupLevelName { get; set; }
        //public string PhotoBooth { get; set; }
        //public string NumPlayers { get; set; }
        //public string MD5 { get; set; }
        //public string GMTemplate { get; set; }

        [CategoryAttribute("Mod")]
        //[TypeConverter(typeof(ListConverter))]
        public List<string> TargetModes { get; set; }
        [CategoryAttribute("Mod")]
        //[TypeConverter(typeof(ListConverter))]
        public List<string> Dependencies { get; set; }

        [CategoryAttribute("Mod")]
        public List<string> Files { get; set; }


       
    }


}
