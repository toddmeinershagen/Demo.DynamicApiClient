using System;

namespace Demo.PolicyApiClient.Functions
{
    public static partial class Function1
    {
        public class TimeCard
        {
            public Guid Id { get; set; }
            public DateTime DueDate { get; set; }
        }
    }
}
