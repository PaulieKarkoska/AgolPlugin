using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AgolPlugin.Models.Agol
{
    public class AgolRecordGroup : ObservableCollection<AgolRecord>
    {
        public AgolRecordGroup(string name, IEnumerable<AgolRecord> records) : base(records)
        {
            Name = name;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(new PropertyChangedEventArgs(nameof(Name))); }
        }
    }
}