namespace PICalculator.Model.Input
{
    /// <summary>
    /// The poverty status of an individual in a given year
    /// </summary>
    public class PovertyStatus
    {
        public PovertyStatus(int year, bool isPoor, double povertyGap) {
            Year = year;
            IsPoor = isPoor;
            PovertyGap = povertyGap;
        }

        public int Year { get; private set; }
        public bool IsPoor { get; private set; }
        public double PovertyGap { get; private set; }
    }
}
