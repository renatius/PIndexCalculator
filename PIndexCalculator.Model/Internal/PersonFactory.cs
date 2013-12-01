using PIndexCalculator.Model.Input;
using System;
using System.Collections.Generic;

namespace PIndexCalculator.Model.Internal
{
    class PersonFactory
    {
        private Dictionary<string, Person> peopleCache;

        public PersonFactory() {
            peopleCache = new Dictionary<string, Person>();
        }

        public IEnumerable<Person> GetPeople() {
            return peopleCache.Values;
        }

        public Person CreateInstance(Observation observation) {
            string key = GetPersonKeyFor(observation);

            if (peopleCache.ContainsKey(key)) {
                return peopleCache[key];
            }
            else {
                string country = observation.Country;
                string personId = observation.PersonId;
                Person person = new Person(country, personId);
                peopleCache[key] = person;
                return person;
            }
        }

        private string GetPersonKeyFor(Observation observation) {
            return string.Format("{0}_{1}", observation.PersonId, observation.Country);
        }
    }
}
