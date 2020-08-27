using System;

namespace Demo.PolicyApiClient.Functions
{
    public static partial class Function1
    {
        public class TimeCardSpecification : CompositeSpecification
        {
            public override bool IsSatisfiedBy(dynamic item)
            {
                var dueDate = Convert.ToDateTime(item.dueDate);
                return dueDate.Date == DateTime.Now.Date.AddDays(2);
            }
        }
    }
}
