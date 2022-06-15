using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VideoPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string VideoFilePrimary;
        private string VideoFileSecondary;

        bool IsPausedPrimary;
        bool IsPausedSecondary;


        bool IsMutedPrimary;
        bool IsMutedSecondary;

        public MainWindow()
        {

            Unosquare.FFME.Library.FFmpegDirectory = @"C:\ffmpeg\x64";
            MediaPrimary = new Unosquare.FFME.MediaElement();
            MediaSecondary = new Unosquare.FFME.MediaElement();

            InitializeComponent();

      
            //LoadingAsync();
        }
        async Task LoadingAsync()
        {
            string p = @"C:\ffmpeg\videos\Thor1.mp4";

            Uri path = new Uri(p);

            await MediaPrimary.Open(path);


            string pw = @"C:\ffmpeg\videos\Thor2.mp4";

            Uri pathw = new Uri(pw);

            await MediaSecondary.Open(pathw);
        }

        private void BrowsePrimary_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Videos"; // Default file name
            dlg.DefaultExt = ".mp4"; // Default file extension
            dlg.Filter = "Video files (.mp4)|*.mp4"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                VideoFilePrimary = filename;
                VideoPathPrimary.Text = VideoFilePrimary;
            }


        }

        private void BrowseSecondary_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Videos"; // Default file name
            dlg.DefaultExt = ".mp4"; // Default file extension
            dlg.Filter = "Video files (.mp4)|*.mp4"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                VideoFileSecondary = filename;
                VideoPathSecondary.Text = VideoFileSecondary;
            }
        }

        private void PlayPrimary_ClickAsync(object sender, RoutedEventArgs e)
        {
            Uri path = new Uri(VideoFilePrimary);
            MediaPrimary.Open(path);
            IsPausedPrimary = false;
            IsMutedPrimary = false;
        }

        private async void PausePrimary_Click(object sender, RoutedEventArgs e)
        {
            if (IsPausedPrimary)
            {
                await MediaPrimary.Play();
                IsPausedPrimary = false;
                return;
            }
            if(!IsPausedPrimary)
            {
                await MediaPrimary.Pause();
                IsPausedPrimary = true;
                return;
            }
        }

        private void StopPrimary_Click(object sender, RoutedEventArgs e)
        {
            MediaPrimary.Stop();
        }

        private void PlaySecondary_Click(object sender, RoutedEventArgs e)
        {
            Uri path = new Uri(VideoFileSecondary);
            MediaSecondary.Open(path);
            IsPausedSecondary = false;
            IsMutedSecondary = false;
        }

        private async void PauseSecondary_Click(object sender, RoutedEventArgs e)
        {

            if (IsPausedSecondary)
            {
                await MediaSecondary.Play();
                IsPausedSecondary = false;
                return;
            }
            if (!IsPausedSecondary)
            {
                await MediaSecondary.Pause();
                IsPausedSecondary = true;
                return;
            }



        }

        private void StopSecondary_Click(object sender, RoutedEventArgs e)
        {
            MediaSecondary.Stop();
        }

        private void MutePrimary_Click(object sender, RoutedEventArgs e)
        {
            if (IsMutedPrimary)
            {
                MediaPrimary.IsMuted = false;
                IsMutedPrimary = false;
                return;
            }
            if (!IsMutedPrimary)
            {
                MediaPrimary.IsMuted = true;
                IsMutedPrimary = true;
                return;
            }


        }

        private void MuteSecondary_Click(object sender, RoutedEventArgs e)
        {
            if (IsMutedSecondary)
            {
                MediaSecondary.IsMuted = false;
                IsMutedSecondary = false;
                return;
            }
            if (!IsMutedSecondary)
            {
                MediaSecondary.IsMuted = true;
                IsMutedSecondary = true;
                return;
            }


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          
        }
    }
}
