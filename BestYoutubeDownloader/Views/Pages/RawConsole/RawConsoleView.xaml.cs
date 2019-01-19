using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BestYoutubeDownloader.Views.Pages.RawConsole
{
    /// <summary>
    /// Interaction logic for RawConsoleView.xaml
    /// </summary>
    public partial class RawConsoleView : UserControl
    {
        public RawConsoleViewModel ViewModel => this.DataContext as RawConsoleViewModel;

        public RawConsoleView()
        {
            this.InitializeComponent();


            this.DataContextChanged += (sender, args) =>
            {
                if (args.NewValue.GetType() == typeof(RawConsoleViewModel))
                    this.ViewModel.Outputs.CollectionChanged += OutputsOnCollectionChanged;
            };
        }

        #region Scrolling

        private void OutputsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.ScrollToEndOfDataGrid();
        }

        private void RawConsoleView_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ScrollToEndOfDataGrid();
        }

        private void ScrollToEndOfDataGrid()
        {
            this.GridScrollViewer.ScrollToBottom();
        }

        private void RawConsoleView_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.GridScrollViewer.ScrollToVerticalOffset(this.GridScrollViewer.VerticalOffset - e.Delta);
        }

        #endregion

        #region Value Copying

        private void OutputListBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                if(this.OutputListBox.SelectedItem is RawConsoleOutputViewModel value)
                    Clipboard.SetText(value.Value);
            }
        }

        private void ListBoxItemContextMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if(((MenuItem) sender).DataContext is RawConsoleOutputViewModel value)
                Clipboard.SetText(value.Value);
        }

        #endregion
    }
}
