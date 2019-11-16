using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.Point;

using Server.MirEnvir;
using Server.MirDatabase;

using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace DatabaseTool
{
    class Program
    {
        static JsonWriterSettings Settings = new JsonWriterSettings() {
            Indent = true,
            NewLineChars = "\n",
        };

        static void register()
        {
            var enumPack = new ConventionPack
            { new EnumRepresentationConvention(BsonType.String) };

            ConventionRegistry.Register("EnumStringConvention", enumPack, t => true);

            BsonSerializer.RegisterSerializer(
                typeof(Point), new BasicStructSerializer<Point>());

            BsonClassMap.RegisterClassMap<Envir>(cm => {
                cm.MapMember(c => c.MapIndex);
                cm.MapMember(c => c.MonsterIndex);
                cm.MapMember(c => c.NPCIndex);
                cm.MapMember(c => c.QuestIndex);
                cm.MapMember(c => c.GameshopIndex);
                cm.MapMember(c => c.ConquestIndex);
                cm.MapMember(c => c.RespawnIndex);
                cm.MapMember(c => c.RespawnTick);
            });

            BsonClassMap.RegisterClassMap<RespawnTimer>(cm => {
                cm.MapMember(c => c.BaseSpawnRate);
                cm.MapMember(c => c.CurrentTickcounter);
                cm.MapMember(c => c.Respawn);
            });

            BsonClassMap.RegisterClassMap<SafeZoneInfo>(cm => {
                cm.AutoMap();
                cm.UnmapMember(c => c.Info);
            });

            BsonClassMap.RegisterClassMap<MapInfo>(cm => {
                cm.AutoMap();
                cm.UnmapMember(c => c.Instance);
            });

            BsonClassMap.RegisterClassMap<MonsterInfo>(cm => {
                cm.AutoMap();
                cm.UnmapMember(c => c.Drops);
                cm.UnmapMember(c => c.HasSpawnScript);
                cm.UnmapMember(c => c.HasDieScript);
            });

            BsonClassMap.RegisterClassMap<NPCInfo>(cm => {
                cm.AutoMap();
                cm.UnmapMember(c => c.Colour);
                cm.UnmapMember(c => c.Sabuk);
                cm.UnmapMember(c => c.IsDefault);
                cm.UnmapMember(c => c.IsRobot);
            });

            BsonClassMap.RegisterClassMap<QuestInfo>(cm => {
                cm.MapMember(c => c.Index);
                cm.MapMember(c => c.Name);
                cm.MapMember(c => c.Group);
                cm.MapMember(c => c.FileName);
                cm.MapMember(c => c.RequiredMinLevel);
                cm.MapMember(c => c.RequiredMaxLevel);
                cm.MapMember(c => c.RequiredQuest);
                cm.MapMember(c => c.RequiredClass);
                cm.MapMember(c => c.Type);
                cm.MapMember(c => c.GotoMessage);
                cm.MapMember(c => c.KillMessage);
                cm.MapMember(c => c.ItemMessage);
                cm.MapMember(c => c.FlagMessage);
            });

            BsonClassMap.RegisterClassMap<ItemInfo>(cm => {
                cm.AutoMap();
                cm.UnmapMember(c => c.RandomStats);
            });
        }

        static string cleanFileName(string filename) =>
            string.Concat(filename.Split(
                Path.GetInvalidFileNameChars()
                .Concat(new [] { '.', '!', '\'', ' ' }).ToArray()));

        static void WriteDirectory<T>(
            List<T> items, string path, Func<T, string> getFileName)
        {
            if (items.Count == 0) return;

            Directory.CreateDirectory(path);

            foreach (var item in items)
            {
                var fileName = Path.Combine(
                    path, cleanFileName(getFileName(item)) + ".json");

                using (var stream = File.CreateText(fileName))
                    stream.WriteLine(item.ToJson(Settings));
            }
        }

        static void ReadDirectory<T>(ref List<T> items, string path)
        {
            if (!Directory.Exists(path)) return;

            foreach (var fileName in Directory.GetFiles(path, "*.json"))
            {
                using (var file = File.OpenText(fileName))
                    items.Add(BsonSerializer.Deserialize<T>(file));
            }
        }

        static void Export(Envir Envir, string directory)
        {
            using (var stream = File.CreateText(
                Path.Combine(directory, "envir.json")))
                stream.WriteLine(Envir.ToJson(Settings));

            using (var stream = File.CreateText(
                Path.Combine(directory, "dragon.json")))
                stream.WriteLine(Envir.DragonInfo.ToJson(Settings));

            WriteDirectory(
                Envir.MapInfoList, Path.Combine(directory, "maps"),
                i => i.Index.ToString("D4") + "--" + i.Title);

            WriteDirectory(
                Envir.ItemInfoList, Path.Combine(directory, "items"),
                i => i.Index.ToString("D4") + "--" + i.Name);

            WriteDirectory(
                Envir.MonsterInfoList, Path.Combine(directory, "monsters"),
                i => i.Index.ToString("D4") + "--" + i.Name);

            WriteDirectory(
                Envir.NPCInfoList, Path.Combine(directory, "npcs"),
                i => i.Index.ToString("D4") + "--" + i.Name);

            WriteDirectory(
                Envir.QuestInfoList, Path.Combine(directory, "quests"),
                i => i.Index.ToString("D4") + "--" + i.Name);

            WriteDirectory(
                Envir.MagicInfoList, Path.Combine(directory, "magic"),
                i => i.Name);

            WriteDirectory(
                Envir.GameShopList, Path.Combine(directory, "gameshop"),
                i => i.GIndex.ToString("D4") + "--" + i.Info.Name);

            WriteDirectory(
                Envir.ConquestInfos, Path.Combine(directory, "conquests"),
                i => i.Index.ToString("D4") + "--" + i.Name);
        }

        static Envir Import(string directory)
        {
            Envir Envir;

            using (var file = File.OpenText(
                Path.Combine(directory, "envir.json")))
                Envir = BsonSerializer.Deserialize<Envir>(file);

            using (var file = File.OpenText(
                Path.Combine(directory, "dragon.json")))
                Envir.DragonInfo = BsonSerializer.Deserialize<DragonInfo>(file);

            Envir.MapInfoList.Clear();
            ReadDirectory(
                ref Envir.MapInfoList, Path.Combine(directory, "maps"));
            Envir.MapInfoList.Sort((x, y) => x.Index - y.Index);

            Envir.ItemInfoList.Clear();
            ReadDirectory(
                ref Envir.ItemInfoList, Path.Combine(directory, "items"));
            Envir.ItemInfoList.Sort((x, y) => x.Index - y.Index);

            Envir.MonsterInfoList.Clear();
            ReadDirectory(
                ref Envir.MonsterInfoList, Path.Combine(directory, "monsters"));
            Envir.MonsterInfoList.Sort((x, y) => x.Index - y.Index);

            Envir.NPCInfoList.Clear();
            ReadDirectory(
                ref Envir.NPCInfoList, Path.Combine(directory, "npcs"));
            Envir.NPCInfoList.Sort((x, y) => x.Index - y.Index);

            Envir.QuestInfoList.Clear();
            ReadDirectory(
                ref Envir.QuestInfoList, Path.Combine(directory, "quests"));
            Envir.QuestInfoList.Sort((x, y) => x.Index - y.Index);

            Envir.MagicInfoList.Clear();
            ReadDirectory(
                ref Envir.MagicInfoList, Path.Combine(directory, "magic"));
            Envir.MagicInfoList.Sort((x, y) => (int) x.Spell - (int) y.Spell);

            Envir.GameShopList.Clear();
            ReadDirectory(
                ref Envir.GameShopList, Path.Combine(directory, "gameshop"));
            Envir.GameShopList.Sort((x, y) => x.GIndex - y.GIndex);

            Envir.ConquestInfos.Clear();
            ReadDirectory(
                ref Envir.ConquestInfos, Path.Combine(directory, "conquests"));
            Envir.ConquestInfos.Sort((x, y) => x.Index - y.Index);

            return Envir;
        }

        static void PrintUsage()
        {
            Console.Error.WriteLine("Usage:");
            Console.Error.WriteLine(
                "dotnet {0} export <path/to/Server.MirDB> <path/to/export-dir>",
                Environment.CommandLine);
            Console.Error.WriteLine(
                "dotnet {0} import <path/to/Server.MirDB> <path/to/import-dir>",
                Environment.CommandLine);
        }

        static void Main(string[] args)
        {
            register();

            if (args.Length != 3)
            {
                PrintUsage();
                Environment.Exit(1);
            }

            var command = args[0];
            var filename = args[1];
            var directory = args[2];

            if (command != "export" && command != "import")
            {
                PrintUsage();
                Environment.Exit(1);
            }

            if (command == "export")
            {
                if (!File.Exists(filename))
                {
                    Console.Error.WriteLine("{0} does not exist", filename);
                    Environment.Exit(1);
                }

                Envir Envir = Envir.Main;

                Envir.LoadDB(filename);
                Directory.CreateDirectory(directory);
                Export(Envir, directory);
            }

            if (command == "import")
            {
                if (!Directory.Exists(directory))
                {
                    Console.Error.WriteLine("{0} does not exist", directory);
                    Environment.Exit(1);
                }

                Envir Envir = Import(directory);
                Envir.SaveDB(filename);
            }
        }
    }
}
