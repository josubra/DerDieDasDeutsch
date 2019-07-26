using System;
using System.Collections.Generic;
using System.ComponentModel;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using DerDieDas.Models;
using MonkeyCache.FileStore;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace DerDieDas.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class WortenPage : ContentPage
    {
        DeutschWort CurrentWort = new DeutschWort();
        String ButtonClicked = string.Empty;
        List<int> NumbersKnown = new List<int>();
        const string _maxRichtigWorter = "maxRichtigWorter";
        public WortenPage()
        {
            InitializeComponent();
        }

        protected void LoadWords()
        {
            var url = "https://derdiedasbucket.s3-sa-east-1.amazonaws.com/db_worten.txt";
            try
            {
                if (Util.IsInternetAvailable())
                {
                    Util.Worten = new List<DeutschWort>();
                    string contents;
                    using (var wc = new WebClient())
                    {
                        wc.Timeout = 6000;
                        contents = wc.DownloadString(url);
                        var deutschWorten = JsonConvert.DeserializeObject<List<DeutschWort>>(contents);
                        deutschWorten.ForEach(deutschWort =>
                        {
                            Util.Worten.Add(deutschWort);
                        });
                        ManageCache("save", url, contents, 7);
                    }
                }
                else
                {
                    DisplayAlert("Hallo", "Kein Internet.", "OK");
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
            var json = ManageCache("get", url, null, 7);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var deutschWorten = JsonConvert.DeserializeObject<List<DeutschWort>>(json);
                deutschWorten.ForEach(deutschWort =>
                {
                    Util.Worten.Add(deutschWort);
                });
            }
        }

        protected string ManageCache(string action, string id, string contents = null, int? days = null)
        {
            Barrel.ApplicationId = "DerDieDasDeutsch";
            string retorno = string.Empty;
            switch (action)
            {
                case "get":
                    if (!Barrel.Current.IsExpired(id))
                    {
                        retorno = Barrel.Current.Get<string>(id);
                    }
                    break;
                case "save":
                    Barrel.Current.Add(id, contents, TimeSpan.FromDays(days ?? 7));
                    break;
                default:
                    break;
            }
            return retorno;
        }

        protected void ReadText(string text)
        {
            AmazonPollyClient pc = new AmazonPollyClient(new BasicAWSCredentials("", ""), Amazon.RegionEndpoint.EUWest1);

            SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest();
            sreq.Text = text;
            sreq.OutputFormat = OutputFormat.Mp3;
            sreq.VoiceId = VoiceId.Vicki;
            sreq.LanguageCode = LanguageCode.DeDE;
            
            var sres = pc.SynthesizeSpeechAsync(sreq).Result;

            Util.PlayAudio(sres.AudioStream);
        }

        protected void Initialize()
        {
            LoadWords();
            var maxRichtig = ManageCache("get", _maxRichtigWorter);
            lblMaxRichtig.Text = string.IsNullOrWhiteSpace(maxRichtig) ? "0" : maxRichtig;
            GetNextWord();
        }

        protected void GetNextWord()
        {
            if (Util.Worten == null || Util.Worten.Count == 0)
            {
                Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                {
                    DisplayAlert("Hallo", "Deine Wörterbuch ist leer. Vielleicht dein Handy ist nicht verbindung.", "OK");
                    return false;
                });
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

            if(artikel == ButtonClicked)
            {
                lblRichtig.Text = (Convert.ToInt32(lblRichtig.Text) + 1).ToString();
                var countRichtig = Convert.ToInt32(lblRichtig.Text);
                var countMaxRichtig = Convert.ToInt32(lblMaxRichtig.Text);
                if(countRichtig > countMaxRichtig)
                {
                    lblMaxRichtig.Text = (countMaxRichtig + 1).ToString();
                    ManageCache("save", _maxRichtigWorter, lblMaxRichtig.Text);
                }
            }
            else
                lblFalsch.Text = (Convert.ToInt32(lblFalsch.Text) + 1).ToString();

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
        void Horen_Clicked(object sender, System.EventArgs e)
        {
            if(Util.IsInternetAvailable())
                ReadText(CurrentWort.Wort);
            else
                DisplayAlert("Hallo", "Kein Internet.", "OK");
            
        }
    }
}