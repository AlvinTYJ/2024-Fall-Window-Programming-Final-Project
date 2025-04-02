using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Final_Project
{
    public partial class MusicForm : Form
    {
        private PictureBoxButton folderButton, prevButton, playPauseButton, nextButton;
        private PictureBox albumArt;
        private List<string> mp3Files = new List<string>();
        private int currentTrackIndex = 0;
        private bool isPlaying = false;
        private bool isManuallyStopped = false;

        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        public MusicForm()
        {
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            this.Text = "音樂播放器";
            this.Size = new Size(600, 400);
            this.BackColor = Form1.backgroundColor;

            // Album Art
            albumArt = new PictureBox()
            {
                Size = new Size(100, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Form1.musicImage
            };
            this.Controls.Add(albumArt);

            // Folder Button (Select Music)
            folderButton = CreateButton(Form1.folderImage, FolderButton_Click);

            // Previous Button
            prevButton = CreateButton(Form1.leftButtonImage, PrevButton_Click);

            // Play/Pause Button
            playPauseButton = CreateButton(Form1.playImage, PlayPauseButton_Click);

            // Next Button
            nextButton = CreateButton(Form1.rightButtonImage, NextButton_Click);

            this.Resize += MusicForm_Resize;
            MusicForm_Resize(null, null);
        }

        // Create buttons with unified properties
        private PictureBoxButton CreateButton(Image buttonImage, EventHandler onClick)
        {
            PictureBoxButton button = new PictureBoxButton()
            {
                Size = new Size(75, 75)
            };
            button.SetOverlayImage(buttonImage);
            button.Click += onClick;
            this.Controls.Add(button);
            return button;
        }

        // Folder Button Click (Select MP3 Files directly from File Explorer)
        private void FolderButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Mp3 Files|*.mp3";
                ofd.Multiselect = true;  // Allow selecting multiple files
                ofd.Title = "選擇 MP3 文件";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    mp3Files = ofd.FileNames.ToList();
                    currentTrackIndex = 0;
                    StopPlayback();  // Stop any current playback
                    MessageBox.Show($"選擇了 {mp3Files.Count} 首歌曲！", "音樂加載成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("未選擇任何文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Play/Pause Button (Toggle Play/Pause)
        private void PlayPauseButton_Click(object sender, EventArgs e)
        {
            if (mp3Files.Count == 0)
            {
                MessageBox.Show("請先選擇文件並加載音樂", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!isPlaying)
            {
                PlayTrack();
            }
            else
            {
                PauseTrack();
            }
        }

        // Play the current track
        private void PlayTrack()
        {
            try
            {
                StopPlayback();  // Stop current playback before starting a new track

                outputDevice = new WaveOutEvent();
                audioFile = new AudioFileReader(mp3Files[currentTrackIndex]);
                outputDevice.Init(audioFile);

                outputDevice.Play();
                isPlaying = true;

                playPauseButton.SetOverlayImage(Form1.mpauseImage);  // Pause icon
                MessageBox.Show($"播放: {Path.GetFileName(mp3Files[currentTrackIndex])}", "播放音樂", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Unsubscribe to prevent multiple events and re-subscribe
                outputDevice.PlaybackStopped -= OnPlaybackStopped;
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"播放錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Pause the current track
        private void PauseTrack()
        {
            if (outputDevice != null)
            {
                outputDevice.Pause();
                isPlaying = false;
                playPauseButton.SetOverlayImage(Form1.mplayImage);  // Return to play icon
                MessageBox.Show("暫停音樂", "音樂狀態", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Stop and dispose resources
        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (!isManuallyStopped)  // Only reset if the track ends naturally
            {
                isPlaying = false;
                playPauseButton.SetOverlayImage(Form1.mplayImage);  // Show play icon
            }

            // Reset manual stop flag
            isManuallyStopped = false;
        }

        // Stop and dispose resources
        private void StopPlayback()
        {
            if (outputDevice != null)
            {
                isManuallyStopped = true;  // Indicate that this stop is manual

                outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;

                audioFile.Dispose();
                audioFile = null;

                isPlaying = false;
                playPauseButton.SetOverlayImage(Form1.mplayImage);
            }
        }

        // Play the next track
        private void NextButton_Click(object sender, EventArgs e)
        {
            if (mp3Files.Count > 0)
            {
                currentTrackIndex = (currentTrackIndex + 1) % mp3Files.Count;
                StopPlayback();
                PlayTrack();
            }
            else
            {
                MessageBox.Show("沒有可播放的音樂", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Play the previous track
        private void PrevButton_Click(object sender, EventArgs e)
        {
            if (mp3Files.Count > 0)
            {
                currentTrackIndex = (currentTrackIndex - 1 + mp3Files.Count) % mp3Files.Count;
                StopPlayback();
                PlayTrack();
            }
            else
            {
                MessageBox.Show("沒有可播放的音樂", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Resizing Layout
        private void MusicForm_Resize(object sender, EventArgs e)
        {
            ControlPositionHelper.PositionControl(albumArt, this, ControlPosition.OneThirdCenter);

            int spacing = 20;
            int totalWidth = folderButton.Width * 4 + spacing * 3;
            int startX = (this.ClientSize.Width - totalWidth) / 2;
            int buttonY = this.ClientSize.Height - 150;

            folderButton.Location = new Point(startX, buttonY);
            prevButton.Location = new Point(startX + folderButton.Width + spacing, buttonY);
            playPauseButton.Location = new Point(prevButton.Right + spacing, buttonY);
            nextButton.Location = new Point(playPauseButton.Right + spacing, buttonY);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            StopPlayback();
        }
    }
}
