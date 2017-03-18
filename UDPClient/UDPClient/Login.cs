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
	public partial class Login : Form {
		public Login() {
			InitializeComponent();
		}

		private void LoginButton_Click(object sender, EventArgs e) {
			submitLogin();
		}

		private void submitLogin() {
			PacketTransfer.sendPacket("console", new string[] { "login", UserName.Text, Password.Text });
			this.Close();
		}

		private void UserName_KeyPress(object sender, KeyPressEventArgs e) {
			if(e.KeyChar == (char)Keys.Enter && UserName.Text.Length > 0) {
				if(Password.Text.Length > 0) {
					submitLogin();
				} else {
					Password.Focus();
				}
			}
		}

		private void Password_KeyPress(object sender, KeyPressEventArgs e) {
			if(e.KeyChar == (char)Keys.Enter && Password.Text.Length > 0) {
				if(UserName.Text.Length > 0) {
					submitLogin();
				} else {
					UserName.Focus();
				}
			}
		}
	}
}
