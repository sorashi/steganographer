using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Steganographer
{
    public enum FillStart
    {
        FromBeginning = 0, FromEnd = 1, FromCenter = 2
    }

    public static class FillStartUtility
    {
        public static int GetFillStartIndex(FillStart fill, int width, int height, int dataLength) {
            switch (fill) {
                case FillStart.FromBeginning:
                    return new MessageHeader {ChecksumType = new XorChecksumCalculator()}.Length;
                case FillStart.FromCenter:
                    return (width * height - dataLength) / 2;
                case FillStart.FromEnd:
                    return width * height - dataLength;
                default:
                    throw new NotSupportedException();
            }
        }
    }
    /// <summary>
    /// Interaction logic for MessageHiderView.xaml
    /// </summary>
    public partial class MessageHiderView : UserControl
    {
        private readonly ChecksumCalculator _checksumCalculator = new XorChecksumCalculator();

        public MessageHiderView() {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            if (OutputFileChoose?.Filename == null) return;
            if (IsOverCapacity) {
                MessageBox.Show("The message is too long");
                return;
            }
            using (var fileStream = new FileStream(OutputFileChoose.Filename, FileMode.Create)) {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)OutputImage.Source));
                encoder.Save(fileStream);
            }
        }

        private bool IsOverCapacity { get; set; }
        private void MessageTextChanged(object sender, TextChangedEventArgs e) {
            if (OutputImage?.Source == null) return;
            var w = (WriteableBitmap)OutputImage.Source;

            var textBox = sender as TextBox;

            IsOverCapacity = false;
            var data = Encoding.UTF8.GetBytes(textBox.Text);
            var header = new MessageHeader {
                Fill = (FillStart) FillComboBox.SelectedIndex,
                ChecksumType = new XorChecksumCalculator(),
                DataLength = checked((ushort) data.Length)
            };
            try {
                header.DataSpacing = byte.Parse(SpaceTextBox.Text);
                if(header.DataSpacing > 254) throw new Exception();
            }
            catch {
                MessageBox.Show("Invalid spacing, allowed range is 0-254");
                return;
            }

            var capacity = Math.Min(ushort.MaxValue,
                (w.PixelWidth * w.PixelHeight - header.Length) / (header.DataSpacing + 1));
            ProgressBar.Maximum = capacity;
            var textDataLength = Encoding.UTF8.GetByteCount(textBox.Text);
            ProgressBar.Value = textDataLength;
            if (textDataLength > capacity) {
                IsOverCapacity = true;
                return;
            }

            header.SetChecksumFromData(data);
            int dataStartPixelIndex =
                FillStartUtility.GetFillStartIndex(header.Fill, w.PixelWidth, w.PixelHeight, data.Length * (header.DataSpacing + 1));
            var original = new WriteableBitmap((BitmapSource) InputImage.Source);
            w.Lock();
            long counter = 0;
            unsafe {
                uint* originalPixel = (uint*) original.BackBuffer;
                uint* pixel = (uint*)w.BackBuffer;
                var headerBytes = header.GetBytes();
                // place header bytes
                foreach (byte b in headerBytes) {
                    *pixel = ByteHider.HideByte(*originalPixel, b);
                    pixel++;
                    originalPixel++;
                    counter++;
                }
                // copy unmodified bytes
                while (counter < dataStartPixelIndex) {
                    *pixel = *originalPixel;
                    pixel++;
                    originalPixel++;
                    counter++;
                }
                // place data
                foreach (byte b in data) {
                    *pixel = ByteHider.HideByte(*originalPixel, b);
                    pixel++;
                    originalPixel++;
                    counter++;
                    // copy spacing pixels
                    for (int i = 0; i < header.DataSpacing; i++) {
                        *pixel = *originalPixel;
                        pixel++;
                        originalPixel++;
                        counter++;
                    }
                }
                // copy unmodified bytes
                while (counter < w.PixelWidth * w.PixelHeight) {
                    *pixel = *originalPixel;
                    pixel++;
                    originalPixel++;
                    counter++;
                }
            }

            w.AddDirtyRect(new Int32Rect(0, 0, w.PixelWidth, w.PixelHeight));
            w.Unlock();
        }
    }
}
