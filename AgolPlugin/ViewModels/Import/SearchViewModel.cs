using AgolPlugin.Models.Agol;
using AgolPlugin.Models.Common;
using AgolPlugin.Services.Agol;
using AgolPlugin.Services.Background;
using System.ComponentModel;

namespace AgolPlugin.ViewModels.Import
{
    public class SearchViewModel : ViewModelBase
    {
        private AgolClient _agol;

        public BasicCommand Search_Command { get; private set; }
        public BasicCommand Continue_Command { get; private set; }
        public BasicCommand PageLeft_Command { get; private set; }
        public BasicCommand PageOver_Command { get; private set; }

        public SearchViewModel()
        {
            _agol = new AgolClient(Plugin.CredContainer.Credential, Plugin.CredContainer.UrlKey);

            Search_Command = new BasicCommand(Search_Execute, Search_CanExecute);
            Continue_Command = new BasicCommand(Continue_Execute, Continue_CanExecute);
            PageOver_Command = new BasicCommand(PageOver_Execute, PageOver_CanExecute);

            Title = "Feature Server Search";
            SearchIsExpanded = true;
            SearchOnlyMyContent = true;
        }

        #region Properties
        private bool _showNoResultsText;
        public bool ShowNoResultsText
        {
            get { return _showNoResultsText; }
            set { _showNoResultsText = value; OnPropertyChanged(); }
        }

        private bool _searchOnlyMyContent;
        public bool SearchOnlyMyContent
        {
            get { return _searchOnlyMyContent; }
            set { _searchOnlyMyContent = value; OnPropertyChanged(); }
        }

        private bool _searchIsExpanded;
        public bool SearchIsExpanded
        {
            get { return _searchIsExpanded; }
            set { _searchIsExpanded = value; OnPropertyChanged(); }
        }

        private string _queryText;
        public string QueryText
        {
            get { return _queryText; }
            set { _queryText = value; OnPropertyChanged(); }
        }

        private string _ownerText;
        public string OwnerText
        {
            get { return _ownerText; }
            set { _ownerText = value; OnPropertyChanged(); }
        }

        private string _queryType = "Feature Service";
        public string QueryType
        {
            get { return _queryType; }
            set { _queryType = value; OnPropertyChanged(); }
        }

        private int _itemsPerPage;
        public int ItemsPerPage
        {
            get { return _itemsPerPage; }
            set { _itemsPerPage = value; OnPropertyChanged(); }
        }

        private AgolSearchResultCollection _resultCollection;
        public AgolSearchResultCollection ResultCollection
        {
            get { return _resultCollection; }
            set { _resultCollection = value; OnPropertyChanged(); }
        }
        #endregion

        #region Interaction
        private void Search_Execute(object param)
        {
            ResultCollection = null;
            Worker.DoWork(WorkerDoSearch, WorkerFinishSearch);
        }
        private bool Search_CanExecute(object param)
        {
            return !IsBusy;
        }
        private void WorkerDoSearch(object s, DoWorkEventArgs e)
        {
            IsBusy = true;
            var owner = SearchOnlyMyContent ? Plugin.CredContainer.Credential.UserName : OwnerText;
            e.Result = _agol.Search(QueryText, owner, resultCount: ItemsPerPage).Result;
        }
        private void WorkerFinishSearch(object s, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is AgolSearchResultCollection collection)
            {
                ResultCollection = collection;
                if (ResultCollection?.SearchResults?.Count > 0)
                {
                    ShowNoResultsText = false;
                    SearchIsExpanded = false;
                }
                else
                {
                    ShowNoResultsText = true;
                    SearchIsExpanded = true;
                }
            }

            IsBusy = false;
        }

        private void Continue_Execute(object param)
        {
            Plugin.ImportPage.ViewModel.GoToNextControl(ViewType.FeatureSelector, (AgolSearchResult)param);
        }
        private bool Continue_CanExecute(object param)
        {
            return param is AgolSearchResult
                   && !IsBusy;
        }

        private void PageOver_Execute(object param)
        {
            Worker.DoWork(WorkerDoPageOver, WorkerFinishPageOver, argument: param);
        }
        private bool PageOver_CanExecute(object param)
        {
            return !IsBusy
                    && param is string direction
                    && (direction == "Left" ? ResultCollection != null && ResultCollection.Start != 1 : true)
                    && (direction == "Right" ? ResultCollection != null && ResultCollection.NextStart != -1 : true);
        }

        private void WorkerDoPageOver(object s, DoWorkEventArgs e)
        {
            IsBusy = true;
            if (e.Argument is string direction)
            {
                switch (direction)
                {
                    case "Left":
                        e.Result = _agol.SearchByPage(ResultCollection.Query, ResultCollection.ResultDisplayCount, ResultCollection.Start - ResultCollection.ResultDisplayCount).Result;
                        break;
                    case "Right":
                        e.Result = _agol.SearchByPage(ResultCollection.Query, ResultCollection.ResultDisplayCount, ResultCollection.Start + ResultCollection.ResultDisplayCount).Result;
                        break;
                }
            }
        }
        private void WorkerFinishPageOver(object s, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is AgolSearchResultCollection collection)
            {
                ResultCollection = collection;
                if (ResultCollection?.SearchResults?.Count > 0)
                {
                    ShowNoResultsText = false;
                    SearchIsExpanded = false;
                }
                else
                {
                    ShowNoResultsText = true;
                    SearchIsExpanded = true;
                }
            }
            PageOver_Command.Refresh();
            IsBusy = false;
        }
        #endregion
    }
}