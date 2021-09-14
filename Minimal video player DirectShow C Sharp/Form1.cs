using System;
using System.Drawing;
using System.Windows.Forms;
using static Minimal_video_player_DirectShow_C_Sharp.ZeratoolPlayerEngine;
using static Minimal_video_player_DirectShow_C_Sharp.DirectShowUtils;

namespace Minimal_video_player_DirectShow_C_Sharp
{
    public partial class Form1 : Form
    {
        private ZeratoolPlayerEngine player;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            player = new ZeratoolPlayerEngine();
            player.VideoOutputWindow = panelVideoOutput;
            player.FileName = @"H:\Downloads\completed\Doctor.Who.s11.720p.WEBRip.BaibaKo\Doctor.Who.s11e00.720p.WEBRip.BaibaKo.mkv";
            int errorCode = player.Play();
            if (errorCode == S_OK)
            {
                timer1.Enabled = true;
            }
            else
            {
                ShowErrorMessage(errorCode);
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

                case Keys.R:
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
                    break;
            }
        }

        private void panelVideoOutput_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && player != null)
            {
                PlayerPlayPause(player);
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

        private void PlayerPlayPause(ZeratoolPlayerEngine playerEngine)
        {
            switch (playerEngine.State)
            {
                case PlayerState.Paused:
                    playerEngine.Play();
                    break;

                case PlayerState.Playing:
                    playerEngine.Pause();
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

                default:
                    MessageBox.Show(errorCode.ToString(), "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
    }
}
