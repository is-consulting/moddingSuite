namespace moddingSuite.ZoneEditor
{
    partial class Editor
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
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Label = new System.Windows.Forms.ToolStripTextBox();
            this.Zone = new System.Windows.Forms.ToolStripMenuItem();
            this.Spawn = new System.Windows.Forms.ToolStripMenuItem();
            this.Land = new System.Windows.Forms.ToolStripMenuItem();
            this.Air = new System.Windows.Forms.ToolStripMenuItem();
            this.Sea = new System.Windows.Forms.ToolStripMenuItem();
            this.StartPosition = new System.Windows.Forms.ToolStripMenuItem();
            this.CV = new System.Windows.Forms.ToolStripMenuItem();
            this.FOB = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Desktop;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(500, 486);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(835, 488);
            this.splitContainer1.SplitterDistance = 502;
            this.splitContainer1.TabIndex = 7;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.button1);
            this.splitContainer2.Panel1.Controls.Add(this.listBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel1);
            this.splitContainer2.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer2_Panel2_Paint_1);
            this.splitContainer2.Size = new System.Drawing.Size(329, 488);
            this.splitContainer2.SplitterDistance = 204;
            this.splitContainer2.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Location = new System.Drawing.Point(0, 179);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(327, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Delete Selected Object";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(3, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(325, 134);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(327, 278);
            this.panel1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Label,
            this.Zone,
            this.Spawn,
            this.StartPosition});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(161, 95);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // Label
            // 
            this.Label.AcceptsReturn = true;
            this.Label.AcceptsTab = true;
            this.Label.Enabled = false;
            this.Label.MaxLength = 0;
            this.Label.Name = "Label";
            this.Label.ReadOnly = true;
            this.Label.Size = new System.Drawing.Size(100, 23);
            this.Label.Text = "Add...";
            // 
            // Zone
            // 
            this.Zone.Name = "Zone";
            this.Zone.Size = new System.Drawing.Size(160, 22);
            this.Zone.Text = "Zone";
            // 
            // Spawn
            // 
            this.Spawn.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Land,
            this.Air,
            this.Sea});
            this.Spawn.Name = "Spawn";
            this.Spawn.Size = new System.Drawing.Size(160, 22);
            this.Spawn.Text = "Spawn";
            // 
            // Land
            // 
            this.Land.Name = "Land";
            this.Land.Size = new System.Drawing.Size(100, 22);
            this.Land.Text = "Land";
            // 
            // Air
            // 
            this.Air.Name = "Air";
            this.Air.Size = new System.Drawing.Size(100, 22);
            this.Air.Text = "Air";
            // 
            // Sea
            // 
            this.Sea.Name = "Sea";
            this.Sea.Size = new System.Drawing.Size(100, 22);
            this.Sea.Text = "Sea";
            // 
            // StartPosition
            // 
            this.StartPosition.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CV,
            this.FOB});
            this.StartPosition.Name = "StartPosition";
            this.StartPosition.Size = new System.Drawing.Size(160, 22);
            this.StartPosition.Text = "Start Position";
            // 
            // CV
            // 
            this.CV.Name = "CV";
            this.CV.Size = new System.Drawing.Size(96, 22);
            this.CV.Text = "CV";
            // 
            // FOB
            // 
            this.FOB.Name = "FOB";
            this.FOB.Size = new System.Drawing.Size(96, 22);
            this.FOB.Text = "FOB";
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 488);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Editor";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripTextBox Label;
        private System.Windows.Forms.ToolStripMenuItem Zone;
        private System.Windows.Forms.ToolStripMenuItem Spawn;
        private System.Windows.Forms.ToolStripMenuItem Land;
        private System.Windows.Forms.ToolStripMenuItem Air;
        private System.Windows.Forms.ToolStripMenuItem Sea;
        private System.Windows.Forms.ToolStripMenuItem StartPosition;
        private System.Windows.Forms.ToolStripMenuItem CV;
        private System.Windows.Forms.ToolStripMenuItem FOB;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;

    }
}