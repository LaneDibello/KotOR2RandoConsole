using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KotOR_IO;
using System.IO;
using System.Diagnostics;

namespace KotOR2RandoConsole
{
    public static class ModuleRando
    {
        #region Constants
        //Zones
        private const string AREA_PER_ADMIN     = "101PER";
        private const string AREA_PER_FUEL      = "103PER";
        private const string AREA_PER_ASTROID   = "104PER";
        private const string AREA_PER_DORMS     = "105PER";
        private const string AREA_PER_HANGER    = "106PER";
        private const string AREA_TEL_RES       = "203TEL";
        private const string AREA_TEL_ENTER_WAR = "222TEL";
        private const string AREA_TEL_ACAD      = "262TEL";
        private const string AREA_NAR_DOCKS     = "303NAR";
        private const string AREA_NAR_JEKK      = "304NAR";
        private const string AREA_NAR_J_TUNNELS = "305NAR";
        private const string AREA_NAR_G0T0      = "351NAR";
        private const string AREA_DAN_COURTYARD = "605DAN";
        private const string AREA_KOR_ACAD      = "702KOR";
        private const string AREA_KOR_SHY       = "710KOR";
        private const string AREA_MAL_SURFACE   = "901MAL";

        //Locked Doors
        private const string LABEL_101PERTODORMS = "sw_door_per006";         // Dorms from Admin
		private const string LABEL_101PERTOMININGTUNNELS = "sw_door_per004"; // Mining Tunnels from Admin
		private const string LABEL_101PERTOFUELDEPOT = "sw_door_per007";     // Fuel Depot from Admin
		private const string LABEL_101PERTOHARBINGER = "sw_door_taris009";   // Though the docking bay form Admin
		private const string LABEL_103PERTOMININGTUNNELS = "sw_door_per006"; // Explosion door at start of module
		private const string LABEL_103PERFORCESHIELDS = "sw_door_per005";    // Force fields splitting fuel depot into two sections
        private const string LABEL_103PERSHIELD2 = "sw_door_per010";         // Secondary Fuel Depot Shield blocking way down
        private const string LABEL_105PERTOASTROID = "sw_door_per005";       // Return to astroid exterior from Dormitory
        private const string LABEL_106PEREASTDOOR = "sw_door_per003";        // Door leading to Ebon Hawk
		private const string LABEL_203TELAPPTDOOR = "adoor_intro";           // Appartment door we spawn behind
		private const string LABEL_203TELEXCHANGE = "sw_door_telos002";      // Entacnce to Exchange base
		private const string LABEL_222TELRAVAGER = "d_222doorseal001";       // Board the ravager with mandalore, Visas, or the horrid DLZ
		private const string LABEL_262TELPLATEAU = "sw_door_sforg001";       // Polar Plateau from Atris's academy
		private const string LABEL_303NARZEZDOOR = "door_flophouse_s";       // Into the Secret zone with Mira and Zez that frequently breaks even in normal gameplay
		private const string LABEL_304NARBACKROOM = "visquisdoor";           // Door in Jekk'Jekk Tarr leading to Visquis's private suit, and the Tunnels
		private const string LABEL_305NARTOJEKKJEKK = "door_narshad002";     // Leave the tunnels and return to the cantina for once
		private const string LABEL_351NARG0T0EBONHAWK = "door_narshad008";   // Reboard the Ebon Hawk without doing the entirity of G0T0's yacht, though the CS it leads to breaks frequently, look into other options
		private const string LABEL_605DANREBUILTENCLAVE = "door_650";        // Enter the Rebuilt Jedi Enclave Early
		private const string LABEL_702KORVALLEY = "door_enter";              // Leave the Sith acadmeny without doing 10 minutes of puzzles or a DLZ
		private const string LABEL_710KORLUDOKRESSH = "sealeddoor";          // Enter the secret tomb in the shyrack cave without heavy alignment
        #endregion

        internal static Dictionary<string, string> LookupTable { get; private set; } = new Dictionary<string, string>();
        public static List<string> RandomizedModules { get; set; }
        public static List<string> OmittedModules { get; set; }

        public static void Module_rando(K2Paths paths)
        {
            List<Task> tasks = new();

            paths.BackUpModulesDirectory();
            paths.BackUpLipsDirectory();
            paths.BackUpOverrideDirectory();
            OmittedModules = new(File.ReadAllLines("OmitMods.txt").Distinct());
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
                tasks.Add(Task.Run(() => File.Copy($"{paths.modules_backup}{name.Key}.rim", $"{paths.modules}{name.Value}.rim", true)));
                tasks.Add(Task.Run(() => File.Copy($"{paths.modules_backup}{name.Key}_s.rim", $"{paths.modules}{name.Value}_s.rim", true)));
                if (File.Exists($"{paths.lips_backup}{name.Key}_loc.mod")) 
                    tasks.Add(Task.Run(() => File.Copy($"{paths.lips_backup}{name.Key}_loc.mod", $"{paths.lips}{name.Value}_loc.mod", true)));
                tasks.Add(Task.Run(() => File.Copy($"{paths.modules_backup}{name.Key}_dlg.erf", $"{paths.modules}{name.Value}_dlg.erf", true)));

                //TEMPORARY SPOLIERS
                File.AppendAllText(paths.RANDOMIZED_LOG, $"{name.Key} -> {name.Value}\n");

            }
            tasks.Add(Task.Run(() => File.Copy($"{paths.lips_backup}localization.mod", $"{paths.lips}{"localization.mod"}", true)));
            File.AppendAllText(paths.RANDOMIZED_LOG, "----------------\n");

            //Misc Patches
            tasks.Add(Task.Run(() => File.WriteAllBytes(paths.Override + "a_disc_join.ncs", Properties.Resources.a_disc_join))); //Disciple Crash Patch

            //Unlock Galxy Map
            if (Properties.UserSettings.Default.GalaxyMapUnlocked)
            {
                tasks.Add(Task.Run(() => File.WriteAllBytes(paths.Override + "a_galaxymap.ncs", Properties.Resources.a_galaxymap)));
            }

            //Module Save Patch
            if (Properties.UserSettings.Default.ModuleSavePatch)
            {
                tasks.Add(Task.Run(() => File.WriteAllBytes(paths.Override + "modulesave.2da", Properties.Resources.modulesave)));
            }

            Task.WhenAll(tasks).Wait();

            //Unlock Doors
            if (Properties.UserSettings.Default.DoorUnlocks)
            {
                UnlockDoors(paths);
            }
        }

        /// <summary>
        /// Allows a door to transition to it's listed module
        /// </summary>
        /// <param name="paths">KPaths object for this game.</param>
        /// <param name="area">Name of the SRim file to modify.</param>
        /// <param name="label">Label of the door to unlock.</param>
        /// <param name="destination">The module this will transition to, if null then leave field unchanged.</param>
        private static void EnableDoorTransition(K2Paths paths, string area, string label, string? destination = null)
        {
            var areaFiles = paths.FilesInModules.Where(fi => fi.Name.Contains(LookupTable[area]));
            foreach (FileInfo fi in areaFiles)
            {
                // Skip any files that aren't the default format.
                if (fi.Name.Length > 10) { continue; }

                RIM r = new RIM(fi.FullName);   // Open what replaced this area.
                RIM.rFile rf = r.File_Table.FirstOrDefault(x => x.TypeID == (int)ResourceType.GIT);
                GFF g = new GFF(rf.File_Data);  // Grab the git out of the file.

                //Get ready for the nastiest Linq query you've ever seen, we may want to clean this up some
                ((g.Top_Level.Fields.FirstOrDefault(x => x.Label == "Door List") as GFF.LIST).Structs.FirstOrDefault(y => (y.Fields.FirstOrDefault(z => z.Label == "TemplateResRef") as GFF.ResRef).Reference == label).Fields.FirstOrDefault(a => a.Label == "LinkedToFlags") as GFF.BYTE).Value = 2;

                if (destination is not null)
                {
                    ((g.Top_Level.Fields.FirstOrDefault(x => x.Label == "Door List") as GFF.LIST).Structs.FirstOrDefault(y => (y.Fields.FirstOrDefault(z => z.Label == "TemplateResRef") as GFF.ResRef).Reference == label).Fields.FirstOrDefault(a => a.Label == "LinkedToModule") as GFF.ResRef).Reference = destination;
                }

                // Write change(s) to file.
                rf.File_Data = g.ToRawData();
                r.WriteToFile(fi.FullName);
            }
        }

        /// <summary>
        /// Unlock a specific door within an SRim file.
        /// </summary>
        /// <param name="paths">KPaths object for this game.</param>
        /// <param name="area">Name of the SRim file to modify.</param>
        /// <param name="label">Label of the door to unlock.</param>
        private static void UnlockDoorInFile(K2Paths paths, string area, string label)
        {
            var areaFiles = paths.FilesInModules.Where(fi => fi.Name.Contains(LookupTable[area]));
            foreach (FileInfo fi in areaFiles)
            {
                // Skip any files that don't end in "s.rim".
                if (fi.Name[fi.Name.Length - 5] != 's') { continue; }

                RIM r = new RIM(fi.FullName);   // Open what replaced this area.
                RIM.rFile rf = r.File_Table.FirstOrDefault(x => x.TypeID == (int)ResourceType.UTD && x.Label == label);
                GFF g = new GFF(rf.File_Data);  // Grab the door out of the file.

                // Set fields related to opening and unlocking.
                (g.Top_Level.Fields.FirstOrDefault(x => x.Label == "KeyRequired") as GFF.BYTE).Value = 0;
                (g.Top_Level.Fields.FirstOrDefault(x => x.Label == "Locked") as GFF.BYTE).Value = 0;
                (g.Top_Level.Fields.FirstOrDefault(x => x.Label == "OpenLockDC") as GFF.BYTE).Value = 0;
                (g.Top_Level.Fields.FirstOrDefault(x => x.Label == "Plot") as GFF.BYTE).Value = 0;

                // Set fields related to bashing open.
                (g.Top_Level.Fields.FirstOrDefault(x => x.Label == "Hardness") as GFF.BYTE).Value = 0;
                (g.Top_Level.Fields.FirstOrDefault(x => x.Label == "HP") as GFF.SHORT).Value = 1;
                (g.Top_Level.Fields.FirstOrDefault(x => x.Label == "CurrentHP") as GFF.SHORT).Value = 1;
                (g.Top_Level.Fields.FirstOrDefault(x => x.Label == "Min1HP") as GFF.BYTE).Value = 0;

                //Set Fields related to interacting
                (g.Top_Level.Fields.FirstOrDefault(x => x.Label == "Static") as GFF.BYTE).Value = 0;

                // Write change(s) to file.
                rf.File_Data = g.ToRawData();
                r.WriteToFile(fi.FullName);
            }
        }

        /// <summary>
        /// Unlock the doors requested by the user.
        /// </summary>
        /// <param name="paths">KPaths object for this game.</param>
        private static void UnlockDoors(K2Paths paths)
        {
            //In the future these'll be split into options, but for now here's all of them
            UnlockDoorInFile(paths, AREA_PER_ADMIN    , LABEL_101PERTODORMS);
            UnlockDoorInFile(paths, AREA_PER_ADMIN    , LABEL_101PERTOMININGTUNNELS);
            UnlockDoorInFile(paths, AREA_PER_ADMIN    , LABEL_101PERTOFUELDEPOT);
            UnlockDoorInFile(paths, AREA_PER_ADMIN    , LABEL_101PERTOHARBINGER);
            UnlockDoorInFile(paths, AREA_PER_FUEL     , LABEL_103PERTOMININGTUNNELS);
            UnlockDoorInFile(paths, AREA_PER_FUEL     , LABEL_103PERFORCESHIELDS);
            UnlockDoorInFile(paths, AREA_PER_FUEL     , LABEL_103PERSHIELD2);
            UnlockDoorInFile(paths, AREA_PER_DORMS    , LABEL_105PERTOASTROID);
            UnlockDoorInFile(paths, AREA_PER_HANGER   , LABEL_106PEREASTDOOR);
            UnlockDoorInFile(paths, AREA_TEL_RES      , LABEL_203TELAPPTDOOR); 
            UnlockDoorInFile(paths, AREA_TEL_RES      , LABEL_203TELEXCHANGE);
            UnlockDoorInFile(paths, AREA_TEL_ENTER_WAR, LABEL_222TELRAVAGER);
            UnlockDoorInFile(paths, AREA_TEL_ACAD     , LABEL_262TELPLATEAU); 
            UnlockDoorInFile(paths, AREA_NAR_DOCKS    , LABEL_303NARZEZDOOR);
            UnlockDoorInFile(paths, AREA_NAR_JEKK     , LABEL_304NARBACKROOM);
            UnlockDoorInFile(paths, AREA_NAR_J_TUNNELS, LABEL_305NARTOJEKKJEKK);
            UnlockDoorInFile(paths, AREA_NAR_G0T0     , LABEL_351NARG0T0EBONHAWK);
            UnlockDoorInFile(paths, AREA_DAN_COURTYARD, LABEL_605DANREBUILTENCLAVE);
            UnlockDoorInFile(paths, AREA_KOR_ACAD     , LABEL_702KORVALLEY);
            UnlockDoorInFile(paths, AREA_KOR_SHY      , LABEL_710KORLUDOKRESSH);

            //Enable tranistions for these doors with linking modules but no flags
            EnableDoorTransition(paths, AREA_PER_FUEL, LABEL_103PERTOMININGTUNNELS);
            EnableDoorTransition(paths, AREA_PER_DORMS, LABEL_105PERTOASTROID, AREA_PER_ASTROID);
            EnableDoorTransition(paths, AREA_TEL_ACAD, LABEL_262TELPLATEAU);

            //Add a transition to the Astroid Exterior
            Add104PERTransition(paths);

            //Add elevator to 901MAL
            Add901MALEbonElevator(paths);
        }

        private static void Add104PERTransition(K2Paths paths)
        {
            string filename = LookupTable[AREA_PER_ASTROID] + ".rim";
            var fi = paths.FilesInModules.FirstOrDefault(f => f.Name == filename);
            if (fi.Exists)
            {
                RIM r = new RIM(fi.FullName);   // Open what replaced this the astroid exterior.
                RIM.rFile rf = r.File_Table.FirstOrDefault(x => x.TypeID == (int)ResourceType.GIT);
                GFF g = new GFF(rf.File_Data);  // Grab the git out of the file.

                //Create Tranistion Struct
                GFF.STRUCT TransitionStruct = new GFF.STRUCT("", 1, new List<GFF.FIELD>()
                {
                    new GFF.LIST("Geometry", new List<GFF.STRUCT>()
                    {
                        new GFF.STRUCT("", 3, new List<GFF.FIELD>()
                        {
                            new GFF.FLOAT("PointX", 0.0f),
                            new GFF.FLOAT("PointY", 0.0f),
                            new GFF.FLOAT("PointZ", 0.0f)
                        }),
                        new GFF.STRUCT("", 3, new List<GFF.FIELD>()
                        {
                            new GFF.FLOAT("PointX", 0.0f),
                            new GFF.FLOAT("PointY", -1.4f),
                            new GFF.FLOAT("PointZ", 2.0f)
                        }),
                        new GFF.STRUCT("", 3, new List<GFF.FIELD>()
                        {
                            new GFF.FLOAT("PointX", 6.0f),
                            new GFF.FLOAT("PointY", -1.4f),
                            new GFF.FLOAT("PointZ", 2.0f)
                        }),
                        new GFF.STRUCT("", 3, new List<GFF.FIELD>()
                        {
                            new GFF.FLOAT("PointX", 6.0f),
                            new GFF.FLOAT("PointY", 0.0f),
                            new GFF.FLOAT("PointZ", 0.0f)
                        })
                    }),
                    new GFF.CExoString("LinkedTo", "From_104PER"),
                    new GFF.BYTE("LinkedToFlags", 2),
                    new GFF.ResRef("LinkedToModule", AREA_PER_FUEL),
                    new GFF.CExoString("Tag", "To_103PER"),
                    new GFF.ResRef("TemplateResRef", "newtransition"),
                    new GFF.CExoLocString("TransitionDestin", 75950, new()),
                    new GFF.FLOAT("XOrientation", 0.0f),
                    new GFF.FLOAT("YOrientation", 0.0f),
                    new GFF.FLOAT("ZOrientation", 0.0f),
                    new GFF.FLOAT("XPosition", 70.6f),
                    new GFF.FLOAT("YPosition", -115.1f),
                    new GFF.FLOAT("ZPosition", 255.0f)
                });

                (g.Top_Level.Fields.FirstOrDefault(f => f.Label == "TriggerList") as GFF.LIST).Structs.Add(TransitionStruct);

                // Write change(s) to file.
                rf.File_Data = g.ToRawData();
                r.WriteToFile(fi.FullName);
            }
        }
        
        private static void Add901MALEbonElevator(K2Paths paths)
        {
            string filename = LookupTable[AREA_MAL_SURFACE] + ".rim";
            var fi = paths.FilesInModules.FirstOrDefault(f => f.Name == filename);
            if (fi.Exists)
            {
                RIM r = new RIM(fi.FullName);   // Open what replaced this the astroid exterior.
                RIM.rFile rf = r.File_Table.FirstOrDefault(x => x.TypeID == (int)ResourceType.GIT);
                GFF g = new GFF(rf.File_Data);  // Grab the git out of the file.

                //Create Tranistion Struct
                GFF.STRUCT PlaceStruct = new GFF.STRUCT("", 9, new List<GFF.FIELD>()
                {
                    new GFF.FLOAT("Bearing", 0.0f),
                    new GFF.ResRef("TemplateResRef", "ebo_elev"),
                    new GFF.DWORD("TweakColor",0),
                    new GFF.BYTE("UseTweakColor",0),
                    new GFF.FLOAT("X", 6.23f),
                    new GFF.FLOAT("Y", -24.63f),
                    new GFF.FLOAT("Z", 84.43f)
                });
                (g.Top_Level.Fields.FirstOrDefault(f => f.Label == "Placeable List") as GFF.LIST).Structs.Add(PlaceStruct);

                //Add Placeable and script to overide
                File.WriteAllBytes(paths.Override + "ebo_elev.utp", Properties.Resources.ebo_elev);
                File.WriteAllBytes(paths.Override + "r_to003EBO.ncs", Properties.Resources.r_to003EBO);

                // Write change(s) to file.
                rf.File_Data = g.ToRawData();
                r.WriteToFile(fi.FullName);
            }
        }
    }
}
