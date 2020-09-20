using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Steganographer
{
    /// <summary>
    /// Interaction logic for MessageHiderView.xaml
    /// </summary>
    public partial class MessageHiderView : UserControl
    {
        public MessageHiderView() {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            using (var fileStream = new FileStream(OutputFileChoose.Filename, FileMode.Create)) {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)OutputImage.Source));
                encoder.Save(fileStream);
            }
        }

        private void MessageTextChanged(object sender, TextChangedEventArgs e) {
            if (OutputImage?.Source == null) return;
            var w = (WriteableBitmap)OutputImage.Source;
            var textBox = sender as TextBox;
            var data = new byte[Encoding.UTF8.GetByteCount(textBox.Text) + 2];
            Encoding.UTF8.GetBytes(textBox.Text, 0, textBox.Text.Length, data, 2);
            var lengthBytes = BitConverter.GetBytes((ushort) (data.Length - 2));
            if(lengthBytes.Length != 2) throw new Exception();
            Array.Copy(lengthBytes, data, 2);
            var original = new WriteableBitmap((BitmapSource) InputImage.Source);
            w.Lock();
            long counter = 0;
            unsafe {
                uint* originalPixel = (uint*) original.BackBuffer;
                uint* pixel = (uint*)w.BackBuffer;
                for (int y = 0; y < w.PixelHeight; y++) {
                    for (int x = 0; x < w.PixelWidth; x++) {
                        if (counter < data.Length)
                            *pixel = ByteHider.HideByte(*originalPixel, data[counter]);
                        else
                            *pixel = *originalPixel;
                        ++pixel;
                        ++originalPixel;
                        ++counter;
                    }
                }
            }

            w.AddDirtyRect(new Int32Rect(0, 0, w.PixelWidth, w.PixelHeight));
            w.Unlock();
        }
    }
}
