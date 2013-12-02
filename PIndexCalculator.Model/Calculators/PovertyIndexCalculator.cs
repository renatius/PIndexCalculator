using System;
using System.Collections.Generic;
using System.Linq;

namespace PICalculator.Model.Calculators
{
    public class PovertyIndexCalculator
    {
        private double alpha;

        public PovertyIndexCalculator(double alpha) {
            this.alpha = alpha;
        }

        public double Calculate(double sequenceEffect, double emergencyEffect) {
            return (alpha * sequenceEffect) + ((1.0 - alpha) * emergencyEffect);
        }
    }
}
