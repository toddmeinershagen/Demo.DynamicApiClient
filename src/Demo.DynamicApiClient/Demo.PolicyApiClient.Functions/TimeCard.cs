using System;

namespace Demo.PolicyApiClient.Functions
{
    public static partial class Functions
    {
        public class TimeCard
        {
            public Guid Id { get; set; }
            public DateTime DueDate { get; set; }
        }
    }
}
