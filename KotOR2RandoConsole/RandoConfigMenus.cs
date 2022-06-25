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
