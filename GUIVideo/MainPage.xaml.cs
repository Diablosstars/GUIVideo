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
        private StorageFolder currentFolder = KnownFolders.VideosLibrary;

        public MainPage()
        {
            this.InitializeComponent();
            BuildPlaylists();
            this.playButton.IsEnabled = false;
        }

        #region HamburgerMenu
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }
        private async void MenuButton2_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog aboutDialog = new AboutDialog();
            await aboutDialog.ShowAsync();
        }

        #endregion


        #region Play Media

        private async void Start_Media()
        {
            StorageFolder storageFolder = await currentFolder.GetFolderAsync(playList.SelectedItem.ToString());
            var results = await storageFolder.TryGetItemAsync(videoList.SelectedItem.ToString() + ".mp4");

            StorageFile file = (StorageFile)results;
            this.Frame.Navigate(typeof(VideoPlayer), file);
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

        #endregion

        #region Build Lists

        //https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-listing-files-and-folders
        private async void BuildPlaylists()
        {
            playList.Items.Clear();
            IReadOnlyList<IStorageItem> itemsList = await videosFolder.GetItemsAsync();

            foreach (var item in itemsList)
                if (item is StorageFolder)
                    playList.Items.Add(item.Name);
        }

        //https://docs.microsoft.com/en-us/uwp/api/windows.storage.storagefolder
        //https://stackoverflow.com/questions/45866872/get-all-files-inside-a-specific-folder-in-a-library-with-uwp
        //Péter Bozsó
        private async void BuildVideoList()
        {
            videoList.Items.Clear();
            this.playButton.IsEnabled = false;
            StorageFolder storageFolder = await currentFolder.GetFolderAsync(playList.SelectedItem.ToString());
            StorageFileQueryResult results = storageFolder.CreateFileQuery();

            IReadOnlyList<StorageFile> filesInFolder = await results.GetFilesAsync();
            foreach (StorageFile item in filesInFolder)
            {
                string name = item.Name;
                name = name.Substring(0, name.Length - 4);
                videoList.Items.Add(name);
            }
        }
        private async void playList_ItemClick(object sender, ItemClickEventArgs e)
        {
            videoList.Items.Clear();
            this.playButton.IsEnabled = false;
            try
            {
                StorageFolder storageFolder = await currentFolder.GetFolderAsync(e.ClickedItem.ToString());
                StorageFileQueryResult results = storageFolder.CreateFileQuery();

                IReadOnlyList<StorageFile> filesInFolder = await results.GetFilesAsync();
                foreach (StorageFile item in filesInFolder)
                {
                    string name = item.Name;
                    name = name.Substring(0, name.Length - 4);
                    videoList.Items.Add(name);
                }
            }
            catch (Exception exception)
            {
                playList.SelectedIndex = 0;

            }
        }
#endregion

        #region PlayList Menu

        //https://stackoverflow.com/questions/36741757/uwp-listview-item-context-menu 
        //     Grace Feng
        private void playList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            if(playList.SelectedItem == null)
            {
                Delete_PlayList.IsEnabled = false;
                Rename_Playlist.IsEnabled = false;
            }
            else
            {
                Delete_PlayList.IsEnabled = true;
                Rename_Playlist.IsEnabled = true;
            }
            playListMenuFlyout.ShowAt(listView, e.GetPosition(listView));
        }

        //https://docs.microsoft.com/en-us/uwp/api/windows.storage.storagefolder
        private async void New_PlayList_Click(object sender, RoutedEventArgs e)
        {
            NewPlayListDialog newPlayList = new NewPlayListDialog();
            await newPlayList.ShowAsync();

            if (newPlayList.result == null) { }
            else
            {
                StorageFolder newFolder = await currentFolder.CreateFolderAsync(newPlayList.result, CreationCollisionOption.FailIfExists);
                BuildPlaylists();
            }
        }

        private async void Delete_PlayList_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteDialog = new ContentDialog()
            {
                Title = "Delete list permanently?",
                Content = "If you delete this list, all contents are deleted with it.",
                SecondaryButtonText = "Cancel",
                PrimaryButtonText = "Delete"
            };
            ContentDialogResult result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                StorageFolder folder = await currentFolder.GetFolderAsync(playList.SelectedItem.ToString());
                await folder.DeleteAsync();
                deleteDialog.Hide();
                BuildPlaylists();
            }
            else
            {
                deleteDialog.Hide();
            }
        }

        private async void Rename_Playlist_Click(object sender, RoutedEventArgs e)
        {
            NewPlayListDialog newPlayList = new NewPlayListDialog();
            await newPlayList.ShowAsync();

            if (newPlayList.result == null) { }
            else
            {
                StorageFolder folder = await currentFolder.GetFolderAsync(playList.SelectedItem.ToString());
                await folder.RenameAsync(newPlayList.result);
                BuildPlaylists();
            }
        }

        private async void playList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            StorageFolder selectedFolder = await currentFolder.GetFolderAsync(playList.SelectedItem.ToString());
            playList.Items.Clear();
            currentFolder = selectedFolder;
            IReadOnlyList<IStorageItem> itemsList = await selectedFolder.GetItemsAsync();

            foreach (var item in itemsList)
                if (item is StorageFolder)
                    playList.Items.Add(item.Name);
        }
        #endregion

        #region VideoList Menu



        private async void Delete_Video_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteDialog = new ContentDialog()
            {
                Title = "Delete video permanently?",
                SecondaryButtonText = "Cancel",
                PrimaryButtonText = "Delete"
            };
            ContentDialogResult result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                StorageFolder playlist = await videosFolder.GetFolderAsync(playList.SelectedItem.ToString());
                StorageFile video = await playlist.GetFileAsync(videoList.SelectedItem.ToString() + ".mp4");
                await video.DeleteAsync();
                deleteDialog.Hide();

                BuildVideoList();
                
            }
            else
            {
                deleteDialog.Hide();
            }
        }

        private void videoList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            if (videoList.SelectedItem == null)
            {
                delete_Video.IsEnabled = false;
                rename_Video.IsEnabled = false;
            }
            else
            {
                delete_Video.IsEnabled = true;
                rename_Video.IsEnabled = true;
            }
            videoListMenuFlyout.ShowAt(listView, e.GetPosition(listView));
        }

        private async void rename_Video_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder playlist = await videosFolder.GetFolderAsync(playList.SelectedItem.ToString());
            StorageFile video = await playlist.GetFileAsync(videoList.SelectedItem.ToString() + ".mp4");

            NewPlayListDialog rename = new NewPlayListDialog();
            await rename.ShowAsync();

            if (rename.result == null) { }
            else
            {
                await video.RenameAsync(rename.result + ".mp4");
            }

    BuildVideoList();
        }
    }
}


#endregion
