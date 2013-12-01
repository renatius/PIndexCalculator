using FileHelpers;
using PIndexCalculator.Model.Exceptions;
using PIndexCalculator.Model.Input;
using PIndexCalculator.Model.Output;
using System;
using System.Collections.Generic;

namespace PIndexCalculator.Model.Application
{
    public class CalculatorApplication : ICalculatorApplication
    {
        private IDataset ds;

        public CalculatorApplication() {
            ds = new NullDataset();
        }

        public event EventHandler BeginUpdate;
        public event EventHandler EndUpdate;

        protected void OnBeginUpdate() {
            if (BeginUpdate != null) 
                BeginUpdate(this, EventArgs.Empty);
        }

        protected void OnEndUpdate() {
            if (EndUpdate != null) 
                EndUpdate(this, EventArgs.Empty);
        }

        public IEnumerable<Observation> Observations {
            get { return ds.Observations; }
        }

        public IEnumerable<Person> People {
            get { return ds.People; }
        }

        public IEnumerable<DatasetError> Errors {
            get { return ds.Errors; }
        }

        public IEnumerable<PovertyPersistenceRatio> PovertyPersistenceRatios {
            get { return ds.PovertyPersistenceRatios; }
        }

        public void LoadCsvFile(string filename) {
            try {
                try {
                    OnBeginUpdate();
                    ds = Dataset.LoadFromCsvFile(filename);                    
                }
                finally {
                    OnEndUpdate();
                }
            }
            catch (BusinessException) {
                throw;
            }
            catch (Exception e) {
                throw new BusinessException(e.Message, e);
            }
        }
    }
}
