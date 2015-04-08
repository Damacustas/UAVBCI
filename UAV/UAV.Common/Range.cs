using System;
using System.Collections.Generic;

namespace UAV.Common
{
    public class Range : IEnumerable<double>, IReadOnlyCollection<double>
    {
        public double Start { get; private set; }
        public double End { get; private set; }
        public double Delta { get; private set; }

        public Range(double start, double end, double delta)
        {
            Start = start;
            End = end;
            Delta = delta;
        }


        #region IEnumerable implementation
        public IEnumerator<double> GetEnumerator()
        {
            return new RangeEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        public int Count
        {
            get
            {
                int x = (int)((End - Start) / Delta);
                return x;
            }
        }
    }
}

