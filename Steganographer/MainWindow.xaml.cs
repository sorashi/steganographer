using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Steganographer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BitmapSource OriginalBitmap { get; set; }
        public BitmapSource EditedBitmap { get; set; }

        public MainWindow() {
            InitializeComponent();
        }
    }
}
