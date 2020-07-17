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
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace UE4_Test_Bench
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Process ServerProcess;
        List<Process> ClientProcesses = new List<Process>();

        string localIP = "";

        public MainWindow()
        {
            InitializeComponent();
            localIP = GetLocalIPAddress();
            UProjectFilePathTxtBox.Text = MySettings.Default.UProjectFilePath;
            EngineFilePathTxtBox.Text = MySettings.Default.UnrealEngineFilePath;
            MySettings.Default.ServerArgs = "\"" + MySettings.Default.UProjectFilePath + "\"" + " -server -nosteam";
            MySettings.Default.ClientArgs = "\"" + MySettings.Default.UProjectFilePath + "\" " + localIP + " -game -ResX=800 -ResY=900 -WinX=0 -WinY=20 -nosteam";
            MySettings.Default.Save();
        }

        private void OpenUProjectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog FileOpener = new OpenFileDialog()
            {
                Title = "Select File",
                Filter = "uproject file | *.uproject",
                FileName = " "
            };

            if (FileOpener.ShowDialog() == true)
            {
                UProjectFilePathTxtBox.Text = FileOpener.FileName;

                MySettings.Default.UProjectFilePath = UProjectFilePathTxtBox.Text;
                MySettings.Default.Save();
                MySettings.Default.ServerArgs = "\"" + MySettings.Default.UProjectFilePath + "\"" + " -server -nosteam";
                MySettings.Default.ClientArgs = "\"" + MySettings.Default.UProjectFilePath + "\" " + localIP + " -game -ResX=800 -ResY=900 -WinX=0 -WinY=20 -nosteam";
            }
        }
        private void OpenEngineFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog FileOpener = new OpenFileDialog()
            {
                Title = "Select File",
                Filter = "UnrealEngine | *.exe",
                FileName = " "
            };

            if (FileOpener.ShowDialog() == true)
            {
                EngineFilePathTxtBox.Text = FileOpener.FileName;
                MySettings.Default.UnrealEngineFilePath = EngineFilePathTxtBox.Text;
                MySettings.Default.Save();
                MySettings.Default.ServerArgs = "\"" + MySettings.Default.UProjectFilePath + "\"" + " -server -nosteam";
                MySettings.Default.ClientArgs = "\"" + MySettings.Default.UProjectFilePath + "\" " + localIP + " -game -ResX=800 -ResY=900 -WinX=0 -WinY=20 -nosteam";
            }
        }


        /*string serverArgs = *//*"\"" + *//*UProjectFilePathTxtBox.Text*//* + "\""*//* + " -server -log -nosteam";
        string clientArgs = UProjectFilePathTxtBox.Text + localIP + " -game -ResX=800 -ResY=900 -WinX=0 -WinY=20 -log -nosteam";*/

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            if (ServerProcess == null)
            {
                ServerProcess = new Process();
                ServerProcess.EnableRaisingEvents = true;
                ServerProcess.Exited += new EventHandler(ServerProcess_Exited);
                string g = MySettings.Default.ServerArgs;
                ServerProcess.StartInfo = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    FileName = EngineFilePathTxtBox.Text,
                    Arguments = MySettings.Default.ServerArgs,
                };


                if (ServerProcess.Start())
                {
                    StartServer.Background = new SolidColorBrush(Color.FromRgb(131, 251, 186));
                }
            }
            else
            {
                ServerProcess?.Kill();
                StartServer.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
            }
        }

        private void ServerProcess_Exited(object sender, System.EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                StartServer.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));  // access button from main thread
            });
            
        }






        private void StartClient_Click(object sender, RoutedEventArgs e)
        {
            Process NewClient = new Process();
            NewClient.EnableRaisingEvents = true;
            NewClient.Exited += new EventHandler(ClientProcess_Exited);
            NewClient.StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                FileName = EngineFilePathTxtBox.Text,
                Arguments = MySettings.Default.ClientArgs
            };

            if (NewClient.Start())
            {
                ClientProcesses.Add(NewClient);
                ClientCountTxt.Text = ClientProcesses.Count().ToString();
                
            }
        }

        private void ClientProcess_Exited(object sender, System.EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                /*ClientProcesses.Remove(process);*/ // Locate the process object so we can remove it from the list
                /*ClientCountTxt.Text = (int.Parse(ClientCountTxt.Text) - 1).ToString();*/      //  Client processes count is currently faked when decrementing but shouldn't be innacturate. Quick implementation for now
            });

        }












        public static string GetLocalIPAddress()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }

        private void OnApplicationEnd(object sender, CancelEventArgs e)
        {
            // Faster to kill clients first since server won't have to redirect all clients to their own maps before server gets killed
            foreach (Process P in ClientProcesses)
            {
                P.Exited -= new EventHandler(ClientProcess_Exited);
                P?.Kill();
            }
            ServerProcess.Exited -= new EventHandler(ServerProcess_Exited);
            ServerProcess?.Kill();
        }

        
    }
}
