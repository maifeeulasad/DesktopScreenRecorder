using Accord.Video.FFMPEG;
using MahApps.Metro.Controls;
using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DesktopScreenRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Global Variables
        /// </summary>
        private static VideoFileWriter vf = new VideoFileWriter();
        private static bool rec = false;
        private Thread recThread = new Thread(record);

        /// <summary>
        /// Initialize Main Window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handel recording button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // get calling button
                Button bt = (Button)sender;

                // Check recording on/off
                if (!rec)
                {                    
                    bt.Content = "Recording...";
                    rec = true;
                    
                    // Open new video file
                    vf.Open("files\\Exported_Video_" + Guid.NewGuid().ToString() + ".avi", Convert.ToInt32(SystemParameters.PrimaryScreenWidth), Convert.ToInt32(SystemParameters.PrimaryScreenHeight), 25, VideoCodec.MPEG4, 1000000);

                    // Start recording thread.
                    recThread.Start();
                }
                else
                {
                    rec = false;

                    // Close video writeing
                    vf.Close();

                    // Stop recording thread.
                    recThread.Abort();
                    bt.Content = "Click to record...";
                }
            }
            catch
            {
                //TODO: Log Error
            }

        }

        /// <summary>
        /// Record method for recording thread
        /// </summary>
        private static void record()
        {
            try
            {
                while (true)
                {
                    if (rec && vf.IsOpen)
                    {
                        // Set bitmap size to full screen
                        Bitmap bp = new Bitmap(Convert.ToInt32(SystemParameters.PrimaryScreenWidth), Convert.ToInt32(SystemParameters.PrimaryScreenHeight));

                        // create graphic object from bitmap
                        Graphics gr = Graphics.FromImage(bp);
                        gr.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(bp.Width, bp.Height));
                        try
                        {
                            // Write graphic to video file
                            vf.WriteVideoFrame(bp);
                        }
                        catch(System.AccessViolationException es)
                        {
                            //TODO: Log Error
                        }
                    }
                }
            }
            catch(Exception e)
            {
                //TODO: Log Error
            }
        }
    }
}
