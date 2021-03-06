using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using Unosquare.FFME;
using Unosquare.FFME.ClosedCaptions;
using Unosquare.FFME.Common;
using Path = System.IO.Path;

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

        public MediaOptions CurrentMediaOptions { get; set; }
        public RendererOptions RendererOptions { get; set; }




        public MainWindow()
        {

            Unosquare.FFME.Library.FFmpegDirectory = Directory.GetCurrentDirectory() + @"\Library\ffmpeg\";
            MediaPrimary = new Unosquare.FFME.MediaElement();
            MediaSecondary = new Unosquare.FFME.MediaElement();

            InitializeComponent();


            //PlayPrimary_ClickAsync += OnMediaOpening();
            //LoadingAsync();
        }

        public event EventHandler<MediaOpeningEventArgs> MediaOpening;
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



        private async void PlayPrimary_ClickAsync(object sender, RoutedEventArgs e)
        {



            Uri path = new Uri(VideoFilePrimary);

            MediaPrimary.MediaOpening += OnMediaOpening;
            MediaPrimary.MediaClosed -= OnMediaClosed;

            //MediaPrimary.Open(path);

            var target = new Uri(VideoFilePrimary);
            //if (target.ToString().StartsWith(FileInputStream.Scheme, StringComparison.OrdinalIgnoreCase))
            await MediaPrimary.Open(new FileInputStream(target.LocalPath));


            MediaPrimary.LoopingBehavior = MediaPlaybackState.Play;
            IsPausedPrimary = false;
            IsMutedPrimary = false;


            CurrentMediaOptions.VideoBlockCache = 100000;
            CurrentMediaOptions.MinimumPlaybackBufferPercent = 1.0;
            CurrentMediaOptions.UseParallelRendering = true;
            CurrentMediaOptions.UseParallelDecoding = true;

            var a = CurrentMediaOptions.VideoHardwareDevice;
        }

        private async void PausePrimary_Click(object sender, RoutedEventArgs e)
        {
            if (IsPausedPrimary)
            {
                await MediaPrimary.Play();
                IsPausedPrimary = false;
                return;
            }
            if (!IsPausedPrimary)
            {
                await MediaPrimary.Pause();
                IsPausedPrimary = true;
                return;
            }
        }

        private void StopPrimary_Click(object sender, RoutedEventArgs e)
        {
            MediaPrimary.Stop();
            MediaPrimary.Close();

        }
        private readonly object RecorderSyncLock = new object();
        private void OnMediaClosed(object sender, EventArgs e)
        {
            // Always close the recorder so that the file trailer is written.
            lock (RecorderSyncLock)
            {

            }

            CurrentMediaOptions = null;
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

        private async void OnMediaOpening(object sender, MediaOpeningEventArgs e)
        {
            // Capture a reference to the MediaOptions object for real-time change
            // This usage of MediaOptions is unsupported.
            CurrentMediaOptions = e.Options;


            // the event sender is the MediaElement itself
            var media = sender as Unosquare.FFME.MediaElement;

            // You can start off by adjusting subtitles delay
            // This defaults to 0 but you can delay (or advance with a negative delay)
            // the subtitle timestamps.
            e.Options.SubtitlesDelay = TimeSpan.Zero; // See issue #216

            // You can render audio and video as it becomes available but the downside of disabling time
            // synchronization is that video and audio will run on their own independent clocks.
            // Do not disable Time Sync for streams that need synchronized audio and video.
            e.Options.IsTimeSyncDisabled =
                e.Info.Format == "libndi_newtek" ||
                e.Info.MediaSource.StartsWith("rtsp://uno", StringComparison.OrdinalIgnoreCase);

            // You can disable the requirement of buffering packets by setting the playback
            // buffer percent to 0. Values of less than 0.5 for live or network streams are not recommended.
            e.Options.MinimumPlaybackBufferPercent = e.Info.Format == "libndi_newtek" ? 0 : 0.5;

            // The audio renderer will try to keep the audio hardware synchronized
            // to the playback position by default.
            // A few WMV files I have tested don't have continuous enough audio packets to support
            // perfect synchronization between audio and video so we simply disable it.
            // Also if time synchronization is disabled, the recommendation is to also disable audio synchronization.
            media.RendererOptions.AudioDisableSync =
                e.Options.IsTimeSyncDisabled ||
                e.Info.MediaSource.EndsWith(".wmv", StringComparison.OrdinalIgnoreCase);

            // Legacy audio out is the use of the WinMM api as opposed to using DirectSound
            // Enable legacy audio out if you are having issues with the DirectSound driver.
            media.RendererOptions.UseLegacyAudioOut = e.Info.MediaSource.EndsWith(".wmv", StringComparison.OrdinalIgnoreCase);

            // You can limit how often the video renderer updates the picture.
            // We keep it as 0 to refresh the video according to the native stream specification.
            media.RendererOptions.VideoRefreshRateLimit = 0;

            // Get the local file path from the URL (if possible)
            var mediaFilePath = string.Empty;
            try
            {
                var url = new Uri(e.Info.MediaSource);
                mediaFilePath = url.IsFile || url.IsUnc ? Path.GetFullPath(url.LocalPath) : string.Empty;
            }
            catch { /* Ignore Exceptions */ }

            // Example of automatically side-loading SRT subs
            if (string.IsNullOrWhiteSpace(mediaFilePath) == false)
            {
                var srtFilePath = Path.ChangeExtension(mediaFilePath, "srt");
                if (File.Exists(srtFilePath))
                    e.Options.SubtitlesSource = srtFilePath;
            }

            // You can also force video FPS if necessary
            // see: https://github.com/unosquare/ffmediaelement/issues/212
            // e.Options.VideoForcedFps = 25;

            // An example of selecting a specific subtitle stream
            var subtitleStreams = e.Info.Streams.Where(kvp => kvp.Value.CodecType == AVMediaType.AVMEDIA_TYPE_SUBTITLE).Select(kvp => kvp.Value);
            var englishSubtitleStream = subtitleStreams
                .FirstOrDefault(s => s.Language != null && s.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));

            if (englishSubtitleStream != null)
                e.Options.SubtitleStream = englishSubtitleStream;

            // An example of selecting a specific audio stream
            var audioStreams = e.Info.Streams.Where(kvp => kvp.Value.CodecType == AVMediaType.AVMEDIA_TYPE_AUDIO).Select(kvp => kvp.Value);
            var englishAudioStream = audioStreams
                .FirstOrDefault(s => s.Language != null && s.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));

            if (englishAudioStream != null)
                e.Options.AudioStream = englishAudioStream;

            // Setting Advanced Video Stream Options is also possible
            if (e.Options.VideoStream is StreamInfo videoStream)
            {
                // Example of forcing a codec for a stream
                // e.Options.DecoderCodec[videoStream.StreamIndex] = "mjpeg";

                // If we have a valid seek index let's use it!
                mediaFilePath = "C:\\Users\\Viva_\\Desktop\\2022-07-20 17-09-39Z_ef8a55_.mp4";
                if (string.IsNullOrWhiteSpace(mediaFilePath) == false)
                {
                    try
                    {
                        // Try to Create or Load a Seek Index
                        var durationSeconds = e.Info.Duration.TotalSeconds > 0 ? e.Info.Duration.TotalSeconds : 0;
                        var seekIndex = LoadOrCreateVideoSeekIndex(mediaFilePath, videoStream.StreamIndex, durationSeconds);

                        // Make sure the seek index belongs to the media file path
                        if (seekIndex != null &&
                            !string.IsNullOrWhiteSpace(seekIndex.MediaSource) &&
                            seekIndex.MediaSource.Equals(mediaFilePath, StringComparison.OrdinalIgnoreCase) &&
                            seekIndex.StreamIndex == videoStream.StreamIndex)
                        {
                            // Set the index on the options object.
                            e.Options.VideoSeekIndex = seekIndex;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception, and ignore it. Continue execution.
                        Debug.WriteLine($"Error loading seek index data. {ex.Message}");
                    }
                }

                // Hardware device priorities
                var deviceCandidates = new[]
                {
                    AVHWDeviceType.AV_HWDEVICE_TYPE_CUDA,
                    AVHWDeviceType.AV_HWDEVICE_TYPE_D3D11VA,
                    AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2
                };

                // Hardware device selection
                if (videoStream.FPS <= 30)
                {
                    foreach (var deviceType in deviceCandidates)
                    {
                        var accelerator = videoStream.HardwareDevices.FirstOrDefault(d => d.DeviceType == deviceType);
                        if (accelerator == null) continue;
                        if (Debugger.IsAttached)
                            e.Options.VideoHardwareDevice = accelerator;

                        break;
                    }
                }

                // Start building a video filter
                var videoFilter = new StringBuilder();

                // The yadif filter de-interlaces the video; we check the field order if we need
                // to de-interlace the video automatically
                if (videoStream.IsInterlaced)
                    videoFilter.Append("yadif,");

                // Scale down to maximum 1080p screen resolution.
                if (videoStream.PixelHeight > 1080)
                {
                    // e.Options.VideoHardwareDevice = null;
                    videoFilter.Append("scale=-1:1080,");
                }

                // Example of fisheye correction filter:
                // videoFilter.Append("lenscorrection=cx=0.5:cy=0.5:k1=-0.85:k2=0.25,")
                e.Options.VideoFilter = videoFilter.ToString().TrimEnd(',');

                // Since the MediaElement control belongs to the GUI thread
                // and the closed captions channel property is a dependency
                // property, we need to set it on the GUI thread.
                await media.Dispatcher?.InvokeAsync(() =>
                {
                    media.ClosedCaptionsChannel = videoStream.HasClosedCaptions ?
                        CaptionsChannel.CC1 : CaptionsChannel.CCP;
                });
            }

            // Examples of setting audio filters.
            // e.Options.AudioFilter = "aecho=0.8:0.9:1000:0.3";
            // e.Options.AudioFilter = "chorus=0.5:0.9:50|60|40:0.4|0.32|0.3:0.25|0.4|0.3:2|2.3|1.3";
            // e.Options.AudioFilter = "aphaser";
        }


        private VideoSeekIndex LoadOrCreateVideoSeekIndex(string mediaFilePath, int streamIndex, double durationSeconds)
        {
            var seekFileName = $"{Path.GetFileNameWithoutExtension(mediaFilePath)}.six";
            var sindx = Directory.GetCurrentDirectory() + @"\SeekIndexes";
            var seekFilePath = Path.Combine(sindx, seekFileName);

            if (!Directory.Exists(sindx))
            {
                Directory.CreateDirectory(sindx);
            }


            if (string.IsNullOrWhiteSpace(seekFilePath)) return null;

            if (File.Exists(seekFilePath))
            {
                using var stream = File.OpenRead(seekFilePath);
                return VideoSeekIndex.Load(stream);
            }
            else
            {
                if (!Debugger.IsAttached || durationSeconds <= 0 || durationSeconds >= 60)
                    return null;

                var seekIndex = Library.CreateVideoSeekIndex(mediaFilePath, streamIndex);
                if (seekIndex.Entries.Count <= 0) return null;

                using (var stream = File.OpenWrite(seekFilePath))
                    seekIndex.Save(stream);

                return seekIndex;
            }
        }

        private struct EqualizerFilterValues
        {
            public double Contrast;
            public double Brightness;
            public double Saturation;
        }

        // https://unix.stackexchange.com/questions/233832/merge-two-video-clips-into-one-placing-them-next-to-each-other
        // https://video.stackexchange.com/questions/20962/ffmpeg-color-correction-gamma-brightness-and-saturation

        private void FilterPrimary_Click(object sender, RoutedEventArgs e)
        {

            var currentValues = new EqualizerFilterValues { Contrast = 1d, Brightness = 0d, Saturation = 1d };

            double? saturation = PrimarySaturation.Value;
            double? contrast = PrimaryContrast.Value;
            double? brightness = PrimaryBrightness.Value;
            //string scale = Math.Round(PrimaryScale.Value, 3).ToString().Replace(",", ":");

            var ex = "scale=-1:5080,eq=contrast=+1.300:brightness=+0.200:saturation=+1.000";

            var VideoEqContrast = "eq=contrast=";
            var VideoEqBrightness = ":brightness=";
            var VideoEqSaturation = ":saturation=";

            contrast = contrast == null ? currentValues.Contrast : contrast < -2d ? -2d : contrast > 2d ? 2d : contrast;
            brightness = brightness == null ? currentValues.Brightness : brightness < -1d ? -1d : brightness > 1d ? 1d : brightness;
            saturation = saturation == null ? currentValues.Saturation : saturation < 0d ? 0d : saturation > 3d ? 3d : saturation;

            var targetFilter = $"{VideoEqContrast}{contrast:+0.000;-0.000}{VideoEqBrightness}{brightness:+0.000;-0.00}{VideoEqSaturation}{saturation:+0.000;-0.000}";

            targetFilter = targetFilter.Replace(',', '.');

            // VideoFilter = string.Format(CultureInfo.InvariantCulture, "scale={3},eq=contrast=+1.{1}:brightness=+0.{2}:saturation=+1.{0}", saturation, contrast, brightness, scale);

            CurrentMediaOptions.VideoFilter = targetFilter;
        }

        private void ScalePrimary_Click(object sender, RoutedEventArgs e)
        {
            var scale = PrimaryScale.Value;

            var m = MediaPrimary;
            if (m == null) return;

            var transform = m.RenderTransform as ScaleTransform;
            if (transform == null)
            {
                transform = new ScaleTransform(1, 1);
                m.RenderTransformOrigin = new Point(0.5, 0.5);
                m.RenderTransform = transform;
            }

            //if (transform.ScaleX < 0.1d || transform.ScaleY < 0.1)
            //{
            //    transform.ScaleX = 0.1d;
            //    transform.ScaleY = 0.1d;
            //}
            //else if (transform.ScaleX > 5d || transform.ScaleY > 5)
            //{
            //    transform.ScaleX = 5;
            //    transform.ScaleY = 5;
            //}

            transform.ScaleX = scale;
            transform.ScaleY = scale;


            transform = new ScaleTransform(scale, scale);
            transform.CenterX = m.TransformToAncestor(StackMain).Transform(new Point(0, 0)).X;
            transform.CenterY = m.TransformToAncestor(StackMain).Transform(new Point(0, 0)).Y;
            GeneralTransform generalTransform = StackMain.TransformToDescendant(m);
            Point currentPoint = generalTransform.Transform(new Point(0, 0));
            //m.RenderTransformOrigin = new Point(scale, scale);
            //m.RenderTransform = transform;



        }

        private void SlowDownPrimary_Click(object sender, RoutedEventArgs e)
        {
            MediaPrimary.SpeedRatio = MediaPrimary.SpeedRatio * 0.9;
        }

        private void SpeepUpPrimary_Click(object sender, RoutedEventArgs e)
        {
            MediaPrimary.SpeedRatio = MediaPrimary.SpeedRatio * 1.1;
        }

        private async void ScreenShot_Click(object sender, RoutedEventArgs e)
        {
            //// Obtain the bitmap
            //var bmp = await MediaPrimary.CaptureBitmapAsync().GetAwaiter().GetResult();

            //var scrDir = Directory.GetCurrentDirectory() + @"\Screenshots";

            //// prevent firther processing if we did not get a bitmap.
            //bmp?.Save(scrDir + @"\" + DateTime.Now.ToString("hh-mm-ss") + ".png");

            var captureTask = Task.Run(() =>
            {
                try
                {
                    // Obtain the bitmap
                    var bmp = MediaPrimary.CaptureBitmapAsync().GetAwaiter().GetResult();

                    // prevent firther processing if we did not get a bitmap.
                    bmp?.Save(GetCaptureFilePath("Screenshot", "png"), ImageFormat.Png);
                    //ViewModel.NotificationMessage = "Captured screenshot.";
                }
                catch (Exception ex)
                {
                    var messageTask = Dispatcher.InvokeAsync(() =>
                    {
                        MessageBox.Show(
                            this,
                            $"Capturing Video Frame Failed: {ex.GetType()}\r\n{ex.Message}",
                            $"{nameof(Unosquare.FFME.MediaElement)} Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error,
                            MessageBoxResult.OK);
                    });
                }
                finally
                {
                    // unlock for further captures.
                    // IsCaptureInProgress = false;
                }
            });


        }

        public static string GetCaptureFilePath(string mediaPrefix, string extension)
        {
            var date = DateTime.UtcNow;
            var dateString = $"{date.Year:0000}-{date.Month:00}-{date.Day:00} {date.Hour:00}-{date.Minute:00}-{date.Second:00}.{date.Millisecond:000}";
            //var targetFolder = Path.Combine(
            //    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            //    "ffmeplay");

            var targetFolder = Directory.GetCurrentDirectory() + @"\Screenshots";

            if (Directory.Exists(targetFolder) == false)
                Directory.CreateDirectory(targetFolder);

            var targetFilePath = Path.Combine(targetFolder, $"{mediaPrefix} {dateString}.{extension}");
            if (File.Exists(targetFilePath))
                File.Delete(targetFilePath);

            return targetFilePath;
        }

        // Таймер для запуска показа изображений как видео
        public Timer ImageVideoTimer;
        public double FPS { get; set; }
        public TimeSpan SeekingTimeSpan { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public TimeSpan ActualPosition { get; set; }
        public TimeSpan FrameDuration { get; set; }


        public TimeSpan StartTime = new TimeSpan(0, 0, 0, 0, 0);
        public TimeSpan EndTime { get; set; }

        public TimeSpan PredFrame { get; set; }

        private async void RunBoth_Click(object sender, RoutedEventArgs e)
        {
            //ImageVideoTimer.Change(Timeout.Infinite, Timeout.Infinite);
            PredFrame = new TimeSpan();
            FPS = MediaPrimary.VideoFrameRate;
            MediaPrimary.ScrubbingEnabled = false;
            ActualPosition = (TimeSpan)MediaPrimary.ActualPosition;

            TotalDuration = (TimeSpan)MediaPrimary.NaturalDuration;
            EndTime = (TimeSpan)MediaPrimary.NaturalDuration;

            FrameDuration = TotalDuration / FPS;
            //var FramePerMs = 
            //FrameDuration = MediaPrimary

            // go to end
            MediaPrimary.Seek((TimeSpan)MediaPrimary.PlaybackStartTime);

            // Thread.Sleep(500);
            object objxr = 0;
            TimerCallback xrtm = new TimerCallback(ReverseVideoPlayback);

            PredFrame = TotalDuration - FrameDuration;

            var MSecPerFrame = Convert.ToInt32((1.0 / FPS) * 1000);
            //RecordedImagesCount = FPS * RecordTime;

            ImageVideoTimer = new Timer(xrtm, objxr, 0, (int)((int)MSecPerFrame * 1000));

            //MediaPrimary.Seek(new TimeSpan(0, 0, 0, 30, 0));
        }

        public void ReverseVideoPlayback(object obj)
        {
            int x = (int)obj;

            //PredFrame = PredFrame - FrameDuration;

            // Thread.Sleep(500);
            MediaPrimary.StepForward();
            if (ActualPosition <= new TimeSpan(0, 0, 0, 1, 0))
            {
                ImageVideoTimer.Change(Timeout.Infinite, Timeout.Infinite);
                MediaPrimary.Stop();
                MediaPrimary.Play();
            }

        }


        int tie = 100;
        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {



            MediaPrimary.ScrubbingEnabled = false;
            MediaPrimary.Position = new TimeSpan(0, 0, 0, tie, 0);
            tie = tie - 1;
        }

        private async void NextFrame_Click(object sender, RoutedEventArgs e)
        {
            TotalDuration = (TimeSpan)MediaPrimary.NaturalDuration;
            FPS = MediaPrimary.VideoFrameRate;
            ActualPosition = (TimeSpan)MediaPrimary.ActualPosition;
            FrameDuration = TotalDuration / FPS;

            var nextFrameTime = ActualPosition + FrameDuration / FPS;
            //MediaPrimary.Pause();
            MediaPrimary.StepForward();
            //MediaPrimary.Play();
            //MediaPrimary.Pause();
        }

        private async void BackFrame_Click(object sender, RoutedEventArgs e)
        {
            TotalDuration = (TimeSpan)MediaPrimary.NaturalDuration;
            FPS = MediaPrimary.VideoFrameRate;
            ActualPosition = (TimeSpan)MediaPrimary.ActualPosition;
            FrameDuration = TotalDuration / FPS;

            var info = MediaPrimary.MediaInfo.Duration;

            var backFrameTime = ActualPosition - FrameDuration / FPS;

            await MediaPrimary.StepBackward();

            ActualPosition = (TimeSpan)MediaPrimary.ActualPosition;
        }

        //System.Windows.Threading.DispatcherTimer dispatcherTimer;
        //int t = 240000; // 4 minutes = 240,000 milliseconds
        //private void dispatcherTimer_Tick(object sender, EventArgs e)
        //{
        //    // Go back 1 frame every 42 milliseconds (or 24 fps)
        //    t = t - 33;
        //    MediaPrimary.Position = TimeSpan.FromMilliseconds(t);
        //}

    }
    public sealed unsafe class FileInputStream : IMediaInputStream
    {
        private readonly FileStream BackingStream;
        private readonly object ReadLock = new object();
        private readonly byte[] ReadBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInputStream"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public FileInputStream(string path)
        {
            var fullPath = Path.GetFullPath(path);
            BackingStream = File.OpenRead(fullPath);
            var uri = new Uri(fullPath);
            StreamUri = new Uri(uri.ToString().ReplaceOrdinal("file://", Scheme));
            CanSeek = true;
            ReadBuffer = new byte[ReadBufferLength];
        }

        /// <summary>
        /// The custom file scheme (URL prefix) including the :// sequence.
        /// </summary>
        public static string Scheme => "customfile://";

        /// <inheritdoc />
        public Uri StreamUri { get; }

        /// <inheritdoc />
        public bool CanSeek { get; }

        /// <inheritdoc />
        public int ReadBufferLength => 1024 * 16;

        /// <inheritdoc />
        public InputStreamInitializing OnInitializing { get; }

        /// <inheritdoc />
        public InputStreamInitialized OnInitialized { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            BackingStream?.Dispose();
        }

        /// <summary>
        /// Reads from the underlying stream and writes up to <paramref name="targetBufferLength" /> bytes
        /// to the <paramref name="targetBuffer" />. Returns the number of bytes that were written.
        /// </summary>
        /// <param name="opaque">The opaque.</param>
        /// <param name="targetBuffer">The target buffer.</param>
        /// <param name="targetBufferLength">Length of the target buffer.</param>
        /// <returns>
        /// The number of bytes that have been read.
        /// </returns>
        public int Read(void* opaque, byte* targetBuffer, int targetBufferLength)
        {
            lock (ReadLock)
            {
                try
                {
                    var readCount = BackingStream.Read(ReadBuffer, 0, ReadBuffer.Length);
                    if (readCount > 0)
                        Marshal.Copy(ReadBuffer, 0, (IntPtr)targetBuffer, readCount);

                    return readCount;
                }
                catch (Exception)
                {
                    return ffmpeg.AVERROR_EOF;
                }
            }
        }

        /// <inheritdoc />
        public long Seek(void* opaque, long offset, int whence)
        {
            lock (ReadLock)
            {
                try
                {
                    return whence == ffmpeg.AVSEEK_SIZE ?
                        BackingStream.Length : BackingStream.Seek(offset, SeekOrigin.Begin);
                }
                catch
                {
                    return ffmpeg.AVERROR_EOF;
                }
            }
        }
    }
}
