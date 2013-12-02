using PIndex.Model.Internal;
using PIndexCalculator.Model.Exceptions;
using PIndexCalculator.Model.Input;
using PIndexCalculator.Model.Internal;
using PIndexCalculator.Model.Output;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PIndexCalculator.Model.Calculators
{
    public class SequenceEffectCalculator
    {
        private class Numerator
        {
            public Sum S1 { get; private set; }
            public Sum S2 { get; private set; }
            public Sum S3 { get; private set; }
            public Sum S4 { get; private set; }
            public Sum S5 { get; private set; }

            public static readonly Numerator Zero = Create(0.0);

            public static Numerator Create(double term) {
                var result = new Numerator();
                result.S1.Terms.Add(term);
                result.S2.Terms.Add(term);
                result.S3.Terms.Add(term);
                result.S4.Terms.Add(term);
                result.S5.Terms.Add(term);
                return result;
            }

            public Numerator() {
                S1 = new Sum();
                S2 = new Sum();
                S3 = new Sum();
                S4 = new Sum();
                S5 = new Sum();
            }
        }

        private readonly int minYear;
        private readonly int maxYear;
        private readonly int waveCount;
        private readonly double denominator1;
        private readonly double denominator2;
        private readonly IEnumerable<PovertyPersistenceRatio> povertyPersistenceRatios;

        public SequenceEffectCalculator
        (
            int minYear,
            int maxYear,
            IEnumerable<PovertyPersistenceRatio> povertyPersistenceRatios
        ) {
            Precondition.Require(maxYear > minYear,
                "Max year must be greater than min year");

            Precondition.Require(povertyPersistenceRatios != null,
                "the set of poverty persistence ratios cannot be null");

            this.minYear = minYear;
            this.maxYear = maxYear;

            waveCount = (maxYear - minYear) + 1;

            denominator1 = BinomialCoefficientChooseTwo.Of(waveCount);
            denominator2 = CalculateDenominator2(waveCount);

            this.povertyPersistenceRatios = povertyPersistenceRatios;
        }

        private static double CalculateDenominator2(int waveCount) {
            var sum = new Sum();

            for (int i = 1; i < waveCount; i++) {
                double a = i;
                double b = waveCount - i + 1;
                sum.Terms.Add(a / b);
            }

            return sum.Value;
        }

        public SequenceEffect CalculateFor(Person p) {
            var result = new SequenceEffect();

            var numerator = CalculateNumeratorFor(p);

            result.V1 = numerator.S1.Value / denominator1;
            result.V2 = numerator.S2.Value / denominator1;
            result.V3 = numerator.S3.Value / denominator2;
            result.V4 = numerator.S4.Value / denominator2;
            result.V5 = numerator.S5.Value / denominator2;

            return result;
        }

        private double GetPovertyPersistenceRatioValue(string country, int lowYear, int highYear) {
            var x = (from r in povertyPersistenceRatios
                     where r.Country == country
                     where r.LowYear == lowYear
                     where r.HighYear == highYear
                     select r.PermanenceProbability).SingleOrDefault();

            return x;
        }

        private Numerator CalculateNumeratorFor(Person p) {
            var povertyYears = p.GetPovertyYears();

            int numPovertyYears = povertyYears.Count;

            if (numPovertyYears == 0)
                return Numerator.Zero; ;

            if (numPovertyYears == 1)
                return Numerator.Create(p.GetPovertyGapForYear(povertyYears[0]));

            var result = new Numerator();

            for (int n = 0; n < numPovertyYears - 1; n++) {
                for (int m = n + 1; m < numPovertyYears; m++) {
                    int lowYear = povertyYears[n];
                    int highYear = povertyYears[m];
                    int i = (highYear - minYear) + 1;
                    int j = (lowYear - minYear) + 1;

                    double Oij = p.GetNonPovertyYearsBetween(lowYear, highYear);
                    double Wij = p.GetPovertyGapAverage(lowYear, highYear);
                    double Pij = GetPovertyPersistenceRatioValue(p.Country, lowYear, highYear);

                    // double term = (Math.Pow((i - j + 1), -(Pij * (Oij + 1)))) * Wij;

                    result.S1.Terms.Add(Wij * Math.Pow((i - j + 1), -(Pij * (Oij + 1))));
                    result.S2.Terms.Add(Math.Pow((i - j + 1), -(Pij * (Oij + 1))));
                    result.S3.Terms.Add(Wij * Math.Pow((i - j + 1), -(Oij + 1)));
                    result.S4.Terms.Add(Wij * Math.Pow((i - j + 1), -1.0));
                    result.S5.Terms.Add(Math.Pow((i - j + 1), -1.0));
                }
            }

            return result;
        }
    }
}
