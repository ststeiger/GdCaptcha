using System;
using System.Collections.Generic;
using System.Text;

namespace GdTest
{
    class libGD3dCaptcha
    {




        public class Rect
        {
            public int p1X;
            public int p1Y;

            public int p2X;
            public int p2Y;


            public Rect(int minX, int minY, int maxX, int maxY)
            {
                p1X = minX;
                p1Y = minY;

                p2X = maxX;
                p2Y = maxY;
            }

            public int Width
            {
                get
                {
                    return System.Math.Abs(p1X - p2X);
                }
            }

            public int Height
            {
                get
                {
                    return System.Math.Abs(p1Y - p2Y);
                }
            }

        } // End Class Rect 


        public static Rect Gettfbbox(Ntx.GD.GD image, string fontFile, int fontsize, double angle, string text)
        {
            System.Collections.ArrayList details = new System.Collections.ArrayList();
            // details = imagettfbbox(fontsize, 0, fontfile, captchaText);
            image.StringFT(details, 1, fontFile, fontsize, angle, 0, 0, text, false);

            int minX = System.Int32.MaxValue;
            int minY = System.Int32.MaxValue;
            int maxX = System.Int32.MinValue;
            int maxY = System.Int32.MinValue;


            foreach (object obj in details)
            {
                var p = (Ntx.GD.Point)obj;

                if (p.X < minX)
                    minX = p.X;

                if (p.X > maxX)
                    maxX = p.X;

                if (p.Y < minY)
                    minY = p.Y;

                if (p.Y > maxY)
                    maxY = p.Y;
            } // Next obj 

            return new Rect(minX, minY, maxX, maxY);
        } // End Sub Gettfbbox 


        public static void Test()
        {
            string captchaText = "hello";
            captchaText = "        Rico Luder     ";
            captchaText = "             Stefan Steiger          ";
            captchaText = "             Rico Luder          ";

            using (Ntx.GD.GD image = new Ntx.GD.GD(256 + 384, 384, true))
            {
                // 3dcha parameters
                int fontsize = 24;

                string fontfile = FileHelper.MapProjectPath("Img/3DCaptcha.ttf");
                // fontfile = image.MapFont("Arial.ttf")

                // details = imagettfbbox(fontsize, 0, fontfile, captchaText);
                Rect details = Gettfbbox(image, fontfile, fontsize, 0, captchaText);


                //var p = (Ntx.GD.Point) details[3];
                // int image2d_x = (int)details[3] + 4;
                // int image2d_x = 110;
                int image2d_x = details.Width;
                // double dblimage2d_y = System.Math.Round( ( fontsize * 1.3f) , 1);
                int image2d_y = (int)(fontsize * 1.3f);
                int bevel = 4;


                // Create 2d image
                // image2d = imagecreatetruecolor(image2d_x, image2d_y);
                using (Ntx.GD.GD image2d = new Ntx.GD.GD(image2d_x, image2d_y, true))
                {
                    Ntx.GD.GDColor black = image2d.ColorAllocate(0, 0, 0);
                    Ntx.GD.GDColor white = image2d.ColorAllocate(255, 255, 255);

                    // Paint 2d image
                    // imagefill(image2d, 0, 0, black);
                    image2d.Fill(0, 0, black);

                    // imagettftext(image2d, fontsize, 0, 2, fontsize, white, fontfile, captchaText);
                    System.Collections.ArrayList dimension = new System.Collections.ArrayList(); // fontfile

                    int halfRestSize = (int)((image2d_y - fontsize) / 2.0);

                    image2d.StringFT(dimension, white, fontfile, fontsize - halfRestSize, 0, 2, (int)fontsize, captchaText, true);
                    // image2d.Save(Ntx.GD.GD.FileType.Png, "/root/Projects/GdCaptcha/GdTest/Img/ftstring.png", 1);


                    // Calculate projection matrix
                    double[] T = MathHelpers.cameraTransform(
                        // new double[] { 45, -200, 220 },
                        new double[] { PhpHelpers.rand(-90, 90), -200, PhpHelpers.rand(150, 250) },
                        new double[] { 0, 0, 0 }
                    );


                    T = MathHelpers.matrixProduct(
                        T,
                        MathHelpers.viewingTransform(60, 300, 3000)
                    );

                    double[][] coord = new double[image2d_x * image2d_y][]; // { image2d_x * image2d_y };
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
                            double yc = -(image2d.GetPixel(x, y).Index & 0xff) / 256.0 * bevel;
                            double[] xyz = new double[] { xc, yc, zc, 1 };
                            xyz = MathHelpers.vectorProduct(xyz, T);

                            coord[count] = xyz;
                            count++;
                        } // Next x 
                    } // Next y


                    // Create 3d image
                    int image3d_x = 256;
                    //image3d_y = image3d_x / 1.618;
                    int image3d_y = image3d_x * 9 / 16;

                    // image3d = imagecreatetruecolor(image3d_x, image3d_y);
                    using (Ntx.GD.GD image3d = new Ntx.GD.GD(image3d_x, image3d_y, true))
                    {
                        // var fgcolor = image3d.ColorAllocate(255, 255, 255);
                        // var bgcolor = image3d.ColorAllocate(0, 0, 0);

                        var bgcolor = image3d.ColorAllocate(255, 255, 255);
                        var fgcolor = image3d.ColorAllocate(0, 0, 255);

                        // imageantialias(image3d, true);
                        //image3d.SetAntiAliased();

                        // imagefill(image3d, 0, 0, bgcolor);
                        image3d.Fill(0, 0, bgcolor);

                        count = 0;
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

                                    double c0 = c[0];
                                    double c1 = c[1];

                                    double x0 = coord[count - 1][0] * scale + image3d_x / 2.0;
                                    double y0 = coord[count - 1][1] * scale + image3d_y / 2.0;
                                    double x1 = coord[count][0] * scale + image3d_x / 2.0;
                                    double y1 = coord[count][1] * scale + image3d_y / 2.0;

                                    // imageline(image3d, x0, y0, x1, y1, fgcolor);
                                    image3d.Line((int)x0, (int)y0, (int)x1, (int)y1, fgcolor);
                                } // End if (x > 0) 

                                count++;
                            } // Next x 

                        } // Next y 

                        if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                            image3d.Save(Ntx.GD.GD.FileType.Png, "/root/Projects/GdCaptcha/GdTest/Img/mesh.png", 1);
                        else
                            image3d.Save(Ntx.GD.GD.FileType.Png, "mesh.png", 1);
                    } // End using image3d 

                } // End Using image2d 

            } // End Using image 

        } // End Sub Test 


    }
}
