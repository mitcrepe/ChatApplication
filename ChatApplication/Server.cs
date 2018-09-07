using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatApplication.Server {
    public partial class Server : Form {

        private readonly object _lock = new object();

        Socket _listener;
        List<Socket> _clients;
        System.Windows.Forms.Timer timer;
        bool _newConnectionEstablished;

        public int Port { get { return int.Parse(txtPort.Text); } }

        public Server() {
            InitializeComponent();

            _clients = new List<Socket>();
            _newConnectionEstablished = false;

            timer = new System.Windows.Forms.Timer();
            timer.Enabled = false;
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
        }

        private void OnStart(object sender, EventArgs e) {
            WriteToLog("Server started");
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[2];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);

            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try {
                _listener.Bind(localEndPoint);
                _listener.Listen(10);

                _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);
                timer.Start();
                
            } catch (Exception ex) {
                WriteToLog($"Error occured: {ex.Message}");
            }
        }

        private void AcceptCallback(IAsyncResult ar) {
            lock (_lock) {
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);
                _clients.Add(handler);
                WriteToLog("User joined the room");
                _newConnectionEstablished = true;
            }
        }

        private void Timer_Tick(object sender, EventArgs e) {
            lock (_lock) {
                if (_newConnectionEstablished) {
                    _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);
                    _newConnectionEstablished = false;
                }

                // block this when adding client
                foreach (Socket client in _clients) {
                    if (client.Available > 0) {
                        byte[] buffer = new byte[1024];
                        int bytesReceived = client.Receive(buffer);
                        string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                        WriteToLog($"User sent message: {message}");
                        BroadCast(message);
                    }
                }
            }
            
        }

        private void BroadCast(string message) {
            foreach (Socket client in _clients) {
                byte[] msg = Encoding.ASCII.GetBytes(message);
                client.Send(msg);
            }
        }

        private void WriteToLog(string message) {
            txtLog.Text += $"{message}{Environment.NewLine}";
        }
    }
}
