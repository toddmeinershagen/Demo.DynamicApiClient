using System;

namespace Demo.PolicyApiClient.Functions
{
    public static partial class Functions
    {
        public class PersonSpecification : CompositeSpecification
        {
            public override bool IsSatisfiedBy(dynamic item)
            {
                var birthDate = Convert.ToDateTime(item.birthDate);
                return birthDate.Date == new DateTime(1972, 11, 23);
            }
        }
    }
}
