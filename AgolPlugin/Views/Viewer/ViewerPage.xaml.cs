using AgolPlugin.ViewModels.Viewer;
using System.Windows;
using System.Windows.Controls;

namespace AgolPlugin.Views.Viewer
{
    public partial class ViewerPage : Page
    {
        public ViewerViewModel ViewModel { get; private set; }

        public ViewerPage()
        {
            ViewModel = new ViewerViewModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void CollapseAll_Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (object item in RelatedRecords_TreeView.Items)
            {
                var treeItem = RelatedRecords_TreeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null)
                    SetExpansion(treeItem, false);
                treeItem.IsExpanded = false;
            }
        }

        private void ExpandAll_Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (object item in RelatedRecords_TreeView.Items)
            {
                var treeItem = RelatedRecords_TreeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null)
                    SetExpansion(treeItem, true);
                treeItem.IsExpanded = true;
            }
        }

        private void SetExpansion(ItemsControl item, bool isExpanded)
        {
            foreach (object obj in item.Items)
            {
                ItemsControl childControl = item.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if(childControl != null)
                {
                    SetExpansion(childControl, isExpanded);
                }
                TreeViewItem tvi = childControl as TreeViewItem;
                if (tvi != null)
                    tvi.IsExpanded = isExpanded;
            }
        }
    }
}
