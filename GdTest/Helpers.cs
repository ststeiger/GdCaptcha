
namespace GdTest 
{


    class Helpers 
    {


        public static double[] addVector(double[] a, double[] b)
        {
            return new double[] { a[0] + b[0], a[1] + b[1], a[2] + b[2] };
        } // End Function addVector 


        public static double[] scalarProduct(double[] vector, double scalar)
        {
            return new double[] { vector[0] * scalar, vector[1] * scalar, vector[2] * scalar };
        } // End Function scalarProduct 


        public static double dotProduct(double[] a, double[] b)
        {
            return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
        } // End Function dotProduct 


        public static double norm(double[] vector)
        {
            return System.Math.Sqrt(dotProduct(vector, vector));
        } // End Function norm 


        public static double[] normalize(double[] vector)
        {
            return scalarProduct(vector, 1 / norm(vector));
        } // End Function normalize 


        // http://en.wikipedia.org/wiki/Cross_product
        public static double[] crossProduct(double[] a, double[] b)
        {
            return new double[]{
		        (a[1] * b[2] - a[2] * b[1]),
		        (a[2] * b[0] - a[0] * b[2]),
		        (a[0] * b[1] - a[1] * b[0])
		    };
        } // End Function crossProduct 


        public static double[] vectorProductIndexed(double[] v, double[] m, int i)
        {
            return new double[]{
		        v[i + 0] * m[0] + v[i + 1] * m[4] + v[i + 2] * m[8] + v[i + 3] * m[12],
		        v[i + 0] * m[1] + v[i + 1] * m[5] + v[i + 2] * m[9] + v[i + 3] * m[13],
		        v[i + 0] * m[2] + v[i + 1] * m[6] + v[i + 2] * m[10]+ v[i + 3] * m[14],
		        v[i + 0] * m[3] + v[i + 1] * m[7] + v[i + 2] * m[11]+ v[i + 3] * m[15]
            };
        } // End Function vectorProductIndexed 


        public static double[] vectorProduct(double[] v, double[] m)
        {
            return vectorProductIndexed(v, m, 0);
        } // End Function vectorProduct 


        public static double[] matrixProduct(double[] a, double[] b)
        {
            double[] o1 = vectorProductIndexed(a, b, 0);
            double[] o2 = vectorProductIndexed(a, b, 4);
            double[] o3 = vectorProductIndexed(a, b, 8);
            double[] o4 = vectorProductIndexed(a, b, 12);

            return new double[]{
		        o1[0], o1[1], o1[2], o1[3],
		        o2[0], o2[1], o2[2], o2[3],
		        o3[0], o3[1], o3[2], o3[3],
		        o4[0], o4[1], o4[2], o4[3]
            };
        } // End Function matrixProduct 


        // http://graphics.idav.ucdavis.edu/education/GraphicsNotes/Camera-Transform/Camera-Transform.html
        public static double[] cameraTransform(double[] C, double[] A)
        {
            double[] w = normalize(addVector(C, scalarProduct(A, -1)));
            double[] y = new double[] { 0, 1, 0 };
            double[] u = normalize(crossProduct(y, w));
            double[] v = crossProduct(w, u);
            double[] t = scalarProduct(C, -1);

            return new double[]{
		        u[0], v[0], w[0], 0,
		        u[1], v[1], w[1], 0,
		        u[2], v[2], w[2], 0,
		        dotProduct(u, t), dotProduct(v, t), dotProduct(w, t), 1
            };
        } // End Function cameraTransform 


        // http://graphics.idav.ucdavis.edu/education/GraphicsNotes/Viewing-Transformation/Viewing-Transformation.html
        public static double[] viewingTransform(double fov, double n, double f)
        {
            fov *= (System.Math.PI / 180.0);
            double cot = 1 / System.Math.Tan(fov / 2);

            return new double[]{
		        cot,	0,		0,		0,
		        0,		cot,	0,		0, 
		        0,		0,		(f + n) / (f - n),		-1,
		        0,		0,		2 * f * n / (f - n),	0
            };
        } // End Function viewingTransform 


        private static System.Random seed = new System.Random();

        public static double rand(int min, int max)
        {
            return seed.Next(min, max + 1);
        } // End Function rand 


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

            int minX=System.Int32.MaxValue;
            int minY = System.Int32.MaxValue;
            int maxX = System.Int32.MinValue;
            int maxY = System.Int32.MinValue;
            

            foreach (object obj in details)
            {
                var p = (Ntx.GD.Point) obj;

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
            captchaText = "abc123";

            using (Ntx.GD.GD image = new Ntx.GD.GD(256 + 384, 384, true))
            {
                // 3dcha parameters
                int fontsize = 24;

                string fontfile = "/root/Projects/GdCaptcha/GdTest/Img/3DCaptcha.ttf";
                // fontfile = image.MapFont("Arial.ttf")
                
                // details = imagettfbbox(fontsize, 0, fontfile, captchaText);
                Rect details = Gettfbbox(image, fontfile, fontsize, 0, captchaText);


                //var p = (Ntx.GD.Point) details[3];
                // int image2d_x = (int)details[3] + 4;
                // int image2d_x = details.Width;
                int image2d_x = 110;
                double dblimage2d_y = System.Math.Round( ( fontsize * 1.3f) , 1);
                int image2d_y = (int)( fontsize * 1.3f);
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
                    double[] T = cameraTransform(
                        //new double[] { rand(-90, 90), -200, rand(150, 250) },
                        new double[] { 45, -200, 220 },
                        new double[] { 0, 0, 0 }
                    );


                    T = matrixProduct(
                        T,
                        viewingTransform(60, 300, 3000)
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
                            double zc = y - dblimage2d_y / 2.0;

                            //yc = -(imagecolorat(image2d, x, y) & 0xff) / 256 * bevel;
                            double yc = -(image2d.GetPixel(x, y).Index & 0xff) / 256.0 * bevel;
                            double[] xyz = new double[] { xc, yc, zc, 1 };
                            xyz = vectorProduct(xyz, T);

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
                        var fgcolor = image3d.ColorAllocate(255, 255, 255);
                        var bgcolor = image3d.ColorAllocate(0, 0, 0);

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
                                    var c = coord[count - 1];
                                    if (c == null)
                                        continue;

                                    var c0 = c[0];
                                    var c1 = c[1];

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

                        if(System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                            image3d.Save(Ntx.GD.GD.FileType.Png, "/root/Projects/GdCaptcha/GdTest/Img/mesh.png", 1);
                        else
                            image3d.Save(Ntx.GD.GD.FileType.Png, "mesh.png", 1);
                    } // End using image3d 

                } // End Using image2d

            } // End Using image 

        } // End Sub Test 


    } // End Class Helpers 


} // End Namespace GdTest 
