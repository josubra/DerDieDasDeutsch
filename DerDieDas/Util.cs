using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using DerDieDas.Models;
using MonkeyCache.FileStore;
using Xamarin.Essentials;

namespace DerDieDas
{
    public static class Util
    {
        public static List<DeutschWort> Worten { get; set; }
        public static List<Verb> Verben { get; set; }
        public const string _barrelApplicationId = "DerDieDasDeutsch";
        public static void PlayAudio(Stream audio)
        {
            var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Load(audio);
            player.Play();
        }

        public static bool IsInternetAvailable()
        {
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Timeout = 6000;
                    var contents = wc.DownloadString("https://google.com.br");
                }
                return true;
            }
            catch
            {
                return false;
            }
            

        }

        public static void ReadText(string text)
        {
            using (AmazonPollyClient pc = new AmazonPollyClient(new BasicAWSCredentials("", ""), Amazon.RegionEndpoint.EUWest1))
            {
                SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest
                {
                    Text = text,
                    OutputFormat = OutputFormat.Mp3,
                    VoiceId = VoiceId.Vicki,
                    LanguageCode = LanguageCode.DeDE
                };

                var sres = pc.SynthesizeSpeechAsync(sreq).Result;

                Util.PlayAudio(sres.AudioStream);
            }
        }

        public static string ManageCache(string action, string id, string contents = null, int? days = null)
        {
            Barrel.ApplicationId = _barrelApplicationId;
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
    }

    public class WebClient : System.Net.WebClient
    {
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest lWebRequest = base.GetWebRequest(uri);
            lWebRequest.Timeout = Timeout;
            ((HttpWebRequest)lWebRequest).ReadWriteTimeout = Timeout;
            return lWebRequest;
        }

        public string GetRequest(string aURL)
        {
            using (var lWebClient = new WebClient())
            {
                lWebClient.Timeout = 600 * 60 * 1000;
                return lWebClient.DownloadString(aURL);
            }
        }
    }
}
