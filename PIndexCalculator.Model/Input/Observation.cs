using System;
using FileHelpers;

namespace PICalculator.Model.Input
{
    /// <summary>
    /// A single observation, as contained in the input file.
    /// 
    /// The input file must be in csv format, using ; as delimiter.
    /// The country field must be enclosed in double quotes.
    /// The first line is assumed to contain headers and is ignored.
    /// </summary>
    [DelimitedRecord(";"), IgnoreFirst(1), IgnoreEmptyLines()]
    public sealed class Observation
    {
        private int year;

        [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForRead)]
        private string country;

        private string householdId;
        private string personId;
        private int isPoor;
        private double povertyGap;

        public Int32 Year {
            get { return year; }
            set { year = value; }
        }
        
        public string Country {
            get { return country; }
            set { country = value; }
        }

        public string HouseholdId {
            get { return householdId; }
            set { householdId = value; }
        }

        public string PersonId {
            get { return personId; }
            set { personId = value; }
        }

        public bool IsPoor {
            get { return isPoor == 1; }
            set { isPoor = value ? 1 : 0; }
        }

        public double PovertyGap {
            get { return povertyGap; }
            set { povertyGap = value; }
        }
    }
}
