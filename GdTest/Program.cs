
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
            } // End Using img2 

        } // End Sub wave_area 


        public static System.Drawing.Font GetCustomFont(string fontFile, float fontSize)
        {
            System.Drawing.Font font = null;

            System.Drawing.Text.PrivateFontCollection collection = new System.Drawing.Text.PrivateFontCollection();
            collection.AddFontFile(fontFile);

            foreach (System.Drawing.FontFamily fontFamily in collection.Families)
            {
                // ffontFamily = new System.Drawing.FontFamily(x.Name, collection);
                font = new System.Drawing.Font(fontFamily, fontSize, System.Drawing.FontStyle.Bold);
                break;
            } // Next fontFamily 

            return font;
        } // End Function GetCustomFont 


        public static void Captcha3d()
        {
            string fontFamily = "Arial";
            fontFamily = "Algerian";
            float fontSize = 20f;
            string captchaText = Markov.generateCaptchaTextMarkovClean(5);
            // captchaText = "Stefan";
            captchaText = "test123";
            captchaText = "Stefan Steiger";

            int bevel = 4;
            double[][] coord = null;
            int image2d_x = 0;
            int image2d_y = 0;


            string fontFile =@"/root/Projects/GdCaptcha/GdTest/Img/3DCaptcha.ttf";
            fontFile =@"/root/Downloads/ufonts.com_algerian.ttf";

            System.Drawing.Font font = GetCustomFont(fontFile, fontSize);;

            using (System.Drawing.Font fontt = new System.Drawing.Font(fontFamily, fontSize, System.Drawing.FontStyle.Bold))
            {
                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(1, 1))
                {
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        System.Drawing.SizeF size = g.MeasureString(captchaText, font, new System.Drawing.PointF(10, 10), System.Drawing.StringFormat.GenericTypographic);
                        image2d_x = (int)size.Width + 5;
                        image2d_y = (int)size.Height;
                    }

                }

                using (System.Drawing.Bitmap image2d = new System.Drawing.Bitmap(image2d_x, image2d_y))
                {
                
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image2d))
                    {
                        g.Clear(System.Drawing.Color.Black);
                        g.DrawString(captchaText, font, System.Drawing.Brushes.White, new System.Drawing.PointF(1, -1));
                        image2d.Save("/root/Projects/GdCaptcha/GdTest/Img/ftstring.png", System.Drawing.Imaging.ImageFormat.Png);

                        // Calculate projection matrix
                        double[] T = Helpers.cameraTransform(
                        // new double[] { 45, -200, 220 },
                                         new double[] { Helpers.rand(-90, 90), -200, Helpers.rand(150, 250) },
                                         new double[] { 0, 0, 0 }
                                     );


                        T = Helpers.matrixProduct(
                            T,
                            Helpers.viewingTransform(60, 300, 3000)
                        );

                        coord = new double[image2d_x * image2d_y][]; // { image2d_x * image2d_y };
                        // Calculate coordinates

                        int count = 0;
                        for (int y = 0; y < image2d_y; y += 2)
                        {
                            for (int x = 0; x < image2d_x; x++)
                            {

                                // calculate x1, y1, x2, y2
                                double xc = x - image2d_x / 2.0;
                                //double zc = y - dblimage2d_y / 2.0;
                                double zc = y - image2d_y / 2.0;

                                //yc = -(imagecolorat(image2d, x, y) & 0xff) / 256 * bevel;
                                double yc = -(image2d.GetPixel(x, y).ToArgb() & 0xff) / 256.0 * bevel;
                                double[] xyz = new double[] { xc, yc, zc, 1 };
                                xyz = Helpers.vectorProduct(xyz, T);

                                coord[count] = xyz;
                                count++;
                            } // Next x 

                        } // Next y

                    } // End Using g

                } // End Using image2d

            } // End Using font



            // Create 3d image
            int image3d_x = 256;
            //image3d_y = image3d_x / 1.618;
            int image3d_y = image3d_x * 9 / 16;

            // image3d = imagecreatetruecolor(image3d_x, image3d_y);
            using (System.Drawing.Bitmap image3d = new System.Drawing.Bitmap(image3d_x, image3d_y))
            {
                //var bgcolor = image3d.ColorAllocate(255, 255, 255);
                //var fgcolor = image3d.ColorAllocate(0, 0, 255);

                // imageantialias(image3d, true);
                //image3d.SetAntiAliased();

                // imagefill(image3d, 0, 0, bgcolor);

                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image3d))
                {
                    g.Clear(System.Drawing.Color.White);
                    int count = 0;
                    double scale = 1.75 - image2d_x / 400.0;

                    for (int y = 0; y < image2d_y; y++)
                    {
                        for (int x = 0; x < image2d_x; x++)
                        {
                            if (x > 0)
                            {
                                double[] c = coord[count - 1];
                                if (c == null)
                                    continue;
                                
                                double x0 = coord[count - 1][0] * scale + image3d_x / 2.0;
                                double y0 = coord[count - 1][1] * scale + image3d_y / 2.0;
                                double x1 = coord[count][0] * scale + image3d_x / 2.0;
                                double y1 = coord[count][1] * scale + image3d_y / 2.0;

                                // imageline(image3d, x0, y0, x1, y1, fgcolor);
                                g.DrawLine(System.Drawing.Pens.Blue, (int)x0, (int)y0, (int)x1, (int)y1);
                            } // End if (x > 0) 

                            count++;
                        } // Next x 

                    } // Next y 

                } // End Using g

                string fileName = "mesh.png";
                if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                    fileName = "/root/Projects/GdCaptcha/GdTest/Img/mesh.png";

                image3d.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            } // End using image3d 

        }



        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [System.STAThread]
        public static void Main(string[] args)
        {
            Captcha3d();
            System.Console.WriteLine("test");
            return;

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
