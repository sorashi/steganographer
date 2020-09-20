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
                byte[] lengthBuffer = new byte[2];
                lengthBuffer[0] = ByteHider.RecoverByte(*pixel);
                pixel++;
                lengthBuffer[1] = ByteHider.RecoverByte(*pixel);
                var dataLength = BitConverter.ToUInt16(lengthBuffer, 0);
                if(dataLength > w.Width * w.Height - 2 - _checksumCalculator.ChecksumSize) ShowImageInvalidMessage();
                var dataBuffer = new byte[dataLength];
                for (int i = 0; i < dataLength; i++) {
                    pixel++;
                    dataBuffer[i] = ByteHider.RecoverByte(*pixel);
                }

                var checksumBuffer = new byte[_checksumCalculator.ChecksumSize];
                for (int i = 0; i < checksumBuffer.Length; i++) {
                    pixel++;
                    checksumBuffer[i] = ByteHider.RecoverByte(*pixel);
                }

                // check the checksum
                var checksumMatches = Enumerable.SequenceEqual(checksumBuffer, _checksumCalculator.GetChecksum(dataBuffer));
                if (!checksumMatches) ShowImageInvalidMessage();
                else OutputTextBox.Text = Encoding.UTF8.GetString(dataBuffer);
            }
        }

        private void ShowImageInvalidMessage() {
            MessageBox.Show("The image does not contain a valid message");
        }
    }
}
