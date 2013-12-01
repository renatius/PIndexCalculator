using FileHelpers;
using PIndexCalculator.Model.Calculators;
using PIndexCalculator.Model.Exceptions;
using PIndexCalculator.Model.Internal;
using PIndexCalculator.Model.Output;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PIndexCalculator.Model.Input
{
    public class Dataset : IDataset
    {
        private List<DatasetError> errors;        
        private IEnumerable<Observation> observations;
        private PersonFactory personFactory;
        private List<PovertyPersistenceRatio> povertyPersistenceRatios;

        public bool IsValid {
            get { return errors.Count > 0; }
        }

        public IEnumerable<Observation> Observations {
            get { return Observations; }
        }

        public IEnumerable<Person> People {
            get { return personFactory.GetPeople(); }
        }

        public IEnumerable<DatasetError> Errors {
            get { return errors; }
        }

        public IEnumerable<PovertyPersistenceRatio> PovertyPersistenceRatios {
            get { return povertyPersistenceRatios; }
        }

        public int YearMin { get; private set; }
        public int YearMax { get; private set; }
        public int YearSpan {
            get { return 1 + (YearMax - YearMin); }
        }

        public Dataset(IEnumerable<Observation> observations) {

            this.povertyPersistenceRatios = new List<PovertyPersistenceRatio>();
            this.errors = new List<DatasetError>();
            this.observations = observations;

            // transform observations in the set of people observed
            personFactory = new PersonFactory();

            foreach (var o in observations) {
                var person = personFactory.CreateInstance(o);
                person.AddObservation(o.Year, o.IsPoor, o.PovertyGap);
            }

            // get year min e year max
            YearMin = (from x in observations
                       select x.Year).Min();

            YearMax = (from x in observations
                       select x.Year).Max();

            // validate dataset
            Validate();

            // if dataset is valid calculate poverty persistence ratios
            if (IsValid) {
                CalculatePovertyPersistenceRatios();
            }
        }

        public static Dataset LoadFromCsvFile(string filename) {
            try {
                var reader = new FileHelperEngine<Observation>();
                var observations = reader.ReadFile(filename);
                return new Dataset(observations);
            }
            catch (Exception exc) {
                string errorMessage = string.Format("Error reading dataset: {0}", exc.Message);
                throw new BusinessException(errorMessage, exc);
            }
        }

        private void AddError(string errorMessage) {
            errors.Add(new DatasetError(errorMessage));
        }

        private void Validate() {
            ValidateObservations();
            ValidatePeople();
        }

        private void ValidateObservations() {
            int i = 2;
            foreach (var o in observations) {
                Validate(o, i);
                i++;
            }
        }

        private void Validate(Observation o, int index) {
            if (string.IsNullOrWhiteSpace(o.Country)) {
                string errorMessage = string.Format("line ({0}): Country is missing", index);
                AddError(errorMessage);
            }

            if (string.IsNullOrWhiteSpace(o.PersonId)) {
                string errorMessage = string.Format("line ({0}): PersonId is missing", index);
                AddError(errorMessage);
            }
        }

        private void ValidatePeople() {
            foreach (var p in People) Validate(p);
        }

        private void Validate(Person p) {
            if (p.MaxYear != YearMax) {
                string errorMessage = string.Format("Person ({0}, {1}): observations do not extend to {2} ", p.PersonId, p.Country, YearMax);
                AddError(errorMessage);
            }
            
            if (p.ObservationsMissingInYearSpan) {
                string errorMessage = string.Format("Person ({0}, {1}): observations do not cover entire personal span", p.PersonId, p.Country);
                AddError(errorMessage);
            }
        }

        private void CalculatePovertyPersistenceRatios() {
            var ratios = new List<PovertyPersistenceRatio>();

            var countries = (from p in People
                             select p.Country).Distinct();

            foreach (var country in countries) {
                var calculator = new PovertyPersistenceRatioCalculator(People);
                ratios.AddRange(calculator.CalculateRatios(country, YearMin, YearMax));
            }

            povertyPersistenceRatios = ratios;
        }
    }
}
