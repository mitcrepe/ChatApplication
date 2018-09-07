﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatApplication.Client {
    public partial class Client : Form {

        Timer _timer;
        Socket _sender;
        bool _connected;
        //byte[] buffer;

        public string ServerAddress { get { return txtAddress.Text; } }
        public int ServerPort { get { return int.Parse(txtPort.Text); } }

        public Client() {
            InitializeComponent();

            txtAddress.Text = "192.168.1.2";
            txtPort.Text = "11500";

            _connected = false;

            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Interval = 100;
            _timer.Tick += _timer_Tick;
        }

        private void _timer_Tick(object sender, EventArgs e) {
            if (_sender.Available > 0) {
                byte[] buffer = new byte[1024];
                int bytesReceived = _sender.Receive(buffer);

                string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                WriteToChatWindow(message);
            }
        }

        private void WriteToChatWindow(string message) {
            txtConversation.Text += $"{message}{Environment.NewLine}";
        }

        private void OnConnect(object sender, EventArgs e) {
            WriteToChatWindow($"Attempting to connect to {ServerAddress}:{ServerPort}");
            IPAddress ipAddress = IPAddress.Parse(ServerAddress);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, ServerPort);

            _sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try {
                _sender.Connect(remoteEP);
                WriteToChatWindow($"Connected!{Environment.NewLine}");
                _connected = true;
                _timer.Start();
            } catch (Exception ex) {
                WriteToChatWindow(ex.Message);
            }
        }

        private void OnSend(object sender, EventArgs e) {
            byte[] msg = Encoding.ASCII.GetBytes(txtMessage.Text);
            _sender.Send(msg);

            txtMessage.Text = string.Empty;
        }

        private void OnMessageReceived() {

        }

    }
}
