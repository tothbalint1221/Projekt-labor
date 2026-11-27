using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ServiceManagerApp.Services.Overview;
using ServiceManagerApp.Services.Users;
using System.Drawing.Printing;

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
        private string _searchText = string.Empty;
        private IReadOnlyList<TicketSummary> _searchResults = new List<TicketSummary>();
        public IReadOnlyList<TicketSummary> ActiveTickets => _data.ActiveTickets;
        public IReadOnlyList<TicketSummary> ReadyTickets => _data.ReadyTickets;
        public IReadOnlyList<TicketSummary> SearchResults => _searchResults;

        // Ha van beírt szöveg, a két lista helyett a találatok látszanak.
        public bool ShowSearchResults => !string.IsNullOrWhiteSpace(_searchText);

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                RunSearch();
            }
        }


        public async Task LoadOverviewAsync()
        {
            var user = _userSession.CurrentUser;

            _data = await _overviewService.GetOverviewAsync(user);

            _userLine = user is null
                ? "Not signed in"
                : $"{user.Name} - {user.Role}";

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty)); 

        }

        private async void RunSearch()
        {
            string text = _searchText;

            try
            {
                var results = await _overviewService
                    .SearchTicketsAsync(_userSession.CurrentUser, text);

                if(text != _searchText)
                {
                    return;
                }

                _searchResults = results;

            }catch
            {
                _searchResults = new List<TicketSummary>();
            }

            OnPropertyChanged(nameof(SearchResults));
            OnPropertyChanged(nameof(ShowSearchResults));


        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
