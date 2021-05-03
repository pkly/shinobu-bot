using System;
using System.Collections.Generic;

namespace Shinobu.Utility
{
    public class RangeHelper<T>
    {
        private List<Range<T>> _ranges = new List<Range<T>>();
        
        public RangeHelper(Range<T>[] ranges)
        {
            foreach (var range in ranges)
            {
                AddRange(range);
            }
        }

        public void AddRange(Range<T> range)
        {
            if (GetValue(range.From) != null || (range.To != null && GetValue((int) range.To) != null))
            {
                throw new Exception("Ranges cannot overlap");
            }

            _ranges.Add(range);
        }

        public T? GetValue(int value)
        {
            foreach (var r in _ranges)
            {
                if (value >= r.From && (r.To == null || value <= r.To))
                {
                    return r.Value;
                }
            }

            return default(T);
        }
    }
}