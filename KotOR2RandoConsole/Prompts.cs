using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace KotOR2RandoConsole
{
    public static class Prompts
    {
        static K2Paths paths = new K2Paths(Properties.UserSettings.Default.K2Path);
        public static void StartUp()
        {
            Console.Clear();
            Console.WriteLine("|| KotOR 2 Console Randomizer ||");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("a : about");
            Console.WriteLine("s : set-up");
            if (Properties.UserSettings.Default.SetUpDone)
            {
                Console.WriteLine("c : Configure Randomizer");
                if (File.Exists(paths.RANDOMIZED_LOG)) Console.WriteLine("u : Unrandomize");
                else Console.WriteLine("r : Run Randomizer");
            }
            Console.WriteLine("x : exit");
            var key = Console.ReadKey();
            switch(key.KeyChar)
            {
                case 'a':
                    About();
                    break;
                case 's':
                    SetUp(); 
                    break;
                case 'c':
                    RandoConfig();
                    break;
                case 'r':
                    Run();
                    break;
                case 'u':
                    UnRun();
                    break;
                case 'x':
                    Exit();
                    return;
                default:
                    StartUp();
                    break;
            }
        }
        static void About()
        {
            Console.Clear();
            Console.WriteLine("This is a thrown together temporary kotor 2 randomization tool.");
            Console.WriteLine("I made this after deciding that it'd be too much work for" +
                " me to go through and do a proper implementation of kotOR 2 in teh existing" +
                "Randomizer. Though this will happen eventually, for now this is what y'all " +
                "are getting. I'll hopefully be adding features as I go with this. Any questions," +
                "hit up @Lane#5847 in Discord.\n");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            StartUp();
        }
        static void SetUp()
        {
            Console.Clear();
            Console.WriteLine("Please enter the path to your KotOR 2 game directory below, and hit enter.");
            Console.WriteLine(@"(This path will look somethign like: 'C:\<your computer's stuff>\steamapps\common\Knights of the Old Republic II')");
            Properties.UserSettings.Default.K2Path = Console.ReadLine();
            DirectoryInfo di = new DirectoryInfo(Properties.UserSettings.Default.K2Path ?? "");
            
            while (!di.Exists)
            {
                Properties.UserSettings.Default.SetUpDone = false;
                Console.WriteLine("That path didn't look so good... Try again maybe?");
                Properties.UserSettings.Default.K2Path = Console.ReadLine();
                di = new DirectoryInfo(Properties.UserSettings.Default.K2Path ?? "");
            }

            Properties.UserSettings.Default.SetUpDone = true;
            paths = new K2Paths(Properties.UserSettings.Default.K2Path);
            Console.WriteLine("\n Path Set. Press any key to return to main menu");
            Console.ReadKey();
            StartUp();
        }
        public static void RandoConfig()
        {
            Console.Clear();
            Console.WriteLine("Please select a randomization setting to configure: ");
            Console.WriteLine("s : set seed");
            Console.WriteLine("m : modules");
            Console.WriteLine("i : items");
            Console.WriteLine("t : textures");
            Console.WriteLine("\nx : done");
            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 's':
                    RandoConfigMenus.Seed();
                    break;
                case 'm':
                    RandoConfigMenus.Modules();
                    break;
                case 'i':
                    RandoConfigMenus.Items();
                    break;
                case 't':
                    RandoConfigMenus.Textures();
                    break;
                case 'x':
                    StartUp();
                    break;
                default:
                    RandoConfig();
                    break;
            }
        }
        static void Run()
        {
            Console.Clear();
            Console.WriteLine("Preparing to run the randomizer with the following settings: ");
            Console.WriteLine($"K2 Path: {Properties.UserSettings.Default.K2Path}");
            Console.WriteLine($"Seed: {Randomize.Seed}");
            if (Properties.UserSettings.Default.DoModuleRando)
            {
                List<string> OmitMods = new List<string>(File.ReadAllLines("OmitMods.txt"));
                if (OmitMods.Last() == "") OmitMods.RemoveAt(OmitMods.Count - 1);
                Console.WriteLine("Module Randomization");
                Console.WriteLine($"\t- Omitting {OmitMods.Count} Modules");
                Console.WriteLine($"\t- Galaxy Map {(Properties.UserSettings.Default.GalaxyMapUnlocked ? "Unlocked" : "Locked")}");
                Console.WriteLine($"\t- Modulesave Patch {(Properties.UserSettings.Default.ModuleSavePatch ? "Enabled" : "Disabled")}");
                Console.WriteLine($"\t- Door Unlocks {(Properties.UserSettings.Default.DoorUnlocks ? "Enabled" : "Disabled")}");
                //List other settings later
            }
            if (Properties.UserSettings.Default.DoItemRando) 
            {
                List<string> OmitItems = new List<string>();
                if (File.Exists("OmitItems.txt")) OmitItems.AddRange(File.ReadAllLines("OmitItems.txt"));
                if (OmitItems.Count > 0 && OmitItems.Last() == "") OmitItems.RemoveAt(OmitItems.Count - 1);
                Console.WriteLine("Item Randomization");
                Console.WriteLine($"\t- Omitting {OmitItems.Count} Items");
                Console.WriteLine($"\t- Item Categories:");
                Console.WriteLine($"\t\t> Armbands - {(RandomizationLevel)Properties.Items.Default.RandomizeArmbands}");
                Console.WriteLine($"\t\t> Armor - {(RandomizationLevel)Properties.Items.Default.RandomizeArmor}");
                Console.WriteLine($"\t\t> Belts - {(RandomizationLevel)Properties.Items.Default.RandomizeBelts}");
                Console.WriteLine($"\t\t> Blasters - {(RandomizationLevel)Properties.Items.Default.RandomizeBlasters}");
                Console.WriteLine($"\t\t> Creature Hides - {(RandomizationLevel)Properties.Items.Default.RandomizeHides}");
                Console.WriteLine($"\t\t> Creature Weapons - {(RandomizationLevel)Properties.Items.Default.RandomizeCreature}");
                Console.WriteLine($"\t\t> Droid Items - {(RandomizationLevel)Properties.Items.Default.RandomizeDroid}");
                Console.WriteLine($"\t\t> Gloves - {(RandomizationLevel)Properties.Items.Default.RandomizeGloves}");
                Console.WriteLine($"\t\t> Grenades - {(RandomizationLevel)Properties.Items.Default.RandomizeGrenades}");
                Console.WriteLine($"\t\t> Implants - {(RandomizationLevel)Properties.Items.Default.RandomizeImplants}");
                Console.WriteLine($"\t\t> Lightsabers - {(RandomizationLevel)Properties.Items.Default.RandomizeLightsabers}");
                Console.WriteLine($"\t\t> Masks - {(RandomizationLevel)Properties.Items.Default.RandomizeMask}");
                Console.WriteLine($"\t\t> Melee Weapons - {(RandomizationLevel)Properties.Items.Default.RandomizeMelee}");
                Console.WriteLine($"\t\t> Mines - {(RandomizationLevel)Properties.Items.Default.RandomizeMines}");
                Console.WriteLine($"\t\t> Pazaak Cards - {(RandomizationLevel)Properties.Items.Default.RandomizePaz}");
                Console.WriteLine($"\t\t> Stims/MedPacks - {(RandomizationLevel)Properties.Items.Default.RandomizeStims}");
                Console.WriteLine($"\t\t> Upgrades - {(RandomizationLevel)Properties.Items.Default.RandomizeUpgrade}");
                Console.WriteLine($"\t\t> Custscene Props - {(RandomizationLevel)Properties.Items.Default.RandomizeProps}");
                Console.WriteLine($"\t\t> Named Player Crystal - {(RandomizationLevel)Properties.Items.Default.RandomizePCrystal}");
                Console.WriteLine($"\t\t> Various - {(RandomizationLevel)Properties.Items.Default.RandomizeVarious}");
            }
            if (Properties.UserSettings.Default.DoTextureRando)
            {
                List<string> OmitTextures = new List<string>();
                if (File.Exists("OmitTextures.txt")) OmitTextures.AddRange(File.ReadAllLines("OmitTextures.txt"));
                if (OmitTextures.Count > 0 && OmitTextures.Last() == "") OmitTextures.RemoveAt(OmitTextures.Count - 1);
                Console.WriteLine("Texture Randomization");
                Console.WriteLine($"\t- Omitting {OmitTextures.Count} Textures");
                Console.WriteLine($"\t- Texture Categories:");
                Console.WriteLine($"\t\t> Creatures - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeCreatures}");
                Console.WriteLine($"\t\t> CubeMaps - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeCubeMaps}");
                Console.WriteLine($"\t\t> Effects - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeEffects}");
                Console.WriteLine($"\t\t> Items - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeItems}");
                Console.WriteLine($"\t\t> NPC - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeNPC}");
                Console.WriteLine($"\t\t> Other - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeOther}");
                Console.WriteLine($"\t\t> Party - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeParty}");
                Console.WriteLine($"\t\t> Placeables - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizePlaceables}");
                Console.WriteLine($"\t\t> EbonHawk - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeEbonHawk}");
                Console.WriteLine($"\t\t> Dantooine - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeDantooine}");
                Console.WriteLine($"\t\t> M4_78 - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeM4_78}");
                Console.WriteLine($"\t\t> Dxun - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeDxun}");
                Console.WriteLine($"\t\t> Harbinger - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeHarbinger}");
                Console.WriteLine($"\t\t> Korriban - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeKorriban}");
                Console.WriteLine($"\t\t> Malachor - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeMalachor}");
                Console.WriteLine($"\t\t> MiniGame - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeMiniGame}");
                Console.WriteLine($"\t\t> NarShadaa - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeNarShadaa}");
                Console.WriteLine($"\t\t> Ravager - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeRavager}");
                Console.WriteLine($"\t\t> Onderon - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeOnderon}");
                Console.WriteLine($"\t\t> Peragus - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizePeragus}");
                Console.WriteLine($"\t\t> Telos - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeTelos}");
                Console.WriteLine($"\t\t> PlayerBodies - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizePlayBodies}");
                Console.WriteLine($"\t\t> PlayerHeads - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizePlayHeads}");
                Console.WriteLine($"\t\t> Stunt - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeStunt}");
                Console.WriteLine($"\t\t> Vehicles - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeVehicles}");
                Console.WriteLine($"\t\t> Weapons - { (RandomizationLevel)Properties.Texture.Default.TextureRandomizeWeapons}");
            }
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Continue? y/(n)");
            if (Console.ReadKey().KeyChar != 'y')
            {
                Console.WriteLine("\nABORTING...");
                Thread.Sleep(800);
                StartUp();
                return;
            }
            //Do randomization
            Console.WriteLine("Randomization Begun... This might take a while");

            //Log
            Console.WriteLine();
            Console.WriteLine("Creating log...");
            File.Create(paths.RANDOMIZED_LOG).Close();
            File.AppendAllText(paths.RANDOMIZED_LOG, $"Game randomized on {DateTime.Now.ToLocalTime().ToLongDateString()} at {DateTime.Now.ToLocalTime().ToLongTimeString()}\n");
            File.AppendAllText(paths.RANDOMIZED_LOG, $"Seed: {Randomize.Seed}\n");

            //Modules
            if (Properties.UserSettings.Default.DoModuleRando)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Console.WriteLine("Randomizing Modules...");
                ModuleRando.Module_rando(paths);
                sw.Stop();
                Console.WriteLine($"Module rando took {sw.Elapsed.TotalSeconds} seconds.");
            }

            //Items
            if (Properties.UserSettings.Default.DoItemRando)
            {
                Console.WriteLine("Randomizing Items...");
                ItemRando.item_rando(paths);
            }

            //Textures
            if (Properties.UserSettings.Default.DoTextureRando)
            {
                Console.WriteLine("Randomizing Textures...");
                TextureRando.texture_rando(paths);
            }

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Randomization is done, press a key to continue...");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            StartUp();
        }
        static void UnRun()
        {
            Console.Clear();
            Console.WriteLine("Restoring Backups...");
            paths.RestoreChitinFile();
            paths.RestoreDialogFile();
            paths.RestoreMusicDirectory();
            paths.RestoreOverrideDirectory();
            paths.RestoreSoundsDirectory();
            paths.RestoreTexturePacksDirectory();
            paths.RestoreModulesDirectory();
            paths.RestoreLipsDirectory();
            File.Delete(paths.RANDOMIZED_LOG);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Done! Press any key...");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            StartUp();
        }

        static void Exit()
        {
            Console.Clear();
            Properties.UserSettings.Default.Save();
        }
    }
}
