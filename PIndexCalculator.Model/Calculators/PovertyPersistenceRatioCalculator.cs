using System;
using System.Linq;
using System.Collections.Generic;
using PICalculator.Model.Input;
using PICalculator.Model.Exceptions;
using PICalculator.Model.Output;

namespace PICalculator.Model.Calculators
{
    public class PovertyPersistenceRatioCalculator
    {
        private IEnumerable<Person> individuals;        
        
        public PovertyPersistenceRatioCalculator(IEnumerable<Person> individuals)
        {
            Precondition.Require(individuals != null, "You must pass a list of individuals");
            Precondition.Require(individuals.Count() > 0, "The list of individuals must not be empty");

            this.individuals = individuals;
        }

        public IEnumerable<PovertyPersistenceRatio> CalculateRatios
        (
            string country, 
            int yearMin, 
            int yearMax 
        )
        {
            var ratios = new List<PovertyPersistenceRatio>();

            for (int lowYear = yearMin; lowYear < yearMax; lowYear++)
            {
                var people = from p in individuals
                             where (p.Country == country && p.MinYear <= lowYear)
                             select p;

                var poors = from p in people
                            where p.IsPoorInYear(lowYear)
                            select p;

                for (int highYear = lowYear + 1; highYear <= yearMax; highYear++)                
                {
                    var stillPoors = from p in poors
                                     where p.IsPoorInYear(highYear)
                                     select p;

                    ratios.Add(new PovertyPersistenceRatio(country, lowYear, highYear, people.Count(), stillPoors.Count()));
                }
            }

            return ratios;
        }
    }
}
