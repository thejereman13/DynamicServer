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

		Login loginWindow;

		public Client() {
			InitializeComponent();
			new PacketTransfer(setText);
		}

		public void setText(String s) {
			if(this.richTextBox1.InvokeRequired) {
				stringpass d = new stringpass(setText);
				this.Invoke(d, new object[] { s });
			} else {
				List<string> a = new List<string>(richTextBox1.Text.Split(new string[] {"\n"}, StringSplitOptions.RemoveEmptyEntries));
				if(a.Count >= 200) {	//Remove Lines above 200
					a.Reverse();
					a.RemoveRange(200, a.Count - 200);
					string old = "";
					for(int i = a.Count - 1; i >= 0; i--) {
						old += (a[i] + "\n");
					}
					richTextBox1.Text = old;
				}
				
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
			if(e.KeyChar == (char)Keys.Enter && textBox1.Text.Length > 0) {
				List<string> args = textBox1.Text.Split(' ').ToList();
				PacketTransfer.sendPacket("console", args.ToArray());
				textBox1.Text = "";
				e.Handled = true;
			}
		}

		private void fileToolStripMenuItem_Click(object sender, EventArgs e) {

		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private void loginToolStripMenuItem_Click(object sender, EventArgs e) {
			loginWindow = new Login();
			loginWindow.Show();
		}

		private void logoutToolStripMenuItem_Click(object sender, EventArgs e) {
			PacketTransfer.sendPacket("console", new string[] { "logout" });
		}
	}
}
