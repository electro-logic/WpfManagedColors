// Author: Leonardo Tazzini @ https://electro-logic.blogspot.com/
using Mscms;
using Mscms.WPF;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfManagedColors
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();
        void Window_Loaded(object sender, RoutedEventArgs e) => Demo();
        void Demo()
        {
            //BasicExample();
            AdvancedExample();
        }
        void BasicExample()
        {
            // Convert a color
            Color unmanaged = Color.FromRgb(248, 218, 69);
            Color managed = unmanaged.ToManagedColor();
            rectUnmanaged.Fill = new SolidColorBrush(unmanaged);
            rectManaged.Fill = new SolidColorBrush(managed);

            // Convert an image
            BitmapImage unmanagedImage = new BitmapImage(new Uri("pack://application:,,,/d8m.jpg", UriKind.Absolute));
            imageUnmanaged.Source = unmanagedImage;
            imageManaged.Source = unmanagedImage.ToManagedImage();
        }
        void AdvancedExample()
        {
            BitmapSource unmanagedImage = new BitmapImage(new Uri("pack://application:,,,/d8m.jpg", UriKind.Absolute));
            Color unmanagedColor = Color.FromRgb(248, 218, 69);
            // If we need to convert multiple colors we can initialize a Color Transform once and convert multiple times.
            // This performance-oriented approach is also used by the Attached Property.
            // The API is similar to lcmsNET, but based on the Windows Color System (Win32 API)
            using (var inputProfile = Profile.FromSRGBColorSpace())
            using (var outputProfile = WpfProfileExtension.FromVisual())
            using (var transform = Mscms.Transform.Create(inputProfile, outputProfile, RenderingIntent.INTENT_PERCEPTUAL, CmsFlags.BEST_MODE | CmsFlags.PRESERVEBLACK))
            {
                // Show used settings
                txtMonitor.Text = _monitor.Device;
                txtInputProfile.Text = Path.GetFileName(transform.InputProfile.Filename);
                txtOutputProfile.Text = Path.GetFileName(transform.OutputProfile.Filename);
                txtRenderingIntent.Text = transform.RenderingIntent.ToString();
                txtFlags.Text = transform.Flags.ToString();

                // Convert a color
                Color managedColor = transform.DoTransform(unmanagedColor);
                rectUnmanaged.Fill = new SolidColorBrush(unmanagedColor);
                rectManaged.Fill = new SolidColorBrush(managedColor);

                // Convert an sRGB image
                BitmapSource managedImage = unmanagedImage.ToManagedImage(transform);
                imageUnmanaged.Source = unmanagedImage;
                imageManaged.Source = managedImage;
            }
        }
        Mscms.Monitor _monitor;
        void Window_LocationChanged(object sender, EventArgs e)
        {
            var newMonitor = WpfMonitorExtension.FromVisual();
            if (newMonitor != _monitor)
            {
                // Window has moved to another monitor, update colors
                _monitor = newMonitor;
                Demo();
            }
        }
    }
}