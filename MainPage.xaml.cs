using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Transfer;
using Facebook;
using Facebook.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
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
        private string amazonPath;
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

        private async Task amazon(string path)
        {
            AWSConfigs.AWSRegion = "us-east-1";

            CognitoAWSCredentials credentials = new CognitoAWSCredentials(
    "us-east-1:594898f7-90b3-41f4-a9c8-3d62fbe8bb06", // Identity Pool ID
    RegionEndpoint.USEast1 // Region
);

            var s3Client = new AmazonS3Client(credentials, RegionEndpoint.USEast1);
            var transferUtility = new TransferUtility(s3Client);

            /*var loggingConfig = AWSConfigs.LoggingConfig;
            loggingConfig.LogMetrics = true;
            loggingConfig.LogResponses = ResponseLoggingOption.Always;
            loggingConfig.LogMetricsFormat = LogMetricsFormatOption.JSON;
            loggingConfig.LogTo = LoggingOptions.SystemDiagnostics;*/

            await transferUtility.UploadAsync(path, "tacomfome");
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

            await trataFoto(photo);
            
            string path = photo.Path;
            string[] brokenPath = path.Split('\\');
            
            amazonPath = "https://s3.amazonaws.com/tacomfome/"+brokenPath.Last();
            amazonPath.Replace(' ', '+');

            await amazon(path);
        }

        private async Task trataFoto(StorageFile photo)
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
            
            FBSession sess = FBSession.ActiveSession;

            if (sess.LoggedIn)
            {
                // Set parameters
                PropertySet parameters = new PropertySet();
                parameters.Add("title", "Ta com fome?");
                parameters.Add("picture", amazonPath);
                parameters.Add("description", "O restaurante BLABLA acaba de doar R$ 0,25 para intituições de combate a fome por este compartilhamento. Compartilhe você também e ajude quem precisa.");

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
