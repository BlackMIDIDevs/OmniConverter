using FFMpegCore;
using FFMpegCore.Enums;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace OmniConverter
{
    public enum AudioCodecType
    {
        PCM,
        FLAC,
        LAME,
        Vorbis,
        Max = Vorbis
    }

    public static class AudioCodecTypeExtensions
    {
        public static string ToExtension(this AudioCodecType codec)
        {
            return codec switch
            {
                AudioCodecType.PCM => ".wav",
                AudioCodecType.FLAC => ".flac",
                AudioCodecType.LAME => ".mp3",
                AudioCodecType.Vorbis => ".ogg",
                _ => ""
            };
        }

        public static Codec? ToFFMpegCodec(this AudioCodecType codec)
        {
            return codec switch
            {
                AudioCodecType.PCM => null, // We don't need to convert PCM
                AudioCodecType.FLAC => FFMpeg.GetCodec("flac"),
                AudioCodecType.LAME => FFMpeg.GetCodec("libmp3lame"),
                AudioCodecType.Vorbis => FFMpeg.GetCodec("libvorbis"),
                _ => null
            };
        }

        // Hacky shit
        public static string? CheckFFMpegDirectory()
        {
            string? query = null;
            string? result = null;
            var ffmpegbin = $"ffmpeg{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "")}";
            var ffmpegoc = $"{AppContext.BaseDirectory}/{ffmpegbin}";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                query = Environment.GetEnvironmentVariable("PATH")?
                    .Split(';')
                    .Where(s => File.Exists(Path.Combine(s, ffmpegbin)))
                    .FirstOrDefault();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                query = $"/usr/bin";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // Homebrew!!!!!
                query = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "/opt/homebrew/bin" : "/usr/local/bin";
            }
            else query = AppContext.BaseDirectory;

            if (query != null)
            {
                query = $"{query}/{ffmpegbin}";

                Debug.PrintToConsole(Debug.LogType.Message, $"Filepath for ffmpeg is: \"{query}\"");

                if (!ffmpegoc.Equals(query) && File.Exists($"{query}"))
                {
                    result = Path.GetDirectoryName(query);
                    Debug.PrintToConsole(Debug.LogType.Message, $"Final directory for ffmpeg is \"{result}\"");
                }
                else result = File.Exists(ffmpegoc) ? AppContext.BaseDirectory : null;
            }

            return result;
        }
    }
}
