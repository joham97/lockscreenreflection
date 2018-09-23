using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LockScreenReflection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(310, 158);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(310, 158));
        }

        private async void ChangeBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            progressRing.IsActive = true;
            await ChangeBackground();
            progressRing.IsActive = false;
        }

        private async Task ChangeBackground()
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                Uri uri = new Uri("https://source.unsplash.com/random/1920x1080");
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(uri);
                        if (response != null && response.StatusCode == Windows.Web.Http.HttpStatusCode.Ok)
                        {
                            string filename = "background.jpg";
                            var imageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                            using (IRandomAccessStream stream = await imageFile.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                await response.Content.WriteToStreamAsync(stream);
                            }
                            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                            UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                            if (!await settings.TrySetWallpaperImageAsync(file))
                            {
                                System.Diagnostics.Debug.WriteLine("Failed");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Success");
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
        
        private async void LockScreenBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            lockScreenBackgroundProgressRing.IsActive = true;
            await LockScreenBackground();
            lockScreenBackgroundProgressRing.IsActive = false;
        }

        private async Task LockScreenBackground()
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {   
                string filename = "background.jpg";

                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                if (!await settings.TrySetWallpaperImageAsync(file))
                {
                    System.Diagnostics.Debug.WriteLine("Failed");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Success");
                }
            }
        }
    }
}
