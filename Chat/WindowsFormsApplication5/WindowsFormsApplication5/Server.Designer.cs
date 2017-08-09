namespace WindowsFormsApplication5
{
    partial class Server
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Clients_listBox1 = new System.Windows.Forms.ListBox();
            this.Messages_listBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ServerIPtextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Clients_listBox1
            // 
            this.Clients_listBox1.FormattingEnabled = true;
            this.Clients_listBox1.Location = new System.Drawing.Point(257, 12);
            this.Clients_listBox1.Name = "Clients_listBox1";
            this.Clients_listBox1.Size = new System.Drawing.Size(120, 238);
            this.Clients_listBox1.TabIndex = 0;
            // 
            // Messages_listBox
            // 
            this.Messages_listBox.FormattingEnabled = true;
            this.Messages_listBox.Location = new System.Drawing.Point(12, 61);
            this.Messages_listBox.Name = "Messages_listBox";
            this.Messages_listBox.Size = new System.Drawing.Size(209, 186);
            this.Messages_listBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "ServerName";
            // 
            // ServerIPtextBox
            // 
            this.ServerIPtextBox.Location = new System.Drawing.Point(94, 9);
            this.ServerIPtextBox.Name = "ServerIPtextBox";
            this.ServerIPtextBox.Size = new System.Drawing.Size(127, 20);
            this.ServerIPtextBox.TabIndex = 3;
            // 
            // Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 273);
            this.Controls.Add(this.ServerIPtextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Messages_listBox);
            this.Controls.Add(this.Clients_listBox1);
            this.Name = "Server";
            this.Text = "Server";
            this.Load += new System.EventHandler(this.Server_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox Clients_listBox1;
        private System.Windows.Forms.ListBox Messages_listBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ServerIPtextBox;
    }
}

