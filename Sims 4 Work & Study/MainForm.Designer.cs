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
            debugJumpTo5LastSeconds = new Button();
            trackBarMainVolume = new TrackBar();
            label1 = new Label();
            playPauseButton = new Button();
            numericUpDown_ChangeChannelChance = new NumericUpDown();
            label2 = new Label();
            button1 = new Button();
            button2 = new Button();
            ContextMenuStripFromTray.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarMainVolume).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_ChangeChannelChance).BeginInit();
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
            // debugJumpTo5LastSeconds
            // 
            debugJumpTo5LastSeconds.Location = new Point(12, 227);
            debugJumpTo5LastSeconds.Name = "debugJumpTo5LastSeconds";
            debugJumpTo5LastSeconds.Size = new Size(341, 24);
            debugJumpTo5LastSeconds.TabIndex = 1;
            debugJumpTo5LastSeconds.Text = "DEBUG Pular para os 5 últimos segundos";
            debugJumpTo5LastSeconds.UseVisualStyleBackColor = true;
            debugJumpTo5LastSeconds.Click += btnSkip_Click;
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
            playPauseButton.Location = new Point(93, 12);
            playPauseButton.Name = "playPauseButton";
            playPauseButton.Size = new Size(179, 73);
            playPauseButton.TabIndex = 4;
            playPauseButton.Text = "Pausar";
            playPauseButton.UseVisualStyleBackColor = true;
            // 
            // numericUpDown_ChangeChannelChance
            // 
            numericUpDown_ChangeChannelChance.Location = new Point(267, 198);
            numericUpDown_ChangeChannelChance.Name = "numericUpDown_ChangeChannelChance";
            numericUpDown_ChangeChannelChance.Size = new Size(86, 23);
            numericUpDown_ChangeChannelChance.TabIndex = 5;
            numericUpDown_ChangeChannelChance.ValueChanged += numericUpDown_ChangeChannelChance_ValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 206);
            label2.Name = "label2";
            label2.Size = new Size(130, 15);
            label2.TabIndex = 6;
            label2.Text = "Chance de alterar canal";
            // 
            // button1
            // 
            button1.Location = new Point(278, 12);
            button1.Name = "button1";
            button1.Size = new Size(75, 73);
            button1.TabIndex = 7;
            button1.Text = "Next";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(12, 12);
            button2.Name = "button2";
            button2.Size = new Size(75, 73);
            button2.TabIndex = 8;
            button2.Text = "Previous";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(365, 253);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(numericUpDown_ChangeChannelChance);
            Controls.Add(playPauseButton);
            Controls.Add(label1);
            Controls.Add(trackBarMainVolume);
            Controls.Add(debugJumpTo5LastSeconds);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "Sims 4 Work & Study";
            Load += MainForm_Load;
            ContextMenuStripFromTray.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)trackBarMainVolume).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_ChangeChannelChance).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NotifyIcon TrayIcon;
        private ContextMenuStrip ContextMenuStripFromTray;
        private ToolStripMenuItem ContextTraySair;
        private ToolStripMenuItem ContextTrayAbrir;
        private Button debugJumpTo5LastSeconds;
        private TrackBar trackBarMainVolume;
        private Label label1;
        private Button playPauseButton;
        private NumericUpDown numericUpDown_ChangeChannelChance;
        private Label label2;
        private Button button1;
        private Button button2;
    }
}
