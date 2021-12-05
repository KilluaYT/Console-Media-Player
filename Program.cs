using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ConsoleMediaPlayer
{
    internal static class Program
    {
        #region stuff

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        #endregion stuff

        private static void Main(string[] args)
        {
            #region more stuff

            var handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);

        /*
"\x1b[48;5;" + s + "m" - set background color by index in table (0-255)
"\x1b[38;5;" + s + "m" - set foreground color by index in table (0-255)
"\x1b[48;2;" + r + ";" + g + ";" + b + "m" - set background by r,g,b values
"\x1b[38;2;" + r + ";" + g + ";" + b + "m" - set foreground by r,g,b values
*/

        #endregion more stuff

        beginning:
            ConsoleHelper.SetCurrentFont("Consolas", 16);
            Color LightBlue = Color.FromArgb(150, 150, 255);
            Console.Write("\x1b[38;2;" + LightBlue.R + ";" + LightBlue.G + ";" + LightBlue.B + "m\u0000" + "Console Media Player ");
            Console.Write("\x1b[38;2;" + 255 + ";" + 255 + ";" + 255 + "m\u0000" + "v1.0 ");
            Console.Write("\x1b[38;2;" + 50 + ";" + 50 + ";" + 50 + "m\u0000" + "(No Video Playback support yet)\n\n");
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine("Choose Color Mode:\n------------------\n(1) GrayScale\n(2) Colored");

            string s = Console.ReadLine();

            int selection = -1;
            if (s == "1")
            {
                selection = 1;
            }
            else if (s == "2")
            {
                selection = 2;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Invalid Input\n");
                goto beginning;
            }

        restart:
            ConsoleHelper.SetCurrentFont("Consolas", 16);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Pls input image path: ");
            string imgPath = Console.ReadLine();
            string result = Extension.CheckFile(imgPath);
            if (result == "It is not image")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nThis is not an Image");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\nPress any key to try again.");
                Console.ReadKey();
                goto restart;
            }


        quality:
            ConsoleHelper.SetCurrentFont("Consolas", 16);

            Console.WriteLine("Choose Quality (1 = best | 10 = worst):\n------------------");
            short quality=0;
            string str = Console.ReadLine();
            try
            {
                 quality = short.Parse(str);
            }
            catch
            {
                Console.Clear();
                Console.WriteLine("Invalid Input\n");
                goto quality;
            }

            if(quality > 0 && quality < 11)
            {
                
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Invalid Input\n");
                goto quality;
            }

            ConsoleHelper.SetCurrentFont("Consolas", quality);
            Bitmap bm = new Bitmap(imgPath);
            if (selection == 1)
            {
                bm = GrayScaleBitmap(bm);
            }

            if (bm.Height > Console.LargestWindowHeight)
            {
                double rescaleFactor = bm.Height / Console.LargestWindowHeight;
                bm = ResizeBitmap(bm, Convert.ToInt32(bm.Width / rescaleFactor), Console.LargestWindowHeight);
            }
            Console.SetBufferSize(2000, 2000);
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            int w = 0;
            int h = 0;
            Point p;
            string line = "";
            string image = "";
            Console.Clear();

            Console.OutputEncoding = System.Text.Encoding.Unicode;

            while (h < bm.Height)
            {
                w = 0;
                p = new Point(w, h);
                while (w < bm.Width)
                {
                    Color tempColor;
                    tempColor = bm.GetPixel(w, h);

                    line = line + "\x1b[38;2;" + tempColor.R + ";" + tempColor.G + ";" + tempColor.B + "m\u2588" + "\u2588";

                    w++;
                    p.Offset(1, 0);
                }
                image = image + line + "\n";
                line = "";
                h++;
            }
            Console.Write(image);
            bm.Dispose();
          
            Console.ReadKey();
            goto restart;
        }

        public static Bitmap GrayScaleBitmap(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                //create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                   new float[][]
                   {
             new float[] {.3f, .3f, .3f, 0, 0},
             new float[] {.59f, .59f, .59f, 0, 0},
             new float[] {.11f, .11f, .11f, 0, 0},
             new float[] {0, 0, 0, 1, 0},
             new float[] {0, 0, 0, 0, 1}
                   });

                //create some image attributes
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    //set the color matrix attribute
                    attributes.SetColorMatrix(colorMatrix);

                    //draw the original image on the new image
                    //using the grayscale color matrix
                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
        }

        public static Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }
    }
}