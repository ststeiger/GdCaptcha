
namespace GdTest
{


    class Captcha3D
    {


        private static System.Drawing.Font GetCustomFont(string fontFile, float fontSize)
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

            if (font == null)
                throw new System.InvalidOperationException("fontFile (\"" + fontFile + "\") doesn't contain a font-family.");

            return font;
        } // End Function GetCustomFont 


        public static byte[] Generate()
        {
            string captchaText = Markov.generateCaptchaTextMarkovClean(5);
            //captchaText = "A quick brown fox jumped over the lazy dog.";
            captchaText = "The quick brown fox jumps over the  lazy dog";
            // captchaText = "test123";
            
            byte[] imageBytes = Generate(captchaText, System.Drawing.Imaging.ImageFormat.Png);
            System.IO.File.WriteAllBytes(FileHelper.MapProjectPath("Img/mesh.png"), imageBytes);
            
            return imageBytes;
        }


        public static byte[] Generate(string captchaText, System.Drawing.Imaging.ImageFormat format)
        {
            byte[] imageBytes = null;

            float fontSize = 20f;
            string fontFamily = "Arial";
            fontFamily = "Algerian";
            

            double bevel = 3;
            double[][] coord = null;
            int image2d_x = 0;
            int image2d_y = 0;


            string fontFile = FileHelper.MapProjectPath("Img/3DCaptcha.ttf");
            // fontFile = Helpers.MapProjectPath("Img/ALGER.ttf");

            System.Drawing.Font font = GetCustomFont(fontFile, fontSize); ;

            using (System.Drawing.Font fontt = new System.Drawing.Font(fontFamily, fontSize, System.Drawing.FontStyle.Bold))
            {
                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(1, 1))
                {
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        System.Drawing.SizeF size = g.MeasureString(captchaText, font, new System.Drawing.PointF(10, 10), System.Drawing.StringFormat.GenericTypographic);
                        image2d_x = (int)(size.Width * 1.15) + 3;
                        image2d_y = (int)(size.Height);
                    } // End Using g 

                } // End Using bmp 

                using (System.Drawing.Bitmap image2d = new System.Drawing.Bitmap(image2d_x, image2d_y))
                {

                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image2d))
                    {
                        g.Clear(System.Drawing.Color.Black);
                        g.DrawString(captchaText, font, System.Drawing.Brushes.White, new System.Drawing.PointF(1, -1));
                        image2d.Save(FileHelper.MapProjectPath("Img/ftstring.png"), System.Drawing.Imaging.ImageFormat.Png);

                        // Calculate projection matrix
                        double[] T = MathHelpers.cameraTransform(
                                         new double[] { 0, -200, 250 },
                            // new double[] { Helpers.rand(-90, 90), -200, Helpers.rand(150, 250) },
                                         new double[] { 0, 0, 0 }
                                     );


                        T = MathHelpers.matrixProduct(
                            T,
                            MathHelpers.viewingTransform(60, 300, 3000)
                            //Helpers.viewingTransform(90, 300, 900000000000)
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
                                xyz = MathHelpers.vectorProduct(xyz, T);

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

            image3d_x = 256 * 4;
            image3d_y = (int)(image3d_x * 0.5);



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
                                if (coord[count - 1] == null)
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


                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    image3d.Save(ms, format);
                    imageBytes = ms.ToArray();
                } // End Using ms 

            } // End using image3d 

            return imageBytes;
        } // End Function Generate 


    } // End Class Captcha3D


} // End Namespace GdTest
