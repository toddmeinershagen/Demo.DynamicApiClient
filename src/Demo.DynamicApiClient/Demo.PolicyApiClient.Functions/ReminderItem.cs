using System;
using System.Collections.Generic;

namespace Demo.PolicyApiClient.Functions
{
    public static partial class Functions
    {
        public class ReminderItem
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public DateTime ReferenceDate { get; set; }
            public string OwnedBy { get; set; }
            public string LinkToItem { get; set; }
            public Dictionary<string, object> AdditionalProperties { get; set; }
        }
    }
}
