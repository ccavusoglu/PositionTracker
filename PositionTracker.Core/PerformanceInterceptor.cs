using System.Linq;
using Castle.DynamicProxy;
using PositionTracker.Core.Attributes;
using PositionTracker.Utility;

namespace PositionTracker.Core
{
    public class PerformanceInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (!(invocation.Method.GetCustomAttributes(typeof(ExecutionTimeLogAttribute), false).FirstOrDefault()
                is ExecutionTimeLogAttribute perfAttribute))
            {
                invocation.Proceed();

                return;
            }

            var e = LogExecutionTime.Begin();

            invocation.Proceed();

            e.End(invocation.Method.DeclaringType.Name, invocation.Method.Name, perfAttribute.Threshold);
        }
    }
}