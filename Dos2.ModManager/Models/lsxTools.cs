using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using LSLib.LS;
using LSLib.LS.Enums;

namespace Dos2.ModManager.Models
{
    public class LsxTools
    {

        public LsxTools()
        {
           
        }

        /// <summary>
        /// Reads playerprofiles.lsb
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public DOS2_UserProfiles GetActiveProfile(string file)
        {
            DOS2_UserProfiles up = new DOS2_UserProfiles();

            if (File.Exists(file))
            {
                try
                {
                    Resource res = ResourceUtils.LoadResource(file);
                    Node root = res.Regions.First().Value;
                    up.ActiveProfile = root.Attributes["ActiveProfile"].Value.ToString();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return up;
        }

        /// <summary>
        /// Reads profile.lsb
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public DOS2_PlayerProfile GetProfile(string file)
        {
            DOS2_PlayerProfile up = new DOS2_PlayerProfile();

            if (File.Exists(file))
            {
                try
                {
                    Resource res = ResourceUtils.LoadResource(file);
                    Node root = res.Regions.First().Value;

                    up.PlayerProfileDisplayName = root.Attributes["PlayerProfileDisplayName"].Value.ToString();
                    up.PlayerProfileID = root.Attributes["PlayerProfileID"].Value.ToString();
                    up.PlayerProfileName = root.Attributes["PlayerProfileName"].Value.ToString();
                    up.Version = root.Attributes["Version"].Value.ToString();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return up;
        }



        /// <summary>
        /// Update Modsettings file.
        /// </summary>
        /// <param name="activeProfile"></param>
        /// <param name="ModsList"></param>
        public void SaveModSettings(Dos2ModsSettings activeProfile, List<Dos2Mod> ModsList)
        {
            string profileDir = Path.Combine(Path.GetDirectoryName(Dos2.ModManager.Properties.Settings.Default.Mods), @"PlayerProfiles");
            List<DirectoryInfo> profileDirList = Directory.GetDirectories(profileDir).Select(x => new DirectoryInfo(x)).ToList();
            string file = Path.Combine(profileDirList.FirstOrDefault(x => x.Name == activeProfile.Name).FullName, @"modsettings.lsx");


            Resource resource = ResourceUtils.LoadResource(file);

            resource = ApplyChangesForProfile(resource, ModsList);

            //dbg
            //string destinationPath = @"E:\out.lsx";
            //dbg

            string destinationPath = file;
            ResourceFormat resourceFormat = ResourceFormat.LSX;
            ResourceUtils.SaveResource(resource, destinationPath, resourceFormat);



        }

        /// <summary>
        /// Get changes made in the Mod Manager
        /// </summary>
        /// <param name="activeProfile"></param>
        private Resource ApplyChangesForProfile(Resource res, List<Dos2Mod> ModsList)
        {
            Resource newResource = res;

            // MOD ORDER
            Node modOrder = WriteModOrder(res, ModsList);

            // MODS
            Node mods = WriteMods(res, ModsList);

            // change nodes inside the resource



            return newResource;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res"></param>
        /// <param name="modsList"></param>
        /// <returns></returns>
        private Node WriteMods(Resource res, List<Dos2Mod> modsList)
        {

            // Get Node to change
            Node Mods = res.Regions.First().Value.Children["Mods"].First();
            // get Dos2 mod
            Node dos2main = Mods.Children["ModuleShortDesc"].First(x => x.Attributes["Name"].Value.ToString() == "Divinity: Original Sin 2");
            // Get changes
            List <ModuleShortDesc> _modDescriptions = new List<ModuleShortDesc>();
            foreach (var item in modsList.Where(x => x.IsEnabled).ToList())
            {
                ModuleShortDesc temp = new ModuleShortDesc
                {
                    Folder = item.Folder,
                    MD5 = item.MD5,
                    Name = item.Name,
                    UUID = item.UUID,
                    Version = item.Version
                };
                _modDescriptions.Add(temp);
            }
            // Write as LS.Node

            Dictionary<string, List<Node>> childrenDict = new Dictionary<string, List<Node>>();
            List<Node> children = new List<Node>();
            children.Add(dos2main);

            foreach (ModuleShortDesc item in _modDescriptions)
            {
                Dictionary<string, NodeAttribute> attributes = new Dictionary<string, NodeAttribute>();
                NodeAttribute Folder = new NodeAttribute(NodeAttribute.DataType.DT_LSWString)
                {
                    Value = item.Folder
                };
                attributes.Add("Folder", Folder);
                NodeAttribute MD5 = new NodeAttribute(NodeAttribute.DataType.DT_LSString)
                {
                    Value = item.MD5
                };
                attributes.Add("MD5", MD5);
                NodeAttribute Name = new NodeAttribute(NodeAttribute.DataType.DT_FixedString)
                {
                    Value = item.Name
                };
                attributes.Add("Name", Name);
                NodeAttribute UUID = new NodeAttribute(NodeAttribute.DataType.DT_FixedString)
                {
                    Value = item.UUID
                };
                attributes.Add("UUID", UUID);
                NodeAttribute Version = new NodeAttribute(NodeAttribute.DataType.DT_Int)
                {
                    Value = item.Version
                };
                attributes.Add("Version", Version);


                Node child = new Node
                {
                    Name = "ModuleShortDesc",
                    Parent = Mods,
                    Attributes = attributes,
                };
                children.Add(child);
            }
            childrenDict.Add("ModuleShortDesc", children);
            // write to resource
            Mods.Children = childrenDict;

            return Mods;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res"></param>
        /// <param name="modsList"></param>
        /// <returns></returns>
        private Node WriteModOrder(Resource res, List<Dos2Mod> modsList)
        {

            // Get Node to change
            Node ModOrder = res.Regions.First().Value.Children["ModOrder"].First();

            // Get changes
            // Get mod UUIDs
            List<string> _mods_ordered = new List<string>();
            for (int i = 0; i < modsList.Count; i++)
            {
                _mods_ordered.Add(modsList.First(x => x.LoadOrder == i).UUID);
            }
            // Write as LS.Node
            Dictionary<string, List<Node>> childrenDict = new Dictionary<string, List<Node>>();
            List<Node> children = new List<Node>();
            foreach (string item in _mods_ordered)
            {
                Dictionary<string, NodeAttribute> attributes = new Dictionary<string, NodeAttribute>();
                NodeAttribute UUID = new NodeAttribute(NodeAttribute.DataType.DT_FixedString)
                {
                    Value = item
                };
                attributes.Add("UUID", UUID);

                Node child = new Node
                {
                    Name = "Module",
                    Parent = ModOrder,
                    Attributes = attributes,
                };
                children.Add(child);
            }
            childrenDict.Add("Module", children);
            // write to resource
            ModOrder.Children = childrenDict;

            return ModOrder;
        }


        /// <summary>
        /// Gets xml attributes by name from a list of XElements
        /// </summary>
        /// <param name="list"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public string GetAttributeByName(List<XElement> list, string v)
        {
            return list.FirstOrDefault(x => x.Attribute("id").Value == v).Attribute("value").Value;
        }


        

    }
}
