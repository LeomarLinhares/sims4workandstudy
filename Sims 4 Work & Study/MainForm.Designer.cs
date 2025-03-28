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
            ContextMenuStripFromTray.SuspendLayout();
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
            ContextTraySair.Size = new Size(180, 22);
            ContextTraySair.Text = "Sair";
            // 
            // ContextTrayAbrir
            // 
            ContextTrayAbrir.Name = "ContextTrayAbrir";
            ContextTrayAbrir.Size = new Size(180, 22);
            ContextTrayAbrir.Text = "Abir";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(365, 450);
            Name = "Form1";
            Text = "Sims 4 Work & Study";
            ContextMenuStripFromTray.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon TrayIcon;
        private ContextMenuStrip ContextMenuStripFromTray;
        private ToolStripMenuItem ContextTraySair;
        private ToolStripMenuItem ContextTrayAbrir;
    }
}
