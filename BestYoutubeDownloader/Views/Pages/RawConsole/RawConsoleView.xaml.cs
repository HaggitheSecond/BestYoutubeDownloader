using System;
using System.Collections.Generic;
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
        public RawConsoleView()
        {
            InitializeComponent();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.ConsoleTextBox.ScrollToEnd();
        }
    }
}
