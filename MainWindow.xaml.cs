using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            Unosquare.FFME.Library.FFmpegDirectory = @"C:\ffmpeg\x64";

          
            MediaPrimary = new Unosquare.FFME.MediaElement();
            MediaSecondary = new Unosquare.FFME.MediaElement();
           

            InitializeComponent();

            LoadingAsync();
        }
        async Task LoadingAsync()
        {
            string p = @"C:\Repozitories\Thor.mp4";

            Uri path = new Uri(p);

            await MediaPrimary.Open(path);


            string pw = @"C:\Repozitories\video.mp4";

            Uri pathw = new Uri(pw);

            await MediaSecondary.Open(pathw);
        }



    }
}
