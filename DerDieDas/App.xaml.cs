﻿using DerDieDas.Views;
using Xamarin.Forms;

namespace DerDieDas
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            MainPage = new MainTabbedPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
