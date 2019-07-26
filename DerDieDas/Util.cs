using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using DerDieDas.Models;
using Xamarin.Essentials;

namespace DerDieDas
{
    public static class Util
    {
        public static List<DeutschWort> Worten { get; set; }
        public static List<Verb> Verben { get; set; }
        public static void PlayAudio(Stream audio)
        {
            var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Load(audio);
            player.Play();
        }

        public static bool IsInternetAvailable()
        {
            return Connectivity.NetworkAccess == NetworkAccess.Internet;
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
