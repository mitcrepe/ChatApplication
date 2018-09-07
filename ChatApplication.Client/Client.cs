using System;
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

        Socket _sender;
        bool _connected;
        byte[] buffer;

        public string ServerAddress { get { return txtAddress.Text; } }
        public int ServerPort { get { return int.Parse(txtPort.Text); } }

        public Client() {
            InitializeComponent();

            txtAddress.Text = "192.168.111.63";
            txtPort.Text = "11500";

            _connected = false;
        }

        private void WriteToChatWindow(string message) {
            txtConversation.Text += $"{message}{Environment.NewLine}";
        }

        private void OnConnect(object sender, EventArgs e) {
            IPAddress ipAddress = IPAddress.Parse(ServerAddress);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, ServerPort);

            _sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try {
                _sender.Connect(remoteEP);
                buffer = new byte[1024];
                //_sender.BeginReceive(buffer, SocketFlags.Broadcast, OnMessageReceived, )
            } catch (Exception ex) {
                WriteToChatWindow(ex.Message);
            }
        }

        private void OnSend(object sender, EventArgs e) {
            byte[] msg = Encoding.ASCII.GetBytes(txtMessage.Text);
            _sender.Send(msg);
        }

        private void OnMessageReceived() {

        }

    }
}
