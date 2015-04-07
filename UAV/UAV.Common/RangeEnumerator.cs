using System;
using System.Collections.Generic;

namespace UAV.Common
{
    public class RangeEnumerator : IEnumerator<double>
    {
        public Range Range { get; private set; }

        double current;

        public RangeEnumerator(Range range)
        {
            Range = range;
            this.Reset();
        }

        #region IEnumerator implementation

        public bool MoveNext()
        {
            if (current >= Range.End)
            {
                return false;
            }
            else
            {
                current += Range.Delta;
                return true;
            }
        }

        public void Reset()
        {
            current = Range.Start;
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return current;
            }
        }

        public void Dispose()
        {
        }


        double System.Collections.Generic.IEnumerator<double>.Current
        {
            get
            {
                return current - Range.Delta;
            }
        }

        #endregion
    }
}

