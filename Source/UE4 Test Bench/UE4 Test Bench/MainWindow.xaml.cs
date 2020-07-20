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

        List<string> ServerArgs = new List<string>();
        List<string> ClientArgs = new List<string>();

        SolidColorBrush NormalColor = new SolidColorBrush(Color.FromRgb(221, 221, 221));
        SolidColorBrush EnabledColor = new SolidColorBrush(Color.FromRgb(158, 255, 148));
        SolidColorBrush ServerRunningColor = new SolidColorBrush(Color.FromRgb(129, 211, 230));

        string localIP = "";
        int clientWinX = 1500;
        int clientWindowOffset = -20;
        bool applyClientWindowOffset = true;

        bool serverNosteam = true;
        bool serverLog = false;

        bool clientAutojoin = true;
        bool clientLog = false;
        bool clientWindowed = true;
        bool clientNosteam = true;
        bool clientInstalled = false;

        public MainWindow()
        {
            InitializeComponent();
            localIP = GetLocalIPAddress();
            UProjectFilePathTxtBox.Text = MySettings.Default.UProjectFilePath;
            EngineFilePathTxtBox.Text = MySettings.Default.UnrealEngineFilePath;
            SetServerArgs();
            SetClientArgs();
            MySettings.Default.Save();
        }

        private void SetServerArgs()
        {
            ServerArgs.Clear();

            ServerArgs.Add(MySettings.Default.UProjectFilePath);
            ServerArgs.Add("-server");
            if (serverLog)
            {
                ServerArgs.Add("-log");
            }
            if (serverNosteam)
            {
                ServerArgs.Add("-nosteam");
            }

        
        }
        private void SetClientArgs()
        {
            ClientArgs.Clear();

            ClientArgs.Add(MySettings.Default.UProjectFilePath);
            if (clientAutojoin)
            {
                ClientArgs.Add(localIP);
            }
            ClientArgs.Add("-game");
            if (clientWindowed)
            {
                ClientArgs.Add("-windowed");
            }
            if (clientLog)
            {
                ClientArgs.Add("-log");
            }
            ClientArgs.Add("-ConsoleX=1000");
            ClientArgs.Add("-ConsoleY=500");
            if (applyClientWindowOffset)
            {
                ClientArgs.Add("-WinX=" + (clientWinX - clientWindowOffset).ToString());
            }
            else
            {
                ClientArgs.Add("-WinX="+clientWinX.ToString());
            }
            ClientArgs.Add("-WinY=190");
            ClientArgs.Add("-ResX=400");
            ClientArgs.Add("-ResY=300");
            if (clientNosteam)
            {
                ClientArgs.Add("-nosteam");
            }
            if (clientInstalled)
            {
                ClientArgs.Add("-installed");
            }
            ClientArgs.Add("-NOSPLASH");
        }
        
        private void OpenUProjectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog FileOpener = new OpenFileDialog()
            {
                Title = "Select File",
                Filter = "uproject file | *.uproject"
            };

            if (FileOpener.ShowDialog() == true)
            {
                UProjectFilePathTxtBox.Text = FileOpener.FileName;
                MySettings.Default.UProjectFilePath = UProjectFilePathTxtBox.Text;
                MySettings.Default.Save();
            }
        }
        private void OpenEngineFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog FileOpener = new OpenFileDialog()
            {
                Title = "Select File",
                Filter = "UnrealEngine | *.exe"
            };

            if (FileOpener.ShowDialog() == true)
            {
                EngineFilePathTxtBox.Text = FileOpener.FileName;
                MySettings.Default.UnrealEngineFilePath = EngineFilePathTxtBox.Text;
                MySettings.Default.Save();
            }
        }
        
        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            if (ServerProcess == null)      // Initial creation (first time function is called)
            {
                ServerProcess = new Process();
                ServerProcess.EnableRaisingEvents = true;
                ServerProcess.Exited += new EventHandler(ServerProcess_Exited);
                ServerProcess.StartInfo = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    FileName = EngineFilePathTxtBox.Text
                };
                SetProcessArgumentsForServer();

                if (ServerProcess.Start())
                {
                    OnServerInstanceProcessCreated();
                }
                return;
            }

            if (ServerProcess.HasExited)
            {
                ServerProcess = new Process();
                ServerProcess.EnableRaisingEvents = true;
                ServerProcess.Exited += new EventHandler(ServerProcess_Exited);
                ServerProcess.StartInfo = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    FileName = EngineFilePathTxtBox.Text
                };
                SetProcessArgumentsForServer();

                if (ServerProcess.Start())
                {
                    OnServerInstanceProcessCreated();
                }
            }
            else
            {
                ServerProcess?.Kill();
            }

        }

        private void OnServerInstanceProcessCreated()
        {
            StartServer.Background = ServerRunningColor;
            ServerButtonTextBlock.Text = "Kill Server Instance";
        }

        private void ServerProcess_Exited(object sender, System.EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                StartServer.Background = NormalColor;  // access button from main thread
                ServerButtonTextBlock.Text = "Create Server Instance";
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
                FileName = EngineFilePathTxtBox.Text
            };
            SetProcessArgumentsForClient(NewClient);

            if (NewClient.Start())
            {
                OnClientInstanceProcessCreated(NewClient);
            }
        }

        private void OnClientInstanceProcessCreated(Process NewClient)
        {
            ClientProcesses.Add(NewClient);
            ClientCountTxt.Text = ClientProcesses.Count().ToString();

            if (applyClientWindowOffset)
            {
                clientWinX = clientWinX + clientWindowOffset;
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



        private void SetProcessArgumentsForServer()
        {
            SetServerArgs();

            foreach (string s in ServerArgs)
            {
                ServerProcess.StartInfo.ArgumentList.Add(s);
            }
        }
        private void SetProcessArgumentsForClient(Process NewClient)
        {
            SetClientArgs();


            foreach (string s in ClientArgs)
            {
                NewClient.StartInfo.ArgumentList.Add(s);
            }
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
            KillAllProcesses();
        }
        private void KillAllProcesses()
        {
            // Faster to kill clients first since server won't have to redirect all clients to their own maps before server gets killed
            foreach (Process P in ClientProcesses)
            {
                P.Exited -= new EventHandler(ClientProcess_Exited);
                P?.Kill();
            }
            if (ServerProcess != null)
            {
                ServerProcess.Exited -= new EventHandler(ServerProcess_Exited);
                ServerProcess.Kill();
            }

            ClientProcesses.Clear();
            StartServer.Background = NormalColor;
            ServerButtonTextBlock.Text = "Create Server Instance";
            ClientCountTxt.Text = ClientProcesses.Count().ToString();



        }

        private void ServerLogButton_Click(object sender, RoutedEventArgs e)
        {
            if (serverLog)
            {
                serverLog = false;
                ServerLogButton.Background = NormalColor;
            }
            else
            {
                serverLog = true;
                ServerLogButton.Background = EnabledColor;
            }
        }
        private void ServerNosteamButton_Click(object sender, RoutedEventArgs e)
        {
            if (serverNosteam)
            {
                serverNosteam = false;
                ServerNosteamButton.Background = NormalColor;
            }
            else
            {
                serverNosteam = true;
                ServerNosteamButton.Background = EnabledColor;
            }
        }
        private void ClientLogButton_Click(object sender, RoutedEventArgs e)
        {
            if (clientLog)
            {
                clientLog = false;
                ClientLogButton.Background = NormalColor;
            }
            else
            {
                clientLog = true;
                ClientLogButton.Background = EnabledColor;
            }
        }
        private void ClientWindowedButton_Click(object sender, RoutedEventArgs e)
        {
            if (clientWindowed)
            {
                clientWindowed = false;
                ClientWindowedButton.Background = NormalColor;
            }
            else
            {
                clientWindowed = true;
                ClientWindowedButton.Background = EnabledColor;
            }
        }
        private void ClientNosteamButton_Click(object sender, RoutedEventArgs e)
        {
            if (clientNosteam)
            {
                clientNosteam = false;
                ClientNosteamButton.Background = NormalColor;
            }
            else
            {
                clientNosteam = true;
                ClientNosteamButton.Background = EnabledColor;
            }
        }
        private void ClientInstalledButton_Click(object sender, RoutedEventArgs e)
        {
            if (clientInstalled)
            {
                clientInstalled = false;
                ClientInstalledButton.Background = NormalColor;
            }
            else
            {
                clientInstalled = true;
                ClientInstalledButton.Background = EnabledColor;
            }
        }
        private void ClientAutoJoinButton_Click(object sender, RoutedEventArgs e)
        {
            if (clientAutojoin)
            {
                clientAutojoin = false;
                ClientAutojoinButton.Background = NormalColor;
            }
            else
            {
                clientAutojoin = true;
                ClientAutojoinButton.Background = EnabledColor;
            }
        }

        private void DisposeAll_Click(object sender, RoutedEventArgs e)
        {
            KillAllProcesses();
        }
    }
}
