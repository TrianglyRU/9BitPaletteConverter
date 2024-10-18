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

            string folderPath = args.Length > 0 ? args[0] : PromptForFolderPath();

            while (!Directory.Exists(folderPath) || !IsSpritesFolder(folderPath))
            {
                Console.WriteLine("Invalid folder path or folder is not named 'sprites'. Please try again.");
                folderPath = PromptForFolderPath();
            }

            ProcessImages(folderPath);
            Console.WriteLine("Processing completed. Press Enter to exit...");
            Console.ReadLine();
        }

        static string PromptForFolderPath()
        {
            Console.WriteLine("Please enter the path to the folder with images or drag the folder here:");
            return Console.ReadLine();
        }

        static bool IsSpritesFolder(string folderPath)
        {
            return Path.GetFileName(folderPath).Equals("sprites", StringComparison.OrdinalIgnoreCase);
        }

        static void ProcessImages(string directory)
        {
            var imageFiles = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            Parallel.ForEach(imageFiles, filePath =>
            {
                if (IsImageFile(filePath))
                {
                    ProcessImage(filePath);
                }
            });
        }

        static bool IsImageFile(string filePath) =>
            filePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
            filePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
            filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
            filePath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase);

        static void ProcessImage(string filePath)
        {
            try
            {
                string modifiedFilePath = GetModifiedFilePath(filePath);

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
                        Console.WriteLine($"Processed image: {modifiedFilePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
            }
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

        static string GetModifiedFilePath(string originalPath)
        {
            string modifiedPath = originalPath.Replace("sprites", "sprites_modified");
            string modifiedDirectory = Path.GetDirectoryName(modifiedPath);

            if (!Directory.Exists(modifiedDirectory))
            {
                Directory.CreateDirectory(modifiedDirectory);
            }

            return modifiedPath;
        }

        static int GetNearestValue(int channel)
        {
            if (channel % 32 == 0)
            {
                return channel;
            }

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