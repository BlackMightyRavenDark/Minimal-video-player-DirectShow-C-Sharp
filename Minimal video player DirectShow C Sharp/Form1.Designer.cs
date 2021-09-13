
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
            this.SuspendLayout();
            // 
            // panelVideoOutput
            // 
            this.panelVideoOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVideoOutput.BackColor = System.Drawing.Color.Black;
            this.panelVideoOutput.Location = new System.Drawing.Point(0, 0);
            this.panelVideoOutput.Name = "panelVideoOutput";
            this.panelVideoOutput.Size = new System.Drawing.Size(342, 170);
            this.panelVideoOutput.TabIndex = 0;
            this.panelVideoOutput.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelVideoOutput_MouseDown);
            // 
            // seekBar
            // 
            this.seekBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.seekBar.Location = new System.Drawing.Point(106, 170);
            this.seekBar.Name = "seekBar";
            this.seekBar.Size = new System.Drawing.Size(236, 20);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 188);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelVideoOutput;
        private System.Windows.Forms.Panel seekBar;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel volumeBar;
    }
}

