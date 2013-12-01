using PIndexCalculator.Model.Output;
using System;
using System.Collections.Generic;

namespace PIndexCalculator.Model.Input
{
    interface IDataset
    {
        bool IsValid { get; }

        IEnumerable<Observation> Observations { get; }
        IEnumerable<Person> People { get; }
        IEnumerable<PovertyPersistenceRatio> PovertyPersistenceRatios { get; }
        IEnumerable<DatasetError> Errors { get; }
        
        int YearMax { get; }
        int YearMin { get; }
        int YearSpan { get; }
    }
}
