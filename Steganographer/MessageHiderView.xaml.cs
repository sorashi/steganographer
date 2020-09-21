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
            var capacity = Math.Min(ushort.MaxValue,
                w.PixelWidth * w.PixelHeight - 2 - _checksumCalculator.ChecksumSize - 1); // 2 is ushort and 1 is fill byte
            ProgressBar.Maximum = capacity;
            var textBox = sender as TextBox;
            var textDataLength = Encoding.UTF8.GetByteCount(textBox.Text);
            ProgressBar.Value = textDataLength;
            if (textDataLength > capacity) {
                IsOverCapacity = true;
                return;
            }

            IsOverCapacity = false;
            // [fill start] [ushort text length] [text data] [checksum]
            var data = new byte[textDataLength + _checksumCalculator.ChecksumSize];

            var fill = (FillStart) FillComboBox.SelectedIndex;

            var lengthBytes = BitConverter.GetBytes((ushort) textDataLength);
            if(lengthBytes.Length != 2) throw new Exception();

            Encoding.UTF8.GetBytes(textBox.Text, 0, textBox.Text.Length, data, 0);

            var checksumData = _checksumCalculator.GetChecksum(data, 0, textDataLength);
            Array.Copy(checksumData, 0, data, textDataLength, checksumData.Length);
            int dataStartPixelIndex;
            switch (fill) {
                case FillStart.FromBeginning:
                    dataStartPixelIndex = 3;
                    break;
                case FillStart.FromCenter:
                    dataStartPixelIndex = (w.PixelWidth * w.PixelHeight - data.Length) / 2;
                    break;
                case FillStart.FromEnd:
                    dataStartPixelIndex = w.PixelWidth * w.PixelHeight - data.Length;
                    break;
                default:
                    throw new NotSupportedException();
            }
            var original = new WriteableBitmap((BitmapSource) InputImage.Source);
            w.Lock();
            long counter = 0;
            int dataIndex = 0;
            unsafe {
                uint* originalPixel = (uint*) original.BackBuffer;
                uint* pixel = (uint*)w.BackBuffer;
                for (int y = 0; y < w.PixelHeight; y++) {
                    for (int x = 0; x < w.PixelWidth; x++) {
                        if (counter == 0)
                            *pixel = ByteHider.HideByte(*originalPixel, (byte) fill);
                        else if (counter <= 2)
                            *pixel = ByteHider.HideByte(*originalPixel, lengthBytes[counter - 1]);
                        else if (counter >= dataStartPixelIndex && counter < dataStartPixelIndex + data.Length) {
                            *pixel = ByteHider.HideByte(*originalPixel, data[dataIndex]);
                            dataIndex++;
                        }
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
