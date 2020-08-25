using System.ComponentModel;

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
			this.ClientsList = new System.Windows.Forms.ListBox();
			this.MessageTextBox = new System.Windows.Forms.TextBox();
			this.SendButton = new System.Windows.Forms.Button();
			this.AcceptButton = new System.Windows.Forms.Button();
			this.DeclineButton = new System.Windows.Forms.Button();
			this.WordTextBox = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.GMComboBox = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.StartButton = new System.Windows.Forms.Button();
			this.HistoryListBox = new System.Windows.Forms.ListBox();
			this.SyncHistoryButton = new System.Windows.Forms.Button();
			this.AutoAcceptCheckBox = new System.Windows.Forms.CheckBox();
			this.RetryButton = new System.Windows.Forms.Button();
			this.TemplateTextBox = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
			this.label1.Location = new System.Drawing.Point(6, 60);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "IP";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 86);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Port";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
			this.JoinButton.Click += new System.EventHandler(this.JoinButton_Click);
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
			this.label3.Location = new System.Drawing.Point(3, 33);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(44, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Name";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ClientsList
			// 
			this.ClientsList.FormattingEnabled = true;
			this.ClientsList.Location = new System.Drawing.Point(12, 144);
			this.ClientsList.Name = "ClientsList";
			this.ClientsList.Size = new System.Drawing.Size(196, 290);
			this.ClientsList.TabIndex = 9;
			this.ClientsList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ClientsList_KeyDown);
			// 
			// MessageTextBox
			// 
			this.MessageTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.MessageTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.MessageTextBox.Location = new System.Drawing.Point(214, 414);
			this.MessageTextBox.Name = "MessageTextBox";
			this.MessageTextBox.Size = new System.Drawing.Size(411, 20);
			this.MessageTextBox.TabIndex = 10;
			// 
			// SendButton
			// 
			this.SendButton.Enabled = false;
			this.SendButton.Location = new System.Drawing.Point(714, 412);
			this.SendButton.Name = "SendButton";
			this.SendButton.Size = new System.Drawing.Size(75, 23);
			this.SendButton.TabIndex = 11;
			this.SendButton.Text = "Send";
			this.SendButton.UseVisualStyleBackColor = true;
			this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
			// 
			// AcceptButton
			// 
			this.AcceptButton.Enabled = false;
			this.AcceptButton.Location = new System.Drawing.Point(631, 382);
			this.AcceptButton.Name = "AcceptButton";
			this.AcceptButton.Size = new System.Drawing.Size(75, 23);
			this.AcceptButton.TabIndex = 12;
			this.AcceptButton.Text = "Accept";
			this.AcceptButton.UseVisualStyleBackColor = true;
			this.AcceptButton.Click += new System.EventHandler(this.AcceptButton_Click);
			// 
			// DeclineButton
			// 
			this.DeclineButton.Enabled = false;
			this.DeclineButton.Location = new System.Drawing.Point(714, 382);
			this.DeclineButton.Name = "DeclineButton";
			this.DeclineButton.Size = new System.Drawing.Size(75, 23);
			this.DeclineButton.TabIndex = 13;
			this.DeclineButton.Text = "Decline";
			this.DeclineButton.UseVisualStyleBackColor = true;
			this.DeclineButton.Click += new System.EventHandler(this.DeclineButton_Click);
			// 
			// WordTextBox
			// 
			this.WordTextBox.BackColor = System.Drawing.SystemColors.Control;
			this.WordTextBox.Location = new System.Drawing.Point(632, 356);
			this.WordTextBox.Name = "WordTextBox";
			this.WordTextBox.ReadOnly = true;
			this.WordTextBox.Size = new System.Drawing.Size(156, 20);
			this.WordTextBox.TabIndex = 14;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(632, 350);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(156, 6);
			this.pictureBox1.TabIndex = 15;
			this.pictureBox1.TabStop = false;
			// 
			// GMComboBox
			// 
			this.GMComboBox.Enabled = false;
			this.GMComboBox.FormattingEnabled = true;
			this.GMComboBox.Items.AddRange(new object[] {
            "Standard",
            "Start at",
            "End on"});
			this.GMComboBox.Location = new System.Drawing.Point(632, 49);
			this.GMComboBox.Name = "GMComboBox";
			this.GMComboBox.Size = new System.Drawing.Size(156, 21);
			this.GMComboBox.TabIndex = 0;
			this.GMComboBox.SelectedIndexChanged += new System.EventHandler(this.GMComboBox_SelectedIndexChanged);
			this.GMComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.GMComboBox_KeyPress);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(632, 33);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(156, 13);
			this.label4.TabIndex = 17;
			this.label4.Text = "Game Mode:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// StartButton
			// 
			this.StartButton.Enabled = false;
			this.StartButton.Location = new System.Drawing.Point(714, 102);
			this.StartButton.Name = "StartButton";
			this.StartButton.Size = new System.Drawing.Size(75, 23);
			this.StartButton.TabIndex = 18;
			this.StartButton.Text = "Start";
			this.StartButton.UseVisualStyleBackColor = true;
			this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
			// 
			// HistoryListBox
			// 
			this.HistoryListBox.FormattingEnabled = true;
			this.HistoryListBox.Location = new System.Drawing.Point(214, 29);
			this.HistoryListBox.Name = "HistoryListBox";
			this.HistoryListBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.HistoryListBox.Size = new System.Drawing.Size(411, 381);
			this.HistoryListBox.TabIndex = 20;
			// 
			// SyncHistoryButton
			// 
			this.SyncHistoryButton.Enabled = false;
			this.SyncHistoryButton.Location = new System.Drawing.Point(631, 102);
			this.SyncHistoryButton.Name = "SyncHistoryButton";
			this.SyncHistoryButton.Size = new System.Drawing.Size(75, 23);
			this.SyncHistoryButton.TabIndex = 21;
			this.SyncHistoryButton.Text = "SyncHistory";
			this.SyncHistoryButton.UseVisualStyleBackColor = true;
			this.SyncHistoryButton.Click += new System.EventHandler(this.SyncHistoryButton_Click);
			// 
			// AutoAcceptCheckBox
			// 
			this.AutoAcceptCheckBox.AutoSize = true;
			this.AutoAcceptCheckBox.Enabled = false;
			this.AutoAcceptCheckBox.Location = new System.Drawing.Point(632, 327);
			this.AutoAcceptCheckBox.Name = "AutoAcceptCheckBox";
			this.AutoAcceptCheckBox.Size = new System.Drawing.Size(85, 17);
			this.AutoAcceptCheckBox.TabIndex = 22;
			this.AutoAcceptCheckBox.Text = "Auto Aceept";
			this.AutoAcceptCheckBox.UseVisualStyleBackColor = true;
			// 
			// RetryButton
			// 
			this.RetryButton.Location = new System.Drawing.Point(631, 411);
			this.RetryButton.Name = "RetryButton";
			this.RetryButton.Size = new System.Drawing.Size(75, 23);
			this.RetryButton.TabIndex = 23;
			this.RetryButton.Text = "Retry";
			this.RetryButton.UseVisualStyleBackColor = true;
			this.RetryButton.Click += new System.EventHandler(this.RetryButton_Click);
			// 
			// TemplateTextBox
			// 
			this.TemplateTextBox.Location = new System.Drawing.Point(632, 76);
			this.TemplateTextBox.Name = "TemplateTextBox";
			this.TemplateTextBox.Size = new System.Drawing.Size(156, 20);
			this.TemplateTextBox.TabIndex = 24;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 446);
			this.Controls.Add(this.TemplateTextBox);
			this.Controls.Add(this.RetryButton);
			this.Controls.Add(this.AutoAcceptCheckBox);
			this.Controls.Add(this.SyncHistoryButton);
			this.Controls.Add(this.HistoryListBox);
			this.Controls.Add(this.StartButton);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.GMComboBox);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.WordTextBox);
			this.Controls.Add(this.DeclineButton);
			this.Controls.Add(this.AcceptButton);
			this.Controls.Add(this.SendButton);
			this.Controls.Add(this.MessageTextBox);
			this.Controls.Add(this.ClientsList);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.NameTextBox);
			this.Controls.Add(this.JoinButton);
			this.Controls.Add(this.HostButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.PortTextBox);
			this.Controls.Add(this.IpTextBox);
			this.MinimumSize = new System.Drawing.Size(16, 277);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Click += new System.EventHandler(this.Form1_Click);
			this.Resize += new System.EventHandler(this.Form1_Resize);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
		private System.Windows.Forms.ListBox ClientsList;
		private System.Windows.Forms.TextBox MessageTextBox;
		private System.Windows.Forms.Button SendButton;
		private System.Windows.Forms.Button AcceptButton;
		private System.Windows.Forms.Button DeclineButton;
		private System.Windows.Forms.TextBox WordTextBox;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ComboBox GMComboBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button StartButton;
		private System.Windows.Forms.ListBox HistoryListBox;
		private System.Windows.Forms.Button SyncHistoryButton;
		private System.Windows.Forms.CheckBox AutoAcceptCheckBox;
		private System.Windows.Forms.Button RetryButton;
		private System.Windows.Forms.TextBox TemplateTextBox;
	}
}

