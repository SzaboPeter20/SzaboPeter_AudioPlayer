using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace AudioPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer.MediaEnded += new EventHandler(Media_Ended);
        }
        readonly MediaPlayer mediaPlayer = new MediaPlayer();
        readonly DispatcherTimer timer = new DispatcherTimer();
        int positionSliderIsMoving = 0;
        int isPlaying = 0;
        int playing = -1;
        int repeatType = 0;

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (SongsListBox.Items.Count != 0)
            {
                if (isPlaying == 0)
                {
                    if (SongsListBox.SelectedIndex != -1)
                    {
                        string file = SongsListBox.Items[SongsListBox.SelectedIndex].ToString();
                        mediaPlayer.Open(new Uri(file));
                        mediaPlayer.Play();
                        isPlaying = 2;
                        playing = Convert.ToInt32(SongsListBox.SelectedIndex);
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += Timer_Tick;
                        timer.Start();
                        PositionSlider.IsEnabled = true;
                        mediaPlayer.MediaFailed += Media_Failed;
                    }
                    else
                    {
                        SongsListBox.SelectedIndex = 0;
                        string file = SongsListBox.Items[0].ToString();
                        mediaPlayer.Open(new Uri(file));
                        mediaPlayer.Play();
                        isPlaying = 2;
                        playing = 0;
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += Timer_Tick;
                        timer.Start();
                        PositionSlider.IsEnabled = true;
                        mediaPlayer.MediaFailed += Media_Failed;
                    }
                }
                else if (isPlaying == 1)
                {
                    mediaPlayer.Play();
                    isPlaying = 2;
                }
                else
                {
                    mediaPlayer.Pause();
                    isPlaying = 1;
                }
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            SongsListBox.SelectedIndex = -1;
            mediaPlayer.Close();
            timer.Stop();
            isPlaying = 0;
            playing = -1;
            PositionSlider.Value = 0;
            PositionSlider.Maximum = 1;
            PositionSlider.IsEnabled = false;
            PositionLabel.Content = "00:00/00:00";
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (SongsListBox.Items.Count != 0)
            {
                if (mediaPlayer.Position.TotalSeconds > 3)
                {
                    if (playing == -1)
                    {
                        SongsListBox.SelectedIndex = 0;
                        string file = SongsListBox.Items[0].ToString();
                        mediaPlayer.Open(new Uri(file));
                        mediaPlayer.Play();
                        isPlaying = 2;
                        playing = 0;
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += Timer_Tick;
                        timer.Start();
                        PositionSlider.IsEnabled = true;
                        mediaPlayer.MediaFailed += Media_Failed;
                    }
                    else
                    {
                        mediaPlayer.Position = TimeSpan.FromSeconds(0);
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += Timer_Tick;
                        timer.Start();
                    }
                }
                else
                {
                    if (playing == 0 || playing == -1)
                    {
                        SongsListBox.SelectedIndex = 0;
                        string file = SongsListBox.Items[0].ToString();
                        mediaPlayer.Open(new Uri(file));
                        mediaPlayer.Play();
                        isPlaying = 2;
                        playing = 0;
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += Timer_Tick;
                        timer.Start();
                        PositionSlider.IsEnabled = true;
                        mediaPlayer.MediaFailed += Media_Failed;
                    }
                    else
                    {
                        playing--;
                        SongsListBox.SelectedIndex = playing;
                        string file = SongsListBox.Items[SongsListBox.SelectedIndex].ToString();
                        mediaPlayer.Open(new Uri(file));
                        mediaPlayer.Play();
                        isPlaying = 2;
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += Timer_Tick;
                        timer.Start();
                        PositionSlider.IsEnabled = true;
                        mediaPlayer.MediaFailed += Media_Failed;
                    }
                }
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (SongsListBox.Items.Count != 0)
            {
                if (SongsListBox.SelectedIndex != SongsListBox.Items.Count - 1)
                {

                    playing++;
                    SongsListBox.SelectedIndex = playing;
                    string file = SongsListBox.Items[SongsListBox.SelectedIndex].ToString();
                    mediaPlayer.Open(new Uri(file));
                    mediaPlayer.Play();
                    isPlaying = 2;
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += Timer_Tick;
                    timer.Start();
                    PositionSlider.IsEnabled = true;
                    mediaPlayer.MediaFailed += Media_Failed;
                }
                else
                {
                    SongsListBox.SelectedIndex = 0;
                    string file = SongsListBox.Items[0].ToString();
                    mediaPlayer.Open(new Uri(file));
                    mediaPlayer.Play();
                    isPlaying = 2;
                    playing = 0;
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += Timer_Tick;
                    timer.Start();
                    PositionSlider.IsEnabled = true;
                    mediaPlayer.MediaFailed += Media_Failed;
                }
            }
        }

        private void PositionSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            positionSliderIsMoving = 1;
        }

        private void PositionSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            positionSliderIsMoving = 0;
            mediaPlayer.Position = TimeSpan.FromSeconds(PositionSlider.Value);
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PositionLabel.Content = String.Format("{0}/{1}", TimeSpan.FromSeconds(PositionSlider.Value).ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if ((mediaPlayer.Source != null) && mediaPlayer.NaturalDuration.HasTimeSpan && (positionSliderIsMoving == 0))
            {
                PositionLabel.Content = String.Format("{0}/{1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                PositionSlider.Minimum = 0;
                PositionSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds - 1;
                PositionSlider.Value = mediaPlayer.Position.TotalSeconds;
            }
        }

        private void VolumeSlider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            mediaPlayer.Volume = VolumeSlider.Value;
        }

        private void SongsListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string file = SongsListBox.Items[SongsListBox.SelectedIndex].ToString();
            mediaPlayer.Open(new Uri(file));
            mediaPlayer.Play();
            isPlaying = 2;
            playing = Convert.ToInt32(SongsListBox.SelectedIndex);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            PositionSlider.IsEnabled = true;
            mediaPlayer.MediaFailed += Media_Failed;
        }
       
        private void AddSongsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "MP3 files (*.mp3)|*.mp3",
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    if (!SongsListBox.Items.Contains(file))
                    {
                        SongsListBox.Items.Add(file);
                    }
                }
            }
        }

        private void RemoveSongButton_Click(object sender, RoutedEventArgs e)
        {
            if (SongsListBox.Items.Count == 0)
            {
                MessageBox.Show("There are no song to be removed.", "Empty", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (SongsListBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Select a song from the playlist first.", "Select a song first", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (SongsListBox.SelectedIndex == playing)
                    {
                        SongsListBox.Items.Remove(SongsListBox.SelectedItem);
                        SongsListBox.SelectedIndex = -1;
                        mediaPlayer.Close();
                        timer.Stop();
                        isPlaying = 0;
                        playing = -1;
                        PositionSlider.Value = 0;
                        PositionSlider.Maximum = 1;
                        PositionLabel.Content = "00:00/00:00";
                        PositionSlider.IsEnabled = false;
                    }
                    else
                    {
                        if (SongsListBox.SelectedIndex > playing)
                        {
                            SongsListBox.Items.Remove(SongsListBox.SelectedItem);
                            SongsListBox.SelectedIndex = playing;
                        }
                        else
                        {
                            playing--;
                            SongsListBox.Items.Remove(SongsListBox.SelectedItem);
                            SongsListBox.SelectedIndex = playing;
                        }
                    }
                }
            }
        }

        private void LoadPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt"
            };
            open.ShowDialog();
            StreamReader sr = new StreamReader(open.FileName);
            string ok = sr.ReadLine();
            if (SongsListBox.Items.Count != 0)
            {
                MessageBoxResult mbr = MessageBox.Show("Opening a saved playlist will clear your playlist. Are you sure you want to open your saved playlist? If you would like to first save your playlist, click \"No\", then click \"Save Playlist...\".", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (mbr == MessageBoxResult.Yes)
                {
                    SongsListBox.Items.Clear();
                    SongsListBox.SelectedIndex = -1;
                    mediaPlayer.Close();
                    timer.Stop();
                    isPlaying = 0;
                    playing = -1;
                    PositionSlider.Value = 0;
                    PositionSlider.Maximum = 1;
                    PositionSlider.IsEnabled = false;
                    PositionLabel.Content = "00:00/00:00";
                    do
                    {
                        SongsListBox.Items.Add(sr.ReadLine());
                    } while (!sr.EndOfStream);
                }
            }
            else
            {
                do
                {
                    string path = sr.ReadLine();
                    SongsListBox.Items.Add(path);
                } while (!sr.EndOfStream);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (SongsListBox.Items.Count != 0)
            {
                MessageBoxResult mbr = MessageBox.Show(" Are you sure? This will clear your playlist. If you would like to first save your playlist, click \"No\", then click \"Save Playlist...\".", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (mbr == MessageBoxResult.Yes)
                {
                    SongsListBox.Items.Clear();
                    SongsListBox.SelectedIndex = -1;
                    mediaPlayer.Close();
                    timer.Stop();
                    isPlaying = 0;
                    playing = -1;
                    PositionSlider.Value = 0;
                    PositionSlider.Maximum = 1;
                    PositionSlider.IsEnabled = false;
                    PositionLabel.Content = "00:00/00:00";
                }
            }
            else
            {
                MessageBox.Show("Your playlist is empty.", "Empty", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SavePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            if (SongsListBox.Items.Count == 0)
            {
                MessageBox.Show("You cannot save an empty playlist.", "Empty", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                SaveFileDialog sf = new SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt"
                };
                sf.ShowDialog();
                StreamWriter sw = new StreamWriter(sf.FileName);
                sw.WriteLine("Audio Player version 1.0. Below is your playlist.");
                for (int i = 0; i < SongsListBox.Items.Count; i++)
                {
                    sw.WriteLine(SongsListBox.Items[i].ToString());
                }
                sw.Close();
                MessageBox.Show("Your playlist has been successfully saved.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Media_Ended(object sender, EventArgs e)
        {
            if (repeatType == 0)
            {
                if (playing < (SongsListBox.Items.Count - 1))
                {
                    playing++;
                    SongsListBox.SelectedIndex = playing;
                    string file = SongsListBox.Items[SongsListBox.SelectedIndex].ToString();
                    mediaPlayer.Open(new Uri(file));
                    mediaPlayer.Play();
                    isPlaying = 2;
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += Timer_Tick;
                    timer.Start();
                    PositionSlider.IsEnabled = true;
                    mediaPlayer.MediaFailed += Media_Failed;
                }
                else
                {
                    SongsListBox.SelectedIndex = -1;
                    mediaPlayer.Close();
                    timer.Stop();
                    isPlaying = 0;
                    playing = -1;
                    PositionSlider.Value = 0;
                    PositionSlider.Maximum = 1;
                    PositionSlider.IsEnabled = false;
                    PositionLabel.Content = "00:00/00:00";
                }
            }
            else if (repeatType == 1)
            {
                if (playing < (SongsListBox.Items.Count - 1))
                {
                    playing++;
                    SongsListBox.SelectedIndex = playing;
                    string file = SongsListBox.Items[SongsListBox.SelectedIndex].ToString();
                    mediaPlayer.Open(new Uri(file));
                    mediaPlayer.Play();
                    isPlaying = 2;
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += Timer_Tick;
                    timer.Start();
                    PositionSlider.IsEnabled = true;
                    mediaPlayer.MediaFailed += Media_Failed;
                }
                else
                {
                    playing = 0;
                    SongsListBox.SelectedIndex = playing;
                    string file = SongsListBox.Items[SongsListBox.SelectedIndex].ToString();
                    mediaPlayer.Open(new Uri(file));
                    mediaPlayer.Play();
                    isPlaying = 2;
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += Timer_Tick;
                    timer.Start();
                    PositionSlider.IsEnabled = true;
                    mediaPlayer.MediaFailed += Media_Failed;
                }
            }
            else if (repeatType == 2)
            {
                mediaPlayer.Position = TimeSpan.FromSeconds(0);
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;
                timer.Start();

            }
        }

        private void Media_Failed(object sender, EventArgs e)
        {
            MessageBox.Show("The requested audio file could not be opened. Please check if the file's location has been changed or remove this audio file from the list, then readd it to the list.", "An error occured", MessageBoxButton.OK, MessageBoxImage.Error);
            SongsListBox.SelectedIndex = -1;
            mediaPlayer.Close();
            timer.Stop();
            isPlaying = 0;
            playing = -1;
            PositionSlider.Value = 0;
            PositionSlider.Maximum = 1;
            PositionSlider.IsEnabled = false;
            PositionLabel.Content = "00:00/00:00";
        }

        private void SongsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
