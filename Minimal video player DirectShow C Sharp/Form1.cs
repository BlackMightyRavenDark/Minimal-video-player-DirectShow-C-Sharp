using System;
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
            int errorCode = player.BuildGraph();
            if (errorCode != S_OK)
            {
                ShowError(errorCode);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            player.Clear();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            player.SetVideoOutputRectangle(panelVideoOutput.ClientRectangle);
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
                    double pos = player.Position;
                    player.Clear();
                    if (player.BuildGraph() == S_OK && pos > 0.0)
                    {
                        player.Position = pos;
                    }
                    break;
            }
        }

        private void ShowError(int errorCode)
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
                default:
                    MessageBox.Show(errorCode.ToString(), "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
    }
}
