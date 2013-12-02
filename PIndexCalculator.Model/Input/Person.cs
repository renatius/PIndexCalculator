using System.Linq;
using System.Collections.Generic;
using PICalculator.Model.Exceptions;

namespace PICalculator.Model.Input
{
    /// <summary>
    /// Each instance of class Person represents an individual
    /// described in the sample data
    /// </summary>
    public class Person
    {
        private readonly Dictionary<int, PovertyStatus> observations;

        // constructor
        public Person(string country, string personId) {
            Country = country;
            PersonId = personId;
            MinYear = int.MaxValue;
            MaxYear = int.MinValue;
            observations = new Dictionary<int, PovertyStatus>();
        }

        // the country the individual lives in
        public string Country { get; private set; }

        // the Person Id
        public string PersonId { get; private set; }

        // the year of the oldest recorded observation
        public int MinYear { get; private set; }

        // the year of the most recent observation
        public int MaxYear { get; private set; }

        // the span of the observations
        public int YearSpan {
            get { return 1 + (MaxYear - MinYear); }
        }

        // the number of observations
        public int ObservationCount {
            get { return observations.Count; }
        }

        public bool ObservationsMissingInYearSpan {
            get { return YearSpan != ObservationCount; }
        }

        public bool IsEverPoor { get; private set; }

        public bool HasObservationForYear(int year) {
            return observations.ContainsKey(year);
        }

        public void AddObservation(int year, bool isPoor, double povertyGap) {

            if (HasObservationForYear(year)) {
                string errorMessage =
                    string.Format("({0}, {1}) An observation for year {2} has been already recorded", Country, PersonId, year);

                throw new BusinessException(errorMessage);
            }

            observations[year] = new PovertyStatus(year, isPoor, povertyGap);

            // update years lower and upper bounds
            if (year < MinYear) MinYear = year;
            if (year > MaxYear) MaxYear = year;

            // update IsEverPoor flag
            if (isPoor) IsEverPoor = true;
        }

        public bool IsPoorInYear(int year) {
            try {
                return observations[year].IsPoor;
            }
            catch (KeyNotFoundException) {
                // should it happen
                // we treat a missing as 
                // a year of non poverty
                return false;
            }
        }

        public double GetPovertyGapForYear(int year) {
            try {
                return observations[year].PovertyGap;
            }
            catch (KeyNotFoundException) {
                // should it happen
                // we arbitrarily treat a missing as 
                // a year of non poverty
                return 0.0d;
            }
        }

        public List<int> GetPovertyYears() {
            List<int> result = new List<int>();

            if (observations.Count > 0) {
                for (int year = MinYear; year <= MaxYear; year++) {
                    if (IsPoorInYear(year))
                        result.Add(year);
                }
            }

            return result;
        }

        public int GetTrailingNonPovertyYears() {
            int result = 0;

            for (int year = MaxYear; year >= MinYear; year--) {
                if (IsPoorInYear(year))
                    break;
                result += 1;
            }

            return result;
        }

        public double GetPovertyGapAverage() {
            if (IsEverPoor) {

                var x = (from p in observations.Values
                         where p.IsPoor
                         select p.PovertyGap).Average();

                return x;
            }
            else {
                return 0.0;
            }
        }

        public int GetNonPovertyYearsBetween(int lowYear, int highYear) {
            int result = 0;
            for (int x = lowYear + 1; x < highYear; ++x) {
                if (IsPoorInYear(x) == false)
                    result++;
            }
            return result;
        }

        public double GetPovertyGapAverage(int lowYear, int highYear) {
            double pgLow = GetPovertyGapForYear(lowYear);
            double pgHigh = GetPovertyGapForYear(highYear);
            return (pgLow + pgHigh) / 2.0d;
        }

        public string GetPovertyVectorAsStringBetween(int lowYear, int highYear) {
            List<string> tmp = new List<string>();

            for (int i = lowYear; i <= highYear; i++) {
                tmp.Add(IsPoorInYear(i) ? "1" : "0");
            }

            return string.Format("[{0}]", string.Join(":", tmp.ToArray()));
        }

        public string GetPovertyGapVectorAsStringBetween(int lowYear, int highYear) {
            List<string> tmp = new List<string>();

            for (int i = lowYear; i <= highYear; i++) {
                double pg = GetPovertyGapForYear(i);
                tmp.Add(string.Format("{0:0.######}", pg));
            }

            return string.Format("[{0}]", string.Join(":", tmp.ToArray()));
        }

        public int GetMaxSpell() {
            int result = 0;
            int stretchLength = 0;

            for (int anno = MinYear; anno <= MaxYear; anno++) {
                if (IsPoorInYear(anno)) {
                    stretchLength++;
                }
                else {
                    if (stretchLength > result)
                        result = stretchLength;
                    stretchLength = 0;
                }
            }

            if (stretchLength > result)
                result = stretchLength;

            return result;
        }

        public double GetBossertIndex() {
            double result = 0.0;
            double accumulatore = 0.0;
            int stretchLength = 0;
            for (int anno = MinYear; anno <= MaxYear; anno++) {
                if (IsPoorInYear(anno)) {
                    stretchLength++;
                    accumulatore += GetPovertyGapForYear(anno);
                }
                else {
                    if (stretchLength > 0) {
                        result += (accumulatore * stretchLength);
                    }
                    accumulatore = 0.0;
                    stretchLength = 0;
                }
            }

            if (stretchLength > 0)
                result += (accumulatore * stretchLength);

            return result / YearSpan;
        }

        public double GetBossert2Index() {
            double result = 0.0;
            int stretchLength = 0;
            List<double> accumulatore = new List<double>();

            for (int anno = MinYear; anno <= MaxYear; anno++) {
                if (IsPoorInYear(anno)) {
                    stretchLength++;
                    accumulatore.Add(GetPovertyGapForYear(anno));
                }
                else {
                    if (stretchLength > 0) {
                        var tmp = accumulatore.ConvertAll<double>(x =>
                            x * System.Math.Pow(2, stretchLength - 1));

                        result += tmp.Sum();
                    }

                    accumulatore.Clear();
                    stretchLength = 0;
                }
            }

            if (stretchLength > 0) {
                var tmp = accumulatore.ConvertAll<double>(x =>
                    x * System.Math.Pow(2, stretchLength - 1));

                result += tmp.Sum();
            }

            return result / YearSpan;
        }
    }
}
