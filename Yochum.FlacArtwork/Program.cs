using Serilog;
using System.Runtime.CompilerServices;
using TagLib;

namespace Yochum.FlacArtwork;

public class Program
{
    public static void Main(string[] args)
    {
        InitLogger();
        if (args.Length == 0)
        {
            Log.Error("Usage: Yochum.FlacArtwork <directory>");
            return;
        }

        var rootDir = args[0];
        if (!Directory.Exists(rootDir))
        {
            Log.Error("Directory not found.");
            return;
        }

        IList<string> noArtDirs = [];
        ProcessDirectory(rootDir, noArtDirs);
        foreach (var noArtDir in noArtDirs)
        {
            Log.Information(noArtDir);
        }
    }

    private static void ProcessDirectory(
        string dir,
        IList<string> noArtDirs
    )
    {
        foreach (var subDir in Directory.GetDirectories(dir).Order())
        {
            // Check if this folder has .flac files and an image
            var flacFiles = Directory.GetFiles(subDir, "*.flac");
            var imageFiles = Directory.GetFiles(subDir, "*.jpg")
                .Concat(Directory.GetFiles(subDir, "*.png"))
                .ToArray();

            if (flacFiles.Length != 0 && imageFiles.Length != 0)
            {
                Log.Information($"Processing folder: {subDir}");
                var imagePath = imageFiles.First(); // pick first image

                foreach (var flac in flacFiles)
                {
                    AddArtwork(flac, imagePath);
                }
            }
            else
            {
                Log.Information($"Skipping dir: {subDir}, {flacFiles.Length} FLAC files, {imageFiles.Length} Image files");
                if (flacFiles.Length != 0)
                {
                    noArtDirs.Add(subDir);
                }
            }

            // Recurse into deeper subdirectories
            ProcessDirectory(subDir, noArtDirs);
        }
    }

    private static void AddArtwork(string flacPath, string imagePath)
    {
        try
        {
            var file = TagLib.File.Create(flacPath);
            var picture = new Picture(imagePath);
            if (file.Tag.Pictures != null && file.Tag.Pictures.Length != 0)
            {
                Log.Information($"Skipped file: {flacPath}, already has artwork");
            }
            else
            {
                file.Tag.Pictures = [picture];
                file.Save();
                Log.Information($"Updated artwork for: {flacPath}");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error processing {flacPath}: {ex.Message}");
        }
    }

    private static void InitLogger()
    {
        var now = DateTimeOffset.UtcNow;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Logs",
                        $"Log_.log"
                    ),
                    rollingInterval: RollingInterval.Hour,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}"
                )
                .CreateLogger();
    }
}
