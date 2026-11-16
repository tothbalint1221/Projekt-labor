using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ServiceManagerApp.Services.Overview;
using ServiceManagerApp.Services.Users;

namespace ServiceManagerApp.ViewModels
{
    public class OverviewViewModel : INotifyPropertyChanged
    {
        private readonly IOverviewService _overviewService;
        private readonly UserSession _userSession;
        private OverviewData _data = new OverviewData();
        private string _userLine = "Not signed in";

        public OverviewViewModel(IOverviewService overviewService, UserSession userSession
           )
        {
            this._overviewService = overviewService;
            this._userSession = userSession;
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public string UserLine => _userLine;
        public bool IsSignedIn => _data.IsSignedIn;

        public string ActiveTitle =>
            _data.ShowAllTickets ? "All active tickets" : "Your active tickets";

        public int AcceptedCount => _data.AcceptedCount;
        public int DiagnosingCount => _data.DiagnosingCount;
        public int InRepairCount => _data.InRepairCount;
        public int FinishedCount => _data.FinishedCount;

        public IReadOnlyList<TicketSummary> ActiveTickets => _data.ActiveTickets;
        public IReadOnlyList<TicketSummary> ReadyTickets => _data.ReadyTickets;

        public async Task LoadOverviewAsync()
        {
            var user = _userSession.CurrentUser;

            _data = await _overviewService.GetOverviewAsync(user);

            _userLine = user is null
                ? "Not signed in"
                : $"{user.Name} - {user.Role}";

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty)); 

        }

    }
}
