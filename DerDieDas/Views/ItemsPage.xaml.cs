using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DerDieDas.Models;
using DerDieDas.Views;
using DerDieDas.ViewModels;
using System.Threading;
using Xamarin.Forms.Internals;
using Xamarin.Essentials;
using Newtonsoft.Json;

namespace DerDieDas.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class ItemsPage : ContentPage
    {
        DeutschWort CurrentWort = new DeutschWort();
        String ButtonClicked = string.Empty;
        List<int> NumbersKnown = new List<int>();

        public ItemsPage()
        {
            InitializeComponent();
        }

        protected void LoadWords()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
                    {
                Util.Worten = new List<DeutschWort>();
                string contents;
                using (var wc = new System.Net.WebClient())
                {
                    contents = wc.DownloadString("https://derdiedasbucket.s3-sa-east-1.amazonaws.com/db_worten.txt");
                    var deutschWorten = JsonConvert.DeserializeObject<List<DeutschWort>>(contents);
                    deutschWorten.ForEach(deutschWort =>
                    {
                        Util.Worten.Add(deutschWort);
                    });
                }
            }
            else
                DisplayAlert("Hallo", "Kein Internet.", "OK");


        }

        protected void Initialize()
        {
            LoadWords();
            GetNextWord();
        }

        protected void GetNextWord()
        {
            if (Util.Worten.Count == 0)
            {
                DisplayAlert("Hallo", "Deine Wörterbuch ist leer. Vielleicht dein Handy ist nicht verbindung.", "OK");
            }
            else
            {
                if (NumbersKnown.Count == Util.Worten.Count)
                    NumbersKnown = new List<int>();

                var index = new Random().Next(0, Util.Worten.Count);
                while (NumbersKnown.Contains(index))
                {
                    index = new Random().Next(0, Util.Worten.Count);
                }
                NumbersKnown.Add(index);
                CurrentWort = Util.Worten[index];
                lblWort.Text = CurrentWort.Wort;
                lblPlural.Text = CurrentWort.Plural;
                lblUbersetzung.Text = CurrentWort.Ubersetzung;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Initialize();
        }

        void DerDieDas_Clicked(object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            ButtonClicked = button.Text;
            btnDer.IsVisible =
                btnDie.IsVisible = 
                    btnDas.IsVisible =
                        lblFixedPlural.IsVisible =  
                            lblPlural.IsVisible = 
                                lblFixedUbersetzung.IsVisible = 
                                    lblUbersetzung.IsVisible = false;

            frmAntwort.IsVisible = true;
            var artikel = CurrentWort.Artikel;
            frmAntwort.BackgroundColor = artikel == ButtonClicked ? Color.Green : Color.Red;
            lblAntwort.Text = CurrentWort.Artikel;

            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                ButtonClicked = string.Empty;
                btnDer.IsVisible =
               btnDie.IsVisible =
                   btnDas.IsVisible =
                       lblFixedPlural.IsVisible =
                           lblPlural.IsVisible =
                               lblFixedUbersetzung.IsVisible =
                                   lblUbersetzung.IsVisible = true;

                frmAntwort.IsVisible = false;
                GetNextWord();
                return false; 
            });
        }
    }
}