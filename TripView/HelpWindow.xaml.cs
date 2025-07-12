using System.Windows;

namespace TripView
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public string HelpTextFromCommandLine { get; set; }
        public HelpWindow(string helptext)
        {
            HelpTextFromCommandLine = helptext; //TODO: this should be a View Model.
            DataContext = this;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
