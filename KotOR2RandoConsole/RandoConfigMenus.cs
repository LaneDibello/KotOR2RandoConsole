using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace KotOR2RandoConsole
{
    public static class RandoConfigMenus
    {
        public static void Seed()
        {
            Console.Clear();

            Console.WriteLine("Enter a new integer seed: ");
            string seed_s = Console.ReadLine() ?? "0";
            int seed;
            while(!int.TryParse(seed_s, out seed))
            {
                Console.WriteLine("I said integer dumbass, try again: ");
                seed_s = Console.ReadLine() ?? "0";
            }
            Randomize.SetSeed(seed);

            Console.WriteLine($"Seed set to {seed}, press any key...");
            Prompts.RandoConfig();
        }
        public static void Modules()
        {
            Console.Clear();

            Console.Write($"Module rando is ");
            if (Properties.UserSettings.Default.DoModuleRando) Console.BackgroundColor = ConsoleColor.Green;
            else Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"{(Properties.UserSettings.Default.DoModuleRando ? "Enabled" : "Disabled")}");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("t : Toggle Module Randomization");
            Console.WriteLine("p : Print omitted modules");
            Console.WriteLine("o : Omit new module");
            if (Properties.UserSettings.Default.GalaxyMapUnlocked) Console.WriteLine("l : Lock Galaxy Map");
            else Console.WriteLine("u : Unlock Galaxy Map");
            Console.WriteLine($"s : Toggle Module Save patch ({(Properties.UserSettings.Default.ModuleSavePatch ? "Enabled" : "Disabled")})");
            Console.WriteLine($"d : Toggle Door Unlocks ({(Properties.UserSettings.Default.DoorUnlocks ? "Enabled" : "Disabled")})");
            Console.WriteLine("x : Return to Config");
            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 's':
                    Properties.UserSettings.Default.ModuleSavePatch = !Properties.UserSettings.Default.ModuleSavePatch;
                    Modules();
                    break;
                case 'd':
                    Properties.UserSettings.Default.DoorUnlocks = !Properties.UserSettings.Default.DoorUnlocks;
                    Modules();
                    break;
                case 'u':
                    Properties.UserSettings.Default.GalaxyMapUnlocked = true;
                    Modules();
                    break;
                case 'l':
                    Properties.UserSettings.Default.GalaxyMapUnlocked = false;
                    Modules();
                    break;
                case 't':
                    Properties.UserSettings.Default.DoModuleRando = !Properties.UserSettings.Default.DoModuleRando;
                    Modules();
                    break;
                case 'p':
                    Console.Clear();
                    //Print
                    Console.WriteLine("Omitted Modules: ");
                    try
                    {
                        Console.WriteLine(File.ReadAllText("OmitMods.txt"));
                    } catch (Exception)
                    {
                        File.Create("OmitMods.txt").Close();
                    }
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Modules();
                    break;
                case 'o':
                    Console.Clear();
                    Console.WriteLine("Module: ");
                    string? mod = Console.ReadLine();
                    //write out
                    File.AppendAllText("OmitMods.txt", mod + "\n");
                    Modules();
                    break;
                case 'x':
                    Prompts.RandoConfig();
                    break;
                default:
                    Modules();
                    break;
            }
        }
        public static void Items()
        {
            Console.Clear();

            Console.Write($"Item rando is ");
            if (Properties.UserSettings.Default.DoItemRando) Console.BackgroundColor = ConsoleColor.Green;
            else Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"{(Properties.UserSettings.Default.DoItemRando ? "Enabled" : "Disabled")}");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("t : Toggle Item Randomization");
            Console.WriteLine("p : Print omitted items");
            Console.WriteLine("o : Omit new item");
            Console.WriteLine("c : Configure item categories");
            Console.WriteLine("x : Return to Config");
            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 't':
                    Properties.UserSettings.Default.DoItemRando = !Properties.UserSettings.Default.DoItemRando;
                    Items();
                    break;
                case 'p':
                    Console.Clear();
                    if (File.Exists("OmitItems.txt")) Console.WriteLine(File.ReadAllText("OmitItems.txt"));
                    else Console.WriteLine("None");
                    Console.WriteLine("Press any key...");
                    Console.ReadKey();
                    Items();
                    break;
                case 'o':
                    Console.Clear();
                    Console.WriteLine("Item: ");
                    string? item = Console.ReadLine();
                    //write out
                    File.AppendAllText("OmitItems.txt", item + "\n");
                    Items();
                    break;
                case 'c':
                    ListItemSettings();
                    Items();
                    break;
                case 'x':
                    Properties.UserSettings.Default.Save();
                    Properties.Items.Default.Save();
                    Prompts.RandoConfig();
                    break;
                default:
                    Items();
                    break;

            }
        }
        public static void Textures()
        {
            Console.Clear();

            Console.Write($"Tetxure rando is ");
            if (Properties.UserSettings.Default.DoTextureRando) Console.BackgroundColor = ConsoleColor.Green;
            else Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"{(Properties.UserSettings.Default.DoTextureRando ? "Enabled" : "Disabled")}");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("t : Toggle Texture Randomization");
            Console.WriteLine("p : Print omitted textures");
            Console.WriteLine("o : Omit new texture");
            Console.WriteLine("c : Configure texture categories");
            Console.WriteLine("x : Return to Config");
            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 't':
                    Properties.UserSettings.Default.DoTextureRando = !Properties.UserSettings.Default.DoTextureRando;
                    Textures();
                    break;
                case 'p':
                    Console.Clear();
                    if (File.Exists("OmitTextures.txt")) Console.WriteLine(File.ReadAllText("OmitTextures.txt"));
                    else Console.WriteLine("None");
                    Console.WriteLine("Press any key...");
                    Console.ReadKey();
                    Textures();
                    break;
                case 'o':
                    Console.Clear();
                    Console.WriteLine("Texture: ");
                    string? item = Console.ReadLine();
                    //write out
                    File.AppendAllText("OmitTextures.txt", item + "\n");
                    Textures();
                    break;
                case 'c':
                    ListTextureSettings();
                    Textures();
                    break;
                case 'x':
                    Properties.UserSettings.Default.Save();
                    Properties.Texture.Default.Save();
                    Prompts.RandoConfig();
                    break;
                default:
                    Textures();
                    break;

            }
        }

        private static void ListTextureSettings()
        {
            Console.Clear();
            Console.WriteLine($"0 : Creatures - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeCreatures}");
            Console.WriteLine($"1 : CubeMaps - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeCubeMaps}");
            Console.WriteLine($"2 : Effects - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeEffects}");
            Console.WriteLine($"3 : Items - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeItems}");
            Console.WriteLine($"4 : NPC - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeNPC}");
            Console.WriteLine($"5 : Other - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeOther}");
            Console.WriteLine($"6 : Party - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeParty}");
            Console.WriteLine($"7 : Placeables - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizePlaceables}");
            Console.WriteLine($"8 : EbonHawk - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeEbonHawk}");
            Console.WriteLine($"9 : Dantooine - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeDantooine}");
            Console.WriteLine($"a : M4_78 - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeM4_78}");
            Console.WriteLine($"b : Dxun - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeDxun}");
            Console.WriteLine($"c : Harbinger - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeHarbinger}");
            Console.WriteLine($"d : Korriban - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeKorriban}");
            Console.WriteLine($"e : Malachor - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeMalachor}");
            Console.WriteLine($"f : MiniGame - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeMiniGame}");
            Console.WriteLine($"g : NarShadaa - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeNarShadaa}");
            Console.WriteLine($"h : Ravager - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeRavager}");
            Console.WriteLine($"i : Onderon - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeOnderon}");
            Console.WriteLine($"j : Peragus - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizePeragus}");
            Console.WriteLine($"k : Telos - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeTelos}");
            Console.WriteLine($"l : PlayerBodies - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizePlayBodies}");
            Console.WriteLine($"o : PlayerHeads - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizePlayHeads}");
            Console.WriteLine($"p : Stunt - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeStunt}");
            Console.WriteLine($"q : Vehicles - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeVehicles}");
            Console.WriteLine($"r : Weapons - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeWeapons}");

            Console.WriteLine("\nn : Set all to 'None'");
            Console.WriteLine("t : Set all to 'Type'");
            Console.WriteLine("m : Set all to 'Max'");

            Console.WriteLine("\nx : Exit...");

            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 'x':
                    break;
                case 'n':
                    SetAllTextureCats(RandomizationLevel.None);
                    ListTextureSettings();
                    break;
                case 't':
                    SetAllTextureCats(RandomizationLevel.Type);
                    ListTextureSettings();
                    break;
                case 'm':
                    SetAllTextureCats(RandomizationLevel.Max);
                    ListTextureSettings();
                    break;
                case '0':
                    Properties.Texture.Default.TextureRandomizeCreatures = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case '1':
                    Properties.Texture.Default.TextureRandomizeCubeMaps = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case '2':
                    Properties.Texture.Default.TextureRandomizeEffects = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case '3':
                    Properties.Texture.Default.TextureRandomizeItems = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case '4':
                    Properties.Texture.Default.TextureRandomizeNPC = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case '5':
                    Properties.Texture.Default.TextureRandomizeOther = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case '6':
                    Properties.Texture.Default.TextureRandomizeParty = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case '7':
                    Properties.Texture.Default.TextureRandomizePlaceables = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case '8':
                    Properties.Texture.Default.TextureRandomizeEbonHawk = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case '9':
                    Properties.Texture.Default.TextureRandomizeDantooine = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'a':
                    Properties.Texture.Default.TextureRandomizeM4_78 = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'b':
                    Properties.Texture.Default.TextureRandomizeDxun = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'c':
                    Properties.Texture.Default.TextureRandomizeHarbinger = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'd':
                    Properties.Texture.Default.TextureRandomizeKorriban = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'e':
                    Properties.Texture.Default.TextureRandomizeMalachor = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'f':
                    Properties.Texture.Default.TextureRandomizeMiniGame = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'g':
                    Properties.Texture.Default.TextureRandomizeNarShadaa = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'h':
                    Properties.Texture.Default.TextureRandomizeRavager = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'i':
                    Properties.Texture.Default.TextureRandomizeOnderon = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'j':
                    Properties.Texture.Default.TextureRandomizePeragus = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'k':
                    Properties.Texture.Default.TextureRandomizeTelos = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'l':
                    Properties.Texture.Default.TextureRandomizePlayBodies = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'o':
                    Properties.Texture.Default.TextureRandomizePlayBodies = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'p':
                    Properties.Texture.Default.TextureRandomizePlayBodies = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'q':
                    Properties.Texture.Default.TextureRandomizePlayBodies = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                case 'r':
                    Properties.Texture.Default.TextureRandomizePlayBodies = ConfigureTextureCat();
                    ListTextureSettings();
                    break;
                default:
                    ListTextureSettings();
                    break;
            }
        }
        private static void SetAllTextureCats(RandomizationLevel r)
        {

            Properties.Texture.Default.TextureRandomizeCreatures = (int)r;
            Properties.Texture.Default.TextureRandomizeCubeMaps = (int)r;
            Properties.Texture.Default.TextureRandomizeEffects = (int)r;
            Properties.Texture.Default.TextureRandomizeItems = (int)r;
            Properties.Texture.Default.TextureRandomizeNPC = (int)r;
            Properties.Texture.Default.TextureRandomizeOther = (int)r;
            Properties.Texture.Default.TextureRandomizeParty = (int)r;
            Properties.Texture.Default.TextureRandomizePlaceables = (int)r;
            Properties.Texture.Default.TextureRandomizeEbonHawk = (int)r;
            Properties.Texture.Default.TextureRandomizeDantooine = (int)r;
            Properties.Texture.Default.TextureRandomizeM4_78 = (int)r;
            Properties.Texture.Default.TextureRandomizeDxun = (int)r;
            Properties.Texture.Default.TextureRandomizeHarbinger = (int)r;
            Properties.Texture.Default.TextureRandomizeKorriban = (int)r;
            Properties.Texture.Default.TextureRandomizeMalachor = (int)r;
            Properties.Texture.Default.TextureRandomizeMiniGame = (int)r;
            Properties.Texture.Default.TextureRandomizeNarShadaa = (int)r;
            Properties.Texture.Default.TextureRandomizeRavager = (int)r;
            Properties.Texture.Default.TextureRandomizeOnderon = (int)r;
            Properties.Texture.Default.TextureRandomizePeragus = (int)r;
            Properties.Texture.Default.TextureRandomizeTelos = (int)r;
            Properties.Texture.Default.TextureRandomizePlayBodies = (int)r;
            Properties.Texture.Default.TextureRandomizePlayHeads = (int)r;
            Properties.Texture.Default.TextureRandomizeStunt = (int)r;
            Properties.Texture.Default.TextureRandomizeVehicles = (int)r;
            Properties.Texture.Default.TextureRandomizeWeapons = (int)r;
        }
        private static int ConfigureTextureCat()
        {
            Console.Clear();
            Console.WriteLine("Select Randomization Level: ");
            Console.WriteLine("n - None");
            Console.WriteLine("t - Type");
            Console.WriteLine("m - Max");
            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 'n':
                    return (int)RandomizationLevel.None;
                case 't':
                    return (int)RandomizationLevel.Type;
                case 'm':
                    return (int)RandomizationLevel.Max;
                default:
                    return 0;
            }
        }
        private static void ListItemSettings()
        {
            Console.Clear();
            Console.WriteLine($"0 : Armbands - {(RandomizationLevel)Properties.Items.Default.RandomizeArmbands}");
            Console.WriteLine($"1 : Armor - {(RandomizationLevel)Properties.Items.Default.RandomizeArmor}");
            Console.WriteLine($"2 : Belts - {(RandomizationLevel)Properties.Items.Default.RandomizeBelts}");
            Console.WriteLine($"3 : Blasters - {(RandomizationLevel)Properties.Items.Default.RandomizeBlasters}");
            Console.WriteLine($"4 : Creature Hides - {(RandomizationLevel)Properties.Items.Default.RandomizeHides}");
            Console.WriteLine($"5 : Creature Weapons - {(RandomizationLevel)Properties.Items.Default.RandomizeCreature}");
            Console.WriteLine($"6 : Droid Items - {(RandomizationLevel)Properties.Items.Default.RandomizeDroid}");
            Console.WriteLine($"7 : Gloves - {(RandomizationLevel)Properties.Items.Default.RandomizeGloves}");
            Console.WriteLine($"8 : Grenades - {(RandomizationLevel)Properties.Items.Default.RandomizeGrenades}");
            Console.WriteLine($"9 : Implants - {(RandomizationLevel)Properties.Items.Default.RandomizeImplants}");
            Console.WriteLine($"a : Lightsabers - {(RandomizationLevel)Properties.Items.Default.RandomizeLightsabers}");
            Console.WriteLine($"b : Masks - {(RandomizationLevel)Properties.Items.Default.RandomizeMask}");
            Console.WriteLine($"c : Melee Weapons - {(RandomizationLevel)Properties.Items.Default.RandomizeMelee}");
            Console.WriteLine($"d : Mines - {(RandomizationLevel)Properties.Items.Default.RandomizeMines}");
            Console.WriteLine($"e : Pazaak Cards - {(RandomizationLevel)Properties.Items.Default.RandomizePaz}");
            Console.WriteLine($"f : Stims/MedPacks - {(RandomizationLevel)Properties.Items.Default.RandomizeStims}");
            Console.WriteLine($"g : Upgrades - {(RandomizationLevel)Properties.Items.Default.RandomizeUpgrade}");
            Console.WriteLine($"h : Custscene Props - {(RandomizationLevel)Properties.Items.Default.RandomizeProps}");
            Console.WriteLine($"i : Named Player Crystal - {(RandomizationLevel)Properties.Items.Default.RandomizePCrystal}");
            Console.WriteLine($"j : Various - {(RandomizationLevel)Properties.Items.Default.RandomizeVarious}");

            Console.WriteLine("\nn : Set all to 'None'");
            Console.WriteLine("t : Set all to 'Type'");
            Console.WriteLine("m : Set all to 'Max'");

            Console.WriteLine("\nx : Exit...");

            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 'x':
                    break;
                case 'n':
                    SetAllItemCats(RandomizationLevel.None);
                    ListItemSettings();
                    break;
                case 't':
                    SetAllItemCats(RandomizationLevel.Type);
                    ListItemSettings();
                    break;
                case 'm':
                    SetAllItemCats(RandomizationLevel.Max);
                    ListItemSettings();
                    break;
                case '0':
                    Properties.Items.Default.RandomizeArmbands = ConfigureItemCat(ItemRando.ArmbandsRegs);
                    ListItemSettings();
                    break;
                case '1':
                    Properties.Items.Default.RandomizeArmor = ConfigureItemCat(ItemRando.ArmorRegs);
                    ListItemSettings();
                    break;
                case '2':
                    Properties.Items.Default.RandomizeBelts = ConfigureItemCat(ItemRando.BeltsRegs);
                    ListItemSettings();
                    break;
                case '3':
                    Properties.Items.Default.RandomizeBlasters = ConfigureItemCat(ItemRando.BlastersRegs);
                    ListItemSettings();
                    break;
                case '4':
                    Properties.Items.Default.RandomizeHides = ConfigureItemCat(ItemRando.HidesRegs);
                    ListItemSettings();
                    break;
                case '5':
                    Properties.Items.Default.RandomizeCreature = ConfigureItemCat(ItemRando.CreatureRegs);
                    ListItemSettings();
                    break;
                case '6':
                    Properties.Items.Default.RandomizeDroid = ConfigureItemCat(ItemRando.DroidRegs);
                    ListItemSettings();
                    break;
                case '7':
                    Properties.Items.Default.RandomizeGloves = ConfigureItemCat(ItemRando.GlovesRegs);
                    ListItemSettings();
                    break;
                case '8':
                    Properties.Items.Default.RandomizeGrenades = ConfigureItemCat(ItemRando.GrenadesRegs);
                    ListItemSettings();
                    break;
                case '9':
                    Properties.Items.Default.RandomizeImplants = ConfigureItemCat(ItemRando.ImplantsRegs);
                    ListItemSettings();
                    break;
                case 'a':
                    Properties.Items.Default.RandomizeLightsabers = ConfigureItemCat(ItemRando.LightsabersRegs);
                    ListItemSettings();
                    break;
                case 'b':
                    Properties.Items.Default.RandomizeMask = ConfigureItemCat(ItemRando.MaskRegs);
                    ListItemSettings();
                    break;
                case 'c':
                    Properties.Items.Default.RandomizeMelee = ConfigureItemCat(ItemRando.MeleeRegs);
                    ListItemSettings();
                    break;
                case 'd':
                    Properties.Items.Default.RandomizeMines = ConfigureItemCat(ItemRando.MinesRegs);
                    ListItemSettings();
                    break;
                case 'e':
                    Properties.Items.Default.RandomizePaz = ConfigureItemCat(ItemRando.PazRegs);
                    ListItemSettings();
                    break;
                case 'f':
                    Properties.Items.Default.RandomizeStims = ConfigureItemCat(ItemRando.StimsRegs);
                    ListItemSettings();
                    break;
                case 'g':
                    Properties.Items.Default.RandomizeUpgrade = ConfigureItemCat(ItemRando.UpgradeRegs);
                    ListItemSettings();
                    break;
                case 'h':
                    Properties.Items.Default.RandomizeProps = ConfigureItemCat(ItemRando.PropsRegs);
                    ListItemSettings();
                    break;
                case 'i':
                    Properties.Items.Default.RandomizePCrystal = ConfigureItemCat(ItemRando.PCrystalRegs);
                    ListItemSettings();
                    break;
                case 'j':
                    Properties.Items.Default.RandomizeVarious = ConfigureItemCat(new());
                    ListItemSettings();
                    break;
                default:
                    ListItemSettings();
                    break;
            }
        }
        private static int ConfigureItemCat(List<Regex> regices)
        {
            Console.Clear();
            Console.WriteLine("Select Randomization Level: ");
            Console.WriteLine("n - None");
            if (regices.Count > 1) Console.WriteLine("s - Subtype");
            Console.WriteLine("t - Type");
            Console.WriteLine("m - Max");
            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 'n':
                   return (int)RandomizationLevel.None;
                case 's':
                   return (int)RandomizationLevel.Subtype;
                case 't':
                   return (int)RandomizationLevel.Type;
                case 'm':
                   return (int)RandomizationLevel.Max;
                default:
                   return 0;
            }
        }
        private static void SetAllItemCats(RandomizationLevel r)
        {
            Properties.Items.Default.RandomizeArmbands = (int)r;
            Properties.Items.Default.RandomizeArmor = (int)r;
            Properties.Items.Default.RandomizeBelts = (int)r;
            Properties.Items.Default.RandomizeBlasters = (int)r;
            Properties.Items.Default.RandomizeHides = (int)r;
            Properties.Items.Default.RandomizeCreature = (int)r;
            Properties.Items.Default.RandomizeDroid = (int)r;
            Properties.Items.Default.RandomizeGloves = (int)r;
            Properties.Items.Default.RandomizeGrenades = (int)r;
            Properties.Items.Default.RandomizeImplants = (int)r;
            Properties.Items.Default.RandomizeLightsabers = (int)r;
            Properties.Items.Default.RandomizeMask = (int)r;
            Properties.Items.Default.RandomizeMelee = (int)r;
            Properties.Items.Default.RandomizeMines = (int)r;
            Properties.Items.Default.RandomizePaz = (int)r;
            Properties.Items.Default.RandomizeStims = (int)r;
            Properties.Items.Default.RandomizeUpgrade = (int)r;
            Properties.Items.Default.RandomizeProps = (int)r;
            Properties.Items.Default.RandomizePCrystal = (int)r;
            Properties.Items.Default.RandomizeVarious = (int)r;
        }
    }
}
