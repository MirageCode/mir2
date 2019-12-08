using System;
using System.Runtime.InteropServices;

namespace SDL
{
    public enum AudioFormat : ushort
    {
        U8 = 0x0008,
        S8 = 0x8008,
        U16LSB = 0x0010,
        S16LSB = 0x8010,
        U16MSB = 0x1010,
        S16MSB = 0x9010,
        U16 = U16LSB,
        S16 = S16LSB,
        S32LSB = 0x8020,
        S32MSB = 0x9020,
        S32 = S32LSB,
        F32LSB = 0x8120,
        F32MSB = 0x9120,
        F32 = F32LSB,
    }

    public abstract class Audio : Resource<SDLException>
    {
        public const string MixerLib = SDLContext.MixerLib;

        protected Audio(IntPtr handle) : base(handle) {}

        public abstract void Play(int loop);

        public static void Halt()
        {
            HaltMusic();
            HaltSound();
        }

        public static void HaltMusic()
        {
            if (Mix_HaltMusic() != 0) throw new SDLException();
        }

        public static void HaltSound()
        {
            if (Mix_HaltChannel(-1) != 0) throw new SDLException();
        }

        public static void Open(
            int frequency = 44100, int channels = 2, int chunksize = 2048)
        {
            var format = BitConverter.IsLittleEndian
                ? AudioFormat.S16LSB : AudioFormat.S16MSB;

            if (Mix_OpenAudio(frequency, format, channels, chunksize) != 0)
                throw new SDLException();
        }

        public static void Close() => Mix_CloseAudio();

        public static int Channels
        {
            get => Mix_AllocateChannels(-1);
            set => Mix_AllocateChannels(value);
        }

        public static int MusicVolume
        {
            get => Mix_VolumeMusic(-1);
            set => Mix_VolumeMusic(value);
        }

        public static int SoundVolume
        {
            get => Mix_Volume(-1, -1);
            set => Mix_Volume(-1, value);
        }

        public static Music LoadMusic(string file) => new Music(file);
        public static Sound LoadSound(string file) => new Sound(file);

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int Mix_QuerySpec(
			out int frequency, out AudioFormat format, out int channels);

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int Mix_OpenAudio(
			int frequency, AudioFormat format, int channels, int chunksize);

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern void Mix_CloseAudio();

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int Mix_HaltMusic();

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int Mix_HaltChannel(int channel);

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int Mix_AllocateChannels(int numchans);

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern int Mix_VolumeMusic(int volume);

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int Mix_Volume(int channel, int volume);
    }

    public class Music : Audio
    {
        internal Music(String file)
        : base(Mix_LoadMUS(Util.FromString(file))) {}

        protected override void Free(IntPtr handle) => Mix_FreeMusic(handle);

        public override void Play(int loops = -1)
        {
            if (Mix_PlayMusic(handle, loops) == -1)
                throw new SDLException();
        }

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr Mix_LoadMUS(byte[] file);

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern void Mix_FreeMusic(IntPtr music);

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int Mix_PlayMusic(IntPtr music, int loops);
    }

    public class Sound : Audio
    {
        internal Sound(String file)
        : base(Mix_LoadWAV_RW(SDLContext.RWFromFile(file, "rb"), 1)) {}

        protected override void Free(IntPtr handle) => Mix_FreeChunk(handle);

        public override void Play(int loops = 0)
        {
            if (Mix_PlayChannelTimed(-1, handle, loops, -1) == -1)
                throw new SDLException();
        }

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr Mix_LoadWAV_RW(IntPtr src, int freesrc);

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern void Mix_FreeChunk(IntPtr chunk);

		[DllImport(MixerLib, CallingConvention = CallingConvention.Cdecl)]
		private static extern int Mix_PlayChannelTimed(
			int channel, IntPtr chunk, int loops, int ticks);
    }
}
