
namespace GdTest
{


    class MainClass
    {




        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [System.STAThread]
        public static void Main(string[] args)
        {
            Captcha3D.Generate();
            
            string ct = Markov.generateCaptchaTextMarkov(10);
            string ctRand = Markov.generateCaptchaTextRandom(10);
            string ctClean = Markov.generateCaptchaTextMarkovClean(10);
            System.Console.WriteLine(ct);
            System.Console.WriteLine(ctRand);
            System.Console.WriteLine(ctClean);


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
