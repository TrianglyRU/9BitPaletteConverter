using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static readonly int[] allowedValues = { 0, 36, 73, 109, 146, 182, 219, 255 };
        static readonly string lastPathFile = "last_path.txt";


        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            while (true)
            {
                string savedPath = File.Exists(lastPathFile) ? File.ReadAllText(lastPathFile).Trim() : null;
                bool savedPathExists = !string.IsNullOrEmpty(savedPath) && Directory.Exists(savedPath);

                if (savedPathExists)
                {
                    Console.WriteLine($"Found a folder: {savedPath}");
                    Console.WriteLine("Leave the input empty to use the saved folder, or drag/drop or enter a new folder path, then press Enter to process the images:");
                }
                else
                {
                    Console.WriteLine("Please enter the path to the folder or drag/drop it here, then press Enter:");
                }

                string input = Console.ReadLine().Trim();
                string folderPath = string.IsNullOrEmpty(input) ? savedPath : input;

                if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                {
                    Console.WriteLine("Invalid or missing folder path. Press Enter to try again.");
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                }

                try
                {
                    File.WriteAllText(lastPathFile, folderPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not save path to {lastPathFile}: {ex.Message}");
                }

                string outputRoot = folderPath + "_converted";
                ProcessImages(folderPath, outputRoot);

                Console.WriteLine("\nConversion completed.");
                Console.WriteLine("Press Enter to convert another folder...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        static void ProcessImages(string inputRoot, string outputRoot)
        {
            var imageFiles = Directory.GetFiles(inputRoot, "*.*", SearchOption.AllDirectories);
            Parallel.ForEach(imageFiles, filePath =>
            {
                if (IsImageFile(filePath))
                {
                    ProcessImage(filePath, inputRoot, outputRoot);
                }
            });
        }

        static bool IsImageFile(string filePath) =>
            filePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
            filePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
            filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
            filePath.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
            filePath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase);

        static void ProcessImage(string filePath, string inputRoot, string outputRoot)
        {
            try
            {
                string modifiedFilePath = GetModifiedFilePath(filePath, inputRoot, outputRoot);

                using (Bitmap originalBitmap = new Bitmap(filePath))
                {
                    Bitmap bitmapToProcess = originalBitmap;

                    if (IsIndexedPixelFormat(originalBitmap.PixelFormat))
                    {
                        bitmapToProcess = ConvertToNonIndexed(originalBitmap);
                    }

                    using (bitmapToProcess)
                    {
                        for (int y = 0; y < bitmapToProcess.Height; y++)
                        {
                            for (int x = 0; x < bitmapToProcess.Width; x++)
                            {
                                Color originalColor = bitmapToProcess.GetPixel(x, y);
                                Color newColor = Color.FromArgb(originalColor.A, GetNearestValue(originalColor.R), GetNearestValue(originalColor.G), GetNearestValue(originalColor.B));
                                bitmapToProcess.SetPixel(x, y, newColor);
                            }
                        }

                        bitmapToProcess.Save(modifiedFilePath);
                        Console.WriteLine($"Converted {modifiedFilePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting file {filePath}: {ex.Message}");
            }
        }
        static string GetModifiedFilePath(string originalPath, string inputRoot, string outputRoot)
        {
            string relativePath = Path.GetRelativePath(inputRoot, originalPath);
            string modifiedPath = Path.Combine(outputRoot, relativePath);
            string modifiedDirectory = Path.GetDirectoryName(modifiedPath);

            if (!Directory.Exists(modifiedDirectory))
            {
                Directory.CreateDirectory(modifiedDirectory);
            }

            return modifiedPath;
        }

        static bool IsIndexedPixelFormat(PixelFormat pixelFormat)
        {
            return (pixelFormat & PixelFormat.Indexed) != 0;
        }

        static Bitmap ConvertToNonIndexed(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.DrawImage(original, 0, 0);
            }

            return newBitmap;
        }

        static int GetNearestValue(int channel)
        {
            int nearestValue = allowedValues[0];
            int minDifference = Math.Abs(channel - nearestValue);

            foreach (var value in allowedValues)
            {
                int difference = Math.Abs(channel - value);
                if (difference < minDifference)
                {
                    minDifference = difference;
                    nearestValue = value;
                }
            }

            return nearestValue;
        }
    }
}