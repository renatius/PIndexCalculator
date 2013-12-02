using FileHelpers;
using PIndexCalculator.Model.Exceptions;
using PIndexCalculator.Model.Input;
using PIndexCalculator.Model.Output;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PIndexCalculator.Model.Application
{
    public class CalculatorApplication : ICalculatorApplication
    {
        private IDataset ds;
        private List<ApplicationError> errors;
        private List<PovertyIndexResult> indices;


        public CalculatorApplication() {
            ds = new NullDataset();
            errors = new List<ApplicationError>();
            indices = new List<PovertyIndexResult>();
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

        public IEnumerable<ApplicationError> Errors {
            get { return errors; }
        }

        public IEnumerable<PovertyPersistenceRatio> PovertyPersistenceRatios {
            get { return ds.PovertyPersistenceRatios; }
        }

        public IEnumerable<PovertyIndexResult> PovertyIndices {
            get { return indices; }
        }

        public void LoadCsvFile(string filename) {
            try {
                try {
                    OnBeginUpdate();
                    Reset();

                    ds = Dataset.LoadFromCsvFile(filename);

                    foreach (var e in ds.Errors) {
                        errors.Add(new ApplicationError(e.Message));
                    }

                    for (int i = ds.YearMin; i < ds.YearMax; i++) {
                        var span = 1 + (ds.YearMax - i);

                        var people = (from p in ds.People
                                      where p.YearSpan == span
                                      select p).ToArray();

                        if (people.Length > 0) {
                            var panel = new PanelData(i, ds.YearMax, people, ds.PovertyPersistenceRatios);

                            foreach (var e in panel.Errors) {
                                errors.Add(new ApplicationError(e.Message));
                            }

                            if (panel.IsValid) {
                                double[] alpha = { 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };

                                foreach (var x in alpha) {
                                    indices.AddRange(panel.CalculatePovertyIndex(x));
                                }
                            }
                        }
                    }
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

        private void Reset() {
            ds = new NullDataset();
            errors = new List<ApplicationError>();
            indices = new List<PovertyIndexResult>();
        }

        public void ExportPovertyPersistenceProbabilitiesToCsv(string filename) {
            var writer = new FileHelperEngine<PovertyPersistenceRatio>();
            writer.WriteFile(filename, PovertyPersistenceRatios);
        }

       public void ExportPovertyIndicesToCsv(string filename) {
            var writer = new FileHelperEngine<PovertyIndexResult>();
            writer.WriteFile(filename, indices);      
        }

    }
}
