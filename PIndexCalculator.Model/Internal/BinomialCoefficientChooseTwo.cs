using PICalculator.Model.Exceptions;
using System;

namespace PICalculator.Model.Internal
{
    // a table of binomial coefficients
    static class BinomialCoefficientChooseTwo
    {
        public const int MinN = 2;
        public const int MaxN = 60;

        public static int Of(int n) {
            CheckValueInRange(n);
            return coefficients[n];
        }

        private static void CheckValueInRange(int n) {
            if (n < MinN)
                throw new BusinessException("n must be greater than 1");

            if (n > MaxN)
                throw new BusinessException("n must be less than 61");
        }

        private static int[] coefficients = new int[61]
        {
               0,  
               0,    1,    3,    6,   10,   15,   21,   28,   36,   45, 
              55,   66,   78,   91,  105,  120,  136,  153,  171,  190, 
             210,  231,  253,  276,  300,  325,  351,  378,  406,  435,
             465,  496,  528,  561,  595,  630,  666,  703,  741,  780,
             820,  861,  903,  946,  990, 1035, 1081, 1128, 1176, 1225,
            1275, 1326, 1378, 1431, 1485, 1540, 1596, 1653, 1711, 1770 
        };
    }
}
