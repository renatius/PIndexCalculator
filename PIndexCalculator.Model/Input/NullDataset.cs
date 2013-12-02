using System;
using System.Collections.Generic;
using PICalculator.Model.Output;

namespace PICalculator.Model.Input
{
    public class NullDataset : IDataset
    {
        public bool IsValid {
            get { return false; }
        }

        public IEnumerable<Observation> Observations {
            get { return new List<Observation>(); }
        }

        public IEnumerable<Person> People {
            get { return new List<Person>(); }
        }

        public IEnumerable<PovertyPersistenceRatio> PovertyPersistenceRatios {
            get { return new List<PovertyPersistenceRatio>(); }
        }

        public IEnumerable<DatasetError> Errors {
            get { return new List<DatasetError>(); }
        }

        public int YearMax {
            get { return 0; }
        }

        public int YearMin {
            get { return 0; }
        }

        public int YearSpan {
            get { return 0; }
        }
    }
}
