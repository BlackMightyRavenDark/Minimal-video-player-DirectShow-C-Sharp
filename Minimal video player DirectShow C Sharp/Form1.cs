using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using static Minimal_video_player_DirectShow_C_Sharp.ZeratoolPlayerEngine;
using static Minimal_video_player_DirectShow_C_Sharp.DirectShowUtils;

namespace Minimal_video_player_DirectShow_C_Sharp
{
    public partial class Form1 : Form
    {
        public const string TITLE = "Minimal video player DirectShow";
        private ZeratoolPlayerEngine player;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            player = new ZeratoolPlayerEngine();
            player.VideoOutputWindow = panelVideoOutput;

            switch (player.GraphMode)
            {
                case DirectShowGraphMode.Automatic:
                    miAutomaticToolStripMenuItem.Checked = true;
                    miIntellectualToolStripMenuItem.Checked = false;
                    miManualToolStripMenuItem.Checked = false;
                    break;

                case DirectShowGraphMode.Intellectual:
                    miAutomaticToolStripMenuItem.Checked = false;
                    miIntellectualToolStripMenuItem.Checked = true;
                    miManualToolStripMenuItem.Checked = false;
                    break;

                case DirectShowGraphMode.Manual:
                    miAutomaticToolStripMenuItem.Checked = false;
                    miIntellectualToolStripMenuItem.Checked = false;
                    miManualToolStripMenuItem.Checked = true;
                    break;
            }

            volumeBar.SetDoubleBuffering(true);
            seekBar.SetDoubleBuffering(true);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            player.Clear();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (player.VideoRendered)
            {
                Size videoSize = player.VideoSize;
                if (videoSize.Width > 0 && videoSize.Height > 0)
                {
                    Rectangle videoRect = new Rectangle(0, 0, videoSize.Width, videoSize.Height);
                    Rectangle r = videoRect.ResizeTo(panelVideoOutput.ClientSize).CenterIn(panelVideoOutput.ClientRectangle);
                    player.SetVideoOutputRectangle(r);
                }
            }

            volumeBar.Refresh();
            seekBar.Refresh();
        }

        private void seekBar_Paint(object sender, PaintEventArgs e)
        {
            Brush brush = new SolidBrush(seekBar.BackColor);
            e.Graphics.FillRectangle(brush, seekBar.ClientRectangle);
            brush.Dispose();

            if (player != null && player.Duration > 0.0)
            {
                int x = (int)(seekBar.Width / player.Duration * player.Position);
                Rectangle r = new Rectangle(0, 0, x, seekBar.Height);
                e.Graphics.FillRectangle(Brushes.Blue, r);

                string elapsedString = new DateTime(TimeSpan.FromSeconds(player.Position).Ticks).ToString("H:mm:ss");
                string remainingString = new DateTime(TimeSpan.FromSeconds(player.Duration - player.Position).Ticks).ToString("H:mm:ss");

                Font fnt = new Font("Tahoma", 11.0f);
                SizeF size = e.Graphics.MeasureString(elapsedString, fnt);

                Rectangle thumbRect = new Rectangle(x - 3, 0, 6, seekBar.Height);
                e.Graphics.FillRectangle(Brushes.White, thumbRect);
                e.Graphics.DrawRectangle(Pens.Black, thumbRect);

                int y = (int)(seekBar.Height / 2 - size.Height / 2);
                e.Graphics.DrawString(elapsedString, fnt, Brushes.White, x - size.Width - 2, y);
                e.Graphics.DrawString(remainingString, fnt, Brushes.Black, x + 4, y);

                fnt.Dispose();
            }
        }

        private void seekBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && player != null)
            {
                player.Position = player.Duration / seekBar.Width * e.X;
                seekBar.Refresh();
            }
        }

        private void seekBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && player != null)
            {
                double pos = player.Duration / seekBar.Width * e.X;
                if (pos >= player.Duration)
                {
                    pos = player.Duration - 3.0;
                }
                if (pos < 0.0)
                {
                    pos = 0.0;
                }

                player.Position = pos;
                seekBar.Refresh();
            }
        }

        //MouseDown and MouseMove doing same actions
        private void volumeBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                player.Volume = (int)(100.0 / volumeBar.Width * e.X);
                volumeBar.Refresh();
            }
        }

        private void volumeBar_Paint(object sender, PaintEventArgs e)
        {
            Brush brush = new SolidBrush(volumeBar.BackColor);
            e.Graphics.FillRectangle(brush, volumeBar.ClientRectangle);
            brush.Dispose();
            if (player != null)
            {
                if (player.Volume > 0)
                {
                    int x1 = (int)(volumeBar.Width / 100.0 * player.Volume);
                    Rectangle r = new Rectangle(0, 0, x1, volumeBar.Height);
                    e.Graphics.FillRectangle(player.AudioRendered ? Brushes.Lime : Brushes.LightGray, r);
                }

                string t = $"Volume: {player.Volume}%";

                Font fnt = new Font("Tahoma", 10.0f);
                SizeF size = e.Graphics.MeasureString(t, fnt);

                int x = volumeBar.Width / 2 - (int)size.Width / 2;
                int y = volumeBar.Height / 2 - (int)size.Height / 2;
                e.Graphics.DrawString(t, fnt, Brushes.Black, x, y);

                fnt.Dispose();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                case Keys.Enter:
                    Close();
                    return;

                case Keys.Up:
                    player.Volume += 5;
                    volumeBar.Refresh();
                    break;

                case Keys.Down:
                    player.Volume -= 5;
                    volumeBar.Refresh();
                    break;

                case Keys.Left:
                    player.Seek(-3.0);
                    seekBar.Refresh();
                    break;

                case Keys.Right:
                    player.Seek(3.0);
                    seekBar.Refresh();
                    break;

                case Keys.Space:
                    PlayerTogglePause();
                    break;

                case Keys.R:
                    PlayerRebuildGraph();
                    break;
            }
        }

        private void panelVideoOutput_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && player != null)
            {
                PlayerTogglePause();
            }
        }

        private void panelVideoOutput_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void panelVideoOutput_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] strings = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (File.Exists(strings[0]))
                {
                    PlayerPlayFile(strings[0]);
                }
            }
        }

        private void btnSettings_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            seekBar.Refresh();
        }

        private void PlayerPlayFile(string fileName)
        {
            timer1.Enabled = false;
            player.Clear();
            panelVideoOutput.Refresh();
            volumeBar.Refresh();
            seekBar.Refresh();
            player.FileName = fileName;
            Text = $"{Path.GetFileName(fileName)} | {TITLE}";
            int errorCode = player.Play();
            if (errorCode == S_OK)
            {
                volumeBar.Refresh();
                seekBar.Refresh();
                timer1.Enabled = true;
            }
            else
            {
                ShowErrorMessage(errorCode);
            }
        }

        private void PlayerRebuildGraph()
        {
            timer1.Enabled = false;
            double pos = player.Position;
            player.Clear();
            panelVideoOutput.Refresh();
            volumeBar.Refresh();
            seekBar.Refresh();
            int errorCode = player.Play();
            if (errorCode == S_OK)
            {
                if (pos > 0.0)
                {
                    player.Position = pos;
                }
                volumeBar.Refresh();
                seekBar.Refresh();
                timer1.Enabled = true;
            }
            else
            {
                ShowErrorMessage(errorCode);
            }
        }

        private void PlayerTogglePause()
        {
            switch (player.State)
            {
                case PlayerState.Paused:
                    player.Play();
                    break;

                case PlayerState.Playing:
                    player.Pause();
                    break;
            }    
        }

        private void ChangeGraphMode(DirectShowGraphMode graphMode)
        {
            timer1.Enabled = false;
            double pos = player.Position;
            player.Clear();
            panelVideoOutput.Refresh();
            volumeBar.Refresh();
            seekBar.Refresh();
            player.GraphMode = graphMode;
            int errorCode = player.Play();
            if (errorCode == S_OK)
            {
                if (pos > 0.0)
                {
                    player.Position = pos;
                }
                volumeBar.Refresh();
                seekBar.Refresh();
                timer1.Enabled = true;
            }
            else
            {
                ShowErrorMessage(errorCode);
            }

            switch (player.GraphMode)
            {
                case DirectShowGraphMode.Automatic:
                    miAutomaticToolStripMenuItem.Checked = true;
                    miIntellectualToolStripMenuItem.Checked = false;
                    miManualToolStripMenuItem.Checked = false;
                    break;

                case DirectShowGraphMode.Intellectual:
                    miAutomaticToolStripMenuItem.Checked = false;
                    miIntellectualToolStripMenuItem.Checked = true;
                    miManualToolStripMenuItem.Checked = false;
                    break;

                case DirectShowGraphMode.Manual:
                    miAutomaticToolStripMenuItem.Checked = false;
                    miIntellectualToolStripMenuItem.Checked = false;
                    miManualToolStripMenuItem.Checked = true;
                    break;
            }
        }

        private void miPlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int errorCode = player.Play();
            if (errorCode != S_OK)
            {
                ShowErrorMessage(errorCode);
            }
            volumeBar.Refresh();
            seekBar.Refresh();
        }

        private void miPauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            player.Pause();
        }

        private void miStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            player.Clear();
            panelVideoOutput.Refresh();
            volumeBar.Refresh();
            seekBar.Refresh();
        }

        private void miAutomaticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeGraphMode(DirectShowGraphMode.Automatic);
        }

        private void miIntellectualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeGraphMode(DirectShowGraphMode.Intellectual);
        }

        private void miManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeGraphMode(DirectShowGraphMode.Manual);
        }

        private void ShowErrorMessage(int errorCode)
        {
            switch (errorCode)
            {
                case ERROR_FILE_NAME_NOT_DEFINED:
                    MessageBox.Show("Не указано имя файла!", "Ошибка!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case ERROR_FILE_NOT_FOUND:
                    MessageBox.Show($"Файл не найден!\n{player.FileName}", "Ошибка!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case ERROR_VIDEO_OUTPUT_WINDOW_NOT_DEFINED:
                    MessageBox.Show("Не указано окно для вывода видео!", "Ошибка!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case ERROR_NOTHING_RENDERED:
                    MessageBox.Show($"Не удалось отрендерить файл!\n{player.FileName}", "Ошибка!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case VFW_S_PARTIAL_RENDER:
                    MessageBox.Show($"Не удалось отрендерить файл!\n{player.FileName}\nОшибка VFW_S_PARTIAL_RENDER", "Ошибка!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                default:
                    MessageBox.Show(errorCode.ToString(), "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
    }
}
