using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
// https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.contentdialog

namespace GUIVideo
{

    public sealed partial class NewPlayListDialog : ContentDialog
    {
        public string result { get; private set; }
        public NewPlayListDialog()
        {
            InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if(playListText.Text == "")
            {
                args.Cancel = true;
                ContentDialog error = new ContentDialog()
                {
                    Title = "Error",
                    Content = "A valid string is required.",
                    PrimaryButtonText = "Ok"
                };
                this.Hide();
                await error.ShowAsync();
            }
            else
            {
                result = playListText.Text;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }
    }
}
