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

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (args.Length == 0)
            {
                Console.WriteLine("Drop files or a folder onto the .exe to convert");
                Task.Delay(1000).Wait();
                Environment.Exit(0);
                return;
            }

            bool anyValid = false;

            foreach (string path in args)
            {
                if (Directory.Exists(path))
                {
                    anyValid = true;
                    ProcessFolder(path, path + "_converted");
                }
                else if (File.Exists(path))
                {
                    if (IsImageFile(path))
                    {
                        anyValid = true;
                        ProcessSingleFile(path);
                    }
                    else
                    {
                        Console.WriteLine($"Skipped (not an image): {path}");
                    }
                }
                else
                {
                    Console.WriteLine($"Not found: {path}");
                }
            }

            if (!anyValid)
            {
                Console.WriteLine("No valid files or folders found");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("\nDone!");
                Task.Delay(1000).Wait();
                Environment.Exit(0);
            }
        }

        static void ProcessFolder(string inputRoot, string outputRoot)
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

        static void ProcessSingleFile(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath);
            string nameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            string ext = Path.GetExtension(filePath);
            string outputPath = Path.Combine(dir, nameWithoutExt + "_converted" + ext);
            ProcessImage(filePath, outputPath);
        }

        static void ProcessImage(string filePath, string inputRoot, string outputRoot)
        {
            string outputPath = GetModifiedFilePath(filePath, inputRoot, outputRoot);
            ProcessImage(filePath, outputPath);
        }

        static void ProcessImage(string filePath, string outputPath)
        {
            try
            {
                using (Bitmap originalBitmap = new Bitmap(filePath))
                {
                    Bitmap bitmapToProcess = originalBitmap;

                    if (IsIndexedPixelFormat(originalBitmap.PixelFormat))
                    {
                        bitmapToProcess = ConvertToNonIndexed(originalBitmap);
                    }

                    using (bitmapToProcess)
                    {
                        int max = GetMaxChannel(bitmapToProcess);
                        double scale = (max == 224 || max == 238) ? 255.0 / max : 1.0;

                        for (int y = 0; y < bitmapToProcess.Height; y++)
                        {
                            for (int x = 0; x < bitmapToProcess.Width; x++)
                            {
                                Color originalColor = bitmapToProcess.GetPixel(x, y);
                                int r = GetNearestValue((int)Math.Round(originalColor.R * scale));
                                int g = GetNearestValue((int)Math.Round(originalColor.G * scale));
                                int b = GetNearestValue((int)Math.Round(originalColor.B * scale));
                                bitmapToProcess.SetPixel(x, y, Color.FromArgb(originalColor.A, r, g, b));
                            }
                        }

                        bitmapToProcess.Save(outputPath);
                        Console.WriteLine($"Converted {outputPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting file {filePath}: {ex.Message}");
            }
        }

        static int GetMaxChannel(Bitmap bitmap)
        {
            int max = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    if (c.R > max) max = c.R;
                    if (c.G > max) max = c.G;
                    if (c.B > max) max = c.B;
                }
            }
            return max;
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

        static bool IsImageFile(string filePath) =>
            filePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
            filePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
            filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
            filePath.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
            filePath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase);

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