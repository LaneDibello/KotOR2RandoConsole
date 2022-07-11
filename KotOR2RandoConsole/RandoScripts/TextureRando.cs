using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using KotOR_IO;
using System.IO;

namespace KotOR2RandoConsole
{
    public static class TextureRando
    {
        private static List<int> MaxRando { get; } = new List<int>();

        private static List<List<int>> TypeLists { get; } = new List<List<int>>();

        /// <summary>
        /// Lookup table for how models are randomized.
        /// Usage: LookupTable[OriginalID] = RandomizedID;
        /// </summary>
        private static Dictionary<int, int> LookupTable { get; set; } = new Dictionary<int, int>();

        /// <summary>
        /// Lookup table for the name of each Texture ID.
        /// </summary>
        private static Dictionary<int, string> NameLookup { get; set; } = new Dictionary<int, string>();

        private static RandomizationLevel RandomizeCreatures { get; set; }
        private static RandomizationLevel RandomizeCubeMaps { get; set; }
        private static RandomizationLevel RandomizeEffects { get; set; }
        private static RandomizationLevel RandomizeItems { get; set; }
        private static RandomizationLevel RandomizeNPC { get; set; }
        private static RandomizationLevel RandomizeOther { get; set; }
        private static RandomizationLevel RandomizeParty { get; set; }
        private static RandomizationLevel RandomizePlaceables { get; set; }
        private static RandomizationLevel RandomizeEbonHawk { get; set; }
        private static RandomizationLevel RandomizeDantooine { get; set; }
        private static RandomizationLevel RandomizeM4_78 { get; set; }
        private static RandomizationLevel RandomizeDxun { get; set; }
        private static RandomizationLevel RandomizeHarbinger { get; set; }
        private static RandomizationLevel RandomizeKorriban { get; set; }
        private static RandomizationLevel RandomizeMalachor { get; set; }
        private static RandomizationLevel RandomizeMiniGame { get; set; }
        private static RandomizationLevel RandomizeNarShadaa { get; set; }
        private static RandomizationLevel RandomizeRavager { get; set; }
        private static RandomizationLevel RandomizeOnderon { get; set; }
        private static RandomizationLevel RandomizePeragus { get; set; }
        private static RandomizationLevel RandomizeTelos { get; set; }
        private static RandomizationLevel RandomizePlayerBodies { get; set; }
        private static RandomizationLevel RandomizePlayerHeads { get; set; }
        private static RandomizationLevel RandomizeStunt { get; set; }
        private static RandomizationLevel RandomizeVehicles { get; set; }
        private static RandomizationLevel RandomizeWeapons { get; set; }
        private static TexturePack SelectedPack { get; set; }

        #region Regexes
        private static readonly Regex RegexCubeMaps = new Regex("^CM_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexCreatures = new Regex("^C_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexEffects = new Regex("^FX_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexItems = new Regex("^I_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexK1Levels = new Regex("^L.{2}_", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex RegexEbonHawk = new Regex("^EBO_|^002_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexDantooine = new Regex("^DAN_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexM4_78 = new Regex("^DRO_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexDxun = new Regex("^DXN_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexHarbinger = new Regex("^HAR_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexKorriban = new Regex("^KOR_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexMalachor = new Regex("^MAL_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexMiniGame = new Regex("^MGF?_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexNarShadaa = new Regex("^NAR_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexRavager = new Regex("^NIH_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexOnderon = new Regex("^OND_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexPeragus = new Regex("^PER_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexTelos = new Regex("^TEL_", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex RegexNPC = new Regex("^N_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexPlayHeads = new Regex("^P(F|M)H", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexPlayBodies = new Regex("^P(F|M)B", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexPlaceables = new Regex("^PLC_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexParty = new Regex("^P_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexStunt = new Regex("^Stunt", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexVehicles = new Regex("^V_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexWeapons = new Regex("^W_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        #endregion

        public static void texture_rando(K2Paths paths)
        {
            // Prepare for new randomization.
            Reset();
            AssignSettings();

            // Load in texture pack.
            string pack_name;
            switch (SelectedPack)
            {
                default:
                case TexturePack.HighQuality:
                    pack_name = "\\swpc_tex_tpa.erf";
                    break;
                case TexturePack.MedQuality:
                    pack_name = "\\swpc_tex_tpb.erf";
                    break;
                case TexturePack.LowQuality:
                    pack_name = "\\swpc_tex_tpc.erf";
                    break;
            }

            ERF e = new ERF(paths.TexturePacks + pack_name);

            foreach (var key in e.Key_List)
            {
                if (!NameLookup.ContainsKey(key.ResID))
                    NameLookup.Add(key.ResID, key.ResRef);
            }

            // Handle categories.
            HandleCategory(e, RegexCubeMaps, RandomizeCubeMaps);
            HandleCategory(e, RegexCreatures, RandomizeCreatures);
            HandleCategory(e, RegexEffects, RandomizeEffects);
            HandleCategory(e, RegexItems, RandomizeItems);
            HandleCategory(e, RegexEbonHawk, RandomizeEbonHawk);
            HandleCategory(e, RegexDantooine, RandomizeDantooine);
            HandleCategory(e, RegexM4_78, RandomizeM4_78);
            HandleCategory(e, RegexDxun, RandomizeDxun);
            HandleCategory(e, RegexHarbinger, RandomizeHarbinger);
            HandleCategory(e, RegexKorriban, RandomizeKorriban);
            HandleCategory(e, RegexMalachor, RandomizeMalachor);
            HandleCategory(e, RegexMiniGame, RandomizeMiniGame);
            HandleCategory(e, RegexNarShadaa, RandomizeNarShadaa);
            HandleCategory(e, RegexRavager, RandomizeRavager);
            HandleCategory(e, RegexOnderon, RandomizeOnderon);
            HandleCategory(e, RegexPeragus, RandomizePeragus);
            HandleCategory(e, RegexTelos, RandomizeTelos);
            HandleCategory(e, RegexNPC, RandomizeNPC);
            HandleCategory(e, RegexPlayHeads, RandomizePlayerHeads);
            HandleCategory(e, RegexPlayBodies, RandomizePlayerBodies);
            HandleCategory(e, RegexPlaceables, RandomizePlaceables);
            HandleCategory(e, RegexParty, RandomizeParty);
            HandleCategory(e, RegexStunt, RandomizeStunt);
            HandleCategory(e, RegexVehicles, RandomizeVehicles);
            HandleCategory(e, RegexWeapons, RandomizeWeapons);

            // Handle other.
            switch (RandomizeOther)
            {
                default:
                case RandomizationLevel.None:
                    break; // Do nothing.
                case RandomizationLevel.Type:
                    List<int> type = new List<int>(e.Key_List.Where(x => Matches_None(x.ResRef) && !Is_Forbidden(x.ResRef)).Select(x => x.ResID));
                    TypeLists.Add(type);
                    break;
                case RandomizationLevel.Max:
                    MaxRando.AddRange(e.Key_List.Where(x => Matches_None(x.ResRef) && !Is_Forbidden(x.ResRef)).Select(x => x.ResID));
                    break;
            }

            // Max Rando.
            List<int> Max_Rando_Iterator = new List<int>(MaxRando);
            Randomize.FisherYatesShuffle(MaxRando);
            int j = 0;
            foreach (ERF.Key k in e.Key_List.Where(x => Max_Rando_Iterator.Contains(x.ResID)))
            {
                LookupTable.Add(k.ResID, MaxRando[j]);
                k.ResID = MaxRando[j];
                j++;
            }

            // Type Rando.
            foreach (List<int> li in TypeLists)
            {
                List<int> type_copy = new List<int>(li);
                Randomize.FisherYatesShuffle(type_copy);
                j = 0;
                foreach (ERF.Key k in e.Key_List.Where(x => li.Contains(x.ResID)))
                {
                    LookupTable.Add(k.ResID, type_copy[j]);
                    k.ResID = type_copy[j];
                    j++;
                }
            }

            e.WriteToFile(paths.TexturePacks + pack_name);
        }

        private static void AssignSettings()
        {
            RandomizeCreatures = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeCreatures;
            RandomizeCubeMaps = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeCubeMaps;
            RandomizeEffects = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeEffects;
            RandomizeItems = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeItems;
            RandomizeNPC = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeNPC;
            RandomizeOther = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeOther;
            RandomizeParty = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeParty;
            RandomizePlaceables = (RandomizationLevel)Properties.Texture.Default.TextureRandomizePlaceables;
            RandomizeEbonHawk = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeEbonHawk;
            RandomizeDantooine = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeDantooine;
            RandomizeM4_78 = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeM4_78;
            RandomizeDxun = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeDxun;
            RandomizeHarbinger = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeHarbinger;
            RandomizeKorriban = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeKorriban;
            RandomizeMalachor = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeMalachor;
            RandomizeMiniGame = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeMiniGame;
            RandomizeNarShadaa = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeNarShadaa;
            RandomizeRavager = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeRavager;
            RandomizeOnderon = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeOnderon;
            RandomizePeragus = (RandomizationLevel)Properties.Texture.Default.TextureRandomizePeragus;
            RandomizeTelos = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeTelos;
            RandomizePlayerBodies = (RandomizationLevel)Properties.Texture.Default.TextureRandomizePlayBodies;
            RandomizePlayerHeads = (RandomizationLevel)Properties.Texture.Default.TextureRandomizePlayHeads;
            RandomizeStunt = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeStunt;
            RandomizeVehicles = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeVehicles;
            RandomizeWeapons = (RandomizationLevel)Properties.Texture.Default.TextureRandomizeWeapons;
            SelectedPack = (TexturePack)Properties.Texture.Default.TexturePack;
        }

        private static bool Matches_None(string s)
        {
            return
                (
                    !RegexCubeMaps.IsMatch(s) &&
                    !RegexCreatures.IsMatch(s) &&
                    !RegexEffects.IsMatch(s) &&
                    !RegexItems.IsMatch(s) &&
                    !RegexNPC.IsMatch(s) &&
                    !RegexPlayHeads.IsMatch(s) &&
                    !RegexPlayBodies.IsMatch(s) &&
                    !RegexPlaceables.IsMatch(s) &&
                    !RegexEbonHawk.IsMatch(s) &&
                    !RegexDantooine.IsMatch(s) &&
                    !RegexM4_78.IsMatch(s) &&
                    !RegexDxun.IsMatch(s) &&
                    !RegexHarbinger.IsMatch(s) &&
                    !RegexKorriban.IsMatch(s) &&
                    !RegexMalachor.IsMatch(s) &&
                    !RegexMiniGame.IsMatch(s) &&
                    !RegexNarShadaa.IsMatch(s) &&
                    !RegexRavager.IsMatch(s) &&
                    !RegexOnderon.IsMatch(s) &&
                    !RegexPeragus.IsMatch(s) &&
                    !RegexTelos.IsMatch(s) &&
                    !RegexParty.IsMatch(s) &&
                    !RegexStunt.IsMatch(s) &&
                    !RegexVehicles.IsMatch(s) &&
                    !RegexWeapons.IsMatch(s)
                );
        }

        private static bool Is_Forbidden(string s)
        {
            return
            (
                s.ToUpper().Last() == 'B' ||
                s.ToUpper().Contains("BMP") ||
                s.ToUpper().Contains("BUMP") ||
                s == "MGG_ebonhawkB01"
            );
        }

        private static void HandleCategory(ERF e, Regex r, RandomizationLevel randomizationlevel)
        {
            switch (randomizationlevel)
            {
                default:
                case RandomizationLevel.None:
                    break; // Do nothing.
                case RandomizationLevel.Type:
                    List<int> type = new List<int>(e.Key_List.Where(x => r.IsMatch(x.ResRef) && !Is_Forbidden(x.ResRef)).Select(x => x.ResID));
                    TypeLists.Add(type);
                    break;
                case RandomizationLevel.Max:
                    MaxRando.AddRange(e.Key_List.Where(x => r.IsMatch(x.ResRef) && !Is_Forbidden(x.ResRef)).Select(x => x.ResID));
                    break;
            }
        }

        internal static void Reset()
        {
            // Prepare lists for new randomization.
            MaxRando.Clear();
            TypeLists.Clear();
            LookupTable.Clear();
            NameLookup.Clear();
        }

    }
}
