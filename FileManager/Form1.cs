using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using WMPLib;


namespace Player
{
    public partial class Form1 : Form
    {
        WindowsMediaPlayer wmp = new WindowsMediaPlayer();
        private List<string[]> playList = new List<string[]>();
        bool pausePlay;

        public Form1()
        {
            InitializeComponent();
        }

        public void button1_Click(object sender, EventArgs e)//пуск
        {
            try
            {
                wmp.controls.play();
                PositionBar.Enabled = true;
                VolumeBar.Enabled = true;
                VolumeBar.Value = VolumeBar.Maximum;
                timer1.Enabled = true;
                timer1.Interval = 1000;
                wmp.URL = playList[listBox1.SelectedIndex][1];
                pausePlay = !pausePlay;
                if (!pausePlay) wmp.controls.play();
                this.Text = playList[listBox1.SelectedIndex][0];
            }
            catch (Exception ex)
            {
                if (playList.Count == 0)
                {
                    MessageBox.Show("Error!\n" + ex);
                    button4_Click(sender,e);
                    this.Close();
                }

                else
                {
                    listBox1.SelectedIndex = 0;
                    wmp.URL = playList[0][1];
                }
            }
        }

        public void button2_Click(object sender, EventArgs e)//пауза/відновити
        {        
            if (!pausePlay)
            {
                wmp.controls.play();
                PauseButton.Text = "Play";
            }
            else
            {
                wmp.controls.pause();
                PauseButton.Text = "Pause";
            }
            pausePlay = !pausePlay;
        }

        public void button3_Click(object sender, EventArgs e)//стоп
        {
            wmp.controls.stop();
        }

        public void button4_Click(object sender, EventArgs e)//вибрати файл
        {
            var sourceFileOpenFileDialog = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Filter = @"MP3 Files (*.mp3)|*.mp3",
                RestoreDirectory = true,
                Multiselect = true,
                Title = @"Please select MP3 File(s) for opening"
            };

            if (sourceFileOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string fileName in sourceFileOpenFileDialog.FileNames)
                    {
                        string shortPath = Path.GetFileName(fileName);
                        shortPath = shortPath.Substring(0, shortPath.Length - 4);

                        string[] pathElements = new string[2];
                        pathElements[0] = shortPath;
                        pathElements[1] = fileName;

                        playList.Add(pathElements);
                        listBox1.Items.Add(shortPath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }    
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
                PositionBar.Maximum = Convert.ToInt32(wmp.currentMedia.duration);
                PositionBar.Value = Convert.ToInt32(wmp.controls.currentPosition);
            
            if (wmp != null)
            {
                int s = (int)wmp.currentMedia.duration;
                int h = s / 3600;
                int m = (s - (h * 3600)) / 60;
                s = s - (h * 3600 + m * 60);
                label3.Text = String.Format("{0:D}:{1:D2}:{2:D2}", h, m, s);

                s = (int)wmp.controls.currentPosition;
                h = s / 3600;
                m = (s - (h * 3600)) / 60;
                s = s - (h * 3600 + m * 60);
                label2.Text = String.Format("{0:D}:{1:D2}:{2:D2}", h, m, s);
            }
            else
            {
                label2.Text = "0:00:00";
                label3.Text = "0:00:00";
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            wmp.controls.currentPosition = PositionBar.Value;
        }

        public void trackBar2_Scroll(object sender, EventArgs e)
        {
            wmp.settings.volume = VolumeBar.Value;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                listBox1.SelectedIndex += 1;
            }
            catch (Exception) { listBox1.SelectedIndex = 0; }
            //if (listBox1.SelectedIndex >= listBox1.Items.Count) listBox1.SelectedIndex = 0;
            wmp.URL = playList[listBox1.SelectedIndex][1]; 
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBox1.SelectedIndex -= 1;
            if (listBox1.SelectedIndex < 0) listBox1.SelectedIndex = playList.Count-1;                           
            wmp.URL = playList[listBox1.SelectedIndex][1];
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            var sourceFileOpenFileDialog = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Filter = @"Playlist Files (*.pls)|*.pls",
                RestoreDirectory = true,
                Multiselect = false,
                Title = @"Please select playlist file for opening"
            };

            if (sourceFileOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string fileName = sourceFileOpenFileDialog.FileName;

                    using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        using (var sr = new StreamReader(fs, Encoding.GetEncoding(1251)))
                        {
                            playList.Clear();
                            listBox1.Items.Clear();

                            while (!sr.EndOfStream)
                            {
                                string[] pathElements = sr.ReadLine().Split('|');
                                playList.Add(pathElements);
                                listBox1.Items.Add(pathElements[0]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var sourceFileSaveFileDialog = new SaveFileDialog
            {
                InitialDirectory = @"C:\",
                Filter = @"Playlist Files (*.pls)|*.pls",
                RestoreDirectory = true,
                Title = @"Please select playlist file for saving"
            };

            if (sourceFileSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var fs = new FileStream(sourceFileSaveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                    {
                        using (var sw = new StreamWriter(fs, Encoding.GetEncoding(1251)))
                        {
                            foreach (var song in playList)
                            {
                                string outputString = song[0] + "|" + song[1];
                                sw.WriteLine(outputString);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            playList.Clear();
            listBox1.Items.Clear();
        }

    }
}
