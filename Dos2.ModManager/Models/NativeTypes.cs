using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dos2.ModManager.Models
{
    /// <summary>
    /// playerprofile.lsb
    /// </summary>
    public class DOS2_UserProfiles
    {
        public string ActiveProfile { get; set; }
    }

    /// <summary>
    /// profile.lsb
    /// </summary>
    public class DOS2_PlayerProfile
    {
       public string PlayerProfileDisplayName { get; set; }
       public string PlayerProfileID { get; set; }
       public string PlayerProfileName { get; set; }
       public string Version { get; set; }
    }

    public struct ModuleShortDesc
    {
        public string Folder { get; set; }
        public string MD5 { get; set; }
        public string Name { get; set; }
        public string UUID { get; set; }
        public string Version { get; set; }
    }

}
