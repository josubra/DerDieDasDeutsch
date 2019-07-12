using System;
using System.Collections.Generic;
using DerDieDas.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace DerDieDas.Views
{
    public partial class VerbenPage : ContentPage
    {
        Verb CurrentVerb = new Verb();
        String ButtonClicked = string.Empty;
        List<int> NumbersKnown = new List<int>();

        public VerbenPage()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            LadenVerben();
            GetNextVerb();

        }

        public void LadenVerben()
        {
            if(Util.IsInternetAvailable())
            {
                Util.Verben = new List<Verb>();
                string contents;
                using (var wc = new System.Net.WebClient())
                {
                    contents = wc.DownloadString("https://derdiedasbucket.s3-sa-east-1.amazonaws.com/db_verben.js");
                    var deutschWorten = JsonConvert.DeserializeObject<List<Verb>>(contents);
                    deutschWorten.ForEach(verb =>
                    {
                        Util.Verben.Add(verb);
                    });
                }
            }
            else
                DisplayAlert("Hallo", "Kein Internet.", "OK");
        }

        protected void GetNextVerb()
        {
            if (Util.Verben == null || Util.Verben.Count == 0)
            {
                Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                {
                    DisplayAlert("Hallo", "Deine Wörterbuch ist leer. Vielleicht dein Handy ist nicht verbindung.", "OK");
                    return false;
                });
            }
            else
            {
                if (NumbersKnown.Count == Util.Verben.Count)
                    NumbersKnown = new List<int>();

                var index = new Random().Next(0, Util.Verben.Count);

                while (NumbersKnown.Contains(index))
                {
                    index = new Random().Next(0, Util.Verben.Count);
                }

                NumbersKnown.Add(index);
                CurrentVerb = Util.Verben[index];
                lblWort.Text = CurrentVerb.Name;
                lblArt.Text = CurrentVerb.Art;
                lblPerfekt.Text = CurrentVerb.Perfekt;

                grdKonjugation.Children.Clear();
                var lblIndikativPrasens= new Label
                {
                    Text = "Indikativ Präsens",
                    FontSize = 18,
                    HorizontalTextAlignment = TextAlignment.Start,
                    TextColor = Color.Red
                };

                grdKonjugation.Children.Add(lblIndikativPrasens, 0, 0);
                Grid.SetColumnSpan(lblIndikativPrasens, 5);

                var lblIndikativPrateritum = new Label
                {
                    Text = "Indikativ Präteritum",
                    FontSize = 18,
                    HorizontalTextAlignment = TextAlignment.End,
                    TextColor = Color.Red
                };

                grdKonjugation.Children.Add(lblIndikativPrateritum, 4, 0);
                Grid.SetColumnSpan(lblIndikativPrateritum, 4);

                OrganizierenKonjugation(CurrentVerb.Prasens, true);

                OrganizierenKonjugation(CurrentVerb.Prateritum, false);

                var index2 = new Random().Next(0, Util.Verben.Count);
                var index3 = new Random().Next(0, Util.Verben.Count);
                while(index2 == index)
                {
                    index2 = new Random().Next(0, Util.Verben.Count);
                }
                while (index3 == index || index3 == index2)
                {
                    index3 = new Random().Next(0, Util.Verben.Count);
                }

                var indexs = new List<int>() { index, index2, index3 };

                var indexOptionOne = new Random().Next(0, indexs.Count);
                btnOptionOne.Text = Util.Verben[indexs[indexOptionOne]].Ubersetzung;

                var indexOptionTwo = new Random().Next(0, indexs.Count);
                while (indexOptionTwo == indexOptionOne)
                {
                    indexOptionTwo = new Random().Next(0, indexs.Count);
                }
                btnOptionTwo.Text = Util.Verben[indexs[indexOptionTwo]].Ubersetzung;

                var indexOptionThree = new Random().Next(0, indexs.Count);
                while (indexOptionThree == indexOptionOne || indexOptionThree == indexOptionTwo)
                {
                    indexOptionThree = new Random().Next(0, indexs.Count);
                }
                btnOptionThree.Text = Util.Verben[indexs[indexOptionThree]].Ubersetzung;
            }
        }

        protected void OrganizierenKonjugation(List<Konjugation> konjutaions, bool istPrasens)
        {
            for (int i = 0; i < konjutaions.Count; i++)
            {
                var verb = konjutaions[i];
                var lblPronomen = new Label
                {
                    Text = verb.Pronomen == "er/sie/es" ? "e.s.e" : verb.Pronomen,
                    FontSize = 18,
                    HorizontalTextAlignment = TextAlignment.End,
                };

                var lblVerb = new Label
                {
                    Text = verb.Verbe,
                    FontSize = 18,
                    HorizontalTextAlignment = TextAlignment.Start,
                    FontAttributes = FontAttributes.Bold
                };

                grdKonjugation.Children.Add(lblPronomen, istPrasens ? 0 : 5, i + 1);
                Grid.SetColumnSpan(lblPronomen, 1);

                grdKonjugation.Children.Add(lblVerb, istPrasens ? 1 : 6, i + 1);
                Grid.SetColumnSpan(lblVerb, 3);
            }
        }

        void Optionen_Clicked(object sender, System.EventArgs e)
        {
            GetNextVerb();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Initialize();
        }
    }
}
