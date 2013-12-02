using FileHelpers;
using PICalculator.Model.Calculators;
using PICalculator.Model.Exceptions;
using PICalculator.Model.Internal;
using PICalculator.Model.Output;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PICalculator.Model.Input
{
    public class Dataset : IDataset
    {
        private List<DatasetError> errors;        
        private IEnumerable<Observation> observations;
        private IEnumerable<Person> people;
        private List<PovertyPersistenceRatio> povertyPersistenceRatios;

        public bool IsValid {
            get { return errors.Count == 0; }
        }

        public IEnumerable<Observation> Observations {
            get { return observations; }
        }

        public IEnumerable<Person> People {
            get { return people; }
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
            var personFactory = new PersonFactory();

            foreach (var o in observations) {
                var person = personFactory.CreateInstance(o);
                person.AddObservation(o.Year, o.IsPoor, o.PovertyGap);
            }

            people = new List<Person>(personFactory.GetPeople());

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
            foreach (var p in people) Validate(p);
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

            var countries = (from p in people
                             select p.Country).Distinct();

            foreach (var country in countries) {
                var calculator = new PovertyPersistenceRatioCalculator(people);
                ratios.AddRange(calculator.CalculateRatios(country, YearMin, YearMax));
            }

            povertyPersistenceRatios = ratios;
        }
    }
}
