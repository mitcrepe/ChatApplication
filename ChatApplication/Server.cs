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

        public int Port { get { return int.Parse(txtPort.Text); } }

        public Server() {
            InitializeComponent();

            _clients = new List<Socket>();
        }

        private void OnStart(object sender, EventArgs e) {
            WriteToLog("Server started");
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);

            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try {
                _listener.Bind(localEndPoint);
                _listener.Listen(10);

                _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);
                //timer.Start();                
            } catch (Exception ex) {
                WriteToLog($"Error occured: {ex.Message}");
            }
        }

        private void AcceptCallback(IAsyncResult ar) {
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            _clients.Add(handler);
            //_newConnectionEstablished = true;

            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

            _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);
        }

        private void ReadCallback(IAsyncResult ar) {
            String content = String.Empty;

            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0) {
                content = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                BroadCast(content);
            }

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private void Timer_Tick(object sender, EventArgs e) {
            //lock (_lock) {
            //    if (_newConnectionEstablished) {
            //        _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);
            //        _newConnectionEstablished = false;
            //        WriteToLog("User joined the room");
            //    }

            //    // block this when adding client
            //    foreach (Socket client in _clients) {
            //        if (client.Available > 0) {
            //            byte[] buffer = new byte[1024];
            //            int bytesReceived = client.Receive(buffer);
            //            string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
            //            WriteToLog($"User sent message: {message}");
            //            BroadCast(message);
            //        }
            //    }
            //}
            
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

        // State object for reading client data asynchronously  
        public class StateObject {           
            public Socket workSocket = null; 
            public const int BufferSize = 1024;
            public byte[] buffer = new byte[BufferSize];
            public StringBuilder sb = new StringBuilder();
        }
    }
}
