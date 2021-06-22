using AgolPlugin.Helpers;
using AgolPlugin.Models.Agol;
using AgolPlugin.Models.Common;
using AgolPlugin.ViewModels.Import;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AgolPlugin.Views.Import
{
    public partial class LayerSelectorControl : UserControl, IContextIsViewModel
    {
        public LayerSelectorViewModel ViewModel { get; private set; }

        public LayerSelectorControl(AgolSearchResult selectedresult)
        {
            ViewModel = new LayerSelectorViewModel(selectedresult);
            DataContext = ViewModel;
            InitializeComponent();
        }

        public ViewModelBase VM => ViewModel;
        public bool IsViewModelBusy => ViewModel.IsBusy;

        #region Properties
        #endregion

        #region Event Handlers
        private void BlockPropertySortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox box && box.DataContext is LayerConfiguratorViewModel vm)
            {
                vm.PropertySortType = (PropertySortType)box.SelectedIndex;
            }
        }

        private void FieldFilterFieldSelector_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox box)
            {

            }
        }
        private void FieldFilterFieldSelector_DropDownOpened(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //var invalids = ViewModel.FieldFilters;
            //if (sender is ComboBox box)
            //{
            //    foreach (var item in box.Items)
            //    {

            //    }
            //}
        }
        private void FieldFilterSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is FrameworkElement ele && ele.DataContext is FieldFilter filt)
            {
                if (e.AddedItems.Count == 1)
                {
                    var tup = (Tuple<string, string>)e.AddedItems[0];
                    filt.FieldName = tup.Item1;
                    filt.FieldType = tup.Item2;

                }
                else if (e.RemovedItems.Count == 1)
                {
                    filt.FieldName = null;
                    filt.FieldType = null;
                }
                filt.Value1 = null;
                filt.Value2 = null;
            }
        }

        private void OperatorBox_DropDownOpened(object sender, EventArgs e)
        {

        }
        private void OperatorBox_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void OperatorBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        #endregion
    }
}