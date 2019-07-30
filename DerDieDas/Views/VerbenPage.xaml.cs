using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using DerDieDas.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace DerDieDas.Views
{
    public partial class VerbenPage : ContentPage
    {
        Verb CurrentVerb = new Verb();
        List<int> NumbersKnown = new List<int>();
        const string _maxRichtigVerben = "maxRichtigVerben";

        public VerbenPage()
        {
            InitializeComponent();
            lblOptionOne.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => CheckAndGo(cbOptionOne)),
            });
            lblOptionTwo.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => CheckAndGo(cbOptionTwo)),
            });
            lblOptionThree.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => CheckAndGo(cbOptionThree)),
            });
        }

        protected void CheckAndGo(CheckBox check)
        {
            check.IsChecked = true;
            Optionen_Clicked(check, new EventArgs());
        }

        public void Initialize()
        {
            LadenVerben();
            var maxRichtig = Util.ManageCache("get", _maxRichtigVerben);
            lblMaxRichtig.Text = string.IsNullOrWhiteSpace(maxRichtig) ? "0" : maxRichtig;
            GetNextVerb();

        }

        public void LadenVerben()
        {
            var url = "https://derdiedasbucket.s3-sa-east-1.amazonaws.com/db_verben.js";
            try
            {
                Util.Verben = new List<Verb>();
                if (Util.IsInternetAvailable())
                {
                    
                    string contents;
                    using (var wc = new System.Net.WebClient())
                    {
                        contents = wc.DownloadString(url);
                        var deutschWorten = JsonConvert.DeserializeObject<List<Verb>>(contents);
                        deutschWorten.ForEach(verb =>
                        {
                            Util.Verben.Add(verb);
                        });
                        Util.ManageCache("save", url, contents, 7);
                    }
                }
                else
                {
                    ReloadFromCache(url);
                }
            }
            catch (Exception ex)
            {
                ReloadFromCache(url);
            }
                
        }

        protected void ReloadFromCache(string url)
        {
            var json = Util.ManageCache("get", url, null, 7);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var deutschWorten = JsonConvert.DeserializeObject<List<Verb>>(json);
                deutschWorten.ForEach(deutschWort =>
                {
                    Util.Verben.Add(deutschWort);
                });
            }
        }

        protected void GetNextVerb(Verb verb = null)
        {
            if (Util.Verben == null || Util.Verben.Count == 0)
            {
                Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                {
                    DisplayAlert("Hallo", "Deine Verben Wörterbuch ist leer. Vielleicht dein Handy ist nicht verbindung.", "OK");
                    return false;
                });
            }
            else
            {
                if (NumbersKnown.Count == Util.Verben.Count)
                    NumbersKnown = new List<int>();

                var index = 0;

                if (verb == null)
                {
                    index = new Random().Next(0, Util.Verben.Count);

                    while (NumbersKnown.Contains(index))
                    {
                        index = new Random().Next(0, Util.Verben.Count);
                    }

                    NumbersKnown.Add(index);
                    CurrentVerb = Util.Verben[index];
                }
                else
                {
                    CurrentVerb = verb;
                    index = Util.Verben.IndexOf(verb);
                }
                
                lblWort.Text = CurrentVerb.Name;
                lblArt.Text = CurrentVerb.Art;
                lblPerfekt.Text = CurrentVerb.Perfekt;

                grdKonjugation.Children.Clear();
                var lblIndikativPrasens= new Label
                {
                    Text = "Indikativ Präsens",
                    FontSize = 18,
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Color.FromHex("#003994"),
                    FontAttributes = FontAttributes.Bold
                };

                grdKonjugation.Children.Add(lblIndikativPrasens, 0, 0);
                Grid.SetColumnSpan(lblIndikativPrasens, 6);

                var lblIndikativPrateritum = new Label
                {
                    Text = "Indikativ Präteritum",
                    FontSize = 18,
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Color.FromHex("#003994"),
                    FontAttributes = FontAttributes.Bold
                };

                
                OrganizierenKonjugation(CurrentVerb.Prasens, true);

                grdKonjugation.Children.Add(lblIndikativPrateritum, 0, 7);
                Grid.SetColumnSpan(lblIndikativPrateritum, 6);

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
                lblOptionOne.Text = Util.Verben[indexs[indexOptionOne]].Ubersetzung;

                var indexOptionTwo = new Random().Next(0, indexs.Count);
                while (indexOptionTwo == indexOptionOne)
                {
                    indexOptionTwo = new Random().Next(0, indexs.Count);
                }
                lblOptionTwo.Text = Util.Verben[indexs[indexOptionTwo]].Ubersetzung;

                var indexOptionThree = new Random().Next(0, indexs.Count);
                while (indexOptionThree == indexOptionOne || indexOptionThree == indexOptionTwo)
                {
                    indexOptionThree = new Random().Next(0, indexs.Count);
                }
                lblOptionThree.Text = Util.Verben[indexs[indexOptionThree]].Ubersetzung;
            }
        }

        protected void OrganizierenKonjugation(List<Konjugation> konjutaions, bool istPrasens)
        {
            for (int i = 0; i < konjutaions.Count; i++)
            {
                var verb = konjutaions[i];
                var lblPronomen = new Label
                {
                    Text = verb.Pronomen,
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

                grdKonjugation.Children.Add(lblPronomen, 0 , istPrasens ?  i + 1 : 7 + (i == 0 ? 1 : i + 1));
                Grid.SetColumnSpan(lblPronomen, 3);

                grdKonjugation.Children.Add(lblVerb, 3 , istPrasens ? i + 1 : 7 + (i == 0 ? 1 : i + 1));
                Grid.SetColumnSpan(lblVerb, 3);
            }
        }

        void Optionen_Clicked(object sender, System.EventArgs e)
        {
            if (cbOptionOne.IsChecked || cbOptionTwo.IsChecked || cbOptionThree.IsChecked)
            {
                cbOptionOne.IsEnabled =
                    cbOptionTwo.IsEnabled =
                        cbOptionThree.IsEnabled = false;

                lblOptionOne.TextColor = CurrentVerb.Ubersetzung == lblOptionOne.Text ? Color.Green : Color.Red;
                lblOptionTwo.TextColor = CurrentVerb.Ubersetzung == lblOptionTwo.Text ? Color.Green : Color.Red;
                lblOptionThree.TextColor = CurrentVerb.Ubersetzung == lblOptionThree.Text ? Color.Green : Color.Red;
                var cbClicked = (CheckBox)sender;


                if (this.FindByName<Label>(cbClicked.ClassId).Text == CurrentVerb.Ubersetzung)
                {
                    lblRichtig.Text = (Convert.ToInt32(lblRichtig.Text) + 1).ToString();
                    var countRichtig = Convert.ToInt32(lblRichtig.Text);
                    var countMaxRichtig = Convert.ToInt32(lblMaxRichtig.Text);
                    if (countRichtig > countMaxRichtig)
                    {
                        lblMaxRichtig.Text = (countMaxRichtig + 1).ToString();
                        Util.ManageCache("save", _maxRichtigVerben, lblMaxRichtig.Text);
                    }
                }
                else
                    lblFalsch.Text = (Convert.ToInt32(lblFalsch.Text) + 1).ToString();

                Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                {
                    GetNextVerb();
                    cbOptionOne.IsChecked =
                        cbOptionTwo.IsChecked =
                            cbOptionThree.IsChecked = false;
                    cbOptionOne.IsEnabled =
                        cbOptionTwo.IsEnabled =
                            cbOptionThree.IsEnabled = true;
                    lblOptionOne.TextColor =
                        lblOptionTwo.TextColor =
                            lblOptionThree.TextColor = Color.Gray;
                    return false;
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Initialize();
        }

        private void sbVerben_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = sbVerben.Text;
            if (text.Length > 3)
            {
                slVerben.IsVisible = true;
                slQuizz.IsVisible = false;
                lvVerben.ItemsSource = Util.Verben.Where(x => x.Name.ToLower().Contains(text.ToLower()));
            }
            else
            {
                slVerben.IsVisible = false;
                slQuizz.IsVisible = true;
            }
        }

        void Horen_Clicked(object sender, System.EventArgs e)
        {
            if (Util.IsInternetAvailable())
                Util.ReadText(CurrentVerb.Name);
            else
                DisplayAlert("Hallo", "Kein Internet.", "OK");

        }

        public void lvVerben_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var verb = e.SelectedItem as Verb;
            sbVerben.Text = string.Empty;
            GetNextVerb(verb);
        }
    }
}
