namespace Vkt5_RemoteConsole
{
	partial class Vkt5ConsoleControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Vkt5ConsoleControl));
			this.panel1 = new System.Windows.Forms.Panel();
			this.txtLine2 = new System.Windows.Forms.Label();
			this.txtLine1 = new System.Windows.Forms.Label();
			this.btnMenu = new System.Windows.Forms.Button();
			this.btnBack = new System.Windows.Forms.Button();
			this.btnTab = new System.Windows.Forms.Button();
			this.btnLeft = new System.Windows.Forms.Button();
			this.btnEnter = new System.Windows.Forms.Button();
			this.btnDown = new System.Windows.Forms.Button();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnRight = new System.Windows.Forms.Button();
			this.refreshTimer = new System.Windows.Forms.Timer();
			this.cursorTimer = new System.Windows.Forms.Timer();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(180)))), ((int)(((byte)(40)))));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.txtLine2);
			this.panel1.Controls.Add(this.txtLine1);
			this.panel1.Location = new System.Drawing.Point(12, 14);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(318, 67);
			this.panel1.TabIndex = 20;
			// 
			// txtLine2
			// 
			this.txtLine2.Font = new System.Drawing.Font("Lucida Console", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.txtLine2.ForeColor = System.Drawing.Color.DimGray;
			this.txtLine2.Location = new System.Drawing.Point(3, 35);
			this.txtLine2.Name = "txtLine2";
			this.txtLine2.Size = new System.Drawing.Size(310, 29);
			this.txtLine2.TabIndex = 1;
			this.txtLine2.Text = "подключения";
			// 
			// txtLine1
			// 
			this.txtLine1.Font = new System.Drawing.Font("Lucida Console", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.txtLine1.ForeColor = System.Drawing.Color.DimGray;
			this.txtLine1.Location = new System.Drawing.Point(3, 3);
			this.txtLine1.Name = "txtLine1";
			this.txtLine1.Size = new System.Drawing.Size(310, 29);
			this.txtLine1.TabIndex = 0;
			this.txtLine1.Text = "Нет";
			// 
			// btnMenu
			// 
			this.btnMenu.BackColor = System.Drawing.Color.Gray;
			this.btnMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMenu.Image = ((System.Drawing.Image)(resources.GetObject("btnMenu.Image")));
			this.btnMenu.Location = new System.Drawing.Point(222, 102);
			this.btnMenu.Name = "btnMenu";
			this.btnMenu.Size = new System.Drawing.Size(108, 64);
			this.btnMenu.TabIndex = 18;
			this.btnMenu.UseVisualStyleBackColor = false;
			this.btnMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnMenu_MouseDown);
			this.btnMenu.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnMenu_MouseUp);
			// 
			// btnBack
			// 
			this.btnBack.BackColor = System.Drawing.Color.Gray;
			this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnBack.Image = ((System.Drawing.Image)(resources.GetObject("btnBack.Image")));
			this.btnBack.Location = new System.Drawing.Point(12, 102);
			this.btnBack.Name = "btnBack";
			this.btnBack.Size = new System.Drawing.Size(64, 64);
			this.btnBack.TabIndex = 12;
			this.btnBack.UseVisualStyleBackColor = false;
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			// 
			// btnTab
			// 
			this.btnTab.BackColor = System.Drawing.Color.Gray;
			this.btnTab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnTab.Image = ((System.Drawing.Image)(resources.GetObject("btnTab.Image")));
			this.btnTab.Location = new System.Drawing.Point(222, 172);
			this.btnTab.Name = "btnTab";
			this.btnTab.Size = new System.Drawing.Size(108, 64);
			this.btnTab.TabIndex = 19;
			this.btnTab.UseVisualStyleBackColor = false;
			this.btnTab.Click += new System.EventHandler(this.btnTab_Click);
			// 
			// btnLeft
			// 
			this.btnLeft.BackColor = System.Drawing.Color.Gray;
			this.btnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnLeft.Image = ((System.Drawing.Image)(resources.GetObject("btnLeft.Image")));
			this.btnLeft.Location = new System.Drawing.Point(12, 172);
			this.btnLeft.Name = "btnLeft";
			this.btnLeft.Size = new System.Drawing.Size(64, 64);
			this.btnLeft.TabIndex = 15;
			this.btnLeft.UseVisualStyleBackColor = false;
			this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
			// 
			// btnEnter
			// 
			this.btnEnter.BackColor = System.Drawing.Color.Gray;
			this.btnEnter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnEnter.Image = ((System.Drawing.Image)(resources.GetObject("btnEnter.Image")));
			this.btnEnter.Location = new System.Drawing.Point(152, 102);
			this.btnEnter.Name = "btnEnter";
			this.btnEnter.Size = new System.Drawing.Size(64, 64);
			this.btnEnter.TabIndex = 14;
			this.btnEnter.UseVisualStyleBackColor = false;
			this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
			// 
			// btnDown
			// 
			this.btnDown.BackColor = System.Drawing.Color.Gray;
			this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
			this.btnDown.Location = new System.Drawing.Point(82, 172);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(64, 64);
			this.btnDown.TabIndex = 16;
			this.btnDown.UseVisualStyleBackColor = false;
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// btnUp
			// 
			this.btnUp.BackColor = System.Drawing.Color.Gray;
			this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
			this.btnUp.Location = new System.Drawing.Point(82, 102);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(64, 64);
			this.btnUp.TabIndex = 13;
			this.btnUp.UseVisualStyleBackColor = false;
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnRight
			// 
			this.btnRight.BackColor = System.Drawing.Color.Gray;
			this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRight.Image = ((System.Drawing.Image)(resources.GetObject("btnRight.Image")));
			this.btnRight.Location = new System.Drawing.Point(152, 172);
			this.btnRight.Name = "btnRight";
			this.btnRight.Size = new System.Drawing.Size(64, 64);
			this.btnRight.TabIndex = 17;
			this.btnRight.UseVisualStyleBackColor = false;
			this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
			// 
			// refreshTimer
			// 
			this.refreshTimer.Interval = 1000;
			this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
			// 
			// cursorTimer
			// 
			this.cursorTimer.Interval = 500;
			this.cursorTimer.Tick += new System.EventHandler(this.cursorTimer_Tick);
			// 
			// Vkt5ConsoleControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnMenu);
			this.Controls.Add(this.btnBack);
			this.Controls.Add(this.btnTab);
			this.Controls.Add(this.btnLeft);
			this.Controls.Add(this.btnEnter);
			this.Controls.Add(this.btnDown);
			this.Controls.Add(this.btnUp);
			this.Controls.Add(this.btnRight);
			this.Name = "Vkt5ConsoleControl";
			this.Size = new System.Drawing.Size(348, 257);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label txtLine2;
		private System.Windows.Forms.Label txtLine1;
		private System.Windows.Forms.Button btnMenu;
		private System.Windows.Forms.Button btnBack;
		private System.Windows.Forms.Button btnTab;
		private System.Windows.Forms.Button btnLeft;
		private System.Windows.Forms.Button btnEnter;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Button btnRight;
		private System.Windows.Forms.Timer refreshTimer;
		private System.Windows.Forms.Timer cursorTimer;
	}
}
