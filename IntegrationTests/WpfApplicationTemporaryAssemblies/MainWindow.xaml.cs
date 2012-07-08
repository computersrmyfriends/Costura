using ClassLibraryToReference;

namespace WpfApplication
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            textBlock1.Text = Class1.GetTest();
        }
    }
}
