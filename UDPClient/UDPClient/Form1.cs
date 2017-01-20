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
	public partial class Client : Form {

		public delegate void stringpass(String s);
		public Client() {
			InitializeComponent();
			new PacketTransfer(setText);
		}

		public void setText(String s) {
			if(this.richTextBox1.InvokeRequired) {
				stringpass d = new stringpass(setText);
				this.Invoke(d, new object[] { s });
			} else {
				richTextBox1.Text += (s + "\n");
				richTextBox1.SelectionStart = richTextBox1.Text.Length;
				richTextBox1.ScrollToCaret();
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
			PacketTransfer.pullIP();
		}

		private void button1_Click(object sender, EventArgs e) {
			PacketTransfer.pullIP();
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
			if(e.KeyChar == (char)Keys.Enter) {
				List<string> args = textBox1.Text.Split(' ').ToList();
				PacketTransfer.sendPacket("console", args.ToArray());
				textBox1.Text = "";
				e.Handled = true;
			}
		}
	}
}
