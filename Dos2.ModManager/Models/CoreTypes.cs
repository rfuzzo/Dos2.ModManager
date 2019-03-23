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

        [BrowsableAttribute(false)]
        [CategoryAttribute("Meta")]
        private bool _isEnabled;

        [ReadOnlyAttribute(false)]
        [CategoryAttribute("Meta")]
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
        [ReadOnlyAttribute(false)]
        [CategoryAttribute("Meta")]
        [TypeConverter(typeof(ListConverter))]
        public List<string> Files { get; set; }


        //Hidden
        [CategoryAttribute("Meta")]
        [ReadOnlyAttribute(true)]
        [BrowsableAttribute(false)]
        public string Category { get; set; }
        [CategoryAttribute("Meta")]
        [ReadOnlyAttribute(true)]
        [BrowsableAttribute(false)]
        public bool IsUpdated { get; set; }
        [CategoryAttribute("Meta")]
        [ReadOnlyAttribute(true)]
        [BrowsableAttribute(false)]
        public bool HasUnsavedChanges { get; set; }
        [CategoryAttribute("Meta")]
        [ReadOnlyAttribute(true)]
        [BrowsableAttribute(false)]
        public string PakPath { get; set; }


        //Mod Data
        [CategoryAttribute("Mod")]
        [ReadOnlyAttribute(true)]
        public string Name { get; set; }
        [CategoryAttribute("Mod")]
        [ReadOnlyAttribute(true)]
        public string UUID { get; set; }
        [CategoryAttribute("Mod")]
        [ReadOnlyAttribute(true)]
        public string Author { get; set; }
        [CategoryAttribute("Mod")]
        [ReadOnlyAttribute(true)]
        public string Description { get; set; }
        [CategoryAttribute("Mod")]
        [ReadOnlyAttribute(true)]
        public string Folder { get; set; }
        [CategoryAttribute("Mod")]
        [ReadOnlyAttribute(true)]
        public string Tags { get; set; }
        [CategoryAttribute("Mod")]
        [ReadOnlyAttribute(true)]
        public string Type { get; set; }
        [CategoryAttribute("Mod")]
        [ReadOnlyAttribute(true)]

        public string Version { get; set; }

        //public string CharacterCreationLevelName { get; set; }
        //public string LobbyLevelName { get; set; }
        //public string MenuLevelName { get; set; }
        //public string StartupLevelName { get; set; }
        //public string PhotoBooth { get; set; }
        //public string NumPlayers { get; set; }
        [CategoryAttribute("Mod")]
        [ReadOnlyAttribute(true)]
        public string MD5 { get; set; }
        //public string GMTemplate { get; set; }

        [CategoryAttribute("Mod")]
        [ReadOnlyAttribute(false)]
        [TypeConverter(typeof(ListConverter))]
        public List<string> TargetModes { get; set; }
        [CategoryAttribute("Mod")]
        [ReadOnlyAttribute(false)]
        [TypeConverter(typeof(ListConverter))]
        public List<string> Dependencies { get; set; }

        


       
    }


}
