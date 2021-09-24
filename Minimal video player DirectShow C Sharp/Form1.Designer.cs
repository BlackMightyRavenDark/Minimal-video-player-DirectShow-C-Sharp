
namespace Minimal_video_player_DirectShow_C_Sharp
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
            this.components = new System.ComponentModel.Container();
            this.panelVideoOutput = new System.Windows.Forms.Panel();
            this.seekBar = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.volumeBar = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miPlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miPauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.miGraphModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miAutomaticToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miManualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSettings = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.miIntellectualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelVideoOutput
            // 
            this.panelVideoOutput.AllowDrop = true;
            this.panelVideoOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVideoOutput.BackColor = System.Drawing.Color.Black;
            this.panelVideoOutput.Location = new System.Drawing.Point(0, 0);
            this.panelVideoOutput.Name = "panelVideoOutput";
            this.panelVideoOutput.Size = new System.Drawing.Size(342, 170);
            this.panelVideoOutput.TabIndex = 0;
            this.panelVideoOutput.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelVideoOutput_DragDrop);
            this.panelVideoOutput.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelVideoOutput_DragEnter);
            this.panelVideoOutput.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelVideoOutput_MouseDown);
            // 
            // seekBar
            // 
            this.seekBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.seekBar.Location = new System.Drawing.Point(106, 170);
            this.seekBar.Name = "seekBar";
            this.seekBar.Size = new System.Drawing.Size(213, 20);
            this.seekBar.TabIndex = 1;
            this.seekBar.Paint += new System.Windows.Forms.PaintEventHandler(this.seekBar_Paint);
            this.seekBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.seekBar_MouseDown);
            this.seekBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.seekBar_MouseMove);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // volumeBar
            // 
            this.volumeBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.volumeBar.Location = new System.Drawing.Point(0, 170);
            this.volumeBar.Name = "volumeBar";
            this.volumeBar.Size = new System.Drawing.Size(103, 20);
            this.volumeBar.TabIndex = 2;
            this.volumeBar.Paint += new System.Windows.Forms.PaintEventHandler(this.volumeBar_Paint);
            this.volumeBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.volumeBar_MouseDown);
            this.volumeBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.volumeBar_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miPlayToolStripMenuItem,
            this.miPauseToolStripMenuItem,
            this.miStopToolStripMenuItem,
            this.toolStripMenuItem1,
            this.miGraphModeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 120);
            // 
            // miPlayToolStripMenuItem
            // 
            this.miPlayToolStripMenuItem.Name = "miPlayToolStripMenuItem";
            this.miPlayToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.miPlayToolStripMenuItem.Text = "Плей";
            this.miPlayToolStripMenuItem.Click += new System.EventHandler(this.miPlayToolStripMenuItem_Click);
            // 
            // miPauseToolStripMenuItem
            // 
            this.miPauseToolStripMenuItem.Name = "miPauseToolStripMenuItem";
            this.miPauseToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.miPauseToolStripMenuItem.Text = "Пауза";
            this.miPauseToolStripMenuItem.Click += new System.EventHandler(this.miPauseToolStripMenuItem_Click);
            // 
            // miStopToolStripMenuItem
            // 
            this.miStopToolStripMenuItem.Name = "miStopToolStripMenuItem";
            this.miStopToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.miStopToolStripMenuItem.Text = "Стоп";
            this.miStopToolStripMenuItem.Click += new System.EventHandler(this.miStopToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // miGraphModeToolStripMenuItem
            // 
            this.miGraphModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAutomaticToolStripMenuItem,
            this.miIntellectualToolStripMenuItem,
            this.miManualToolStripMenuItem});
            this.miGraphModeToolStripMenuItem.Name = "miGraphModeToolStripMenuItem";
            this.miGraphModeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.miGraphModeToolStripMenuItem.Text = "Режим";
            // 
            // miAutomaticToolStripMenuItem
            // 
            this.miAutomaticToolStripMenuItem.Name = "miAutomaticToolStripMenuItem";
            this.miAutomaticToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.miAutomaticToolStripMenuItem.Text = "Автоматический";
            this.miAutomaticToolStripMenuItem.Click += new System.EventHandler(this.miAutomaticToolStripMenuItem_Click);
            // 
            // miIntellectualToolStripMenuItem
            // 
            this.miIntellectualToolStripMenuItem.Name = "miIntellectualToolStripMenuItem";
            this.miIntellectualToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.miIntellectualToolStripMenuItem.Text = "Интеллектуальный";
            this.miIntellectualToolStripMenuItem.Click += new System.EventHandler(this.miIntellectualToolStripMenuItem_Click);
            // 
            // miManualToolStripMenuItem
            // 
            this.miManualToolStripMenuItem.Name = "miManualToolStripMenuItem";
            this.miManualToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.miManualToolStripMenuItem.Text = "Ручной";
            this.miManualToolStripMenuItem.Click += new System.EventHandler(this.miManualToolStripMenuItem_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.BackgroundImage = global::Minimal_video_player_DirectShow_C_Sharp.Properties.Resources.tools;
            this.btnSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSettings.Location = new System.Drawing.Point(322, 170);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(20, 20);
            this.btnSettings.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnSettings, "Меню");
            this.btnSettings.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnSettings_MouseDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 188);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.volumeBar);
            this.Controls.Add(this.seekBar);
            this.Controls.Add(this.panelVideoOutput);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "Form1";
            this.Text = "Minimal video player DirectShow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelVideoOutput;
        private System.Windows.Forms.Panel seekBar;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel volumeBar;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem miPlayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miPauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miStopToolStripMenuItem;
        private System.Windows.Forms.Panel btnSettings;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem miGraphModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miAutomaticToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miManualToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem miIntellectualToolStripMenuItem;
    }
}

