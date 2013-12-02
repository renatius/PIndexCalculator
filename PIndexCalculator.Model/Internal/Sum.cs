using System;
using System.Collections.Generic;
using System.Linq;

namespace PIndex.Model.Internal
{
    public class Sum
    {
        public List<double> Terms { get; private set; }

        public Sum() {
            Terms = new List<double>();
        }

        public double Value {
            get {
                Terms.Sort();
                return Terms.Sum();
            }
        }
    }
}
