using System;
using System.Collections.Generic;
using System.Text;

namespace GdTest
{
    class PhpHelpers
    {



        private static System.Random seed = new System.Random();

        public static double rand(int min, int max)
        {
            return seed.Next(min, max + 1);
        } // End Function rand 


    }
}
