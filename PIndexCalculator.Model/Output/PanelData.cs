using System.Collections.Generic;
using System.Linq;
using PIndexCalculator.Model.Input;
using PIndexCalculator.Model.Exceptions;
using PIndexCalculator.Model.Internal;
using PIndexCalculator.Model.Calculators;

namespace PIndexCalculator.Model.Output
{
    public class PanelData
    {
        private IEnumerable<Person> people;
        private IEnumerable<PovertyPersistenceRatio> povertyPersistenceRatios;

        private List<PanelError> errors;
        private Dictionary<Person, SequenceEffect> sequenceEffects = new Dictionary<Person, SequenceEffect>();
        private Dictionary<Person, double> emergencyEffects = new Dictionary<Person, double>();
        private Dictionary<Person, double> bossertIndexes = new Dictionary<Person, double>();
        private Dictionary<Person, double> bcd2Indexes = new Dictionary<Person, double>();

        // observation stats
        private int yearMin;
        private int yearMax;
        private int yearSpan;        

        /// <summary>
        /// The list of individuals present in the panel
        /// </summary>
        public IEnumerable<Person> People {
            get { return people; }
        }

        /// <summary>
        /// The pre-calculated poverty persistence ratios
        /// </summary>
        public IEnumerable<PovertyPersistenceRatio> PovertyPersistenceRatios {
            get { return povertyPersistenceRatios; }
        }

        public IEnumerable<PanelError> Errors {
            get { return errors; }
        }

        public bool IsValid {
            get { return errors.Count() == 0; }
        }

        public PanelData(int yearMin, int yearMax, IEnumerable<Person> people, IEnumerable<PovertyPersistenceRatio> povertyPersistenceRatios) {

            Precondition.Require(
                yearMin < yearMax,
                "Year min must be less than year max");

            Precondition.Require(
                people != null && people.Count() > 0,
                "You must pass a non empty population");

            Precondition.Require(
                povertyPersistenceRatios != null && povertyPersistenceRatios.Count() > 0,
                "You must pass the set of poverty persistence ratio to use for calculations");

            this.yearMin = yearMin;
            this.yearMax = yearMax;
            this.yearSpan = 1 + (yearMax - yearMin);

            Precondition.Require(
                yearSpan <= BinomialCoefficientChooseTwo.MaxN,
                string.Format(
                        "The oldest and the most recent observation cannot differ by more than {0} years",
                        BinomialCoefficientChooseTwo.MaxN)
            );


            this.people = people;
            this.povertyPersistenceRatios = povertyPersistenceRatios;

            this.errors = new List<PanelError>();

            ValidatePeopleList();

            if (IsValid) {
                CalculateEmergencyEffects();
                CalculateSequenceEffects();
                CalculateBossertIndexes();
                CalculateBCD2Indexes();
            }
        }

        private void AddError(string errorMessage) {
            errors.Add(new PanelError(errorMessage));
        }

        private void ValidatePeopleList() {
            foreach (var p in people) ValidatePerson(p);
        }

        private static string GetIdStringFor(Person p) {
            return string.Format("({0}, {1})", p.Country, p.PersonId);
        }

        private void ValidatePerson(Person p) {
            if (p.MinYear != yearMin) {
                AddError(string.Format("Panel_{0}: person {1}: first observation year does not correspond to first year for panel",
                    yearSpan,
                    GetIdStringFor(p)));
            }

            if (p.MaxYear != yearMax) {
                AddError(string.Format("Panel_{0}: person {1}: last observation year does not correspond to last year for panel",
                    yearSpan,
                    GetIdStringFor(p)));
            }

            if (p.ObservationCount != yearSpan) {
                AddError(string.Format("Panel_{0}: person {1}: observations do not cover the entire span",
                    yearSpan,
                    GetIdStringFor(p)));
            }
        }

        private void CalculateSequenceEffects() {
            var poors = from p in people
                        where p.IsEverPoor
                        select p;

            var calculator = new SequenceEffectCalculator(yearMin, yearMax, povertyPersistenceRatios);

            foreach (var p in poors) {
                sequenceEffects[p] = calculator.CalculateFor(p);
            }
        }

        private void CalculateEmergencyEffects() {
            var poors = from p in people
                        where p.IsEverPoor
                        select p;

            var calculator = new EmergencyEffectCalculator(yearMin, yearMax);

            foreach (var p in poors) {
                emergencyEffects[p] = calculator.CalculateFor(p);
            }
        }

        private void CalculateBossertIndexes() {
            var poors = from p in people
                        where p.IsEverPoor
                        select p;

            foreach (var p in poors) {
                bossertIndexes[p] = p.GetBossertIndex();
            }
        }

        private void CalculateBCD2Indexes() {
            var poors = from p in people
                        where p.IsEverPoor
                        select p;

            foreach (var p in poors) {
                bcd2Indexes[p] = p.GetBossert2Index();
            }
        }

        private SequenceEffect GetSequenceEffect(Person p) {
            return sequenceEffects[p];
        }

        private double GetEmergencyEffect(Person p) {
            return emergencyEffects[p];
        }

        private double GetBossetIndex(Person p) {
            return bossertIndexes[p];
        }

        private double GetBCD2Index(Person p) {
            return bcd2Indexes[p];
        }

        public List<PovertyIndexResult> CalculatePovertyIndex(double alpha) {
            Precondition.Require(IsValid,
                "Cannot calculate Poverty Index when panel data contain errors");

            var result = new List<PovertyIndexResult>();

            var poors = from p in people
                        where p.IsEverPoor
                        select p;

            var calculator = new PovertyIndexCalculator(alpha);

            foreach (var p in poors) {
                var sequenceEffect = GetSequenceEffect(p);
                double emergencyEffect = GetEmergencyEffect(p);

                var pi = new PovertyIndexResult() {
                    WaveCount = yearSpan,
                    Country = p.Country,
                    PersonId = p.PersonId,
                    PovertySequence = p.GetPovertyVectorAsStringBetween(yearMin, yearMax),
                    PovertyGapSequence = p.GetPovertyGapVectorAsStringBetween(yearMin, yearMax),
                    PovertyGapAverage = p.GetPovertyGapAverage(),
                    MaxSpell = p.GetMaxSpell(),
                    SequenceEffect1 = sequenceEffect.V1,
                    SequenceEffect2 = sequenceEffect.V2,
                    SequenceEffect3 = sequenceEffect.V3,
                    SequenceEffect4 = sequenceEffect.V4,
                    SequenceEffect5 = sequenceEffect.V5,
                    EmergencyEffect = emergencyEffect,
                    BossertIndex = GetBossetIndex(p),
                    BCD2 = GetBCD2Index(p),
                    Alpha = alpha,
                    SE_EE_1 = calculator.Calculate(sequenceEffect.V1, emergencyEffect),
                    SE_EE_2 = calculator.Calculate(sequenceEffect.V2, emergencyEffect),
                    SE_EE_3 = calculator.Calculate(sequenceEffect.V3, emergencyEffect),
                    SE_EE_4 = calculator.Calculate(sequenceEffect.V4, emergencyEffect),
                    SE_EE_5 = calculator.Calculate(sequenceEffect.V5, emergencyEffect)
                };

                result.Add(pi);
            }

            return result;
        }
    }
}
