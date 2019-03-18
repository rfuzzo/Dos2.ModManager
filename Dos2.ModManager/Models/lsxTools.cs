using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Dos2.ModManager.Models
{
    class LsxTools
    {
        private XDocument doc;


        public LsxTools()
        {
           
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

        public void SaveModSettings(Dos2ModsSettings activeProfile, List<Dos2Mod> ModsList)
        {

            //write mod settings to lsx
            LsxTools ls = new LsxTools();

            // Change active profile (playerprofile.lsb)
            // FIXME

            // change profile settings (.../profile.lsx)
            // FIXME

            // change mod order and selected mods (.../modsettings.lsx)
            string profileDir = Path.Combine(Path.GetDirectoryName(Dos2.ModManager.Properties.Settings.Default.Mods), @"PlayerProfiles");
            List<DirectoryInfo> profileDirList = Directory.GetDirectories(profileDir).Select(x => new DirectoryInfo(x)).ToList();
            string file = Path.Combine(profileDirList.FirstOrDefault(x => x.Name == activeProfile.Name).FullName, @"modsettings.lsx");

            // get changes
            List<string> mods_ordered = new List<string>();
            for (int i = 0; i < ModsList.Count; i++)
            {
                mods_ordered.Add(ModsList.First(x => x.LoadOrder == i).UUID);
            }
            //write as XElement
            List<XElement> moElsNew = new List<XElement>();
            foreach (string item in mods_ordered)
            {
                moElsNew.Add(
                    new XElement("node", new XAttribute("id", "Module"),
                        new XElement("attribute", new XAttribute("id", "UUID"), new XAttribute("value", item), new XAttribute("type", "22"))
                        )
                    );
            }
            var a1 = new XElement("node", new XAttribute("id", "ModOrder"));
            var a2 = new XElement("children");
            foreach (XElement item in moElsNew)
            {
                a2.Add(item);
            }
            a1.Add(a2);


            List<ModuleShortDesc> mods = new List<ModuleShortDesc>();
            foreach (var item in ModsList.Where(x => x.IsEnabled).ToList())
            {
                ModuleShortDesc temp = new ModuleShortDesc
                {
                    Folder = item.Folder,
                    MD5 = item.MD5,
                    Name = item.Name,
                    UUID = item.UUID,
                    Version = item.Version
                };
                mods.Add(temp);
            }
            var b1 = new XElement("node", new XAttribute("id", "Mods"));
            var b2 = new XElement("children");
            foreach (var item in mods)
            {

                var b3 = new XElement("node", new XAttribute("id", "ModuleShortDesc"));
                var b3f = new XElement("attribute",
                    new XAttribute("id", "Folder"),
                    new XAttribute("value", item.Folder),
                    new XAttribute("type", "30"));
                var b3m = new XElement("attribute",
                    new XAttribute("id", "MD5"),
                    new XAttribute("value", item.MD5),
                    new XAttribute("type", "30"));
                var b3n = new XElement("attribute",
                    new XAttribute("id", "Name"),
                    new XAttribute("value", item.Name),
                    new XAttribute("type", "30"));
                var b3u = new XElement("attribute",
                    new XAttribute("id", "UUID"),
                    new XAttribute("value", item.UUID),
                    new XAttribute("type", "30"));
                var b3v = new XElement("attribute",
                    new XAttribute("id", "Version"),
                    new XAttribute("value", item.Version),
                    new XAttribute("type", "30"));
                b3.Add(b3f);
                b3.Add(b3m);
                b3.Add(b3n);
                b3.Add(b3u);
                b3.Add(b3v);

                b2.Add(b3);
            }
            b1.Add(b2);



            //build new xml
            try
            {
                //StringBuilder sb = new StringBuilder();
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.OmitXmlDeclaration = false;
                xws.Indent = true;


                XDocument doc = XDocument.Load(file);

                using (XmlWriter xmlWriter = XmlWriter.Create(file, xws))
                {
                    //change nodes
                    XElement ModOrderEl = doc.Descendants("node").FirstOrDefault(x => x.Attribute("id").Value == "ModOrder");
                    ModOrderEl.ReplaceWith(a1);


                    XElement ModsEl = doc.Descendants("node").FirstOrDefault(x => x.Attribute("id").Value == "Mods");
                    b1.Elements().First().AddFirst(ModsEl.Elements().First().Elements().First());
                    ModsEl.ReplaceWith(b1);

                    // save as file
                    doc.Save(xmlWriter);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
