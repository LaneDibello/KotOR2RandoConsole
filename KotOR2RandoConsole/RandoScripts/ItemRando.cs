using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KotOR_IO;
using System.Text.RegularExpressions;

namespace KotOR2RandoConsole
{
    public static class ItemRando
    {
        #region Private Properties
        //private static KPaths             Paths                { get; set; }
        private static RandomizationLevel RandomizeArmbands { get; set; }
        private static RandomizationLevel RandomizeArmor { get; set; }
        private static RandomizationLevel RandomizeBelts { get; set; }
        private static RandomizationLevel RandomizeBlasters { get; set; }
        private static RandomizationLevel RandomizeHides { get; set; }
        private static RandomizationLevel RandomizeCreature { get; set; }
        private static RandomizationLevel RandomizeDroid { get; set; }
        private static RandomizationLevel RandomizeGloves { get; set; }
        private static RandomizationLevel RandomizeGrenades { get; set; }
        private static RandomizationLevel RandomizeImplants { get; set; }
        private static RandomizationLevel RandomizeLightsabers { get; set; }
        private static RandomizationLevel RandomizeMask { get; set; }
        private static RandomizationLevel RandomizeMelee { get; set; }
        private static RandomizationLevel RandomizeMines { get; set; }
        private static RandomizationLevel RandomizePaz { get; set; }
        private static RandomizationLevel RandomizeStims { get; set; }
        private static RandomizationLevel RandomizeUpgrade { get; set; }
        private static RandomizationLevel RandomizePCrystal { get; set; }
        private static RandomizationLevel RandomizeProps { get; set; }
        private static RandomizationLevel RandomizeVarious { get; set; }
        private static List<string> OmittedItems { get; set; }
        private static string OmitPreset { get; set; }
        #endregion Private Properties

        /// <summary>
        /// A lookup table used to know how the items are randomized.
        /// Usage: List(Old ID, New ID)
        /// </summary>
        internal static List<Tuple<string, string>> LookupTable { get; set; } = new List<Tuple<string, string>>();

        /// <summary>
        /// Randomizes the types of items requested.
        /// </summary>
        /// <param name="paths">KPaths object for this game.</param>
        /// <param name="k1rando">Kotor1Randomizer object that contains settings to use.</param>
        public static void item_rando(K2Paths paths)
        {
            // Prepare for new randomization.
            paths.BackUpChitinFile();
            Reset();
            AssignSettings();

            // Load KEY file.
            KEY k = new KEY(paths.chitin);

            // Handle categories
            HandleCategory(k, ArmbandsRegs, RandomizeArmbands);
            HandleCategory(k, ArmorRegs, RandomizeArmor);
            HandleCategory(k, BeltsRegs, RandomizeBelts);
            HandleCategory(k, BlastersRegs, RandomizeBlasters);
            HandleCategory(k, HidesRegs, RandomizeHides);
            HandleCategory(k, CreatureRegs, RandomizeCreature);
            HandleCategory(k, DroidRegs, RandomizeDroid);
            HandleCategory(k, GlovesRegs, RandomizeGloves);
            HandleCategory(k, GrenadesRegs, RandomizeGrenades);
            HandleCategory(k, ImplantsRegs, RandomizeImplants);
            HandleCategory(k, LightsabersRegs, RandomizeLightsabers);
            HandleCategory(k, MaskRegs, RandomizeMask);
            HandleCategory(k, MeleeRegs, RandomizeMelee);
            HandleCategory(k, MinesRegs, RandomizeMines);
            HandleCategory(k, PazRegs, RandomizePaz);
            HandleCategory(k, StimsRegs, RandomizeStims);
            HandleCategory(k, UpgradeRegs, RandomizeUpgrade);
            HandleCategory(k, PropsRegs, RandomizeProps);
            HandleCategory(k, PCrystalRegs, RandomizePCrystal);

            // Handle Various
            switch (RandomizeVarious)
            {
                default:
                case RandomizationLevel.None:
                    break;
                case RandomizationLevel.Type:
                    List<string> type = new List<string>(k.KeyTable.Where(x => Matches_None(x.ResRef) && !Is_Forbidden(x.ResRef) && x.ResourceType == (short)ResourceType.UTI).Select(x => x.ResRef));
                    Type_Lists.Add(type);
                    break;
                case RandomizationLevel.Max:
                    Max_Rando.AddRange(k.KeyTable.Where(x => Matches_None(x.ResRef) && !Is_Forbidden(x.ResRef) && x.ResourceType == (short)ResourceType.UTI).Select(x => x.ResRef));
                    break;
            }

            // Omitted Items
            foreach (var item in OmittedItems)
            {
                LookupTable.Add(new Tuple<string, string>(item, item));
            }

            // Max Rando
            List<string> Max_Rando_Iterator = new List<string>(Max_Rando);
            Randomize.FisherYatesShuffle(Max_Rando);
            int j = 0;
            foreach (KEY.KeyEntry ke in k.KeyTable.Where(x => Max_Rando_Iterator.Contains(x.ResRef)))
            {
                LookupTable.Add(new Tuple<string, string>(ke.ResRef, Max_Rando[j]));
                ke.ResRef = Max_Rando[j];
                j++;
            }

            // Type Rando
            foreach (List<string> li in Type_Lists)
            {
                List<string> type_copy = new List<string>(li);
                Randomize.FisherYatesShuffle(type_copy);
                j = 0;
                foreach (KEY.KeyEntry ke in k.KeyTable.Where(x => li.Contains(x.ResRef)))
                {
                    LookupTable.Add(new Tuple<string, string>(ke.ResRef, type_copy[j]));
                    ke.ResRef = type_copy[j];
                    j++;
                }
            }

            k.WriteToFile(paths.chitin);

            //Temp Log Keeping
            File.AppendAllText(paths.RANDOMIZED_LOG, "----------------\nITEM MAPPINGS\n----------------\n");
            foreach (var name in LookupTable) File.AppendAllText(paths.RANDOMIZED_LOG, $"{name.Item1} -> {name.Item2}\n");
            File.AppendAllText(paths.RANDOMIZED_LOG, "----------------\n");
        }

        private static void AssignSettings()
        {
            //Paths = new KPaths(Properties.Settings.Default.Kotor1Path);
            RandomizeArmbands = (RandomizationLevel)Properties.Items.Default.RandomizeArmbands;
            RandomizeArmor = (RandomizationLevel)Properties.Items.Default.RandomizeArmor;
            RandomizeBelts = (RandomizationLevel)Properties.Items.Default.RandomizeBelts;
            RandomizeBlasters = (RandomizationLevel)Properties.Items.Default.RandomizeBlasters;
            RandomizeHides = (RandomizationLevel)Properties.Items.Default.RandomizeHides;
            RandomizeCreature = (RandomizationLevel)Properties.Items.Default.RandomizeCreature;
            RandomizeDroid = (RandomizationLevel)Properties.Items.Default.RandomizeDroid;
            RandomizeGloves = (RandomizationLevel)Properties.Items.Default.RandomizeGloves;
            RandomizeGrenades = (RandomizationLevel)Properties.Items.Default.RandomizeGrenades;
            RandomizeImplants = (RandomizationLevel)Properties.Items.Default.RandomizeImplants;
            RandomizeLightsabers = (RandomizationLevel)Properties.Items.Default.RandomizeLightsabers;
            RandomizeMask = (RandomizationLevel)Properties.Items.Default.RandomizeMask;
            RandomizeMelee = (RandomizationLevel)Properties.Items.Default.RandomizeMelee;
            RandomizeMines = (RandomizationLevel)Properties.Items.Default.RandomizeMines;
            RandomizePaz = (RandomizationLevel)Properties.Items.Default.RandomizePaz;
            RandomizeStims = (RandomizationLevel)Properties.Items.Default.RandomizeStims;
            RandomizeUpgrade = (RandomizationLevel)Properties.Items.Default.RandomizeUpgrade;
            RandomizeProps = (RandomizationLevel)Properties.Items.Default.RandomizeProps;
            RandomizePCrystal = (RandomizationLevel)Properties.Items.Default.RandomizePCrystal;
            RandomizeVarious = (RandomizationLevel)Properties.Items.Default.RandomizeVarious;
            OmittedItems = new List<string>();
            if (File.Exists("OmitItems.txt")) OmittedItems.AddRange(File.ReadAllLines("OmitItems.txt"));
        }

        private static void HandleCategory(KEY k, List<Regex> r, RandomizationLevel randomizationLevel)
        {
            switch (randomizationLevel)
            {
                case RandomizationLevel.None:
                default:
                    break;
                case RandomizationLevel.Subtype:
                    for (int i = 1; i < r.Count; i++)
                    {
                        List<string> temp = new List<string>(k.KeyTable.Where(x => r[i].IsMatch(x.ResRef) && !Is_Forbidden(x.ResRef) && x.ResourceType == (short)ResourceType.UTI).Select(x => x.ResRef));
                        Type_Lists.Add(temp);
                    }
                    break;
                case RandomizationLevel.Type:
                    List<string> type = new List<string>(k.KeyTable.Where(x => r[0].IsMatch(x.ResRef) && !Is_Forbidden(x.ResRef) && x.ResourceType == (short)ResourceType.UTI).Select(x => x.ResRef));
                    Type_Lists.Add(type);
                    break;
                case RandomizationLevel.Max:
                    Max_Rando.AddRange(k.KeyTable.Where(x => r[0].IsMatch(x.ResRef) && !Is_Forbidden(x.ResRef) && x.ResourceType == (short)ResourceType.UTI).Select(x => x.ResRef));
                    break;
            }
        }

        private static bool Is_Forbidden(string s)
        {
            return OmittedItems.Contains(s);
        }

        private static bool Matches_None(string s)
        {
            return
                (
                    !ArmorRegs[0].IsMatch(s) &&
                    !StimsRegs[0].IsMatch(s) &&
                    !BeltsRegs[0].IsMatch(s) &&
                    !HidesRegs[0].IsMatch(s) &&
                    !DroidRegs[0].IsMatch(s) &&
                    !ArmbandsRegs[0].IsMatch(s) &&
                    !GlovesRegs[0].IsMatch(s) &&
                    !ImplantsRegs[0].IsMatch(s) &&
                    !MaskRegs[0].IsMatch(s) &&
                    !PazRegs[0].IsMatch(s) &&
                    !MinesRegs[0].IsMatch(s) &&
                    !UpgradeRegs[0].IsMatch(s) &&
                    !BlastersRegs[0].IsMatch(s) &&
                    !CreatureRegs[0].IsMatch(s) &&
                    !LightsabersRegs[0].IsMatch(s) &&
                    !GrenadesRegs[0].IsMatch(s) &&
                    !MeleeRegs[0].IsMatch(s) &&
                    !PCrystalRegs[0].IsMatch(s) &&
                    !PropsRegs[0].IsMatch(s)
                );
        }

        private static List<string> Max_Rando = new List<string>();

        private static List<List<string>> Type_Lists = new List<List<string>>();

        internal static void Reset()
        {
            // Prepare lists for new randomization.
            Max_Rando.Clear();
            Type_Lists.Clear();
            LookupTable.Clear();
        }

        #region Regexes
        //Armor Regexes
        public static List<Regex> ArmorRegs = new List<Regex>()
        {
            new Regex("^a_(lig|med|hea|rob|kho)|^g_a_|^g_danceroutfit|^mineruniform", RegexOptions.Compiled | RegexOptions.IgnoreCase), //All Armor

            new Regex("^a_light_", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Light Armor
            new Regex("^a_medium_", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Medium Armor
            new Regex("^a_heavy_", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Heavy Armor
            new Regex("^a_robe_", RegexOptions.Compiled | RegexOptions.IgnoreCase),  //Robes
            new Regex("^a_khoonda|^g_a_|^g_danceroutfit|^mineruniform", RegexOptions.Compiled | RegexOptions.IgnoreCase) //Clothes and generic
        };

        //Stims Regexes
        public static List<Regex> StimsRegs = new List<Regex>()
        {
            new Regex("^g_i_(adrn|cmbt|medeq)", RegexOptions.Compiled | RegexOptions.IgnoreCase),//All Stims/Medpacs

            new Regex("^g_i_adrn", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Adrenals
            new Regex("^g_i_cmbt", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Battle Stims
            new Regex("^g_i_medeq", RegexOptions.Compiled | RegexOptions.IgnoreCase)//Medpacs
        };

        //Belt Regexs
        public static List<Regex> BeltsRegs = new List<Regex>()
        {
            new Regex("^a_belt_|^100_belt", RegexOptions.Compiled | RegexOptions.IgnoreCase)//All Belts
        };

        //Creature Hides
        public static List<Regex> HidesRegs = new List<Regex>()
        {
            new Regex("^g_i_crhide", RegexOptions.Compiled | RegexOptions.IgnoreCase)//Creature Hides
        };

        //Droid equipment 
        public static List<Regex> DroidRegs = new List<Regex>()
        {
            new Regex("^d_|^g_i_drd", RegexOptions.Compiled | RegexOptions.IgnoreCase),//All Droid Equipment

            new Regex("^d_armor_", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Droid Plating
            new Regex("^d_device", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Droid Devices
            new Regex("^d_g0t0", RegexOptions.Compiled | RegexOptions.IgnoreCase),//G0T0 Gear
            new Regex("^d_hk47", RegexOptions.Compiled | RegexOptions.IgnoreCase),//HK Gear
            new Regex("^d_interface", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Droid Interfaces
            new Regex("^d_shield", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Droid Shields
            new Regex("^d_t3m4", RegexOptions.Compiled | RegexOptions.IgnoreCase),//T3M4 Gear
            new Regex("^d_tool", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Droid Tools
            new Regex("^g_i_drd", RegexOptions.Compiled | RegexOptions.IgnoreCase)//Droid Repair Kits
        };

        //Armbands
        public static List<Regex> ArmbandsRegs = new List<Regex>()
        {
            new Regex("^100_fore|^a_band|^a_sheild", RegexOptions.Compiled | RegexOptions.IgnoreCase),//All Armbands

            new Regex("^a_sheild|^100_fore", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Shields
            new Regex("^a_band", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Stats
        };

        //Gauntlets
        public static List<Regex> GlovesRegs = new List<Regex>()
        {
            new Regex("^a_gloves", RegexOptions.Compiled | RegexOptions.IgnoreCase)//Gloves
        };

        //Implants
        public static List<Regex> ImplantsRegs = new List<Regex>()
        {
            new Regex("^e_imp", RegexOptions.Compiled | RegexOptions.IgnoreCase),//All Implants

            new Regex("^e_imp1", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Implant level 1
            new Regex("^e_imp2", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Implant level 2
            new Regex("^e_imp3", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Implant level 3
            new Regex("^e_imp4", RegexOptions.Compiled | RegexOptions.IgnoreCase) //Implant level "4" (just high level 3s)
        };

        //Mask
        public static List<Regex> MaskRegs = new List<Regex>()
        {
            new Regex("^a_helmet|^100_mask", RegexOptions.Compiled | RegexOptions.IgnoreCase),//All Masks

            new Regex("^a_helmet_(01|02|03|07|08|10|11|12|15|16|19|20|23|24|25|26|28|30)|^100_mask", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Mask No Armor Prof
            new Regex("^a_helmet_(04|05|06|13|29)", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Mask Light
            new Regex("^a_helmet_(09|14|17|18|22|27)", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Mask Medium
            new Regex("^a_helmet_21", RegexOptions.Compiled | RegexOptions.IgnoreCase)//Mask Heavy
        };

        //Paz
        public static List<Regex> PazRegs = new List<Regex>()
        {
            new Regex("^g_i_pazcard", RegexOptions.Compiled | RegexOptions.IgnoreCase)//Pazaak Cards
        };

        //Mines
        public static List<Regex> MinesRegs = new List<Regex>()
        {
            new Regex("^g_i_trapkit", RegexOptions.Compiled | RegexOptions.IgnoreCase)//Mines
        };

        //Upgrades
        public static List<Regex> UpgradeRegs = new List<Regex>()
        {
            new Regex("^u_", RegexOptions.Compiled | RegexOptions.IgnoreCase),//All Upgrades

            new Regex("^u_a_over", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Armor Over
            new Regex("^u_a_unde", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Armor Under
            new Regex("^u_l_cell", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Lightsaber Cell
            new Regex("^u_l_colo", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Lightsaber Color
            new Regex("^u_l_crys", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Lightsaber Crystal
            new Regex("^u_l_emit", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Lightsaber Emiter
            new Regex("^u_l_lens", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Lightsaber Lens
            new Regex("^u_m_cell", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Melee Cell
            new Regex("^u_m_edge", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Melee Edge
            new Regex("^u_m_grip", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Melee Grip
            new Regex("^u_r_firi", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Ranged Firing
            new Regex("^u_r_powe", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Ranged Power
            new Regex("^u_r_targ", RegexOptions.Compiled | RegexOptions.IgnoreCase)  //Ranged Targeting
        };

        //Blaster
        public static List<Regex> BlastersRegs = new List<Regex>()
        {
            new Regex("^g_i_bithitem|^killb|^mininglaser|^w_b|^w_drink|^w_empty|^w_pazaak", RegexOptions.Compiled | RegexOptions.IgnoreCase),//All Blasters

            new Regex("w_blaste_|^w_drink|^w_empty|^g_i_bithitem|^killb|^mininglaser|^w_pazaak", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Blaster Pistols
            new Regex("w_brifle_", RegexOptions.Compiled | RegexOptions.IgnoreCase), //Blaster rifles

        };

        //Creature Weapons
        public static List<Regex> CreatureRegs = new List<Regex>()
        {
            new Regex("^g_w_cr(go|sl)", RegexOptions.Compiled | RegexOptions.IgnoreCase),//All Creature weapons

            new Regex("^g_w_crgore", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Piercing Creature Weapons
            new Regex("^g_w_crslash", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Slashing Creature Weapons
            new Regex("^g_w_crslprc", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Piercing/slashing Creature weapons
        };

        //Lightsabers
        public static List<Regex> LightsabersRegs = new List<Regex>()
        {
            new Regex("^g1*_w_.{1,}sbr|^w_s?ls_", RegexOptions.Compiled | RegexOptions.IgnoreCase),//All Lightsabers

            new Regex("^g1*_w_(dblsbr|drkjdisbr002)", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Double Lightsabers
            new Regex("^g1*_w_(lghtsbr|drkjdisbr001)|^w_ls", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Regular Lightsabers
            new Regex("^g1*_w_shortsbr|^w_sls", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Short Lightsabers
        };

        //Grenades and Rockets
        public static List<Regex> GrenadesRegs = new List<Regex>()
        {
            new Regex("^g_w_(.*gren|thermldet|sonicdet)|^w_rocket", RegexOptions.Compiled | RegexOptions.IgnoreCase),//All

            new Regex("^g_w_(.*gren|thermldet|sonicdet)", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Grenades
            new Regex("^w_rocket", RegexOptions.Compiled | RegexOptions.IgnoreCase)//Rockets
        };

        //Melee
        public static List<Regex> MeleeRegs = new List<Regex>()
        {
            new Regex("^w_melee|^killstick|^vibrocutter", RegexOptions.Compiled | RegexOptions.IgnoreCase),//All Melee Weapons

            new Regex("^w_melee_(03|08|18|29)", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Stun Batons
            new Regex("^w_melee_(02|13|24)", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Long Swords
            new Regex("^w_melee_(01|11|17|19|x02|x03)", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Short Swords
            new Regex("^w_melee_(06|21|22)", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Vibro Swords
            new Regex("^w_melee_(05|10|27)|^vibrocutter", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Vibroblades
            new Regex("^w_melee_(04|x01|14|23|26)|^killstick", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Quarter Staves
            new Regex("^w_melee_(07|x12|12|15|20|28)", RegexOptions.Compiled | RegexOptions.IgnoreCase),//Doubleblades
            new Regex("^w_melee_(09|16|25|30)", RegexOptions.Compiled | RegexOptions.IgnoreCase),//War blade/axes
        };

        //Named Player Crystal versions
        public static List<Regex> PCrystalRegs = new List<Regex>()
        {
            new Regex("^qcrystal", RegexOptions.Compiled | RegexOptions.IgnoreCase)//player crystal versions
        };

        //Cutscene Props
        public static List<Regex> PropsRegs = new List<Regex>()
        {
            new Regex("^prop", RegexOptions.Compiled | RegexOptions.IgnoreCase) //Cutscene prop weapons
        };
        #endregion
    }
}
