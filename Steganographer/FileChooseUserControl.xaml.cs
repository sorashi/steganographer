using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;

namespace Steganographer
{
    /// <summary>
    /// Interaction logic for FileChooseView.xaml
    /// </summary>
    public partial class FileChooseView
    {
        public FileChooseView() {
            ChooseFileCommand = new DelegateCommand(ChooseFileCommandHandler);
            InitializeComponent();
        }

        public static readonly DependencyProperty ChooseTypeProperty = DependencyProperty.Register(nameof(ChooseType),
            typeof(FileChooseType), typeof(FileChooseView), new PropertyMetadata(FileChooseType.Open));

        public FileChooseType ChooseType {
            get => (FileChooseType) GetValue(ChooseTypeProperty);
            set => SetValue(ChooseTypeProperty, value);
        }

        public static readonly DependencyProperty FilenameProperty = DependencyProperty.Register(nameof(Filename),
            typeof(string), typeof(FileChooseView), new PropertyMetadata(null));

        public string Filename {
            get => (string) GetValue(FilenameProperty);
            set {
                SetValue(FilenameProperty, value);
                FilenameChanged?.Invoke(this, null);
            }
        }

        public ICommand ChooseFileCommand { get; }

        private void ChooseFileCommandHandler(object parameter) {
            FileDialog dialog;
            switch (ChooseType) {
                case FileChooseType.Open:
                    dialog = new OpenFileDialog();
                    break;
                case FileChooseType.Save:
                    dialog = new SaveFileDialog {
                        Filter = "PNG|*.png"
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            dialog.FileName = Filename;
            if (dialog.ShowDialog() ?? false) {
                Filename = dialog.FileName;
            }
        }

        public event EventHandler FilenameChanged;
    }
}