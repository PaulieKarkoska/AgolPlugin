using AgolPlugin.Models.Agol;
using AgolPlugin.Models.Common;
using AgolPlugin.Views.Import;
using System;
using System.Collections.Generic;

namespace AgolPlugin.ViewModels.Import
{
    public class ImportViewModel : ViewModelBase
    {
        private Stack<IContextIsViewModel> _controlStack = new Stack<IContextIsViewModel>();
        public BasicCommand Previous_Command { get; private set; }

        public ImportViewModel()
        {
            CurrentControl = new SearchControl();

            Previous_Command = new BasicCommand(Previous_Execute, Previous_CanExecute);
        }

        #region Properties
        private bool _canGoBack;
        public bool CanGoBack
        {
            get { return _canGoBack; }
            set { _canGoBack = value; OnPropertyChanged(); }
        }

        private IContextIsViewModel _control;
        public IContextIsViewModel CurrentControl
        {
            get { return _control; }
            set { _control = value; OnPropertyChanged(); }
        }
        #endregion

        #region Navigation
        public void GoToNextControl(ViewType type, object argument)
        {
            OnGoingForward();
            IContextIsViewModel nextControl = null;
            switch (type)
            {
                case ViewType.FeatureServiceSearch:
                    nextControl = new SearchControl();
                    break;
                case ViewType.FeatureSelector:
                    nextControl = new LayerSelectorControl((AgolSearchResult)argument);
                    break;
                case ViewType.DataPreviewer:
                    nextControl = new DataPreviewControl((LayerConfiguratorViewModel)argument);
                    break;
                case ViewType.RunImport:
                    _controlStack.Clear();
                    nextControl = new SearchControl();
                    break;
            }
            _controlStack.Push(CurrentControl);
            CurrentControl = nextControl;
            CanGoBack = _controlStack.Count > 0;
        }
        #endregion

        #region Interaction
        private void Previous_Execute(object param)
        {
            OnGoingBack();
            CurrentControl = _controlStack.Pop();
            CanGoBack = _controlStack.Count > 0;
        }
        private bool Previous_CanExecute(object param)
        {
            return !CurrentControl.IsViewModelBusy
                   && CanGoBack;
        }
        #endregion

        #region Events
        public event EventHandler GoingBack;
        private void OnGoingBack()
        {
            GoingBack?.Invoke(this, new EventArgs());
        }

        public event EventHandler GoingForward;
        private void OnGoingForward()
        {
            GoingForward?.Invoke(this, new EventArgs());
        }
        #endregion
    }
}