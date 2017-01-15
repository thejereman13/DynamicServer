using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPClient {
	public partial class Form1 : Form {

		public delegate void stringpass(String s);
		public Form1() {
			InitializeComponent();
			new PacketTransfer(setText);
		}

		public void setText(String s) {
			if(this.textBox1.InvokeRequired) {
				stringpass d = new stringpass(setText);
				this.Invoke(d, new object[] { s });
			} else {
				textBox1.Text = s;
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
			PacketTransfer.pullIP();
		}

		private void button1_Click(object sender, EventArgs e) {
			PacketTransfer.pullIP();
		}
	}
}
