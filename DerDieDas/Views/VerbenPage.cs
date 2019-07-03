using System;

using Xamarin.Forms;

namespace DerDieDas.Views
{
    public class VerbenPage : ContentPage
    {
        public VerbenPage()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

