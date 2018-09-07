using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatApplication.Server {
    public partial class Server : Form {

        public int Port { get { return int.Parse(txtPort.Text); } }

        public Server() {
            InitializeComponent();
        }

        private void OnStart(object sender, EventArgs e) {

        }
    }
}
