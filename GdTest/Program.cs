
using Ntx.GD;


namespace GdTest
{


    class MainClass
    {


        // http://www.sitepoint.com/forums/showthread.php?520206-Warp-Text-in-PHP-GD
        public static void wave_area(GD img)
        {
            int width = 256 + 384;
            int height = 384;
            double amplitude = 10;
            double period = 10;
            int x = 0;
            int y = 0;

            int width2 = width * 2;
            int height2 = height * 2;

            using (GD img2 = new GD(width, height, true))
            {
                img2.CopyResampled(img, 0, 0, 0, 0, width2, height2, width, height);


                for(int i = 0; i < width2; i += 2)
                    img2.Copy(img2, x + i - 2, (int)(y + System.Math.Sin((double)i / period) * amplitude), x + i, y, 2, height2);

                img.CopyResampled(img2, x, y, 0, 0, width, height, width2, height2);
            }

        }


        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [System.STAThread]
        public static void Main(string[] args)
        {
            PlatformHelper.GetDll();

            string ct = Markov.generateCaptchaTextMarkov(10);
            string ctRand = Markov.generateCaptchaTextRandom(10);
            string ctClean = Markov.generateCaptchaTextMarkovClean(10);
            System.Console.WriteLine(ct);
            System.Console.WriteLine(ctRand);
            System.Console.WriteLine(ctClean);
            

            GdTest.Helpers.Test();

            if (false)
            {
                using (GD image = new GD(256 + 384, 384, true))
                {
                    GDColor white = image.ColorAllocate(255, 255, 255);
                    image.FilledRectangle(0, 0, 256 + 384, 384, white);


                    image.Save(GD.FileType.Png, "text.png", 1);
                } // End Using image 
            }

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }


        public static void Test()
        {
            // Ntx.GD.Tests.Test();

            using (Ntx.GD.GD image = new GD(256 + 384, 384, true))
            {

                using (Font ft = new Font(Font.Type.Large))
                {
                    GDColor red = image.ColorAllocate(255, 0, 0);
                    GDColor blue = image.ColorAllocate(0, 0, 255);
                    GDColor white = image.ColorAllocate(255, 255, 255);
                    image.FilledRectangle(0, 0, 256 + 384, 384, white);


                    // image.SetPixel(10, 10, blue);
                    // GDColor pixCol = image.GetPixel(10, 10);
                    image.Arc( 128, 128, 60, 20, 0, 720, blue );

                    image.SetAntiAliased( red );
                    // image.String(ft, 10, 10, "Hello", red);

                    System.Collections.ArrayList al = new System.Collections.ArrayList();
                    image.StringFT(al, 1, image.MapFont("Arial.ttf"), 72, 0, 10, 100, "test", true);
                }


                wave_area(image);

                image.Interlace = true;
                image.Save( GD.FileType.Png, "text.png", 1 );
            }



            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }


        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [System.STAThread]
        static void OldMain()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Form1());
        }

    }
}
