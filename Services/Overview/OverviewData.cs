using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceManagerApp.Services.Overview
{
    public class OverviewData
    {
        public bool IsSignedIn { get; set; }
        public bool ShowAllTickets { get; set; }

        public int AcceptedCount { get; set; }
        public int DiagnosingCount { get; set; }
        public int InRepairCount { get; set; }
        public int FinishedCount { get; set; }


        public IReadOnlyList<TicketSummary> ActiveTickets { get; set; } = [];
        public IReadOnlyList<TicketSummary> ReadyTickets { get; set;  }  = new List<TicketSummary>();


    }
}
