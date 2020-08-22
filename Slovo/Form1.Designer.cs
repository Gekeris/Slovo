namespace Slovo
{
	partial class Form1
	{
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			this.IpTextBox = new System.Windows.Forms.TextBox();
			this.PortTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.HostButton = new System.Windows.Forms.Button();
			this.JoinButton = new System.Windows.Forms.Button();
			this.NameTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// IpTextBox
			// 
			this.IpTextBox.Location = new System.Drawing.Point(52, 56);
			this.IpTextBox.Name = "IpTextBox";
			this.IpTextBox.Size = new System.Drawing.Size(156, 20);
			this.IpTextBox.TabIndex = 0;
			// 
			// PortTextBox
			// 
			this.PortTextBox.Location = new System.Drawing.Point(52, 82);
			this.PortTextBox.Name = "PortTextBox";
			this.PortTextBox.Size = new System.Drawing.Size(156, 20);
			this.PortTextBox.TabIndex = 1;
			this.PortTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PortTextBox_KeyPress);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(29, 60);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(17, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "IP";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(20, 86);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(26, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Port";
			// 
			// HostButton
			// 
			this.HostButton.Location = new System.Drawing.Point(133, 108);
			this.HostButton.Name = "HostButton";
			this.HostButton.Size = new System.Drawing.Size(75, 22);
			this.HostButton.TabIndex = 4;
			this.HostButton.Text = "Host";
			this.HostButton.Click += new System.EventHandler(this.HostButton_Click);
			// 
			// JoinButton
			// 
			this.JoinButton.Location = new System.Drawing.Point(52, 108);
			this.JoinButton.Name = "JoinButton";
			this.JoinButton.Size = new System.Drawing.Size(75, 22);
			this.JoinButton.TabIndex = 5;
			this.JoinButton.Text = "Join";
			// 
			// NameTextBox
			// 
			this.NameTextBox.Location = new System.Drawing.Point(52, 29);
			this.NameTextBox.Name = "NameTextBox";
			this.NameTextBox.Size = new System.Drawing.Size(156, 20);
			this.NameTextBox.TabIndex = 6;
			this.NameTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NameTextBox_KeyPress);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 33);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Name";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.NameTextBox);
			this.Controls.Add(this.JoinButton);
			this.Controls.Add(this.HostButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.PortTextBox);
			this.Controls.Add(this.IpTextBox);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Click += new System.EventHandler(this.Form1_Click);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox IpTextBox;
		private System.Windows.Forms.TextBox PortTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button HostButton;
		private System.Windows.Forms.Button JoinButton;
		private System.Windows.Forms.TextBox NameTextBox;
		private System.Windows.Forms.Label label3;
	}
}

