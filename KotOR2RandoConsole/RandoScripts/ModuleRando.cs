using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KotOR_IO;
using System.IO;

namespace KotOR2RandoConsole
{
    public static class ModuleRando
    {
        internal static Dictionary<string, string> LookupTable { get; private set; } = new Dictionary<string, string>();
        public static List<string> RandomizedModules { get; set; }
        public static List<string> OmittedModules { get; set; }

        public static void Module_rando(K2Paths paths)
        {
            paths.BackUpModulesDirectory();
            paths.BackUpLipsDirectory();
            paths.BackUpOverrideDirectory();
            OmittedModules = new(File.ReadAllLines("OmitMods.txt"));
            if (OmittedModules.Last() == "") OmittedModules.RemoveAt(OmittedModules.Count - 1);

            var modules = paths.FilesInModules.Where(m => !OmittedModules.Contains(m.Name.Substring(0, 6)));
            RandomizedModules = new(modules.Select(m => m.Name.Substring(0, 6)).Distinct());

            // Shuffle the list of included modules.
            List<string> shuffle = new List<string>(RandomizedModules);
            Randomize.FisherYatesShuffle(shuffle);
            LookupTable.Clear();

            for (int i = 0; i < RandomizedModules.Count; i++)
            {
                LookupTable.Add(RandomizedModules[i], shuffle[i]);
            }

            // Include the unmodified list of excluded modules.
            foreach (string name in OmittedModules)
            {
                LookupTable.Add(name, name);
            }

            File.AppendAllText(paths.RANDOMIZED_LOG, "----------------\nMODULE MAPPINGS\n----------------\n");
            // Copy shuffled modules into the base directory.
            foreach (var name in LookupTable)
            {
                File.Copy($"{paths.modules_backup}{name.Key}.rim", $"{paths.modules}{name.Value}.rim", true);
                File.Copy($"{paths.modules_backup}{name.Key}_s.rim", $"{paths.modules}{name.Value}_s.rim", true);
                try { File.Copy($"{paths.lips_backup}{name.Key}_loc.mod", $"{paths.lips}{name.Value}_loc.mod", true); }
                catch (Exception) {
                    if (name.Key != name.Value)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Skipping lip for {name.Key}, consider omitting?");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    
                }
                File.Copy($"{paths.modules_backup}{name.Key}_dlg.erf", $"{paths.modules}{name.Value}_dlg.erf", true);

                //TEMPORARY SPOLIERS
                File.AppendAllText(paths.RANDOMIZED_LOG, $"{name.Key} -> {name.Value}\n");

            }
            File.Copy($"{paths.lips_backup}localization.mod", $"{paths.lips}{"localization.mod"}", true);
            File.AppendAllText(paths.RANDOMIZED_LOG, "----------------\n");

            //Misc Patches
            File.WriteAllBytes(paths.Override + "a_disc_join.ncs", Properties.Resources.a_disc_join); //Disciple Crash Patch


            //Unlock Galxy Map
            if (Properties.UserSettings.Default.GalaxyMapUnlocked)
            {
                File.WriteAllBytes(paths.Override + "a_galaxymap.ncs", Properties.Resources.a_galaxymap);
            }

            //Module Save Patch
            if (Properties.UserSettings.Default.ModuleSavePatch)
            {
                File.WriteAllBytes(paths.Override + "modulesave.2da", Properties.Resources.modulesave);
            }
        }
    }
}
