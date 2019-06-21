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

namespace DerDieDas.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class ItemsPage : ContentPage
    {
        List<string> Worten;
        String CurrentWort = string.Empty;
        String ButtonClicked = string.Empty;
        List<int> NumbersKnown = new List<int>();

        public ItemsPage()
        {
            InitializeComponent();
        }

        protected void LoadWords()
        {
            Worten = new List<string>();
            string contents;
            using (var wc = new System.Net.WebClient())
            {
                contents = wc.DownloadString("https://derdiedasbucket.s3-sa-east-1.amazonaws.com/worten.txt");
                var worten = contents.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                worten.ForEach(wort =>
                {
                    if (wort != string.Empty)
                        Worten.Add(wort);
                });
            }

        }

        protected void Initialize()
        {
            LoadWords();
            lblWort.Text = GetNextWord();
        }

        protected string GetNextWord()
        {
            if (NumbersKnown.Count == Worten.Count)
                NumbersKnown = new List<int>();

            var index = new Random().Next(0, Worten.Count);
            while (NumbersKnown.Contains(index))
            {
                index = new Random().Next(0, Worten.Count);
            }
            NumbersKnown.Add(index);
            CurrentWort = Worten[index];
            return CurrentWort.Split(' ')[1];
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
                    btnDas.IsVisible = false;

            frmAntwort.IsVisible = true;
            var artikel = CurrentWort.Split(' ')[0];
            frmAntwort.BackgroundColor = artikel == ButtonClicked ? Color.Green : Color.Red;
            lblAntwort.Text = CurrentWort.Split(' ')[0];

            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                ButtonClicked = string.Empty;
                btnDer.IsVisible =
                    btnDie.IsVisible =
                        btnDas.IsVisible = true;

                frmAntwort.IsVisible = false;
                lblWort.Text = GetNextWord();
                return false; 
            });
        }
    }
}