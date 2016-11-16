
namespace GdTest
{


    class libGDTests
    {


        public static void TestDeploy()
        {
            PlatformHelper.GetDll();
            libGD3dCaptcha.Test();


            if (false)
            {
                using (Ntx.GD.GD image = new Ntx.GD.GD(256 + 384, 384, true))
                {
                    Ntx.GD.GDColor white = image.ColorAllocate(255, 255, 255);
                    image.FilledRectangle(0, 0, 256 + 384, 384, white);


                    image.Save(Ntx.GD.GD.FileType.Png, "text.png", 1);
                } // End Using image 
            }
        }


        // http://www.sitepoint.com/forums/showthread.php?520206-Warp-Text-in-PHP-GD
        public static void wave_area(Ntx.GD.GD img)
        {
            int width = 256 + 384;
            int height = 384;
            double amplitude = 10;
            double period = 10;
            int x = 0;
            int y = 0;

            int width2 = width * 2;
            int height2 = height * 2;

            using (Ntx.GD.GD img2 = new Ntx.GD.GD(width, height, true))
            {
                img2.CopyResampled(img, 0, 0, 0, 0, width2, height2, width, height);

                for (int i = 0; i < width2; i += 2)
                    img2.Copy(img2, x + i - 2, (int)(y + System.Math.Sin((double)i / period) * amplitude), x + i, y, 2, height2);

                img.CopyResampled(img2, x, y, 0, 0, width, height, width2, height2);
            } // End Using img2 

        } // End Sub wave_area 


        public static void SimpleTest()
        {
            // Ntx.GD.Tests.Test();

            using (Ntx.GD.GD image = new Ntx.GD.GD(256 + 384, 384, true))
            {

                using (Ntx.GD.Font ft = new Ntx.GD.Font(Ntx.GD.Font.Type.Large))
                {
                    Ntx.GD.GDColor red = image.ColorAllocate(255, 0, 0);
                    Ntx.GD.GDColor blue = image.ColorAllocate(0, 0, 255);
                    Ntx.GD.GDColor white = image.ColorAllocate(255, 255, 255);
                    image.FilledRectangle(0, 0, 256 + 384, 384, white);


                    // image.SetPixel(10, 10, blue);
                    // GDColor pixCol = image.GetPixel(10, 10);
                    image.Arc(128, 128, 60, 20, 0, 720, blue);

                    image.SetAntiAliased(red);
                    // image.String(ft, 10, 10, "Hello", red);

                    System.Collections.ArrayList al = new System.Collections.ArrayList();
                    image.StringFT(al, 1, image.MapFont("Arial.ttf"), 72, 0, 10, 100, "test", true);
                } // End using ft

                wave_area(image);

                image.Interlace = true;
                image.Save(Ntx.GD.GD.FileType.Png, "text.png", 1);
            } // End using image 



            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub SimpleTest 


    } // End Class libGDTests


} // End Namespace GdTest 
