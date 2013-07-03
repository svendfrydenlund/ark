using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dk.arok.data.conventus.Model
{

       
    /// <summary>
    /// Klassen representerer de værdier som returneres af medlemssystemet Conventus omkring et medlem.
    ///  Udover de anvendte felter har Conventus endvidere xml-felter for individuel1.. individuel5, kommunekode, betalingsoplysninger, mangler_bekraftelse,
    /// </summary>
    public class ConventusMedlem
    {
        // TODO 3: Clean up class definition
        public enum KoenTypes {Mand, Kvinde, Ukendt}

        private int _id;
        private int? _altID;
        private String _adresse1;
        private KoenTypes _koen;
        private String _navn;
        private String _adresse2;
        private String _postnummer;
        private String _postnummerBy;
             
        private DateTime? foedselsdato;

        private Boolean offentligAdresse;
        private Boolean offentligTelefon;
        private Boolean offentligMobil;

        /// <summary>
        /// Conventus' Medlemsid
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

       
        /// <summary>
        /// Gammelt medlemsid anvendt i medlemspakken (før 2013).
        /// </summary>
        public int? AltID
        {
            get { return _altID; }
            set { _altID = value; }
        } 

        public KoenTypes Koen
        {
            get { return _koen; }
            set { _koen = value; }
        }


        /// <summary>
        /// Medlemmets fulde navn
        /// </summary>
        public String Navn
        {
            get { return _navn; }
            set { _navn = value; }
        }

        
        public String Adresse1
        {
            get { return _adresse1; }
            set { _adresse1 = value; }
        }



        public String Adresse2
        {
            get { return _adresse2; }
            set { _adresse2 = value; }
        }

       
        public String Postnummer
        {
            get { return _postnummer; }
            set { _postnummer = value; }
        }

        

        public String PostnummerBy
        {
            get { return _postnummerBy; }
            set { _postnummerBy = value; }
        }
        
        private String telefonnummer;

        public String Telefonnummer
        {
            get { return telefonnummer; }
            set { telefonnummer = value; }
        }
        private String mobilnummer;

        public String Mobilnummer
        {
            get { return mobilnummer; }
            set { mobilnummer = value; }
        }
        private String email;

        public String Email
        {
            get { return email; }
            set { email = value; }
        }
        

        public DateTime? Foedselsdato
        {
            get { return foedselsdato; }
            set { foedselsdato = value; }
        }

        private Boolean offfentligNavn;

        public Boolean OfffentligNavn
        {
            get { return offfentligNavn; }
            set { offfentligNavn = value; }
        }
        
        

        public Boolean OffentligAdresse
        {
            get { return offentligAdresse; }
            set { offentligAdresse = value; }
        }
        
        public Boolean OffentligTelefon
        {
            get { return offentligTelefon; }
            set { offentligTelefon = value; }
        }


        public Boolean OffentligMobil
        {
            get { return offentligMobil; }
            set { offentligMobil = value; }
        }
        private Boolean offentligEmail;

        public Boolean OffentligEmail
        {
            get { return offentligEmail; }
            set { offentligEmail = value; }
        }
        private Boolean slettet;

        public Boolean Slettet
        {
            get { return slettet; }
            set { slettet = value; }
        }

        public override string ToString()
        {
            return "Id=" + Id + " AltID=" + AltID + " Navn= " + Navn;
        }

    }
}