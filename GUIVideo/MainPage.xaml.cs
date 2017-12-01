using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace GUIVideo
{

    public sealed partial class MainPage : Page
    {
        StorageFolder videosFolder = KnownFolders.VideosLibrary;
        //VideoPlayer videoPlayer = new VideoPlayer();

        public MainPage()
        {
            this.InitializeComponent();
            BuildPlaylists();
            this.playButton.IsEnabled = false;

        }

        private async void Start_Media()
        {
            StorageFolder storageFolder = await KnownFolders.VideosLibrary.GetFolderAsync(playList.SelectedItem.ToString());
            var results = await storageFolder.TryGetItemAsync(videoList.SelectedItem.ToString() + ".mp4");

            StorageFile file = (StorageFile)results;
            this.Frame.Navigate(typeof(VideoPlayer), file);
        }


        //https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-listing-files-and-folders
        private async void BuildPlaylists()
        {
            IReadOnlyList<IStorageItem> itemsList = await videosFolder.GetItemsAsync();

            foreach (var item in itemsList)
                if (item is StorageFolder)
                    playList.Items.Add(item.Name);
        }

        //https://docs.microsoft.com/en-us/uwp/api/windows.storage.storagefolder
        //https://stackoverflow.com/questions/45866872/get-all-files-inside-a-specific-folder-in-a-library-with-uwp
        //Péter Bozsó
        private async void playList_ItemClick(object sender, ItemClickEventArgs e)
        {
            videoList.Items.Clear();
            this.playButton.IsEnabled = false;
            StorageFolder storageFolder = await KnownFolders.VideosLibrary.GetFolderAsync(e.ClickedItem.ToString());
            StorageFileQueryResult results = storageFolder.CreateFileQuery();

            IReadOnlyList<StorageFile> filesInFolder = await results.GetFilesAsync();
            foreach(StorageFile item in filesInFolder)
            {
                string name = item.Name;
                name = name.Substring(0, name.Length - 4);
                videoList.Items.Add(name);
            }
        }

        private void videoList_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.playButton.IsEnabled = true;
        }

        private void videoList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Start_Media();
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            Start_Media();
        }
    }
}
