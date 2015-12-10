using Facebook;
using Facebook.Graph;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TaComFome
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private StorageFile photo;
        private FBMediaStream fbStream;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);

            //StorageFile result = e.Parameter as StorageFile;
            //photo = result;
            //trataFoto(photo);
            await captureAsync();
        }

        private async Task captureAsync()
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                // User cancelled photo capture
                return;
            }

            IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

            SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap,
        BitmapPixelFormat.Bgra8,
        BitmapAlphaMode.Premultiplied);

            SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
            await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);
            photo_cap.Source = bitmapSource;

            IRandomAccessStreamWithContentType stream2 = await photo.OpenReadAsync();
            fbStream = new FBMediaStream(photo.Name, stream2);
        }

        private async void trataFoto(StorageFile photo)
        {
            IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

            SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap,
        BitmapPixelFormat.Bgra8,
        BitmapAlphaMode.Premultiplied);

            SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
            await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);
            photo_cap.Source = bitmapSource;
        }

        private async void photo_btn_Click(object sender, RoutedEventArgs e)
        {
            /*IRandomAccessStreamWithContentType stream = await photo.OpenReadAsync();
            FBMediaStream fbStream = new FBMediaStream(photo.Name, stream);

            FBSession sess = FBSession.ActiveSession;
            if (sess.LoggedIn)
            {
                FBUser user = sess.User;

                PropertySet parameters = new PropertySet();
                parameters.Add("source", fbStream);

                string path = "/" + user.Id + "/photos";

                FBSingleValue sval = new FBSingleValue(path, parameters,
                               new FBJsonClassFactory(FBPhoto.FromJson));

                FBResult result = await sval.PostAsync();

                if (result.Succeeded)
                {
                    // Uploading succeeded
                }
                else
                {
                    // Uploading failed
                }
            }*/

            // Get active session
            FBSession sess = FBSession.ActiveSession;

            if (sess.LoggedIn)
            {
                // Set parameters
                PropertySet parameters = new PropertySet();
                parameters.Add("source", fbStream);
                parameters.Add("description", "BLABLABLA");

                // Display feed dialog
                FBResult fbresult = await sess.ShowFeedDialogAsync(parameters);

                if (fbresult.Succeeded)
                {
                    // Requests sent
                }
                else
                {
                    // Sending requests failed
                }
            }
        }
    }
}
