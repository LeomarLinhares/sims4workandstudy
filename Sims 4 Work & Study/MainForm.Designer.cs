namespace Sims_4_Work___Study
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            TrayIcon = new NotifyIcon(components);
            ContextMenuStripFromTray = new ContextMenuStrip(components);
            ContextTraySair = new ToolStripMenuItem();
            ContextTrayAbrir = new ToolStripMenuItem();
            button1 = new Button();
            trackBarMainVolume = new TrackBar();
            label1 = new Label();
            playPauseButton = new Button();
            ContextMenuStripFromTray.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarMainVolume).BeginInit();
            SuspendLayout();
            // 
            // TrayIcon
            // 
            TrayIcon.ContextMenuStrip = ContextMenuStripFromTray;
            TrayIcon.Icon = (Icon)resources.GetObject("TrayIcon.Icon");
            TrayIcon.Text = "The Sims 4 Work & Study";
            TrayIcon.Visible = true;
            // 
            // ContextMenuStripFromTray
            // 
            ContextMenuStripFromTray.Items.AddRange(new ToolStripItem[] { ContextTraySair, ContextTrayAbrir });
            ContextMenuStripFromTray.Name = "contextMenuStrip1";
            ContextMenuStripFromTray.Size = new Size(97, 48);
            // 
            // ContextTraySair
            // 
            ContextTraySair.Name = "ContextTraySair";
            ContextTraySair.Size = new Size(96, 22);
            ContextTraySair.Text = "Sair";
            // 
            // ContextTrayAbrir
            // 
            ContextTrayAbrir.Name = "ContextTrayAbrir";
            ContextTrayAbrir.Size = new Size(96, 22);
            ContextTrayAbrir.Text = "Abir";
            // 
            // button1
            // 
            button1.Location = new Point(12, 179);
            button1.Name = "button1";
            button1.Size = new Size(341, 23);
            button1.TabIndex = 1;
            button1.Text = "DEBUG Pular para os 5 últimos segundos";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btnSkip_Click;
            // 
            // trackBarMainVolume
            // 
            trackBarMainVolume.Location = new Point(12, 112);
            trackBarMainVolume.Maximum = 100;
            trackBarMainVolume.Name = "trackBarMainVolume";
            trackBarMainVolume.Size = new Size(341, 45);
            trackBarMainVolume.TabIndex = 2;
            trackBarMainVolume.Value = 100;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 94);
            label1.Name = "label1";
            label1.Size = new Size(47, 15);
            label1.TabIndex = 3;
            label1.Text = "Volume";
            // 
            // playPauseButton
            // 
            playPauseButton.Location = new Point(12, 12);
            playPauseButton.Name = "playPauseButton";
            playPauseButton.Size = new Size(341, 67);
            playPauseButton.TabIndex = 4;
            playPauseButton.Text = "Pausar";
            playPauseButton.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(365, 221);
            Controls.Add(playPauseButton);
            Controls.Add(label1);
            Controls.Add(trackBarMainVolume);
            Controls.Add(button1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "Sims 4 Work & Study";
            ContextMenuStripFromTray.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)trackBarMainVolume).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NotifyIcon TrayIcon;
        private ContextMenuStrip ContextMenuStripFromTray;
        private ToolStripMenuItem ContextTraySair;
        private ToolStripMenuItem ContextTrayAbrir;
        private Button button1;
        private TrackBar trackBarMainVolume;
        private Label label1;
        private Button playPauseButton;
    }
}
