using System.Collections.Generic;
using System;
using System.IO;
using SDL;

namespace Client.MirSounds
{
    static class SoundManager
    {
        private static readonly Dictionary<int, Sound> Sounds = new Dictionary<int, Sound>();
        private static readonly Dictionary<int, string> IndexList = new Dictionary<int, string>();

        public static Music Music;

        public static int Vol
        {
            get => Audio.SoundVolume;
            set => Audio.SoundVolume = Math.Max(0, Math.Min(value, 100));
        }

        public static int MusicVol
        {
            get => Audio.MusicVolume;
            set => Audio.MusicVolume = Math.Max(0, Math.Min(value, 100));
        }

        public static void Create()
        {
            SDLContext.InitSubSystem(SubSystem.Audio);

            Audio.Open();
            Audio.Channels = 100;

            Audio.MusicVolume = 32;

            LoadSoundList();
        }
        public static void LoadSoundList()
        {
            string fileName = Path.Combine(Settings.SoundPath, "SoundList.lst");

            if (!File.Exists(fileName)) return;

            string[] lines = File.ReadAllLines(fileName);

            for (int i = 0; i < lines.Length; i++)
            {
                string[] split = lines[i].Replace(" ", "").Split(':', '\t');

                int index;
                if (split.Length <= 1 || !int.TryParse(split[0], out index)) continue;

                if (!IndexList.ContainsKey(index))
                    IndexList.Add(index, split[split.Length - 1]);
            }

        }


        public static void StopSound(int index)
        {
            // TODO: Stop Sound
        }

        private static string SoundFileName(int index)
        {
            if (IndexList.ContainsKey(index)) return IndexList[index];
            if (index > 20000) return string.Format(
                "M{0:0}-{1:0}.wav", (index - 20000) / 10, (index - 20000) % 10);
            if (index < 10000) return string.Format(
                "{0:000}-{1:0}.wav", index / 10, index % 10);
            return null;
        }

        public static void PlaySound(int index, bool loop = false)
        {
            if (Sounds.ContainsKey(index))
            {
                Sounds[index].Play();
                return;
            }

            var filename = SoundFileName(index);
            if (filename == null) return;

            // TODO: Enable looping sounds
            // Needs StopSound implemented first
            loop = false;

            try
            {
                var sound = Audio.LoadSound(Path.Combine(Settings.SoundPath, filename));
                Sounds.Add(index, sound);
                sound.Play(loop ? -1 : 0);
            }
            catch (Exception)
            {
                CMain.SaveError($"Failed to load sound: {filename}");
            }
        }

        public static void PlayMusic(int index, bool loop = true)
        {
            var filename = $"{index}.wav";

            try
            {
                Music = Audio.LoadMusic(Path.Combine(Settings.SoundPath, filename));
                Music.Play(loop ? -1 : 1);
            }
            catch (Exception)
            {
                Audio.HaltMusic();
                CMain.SaveError($"Failed to load music: {filename}");
            }
        }

        // For some reason, the login and character select music use
        // "sound" file names
        public static void PlaySoundAsMusic(int index, bool loop = true)
        {
            var filename = SoundFileName(index);
            if (filename == null) return;

            try
            {
                Music = Audio.LoadMusic(Path.Combine(Settings.SoundPath, filename));
                Music.Play(loop ? -1 : 1);
            }
            catch (Exception)
            {
                Audio.HaltMusic();
                CMain.SaveError($"Failed to load music: {filename}");
            }
        }

        public static void StopSound() => Audio.HaltSound();
        public static void StopMusic() => Audio.HaltMusic();
    }

    public static class SoundList
    {
        public static int
            None = 0,
            Music = 0,

            IntroMusic = 10146,
            SelectMusic = 10147,
            LoginEffect = 10100,

            ButtonA = 10103,
            ButtonB = 10104,
            ButtonC = 10105,
            Gold = 10106,
            EatDrug = 10107,
            ClickDrug = 10108,

            Teleport = 10110,
            LevelUp = 10156,

            ClickWeapon = 10111,
            ClickArmour = 10112,
            ClickRing = 10113,
            ClickBracelet = 10114,
            ClickNecklace = 10115,
            ClickHelmet = 10116,
            ClickBoots = 10117,
            ClickItem = 10118,

            //Movement
            WalkGroundL = 10001,
            WalkGroundR = 10002,
            RunGroundL = 10003,
            RunGroundR = 10004,
            WalkStoneL = 10005,
            WalkStoneR = 10006,
            RunStoneL = 10007,
            RunStoneR = 10008,
            WalkLawnL = 10009,
            WalkLawnR = 10010,
            RunLawnL = 10011,
            RunLawnR = 10012,
            WalkRoughL = 10013,
            WalkRoughR = 10014,
            RunRoughL = 10015,
            RunRoughR = 10016,
            WalkWoodL = 10017,
            WalkWoodR = 10018,
            RunWoodL = 10019,
            RunWoodR = 10020,
            WalkCaveL = 10021,
            WalkCaveR = 10022,
            RunCaveL = 10023,
            RunCaveR = 10024,
            WalkRoomL = 10025,
            WalkRoomR = 10026,
            RunRoomL = 10027,
            RunRoomR = 10028,
            WalkWaterL = 10029,
            WalkWaterR = 10030,
            RunWaterL = 10031,
            RunWaterR = 10032,
            HorseWalkL = 10033,
            HorseWalkR = 10034,
            HorseRun = 10035,
            WalkSnowL = 10036,
            WalkSnowR = 10037,
            RunSnowL = 10038,
            RunSnowR = 10039,

            //Weapon Swing
            SwingShort = 10050,
            SwingWood = 10051,
            SwingSword = 10052,
            SwingSword2 = 10053,
            SwingAxe = 10054,
            SwingClub = 10055,
            SwingLong = 10056,
            SwingFist = 10056,

            //Struck
            StruckShort = 10060,
            StruckWooden = 10061,
            StruckSword = 10062,
            StruckSword2 = 10063,
            StruckAxe = 10064,
            StruckClub = 10065,

            StruckBodySword = 10070,
            StruckBodyAxe = 10071,
            StruckBodyLongStick = 10072,
            StruckBodyFist = 10073,

            StruckArmourSword = 10080,
            StruckArmourAxe = 10081,
            StruckArmourLongStick = 10082,
            StruckArmourFist = 10083,

            MaleFlinch = 10138,
            FemaleFlinch = 10139,
            MaleDie = 10144,
            FemaleDie = 10145,

            Revive = 20791,
            ZombieRevive = 0705,
            
            //Mounts
            MountWalkL = 10176,
            MountWalkR = 10177,
            MountRun = 10178,
            TigerStruck1 = 10179,
            TigerStruck2 = 10180,
            TigerAttack1 = 10181,
            TigerAttack2 = 10182,
            TigerAttack3 = 10183,

            FishingThrow = 10184,
            FishingPull = 10185,
            Fishing = 10186,

            WolfRide1 = 10188,
            WolfRide2 = 10189,
            WolfAttack1 = 10190,
            WolfAttack2 = 10191,
            WolfAttack3 = 10192,
            WolfStruck1 = 10193,
            WolfStruck2 = 10194,
            TigerRide1 = 10218,
            TigerRide2 = 10219,

            PetPig = 10500,
            PetChick = 10501,
            PetKitty = 10502,
            PetSkeleton = 10503,
            PetPigman = 10504,
            PetWemade = 10505,
            PetBlackKitten = 10506,
            PetDragon = 10507,
            PetOlympic = 10508,
            PetFrog = 10510,
            PetMonkey = 10511,
            PetAngryBird = 10512,
            PetFoxey = 10513,
            PetMedicalRat = 10514,
            PetPickup = 10520;
    }
}
