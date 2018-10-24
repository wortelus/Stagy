using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stagy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("input: ");
            MainInstance instance = new MainInstance(Console.ReadLine());
            Console.ReadLine();
        }
    }

    public class MainInstance
    {
        string rawInput;
        const int height = 1;
        byte[] textEncodedArray;
        int pixelCount;

        byte[] bluePixel;
        byte[] greenPixel;
        byte[] redPixel;

        public MainInstance(string input)
        {
            rawInput = input;
            try
            {
                textEncodedArray = Encoding.ASCII.GetBytes(input);
                for (int i = 0; i < rawInput.Length; i++)
                {
                    if ((int)rawInput[i] > 127)
                    {
                        Console.WriteLine("Non-ASCII character found.. encoding is being changed to UTF-8");
                        ChangeToUTF8();
                    }
                }
            }
            catch
            {
                ChangeToUTF8();
            }

            if (textEncodedArray.Length % 3 == 0)
            {
                pixelCount = textEncodedArray.Length / 3;
            }
            else
            {
                pixelCount = (textEncodedArray.Length / 3) + 1;
            }

            bluePixel = new byte[pixelCount];
            greenPixel = new byte[pixelCount];
            redPixel = new byte[pixelCount];

            for(int i = 0; i < textEncodedArray.Length; i++)
            {
                switch(i % 3)
                {
                    case 0:
                        redPixel[i / 3] = textEncodedArray[i];
                        break;
                    case 1:
                        greenPixel[i / 3] = textEncodedArray[i];
                        break;
                    case 2:
                        bluePixel[i / 3] = textEncodedArray[i];
                        break;
                }
            }

            Bitmap bitmap = new Bitmap(pixelCount, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Rectangle rectangle = new Rectangle(0, 0, pixelCount, height);
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.WriteOnly, bitmap.PixelFormat);

            unsafe
            {
                byte* scan = (byte*)bitmapData.Scan0;
                for(int i = 0; i < pixelCount; i++)
                {
                    scan[2] = bluePixel[i];
                    scan[1] = greenPixel[i];
                    scan[0] = redPixel[i];
                    scan += 3;
                }
            }
            bitmap.UnlockBits(bitmapData);
            Console.Write("Enter an output name: ");
            string name = Console.ReadLine();
            if (name == string.Empty)
            {
                name = "output[" + new Random().Next(1000000, 9999999) + "]";
                Console.WriteLine("Entered an empty name, setting to " + name);
            }
            else if(name.Contains(".bmp") == false)
            {
                name += ".bmp";
            }   
            
            try
            {
                bitmap.Save(name, ImageFormat.Bmp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception during saving occured: " + ex.Message);
            }

        }

        private void ChangeToUTF8()
        {
            
            textEncodedArray = Encoding.UTF8.GetBytes(rawInput);
        }
    }
}
