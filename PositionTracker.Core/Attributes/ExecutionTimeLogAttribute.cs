using System;

namespace PositionTracker.Core.Attributes
{
    public class ExecutionTimeLogAttribute : Attribute
    {
        public long Threshold;

        public ExecutionTimeLogAttribute(long threshold)
        {
            Threshold = threshold;
        }

        public ExecutionTimeLogAttribute()
        {
            Threshold = 0;
        }
    }
}