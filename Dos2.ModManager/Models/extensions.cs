using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dos2.ModManager.Models
{


    /*
    /// <summary>
    /// reads activeProfile lsx and gets UUID
    /// </summary>
    #region GetActiveProfile
    /// <summary>
    /// Return UUID of active profile
    /// </summary>
    /// <returns></returns>
    private string GetActiveProfileID()
    {
        string infile = Path.Combine(Dos2.ModManager.Properties.Settings.Default.WorkingDir, @"playerprofiles.lsx");
        string uuid = "";

        try
        {
            using (XmlReader xmlReader = XmlReader.Create(infile))
            {
                XDocument xml = XDocument.Load(xmlReader);

                XElement rootNode = xml.Descendants("node").FirstOrDefault(x => x.Attribute("id").Value == "root");
                if (rootNode.HasElements)
                {
                    List<XElement> els = rootNode.Elements().ToList();
                    uuid = els.FirstOrDefault(x => x.Attribute("id").Value == "ActiveProfile").Attribute("value").Value;
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
        return uuid;
    }
    #endregion
    */

    /// <summary>
    /// reads playerprofiles.lsb as binary and extract uuid of active profile
    /// </summary>
    /// <returns></returns>
    /*private void BinaryReadLSB()
    {
        string profileDir = Path.Combine(Path.GetDirectoryName(Dos2.ModManager.Properties.Settings.Default.Mods), @"PlayerProfiles");
        string fileName = Path.Combine(profileDir, @"playerprofiles.lsb");
        string uuid = "";

        if (File.Exists(fileName))
        {


            using (BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                //interpret binary

                var version = br.ReadSingle();        
                var filelength = br.ReadUInt32();
                var b = br.ReadBytes(8);
                var time = br.ReadUInt64();
                var major = br.ReadUInt32();
                var minor = br.ReadUInt32();
                var revision = br.ReadUInt32();
                var build = br.ReadUInt32();

                var icount = br.ReadUInt32();
                Dictionary<UInt32, string> idict = new Dictionary<UInt32, string>();
                for (int i = 0; i < icount; i++)
                {
                    var length = br.ReadUInt32();
                    var name = br.ReadBytes((int)length);
                    var key = br.ReadUInt32();

                    string value = System.Text.Encoding.UTF8.GetString(name);
                    idict.Add(key, value);
                }

                var rcount = br.ReadUInt32();
                for (int i = 0; i < rcount; i++)
                {
                    var key = br.ReadUInt32();
                    var value = br.ReadUInt32();
                }

                //read first node
                var identifier = br.ReadUInt32();
                var childrencount = br.ReadUInt32();
                var attcount = br.ReadUInt32();
                Dictionary<UInt32, UInt32> adict = new Dictionary<UInt32, UInt32>();
                for (int i = 0; i < attcount; i++)
                {
                    var id = br.ReadUInt32();
                    var type = br.ReadUInt32();
                }



            }
        }
    }*/

        /*
    /// <summary>
    /// Convertts playerProfile.lsb to lsx and moves to working directory
    /// </summary>
    #region ConvertProfileLSB
    /// <summary>
    /// Convertts playerProfile.lsb to lsx and moves to working directory
    /// </summary>
    private async void ConvertActiveProfileAsync()
    {
        string profileDir = Path.Combine(Path.GetDirectoryName(Dos2.ModManager.Properties.Settings.Default.Mods), @"PlayerProfiles");
        string profileFile = Path.Combine(profileDir, @"playerprofiles.lsb");

        //checks FIXME
        #region checks
        if (!Directory.Exists(Dos2.ModManager.Properties.Settings.Default.WorkingDir))
            return;
        if (!Directory.Exists(profileDir))
            return;
        if (!File.Exists(profileFile))
            return;
        #endregion

        try
        {
            string file = await Task.Run(() => ConvertProfileLSB(profileFile));
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Divine Task call
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private async Task<string> ConvertProfileLSB(string input)
    {
        string outfile = Path.Combine(Dos2.ModManager.Properties.Settings.Default.WorkingDir, @"playerprofiles.lsx");

        string argument = "convert-resource";
        string source = input;
        string destination = outfile;

        string arg = $"-a {argument} -s \"{source}\" -d \"{destination}\"";
        await DivineTaskHandler.RunArgs(arg);

        return outfile;
    }
    #endregion
    */
}
