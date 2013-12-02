using System;
using FileHelpers;
using PICalculator.Model.Exceptions;

namespace PICalculator.Model.Output
{
    [DelimitedRecord(";")]
    public sealed class PovertyPersistenceRatio
    {
        [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForRead)]
        private string country;
        private int lowYear;
        private int highYear;
        private int populationSize;
        private int highYearStillPoorCount;
        private double permanenceProbability;

        public string Country {
            get { return country; }
            set { country = value; }
        }

        public int LowYear {
            get { return lowYear; }
            set { lowYear = value; }
        }
        
        public int HighYear {
            get { return highYear; }
            set { highYear = value; }
        }

        public int PopulationSize {
            get { return populationSize; }
            set { populationSize = value; }
        }

        public int PoorInBothYears {
            get { return highYearStillPoorCount; }
            set { highYearStillPoorCount = value; }
        }

        public double PermanenceProbability {
            get { return permanenceProbability; }
            set { permanenceProbability = value; }
        }

        // needed by FileHelpers Library
        public PovertyPersistenceRatio() { }

        public PovertyPersistenceRatio(
            string country,
            int lowYear,
            int highYear,
            int populationSize,
            int highYearStillPoorCount
        ) {
            Precondition.Require(lowYear < highYear, "The low year argument must be less than the high year argument");
            Precondition.Require(populationSize >= 0, "the population size cannot be a negative value");
            Precondition.Require(highYearStillPoorCount >= 0, "the still poor count cannot be a negative value");
            Precondition.Require(highYearStillPoorCount <= populationSize, "The high year count cannot be greater than the low year count");

            Country = country;
            LowYear = lowYear;
            HighYear = highYear;
            PopulationSize = populationSize;
            PoorInBothYears = highYearStillPoorCount;

            PermanenceProbability = PopulationSize > 0 ?
                ((double)PoorInBothYears) / ((double)PopulationSize) : 0;
        }
    }
}
