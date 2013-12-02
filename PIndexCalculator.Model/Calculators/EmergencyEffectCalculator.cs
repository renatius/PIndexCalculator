using PIndexCalculator.Model.Exceptions;
using PIndexCalculator.Model.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PIndexCalculator.Model.Calculators
{
    public class EmergencyEffectCalculator
    {
        private int minYear;
        private int maxYear;
        private int waveCount;
        private double denominator;

        public EmergencyEffectCalculator
        (
            int minYear,
            int maxYear
        ) {
            Precondition.Require(maxYear > minYear,
                "Max year must be greater than min year");

            this.minYear = minYear;
            this.maxYear = maxYear;

            waveCount = (maxYear - minYear) + 1;
            denominator = (waveCount * (waveCount + 1)) / 2.0;
        }

        public double CalculateFor(Person p) {
            return CalculateNumeratorFor(p) / denominator;
        }

        private double CalculateNumeratorFor(Person p) {
            int result = 0;

            foreach (int year in p.GetPovertyYears()) {
                result += (year - minYear) + 1;
            }

            return (double)result;
        }

    }
}
