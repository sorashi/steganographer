using System.Windows;
using System.Windows.Media.Imaging;

namespace Steganographer
{
    public class MessageHiderViewModel : DependencyObject
    {
        public static readonly DependencyProperty OriginalImageProperty = DependencyProperty.Register(nameof(OriginalImage), typeof(BitmapSource), typeof(MessageHiderViewModel));

        public BitmapSource OriginalImage {
            get => (BitmapSource) GetValue(OriginalImageProperty);
            set => SetValue(OriginalImageProperty, value);
        }

        public static readonly DependencyProperty ResultImageProperty = DependencyProperty.Register(nameof(ResultImage), typeof(BitmapSource), typeof(MessageHiderViewModel));
        public BitmapSource ResultImage {
            get => (BitmapSource)GetValue(ResultImageProperty);
            set => SetValue(ResultImageProperty, value);
        }
    }
}