using System;
using System.Collections.Generic;
using System.IO;
using DerDieDas.Models;
using Xamarin.Essentials;

namespace DerDieDas
{
    public static class Util
    {
        public static List<DeutschWort> Worten { get; set; }
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
}
