using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Steganographer
{
    /// <summary>
    /// Interaction logic for MessageHiderView.xaml
    /// </summary>
    public partial class MessageRecoverView : UserControl
    {
        private ChecksumCalculator _checksumCalculator = new XorChecksumCalculator();
        public MessageRecoverView() {
            InitializeComponent();
        }

        private void InputFileChoose_OnFilenameChanged(object sender, EventArgs e) {
            if (InputImage?.Source == null) return;
            var w = new WriteableBitmap((BitmapSource)InputImage.Source);
            unsafe {
                uint* pixel = (uint*)w.BackBuffer;
                MessageHeader header = new MessageHeader {ChecksumType = new XorChecksumCalculator()};
                var headerData = new byte[header.Length];
                for (int i = 0; i < headerData.Length; i++) {
                    headerData[i] = ByteHider.RecoverByte(*pixel);
                    pixel++;
                }
                try {
                    header = MessageHeader.FromBytes(headerData);
                }
                catch (FormatException) {
                    ShowImageInvalidMessage();
                    return;
                }
                pixel++;
                int startIndex = FillStartUtility.GetFillStartIndex(header.Fill, w.PixelWidth, w.PixelHeight, header.DataLength);

                pixel += startIndex - header.Length - 1;
                var dataBuffer = new byte[header.DataLength];
                for (int i = 0; i < dataBuffer.Length; i++) {
                    dataBuffer[i] = ByteHider.RecoverByte(*pixel);
                    pixel++;
                }

                // check the checksum
                var checksumMatches = header.Checksum.SequenceEqual(_checksumCalculator.GetChecksum(dataBuffer));
                if (!checksumMatches) ShowImageInvalidMessage();
                else OutputTextBox.Text = Encoding.UTF8.GetString(dataBuffer);
            }
        }

        private void ShowImageInvalidMessage() {
            MessageBox.Show("The image does not contain a valid message");
        }
    }
}
