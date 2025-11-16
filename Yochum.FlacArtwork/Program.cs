using TagLib;

namespace Yochum.FlacArtwork;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: app <directory>");
            return;
        }

        var rootDir = args[0];
        if (!Directory.Exists(rootDir))
        {
            Console.WriteLine("Directory not found.");
            return;
        }

        ProcessDirectory(rootDir);
    }

    private static void ProcessDirectory(string dir)
    {
        foreach (var subDir in Directory.GetDirectories(dir))
        {
            // Check if this folder has .flac files and an image
            var flacFiles = Directory.GetFiles(subDir, "*.flac");
            var imageFiles = Directory.GetFiles(subDir, "*.jpg")
                .Concat(Directory.GetFiles(subDir, "*.png"))
                .ToArray();

            if (flacFiles.Length != 0 && imageFiles.Length != 0)
            {
                Console.WriteLine($"Processing folder: {subDir}");
                var imagePath = imageFiles.First(); // pick first image

                foreach (var flac in flacFiles)
                {
                    AddArtwork(flac, imagePath);
                }
            }

            // Recurse into deeper subdirectories
            ProcessDirectory(subDir);
        }
    }

    private static void AddArtwork(string flacPath, string imagePath)
    {
        try
        {
            var file = TagLib.File.Create(flacPath);
            var picture = new Picture(imagePath);
            file.Tag.Pictures = [picture];
            file.Save();
            Console.WriteLine($"Updated artwork for: {flacPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing {flacPath}: {ex.Message}");
        }
    }
}
