using LSLib.LS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dos2.ModManager.ViewModels;

namespace Dos2.ModManager.Models
{
    public class PakTools
    {
        public MainViewModel parentVM { get; set; }

        public PakTools(MainViewModel vm)
        {
            parentVM = vm;
        }


        /// <summary>
        /// Get list files for a .pak
        /// </summary>
        /// <param name="pak"></param>
        /// <returns></returns>
        public List<string> GetFileListForPak(string packagePath)
        {

            List<string> files = new List<string>();

            try
            {
                using (var reader = new PackageReader(packagePath))
                {
                    var package = reader.Read();
                    foreach (var fileInfo in package.Files)
                    {
                        files.Add(fileInfo.Name);
                    }
                }
            }
            catch (NotAPackageException)
            {
                parentVM.Logger.LogString("Failed to list package contents because the package is not an Original Sin package or savegame archive");
            }
            catch (Exception e)
            {
                parentVM.Logger.LogString($"Failed to list package: {e.Message}");
                parentVM.Logger.LogString($"{e.StackTrace}");
            }

            return files;
        }



        /// <summary>
        /// Extract meta.lsxfrom pak
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public string ExtractModMeta(string packagePath)
        {

            string wd = Dos2.ModManager.Properties.Settings.Default.WorkingDir;
            string modName = Path.GetFileNameWithoutExtension(packagePath);
            string destinationPath = Path.Combine(wd, $"{modName}.lsx");

            string packagedPath = "meta.lsx";

            try
            {
                using (var reader = new PackageReader(packagePath))
                {
                    var package = reader.Read();
                    // Try to match by full path
                    var file = package.Files.Find(fileInfo => String.Compare(fileInfo.Name, packagedPath, StringComparison.OrdinalIgnoreCase) == 0);
                    if (file == null)
                    {
                        // Try to match by filename only
                        file = package.Files.Find(fileInfo => String.Compare(Path.GetFileName(fileInfo.Name), packagedPath, StringComparison.OrdinalIgnoreCase) == 0);
                        if (file == null)
                        {
                            parentVM.Logger.LogString($"Package doesn't contain file named '{packagedPath}'");
                            return "";
                        }
                    }

                    using (var fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                    {
                        try
                        {
                            var stream = file.MakeStream();
                            stream.CopyTo(fs);
                        }
                        finally
                        {
                            file.ReleaseStream();
                        }

                    }
                }
            }
            catch (NotAPackageException)
            {
                parentVM.Logger.LogString("Failed to list package contents because the package is not an Original Sin package or savegame archive");
            }
            catch (Exception e)
            {
                parentVM.Logger.LogString($"Failed to list package: {e.Message}");
                parentVM.Logger.LogString($"{e.StackTrace}");
            }

            //check if outputfile was created //this should never trigger
            if (!File.Exists(destinationPath))
                return "";


            return destinationPath;
        }



    }
}
