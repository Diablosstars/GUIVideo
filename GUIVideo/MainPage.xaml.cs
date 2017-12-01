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
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GUIVideo
{

    public sealed partial class MainPage : Page
    {
        private ObservableCollection<string> _items = new ObservableCollection<string>();
        StorageFolder videosFolder = KnownFolders.VideosLibrary;

        public MainPage()
        {
            this.InitializeComponent();
            BuildPlaylists();

        }
        public ObservableCollection<string> Items
        {
            get { return this._items; }
        }

        private void Open_MediaPlayer(object sender, RoutedEventArgs e)
        {

        }

        private void PopulateVideoList(string currentFolderPath)
        {
            videoList.Items.Clear();

            string[] files = Directory.GetFiles(currentFolderPath);
            foreach (string file in files)
            {

                string fileName = Path.GetFileNameWithoutExtension(file);
                ListViewItem item = new ListViewItem();
                item.Name = fileName;
                item.Tag = file;


                videoList.Items.Add(item);

            }
        }

        //https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-listing-files-and-folders
        private async void BuildPlaylists()
        {
            IReadOnlyList<IStorageItem> itemsList = await videosFolder.GetItemsAsync();

            foreach (var item in itemsList)
            {
                if (item is StorageFolder)
                {
                    playList.Items.Add(item.Name);
                }
            }
        }

        //https://docs.microsoft.com/en-us/uwp/api/windows.storage.storagefolder
        //https://stackoverflow.com/questions/45866872/get-all-files-inside-a-specific-folder-in-a-library-with-uwp
        //Péter Bozsó
        private async void playList_ItemClick(object sender, ItemClickEventArgs e)
        {
            videoList.Items.Clear();
            StorageFolder storageFolder = await KnownFolders.VideosLibrary.GetFolderAsync(e.ClickedItem.ToString());
            StorageFileQueryResult results = storageFolder.CreateFileQuery();

            IReadOnlyList<StorageFile> filesInFolder = await results.GetFilesAsync();
            foreach(StorageFile item in filesInFolder)
            {
                videoList.Items.Add(item.Name);
            }
            

        }
    }
}
