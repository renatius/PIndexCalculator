using PIndexCalculator.Model.Input;
using PIndexCalculator.Model.Output;
using System;
using System.Collections.Generic;

namespace PIndexCalculator.Model.Application
{
    public interface ICalculatorApplication
    {
        event EventHandler BeginUpdate;
        event EventHandler EndUpdate;

        // the set of observations contained in the input file
        IEnumerable<Observation> Observations { get; }

        // the set of people to whom the observations are referring
        IEnumerable<Person> People { get; }

        // there are errors in the dataset?
        IEnumerable<ApplicationError> Errors { get; }

        // the Poverty persistance ratios
        IEnumerable<PovertyPersistenceRatio> PovertyPersistenceRatios { get; }

        // The poverty indices
        IEnumerable<PovertyIndexResult> PovertyIndices { get; }

        void LoadCsvFile(string filename);
        void ExportPovertyPersistenceProbabilitiesToCsv(string filename);
        void ExportPovertyIndicesToCsv(string filename);
    }
}
