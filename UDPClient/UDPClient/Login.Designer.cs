namespace UDPClient {
	partial class Login {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.UserName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.Password = new System.Windows.Forms.TextBox();
			this.LoginButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "UserName:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// UserName
			// 
			this.UserName.Location = new System.Drawing.Point(93, 8);
			this.UserName.Name = "UserName";
			this.UserName.Size = new System.Drawing.Size(268, 20);
			this.UserName.TabIndex = 1;
			this.UserName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserName_KeyPress);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(12, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(73, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "Password:";
			// 
			// Password
			// 
			this.Password.Location = new System.Drawing.Point(93, 50);
			this.Password.Name = "Password";
			this.Password.PasswordChar = '*';
			this.Password.Size = new System.Drawing.Size(268, 20);
			this.Password.TabIndex = 3;
			this.Password.UseSystemPasswordChar = true;
			this.Password.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Password_KeyPress);
			// 
			// LoginButton
			// 
			this.LoginButton.Location = new System.Drawing.Point(143, 82);
			this.LoginButton.Name = "LoginButton";
			this.LoginButton.Size = new System.Drawing.Size(75, 23);
			this.LoginButton.TabIndex = 4;
			this.LoginButton.Text = "Login";
			this.LoginButton.UseVisualStyleBackColor = true;
			this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
			// 
			// Login
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(373, 117);
			this.Controls.Add(this.LoginButton);
			this.Controls.Add(this.Password);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.UserName);
			this.Controls.Add(this.label1);
			this.Name = "Login";
			this.Text = "Login";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox UserName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox Password;
		private System.Windows.Forms.Button LoginButton;
	}
}