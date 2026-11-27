using ServiceManagerApp.Services.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceManagerApp.Services.Overview
{
    public interface IOverviewService
    {
        Task<OverviewData> GetOverviewAsync(SignedInUser? user);

        Task<IReadOnlyList<TicketSummary>> SearchTicketsAsync(SignedInUser? use,
            string searchText);

    }
}
