﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GUIVideo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoPlayer : Page
    {
        private StorageFile Selected;

        public VideoPlayer()
        {
            this.InitializeComponent();
        }
        private async void Initialize_Media()
        {
            //https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-reading-and-writing-files
            var stream = await Selected.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mediaPlayer.SetSource(stream, Selected.ContentType);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Selected = (StorageFile)e.Parameter;
            Initialize_Media();
        }
    }
}
