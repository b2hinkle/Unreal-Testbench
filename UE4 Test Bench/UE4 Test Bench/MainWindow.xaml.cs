using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace UE4_Test_Bench
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void OpenFile()
        {
            OpenFileDialog FileOpener = new OpenFileDialog()
            {
                Title = "Select File",
                Filter = "uproject file | *.uproject",
                FileName = " "
            };

            if (FileOpener.ShowDialog() == true)
            {
                FilePathTxtBox.Text = FileOpener.FileName;
            }

        }
    }
}
