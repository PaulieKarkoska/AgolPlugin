namespace AgolPlugin.Models.Common
{
    public class ViewModelBase : ModelBase
    {
        private ViewType _controlType;
        public ViewType ControlType
        {
            get { return _controlType; }
            set { _controlType = value; OnPropertyChanged(); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged(); }
        }
    }
}