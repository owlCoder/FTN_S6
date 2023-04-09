using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public enum EPravaPristupa { }
    class DirektorijumKorisnika
    {
        private const string _pepper = "P&0myWHq";
        private Dictionary<string, Korisnik> korisnici
            = new Dictionary<string, Korisnik>();

        public DirektorijumKorisnika()
        {

        }

        public void DodajKorisnka(string korisnickoIme, string lozinka)
        {

        }

        private string KodiraTekst(string lozinka)
        {
            using (var sha = SHA256.Create())
            {
                var computedHash = sha.ComputeHash(
                Encoding.Unicode.GetBytes(lozinka + _pepper));
                return Convert.ToBase64String(computedHash);
            }
        }

        public string AutentifikacijaKorisnika(string korisnickoIme, string lozinka)
        {
            return null;
        }

        public void KorisnikAutentifikovan(string token)
        {

        }



        public bool KorisnikAutentifikovanIAutorizovan(string token, EPravaPristupa pravo)
        {
            return false;
        }

        public bool DodajPravaKorisniku(string korisnickoIme, EPravaPristupa pravoPristupa)
        {
            if (korisnici.TryGetValue(korisnickoIme, out Korisnik korisnik))
            {
                return korisnik.DodajPravoPristupa(pravoPristupa);
            }
            return false;
        }

        public bool ProveroPravoKorisnika(string korisnickoIme, EPravaPristupa pravoPristupa)
        {
            if (korisnici.TryGetValue(korisnickoIme, out Korisnik korisnik))
            {
                return korisnik.ProveriPravoPristupa(pravoPristupa);
            }
            return false;
        }
    }
}
