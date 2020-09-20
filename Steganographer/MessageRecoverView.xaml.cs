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
    public partial class MessageRecoverView : UserControl
    {
        public MessageRecoverView() {
            InitializeComponent();
        }

        private void InputFileChoose_OnFilenameChanged(object sender, EventArgs e) {
            if (InputImage?.Source == null) return;
            var w = new WriteableBitmap((BitmapSource)InputImage.Source);
            long counter = 0;
            unsafe {
                uint* pixel = (uint*)w.BackBuffer;
                byte[] lengthBuffer = new byte[2];
                lengthBuffer[0] = ByteHider.RecoverByte(*pixel);
                pixel++;
                lengthBuffer[1] = ByteHider.RecoverByte(*pixel);
                var dataLength = BitConverter.ToUInt16(lengthBuffer, 0);
                var dataBuffer = new byte[dataLength];
                for (int i = 0; i < dataLength; i++) {
                    pixel++;
                    dataBuffer[i] = ByteHider.RecoverByte(*pixel);
                }

                OutputTextBox.Text = Encoding.UTF8.GetString(dataBuffer);
            }
        }
    }
}
