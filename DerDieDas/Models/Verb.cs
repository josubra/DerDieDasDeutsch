using System;
using System.Collections.Generic;

namespace DerDieDas.Models
{
    public class Verb
    {
        public long Id { get; set; }        public string Name { get; set; }        public string Ubersetzung { get; set; }        public string Art { get; set; }        public string Perfekt { get; set; }        public List<Konjugation> Prasens { get; set; }        public List<Konjugation> Prateritum { get; set; }
    }
}
