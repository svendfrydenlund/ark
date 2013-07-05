using System;


/* 
 * Denne fil definerer entiteter for websitets database. 
 * For hvert felt er angivet hvilket system, der er dataejer 
 * for feltet. Det vil sige i hvilket system at oplysninger skal opdateres i for at slå igennem.
 * Data fra Conventus synkroniseres periodisk til databasen.
 * 
 * Nedenstående definitioner er skrevet, så de umiddelbart kan kobles op til entity framework. Referencer til entity framework som sådan holdes uden
 * for klasserne for at undgå koblinger mellem database og frontend.
 * 
 * TODO : Evt. en entitet til at huse de ændringer som medlemmet selv indtaster (og som vedligeholdes i Conventus)
 * Entiteten kan bruges til at 
 */

namespace dk.arok.EntityModel
{

    public enum KoenType {
        Kvinde,
        Mand,
        Ukendt
    }
        

    public class Medlem
    {
        /// <summary>
        /// Internt unikt medlemsid anvendt i websitets lokale database. Historisk set er medlemsid
        /// fra økonomisystemet blevet ændret "jævnligt" - derfor undgås en direkte kobling.
        /// Dataejer: Database
        /// </summary>
        public int MedlemId { get; set; }

        /// <summary>
        /// Eksternt medlemsid er det medlemmet kender fra Conventus og det, der anvendes i kommunikationen med medlemmet.
        /// </summary>
        public int EksterntMedlemId {get; set;}

        /// <summary>
        /// Conventus selner ikke mellem for- og efternavn. Derfor bibeholdes fuldt navn her.
        /// Dataejer: Conventus
        /// </summary>
        public string Navn { get; set; }

        /// <summary>
        /// I Conventus kan køn være mand, kvinde eller ukendt.
        /// Dataejer: Conventus
        /// </summary>
        public KoenType Koen { get; set; }

        /// <summary>
        /// Fødselsdato er for mange medlemmer ikke sat i Conventus. Derfor er feltet sat til Nullable.
        /// Dataejer: Conventus
        /// </summary>
        public DateTime? Foedselsdato { get; set; }

        // TODO - tilføj felter omkring adresse m.v. fra Conventus og eventuelt flere som kun vedligeholdes uden for databasen, billede kunne også være sjovt.

        /// System attributter
        public string ChangedBy { get; set; }
        public DateTime LatestChange { get; set; }
    }

    
    public enum MedlemAendringStatusTyper {
        Oprettet,
        Gennemført,
        Annulleret
    }

    /// <summary>
    /// Klassen bruges til at understøtte scenarier, hvor medlemmet giver os mere opdaterede oplysninger end hvad der ligger i
    /// conventus - fx. på en "Min Profil" side. Vi kan så vælge at vise disse for medlemmet fremfor de data der kommer fra Conventus.
    /// Listen kan endvidere bruges til at tilbyde medlemsadministratorer en mail samt en samlet liste over de opdateringer de mangler at foretage.
    /// Måske vi kan få en sådan liste "gratis" via Composite ved at mappe denne datatype til en Composite datatype?
    /// </summary>
    public class MedlemAendring {
       
        public int MedlemAendringId {get; set;}

        // En ændringsentitet indeholder alle de felter, hvor Conventus er dataejer.
        public string Email {get; set;}
        public KoenType Koen {get; set;}
        public DateTime FoedselsDato {get; set;}

        // etc...

        /// <summary>
        /// Opdateres af medlemsadministrator efterhånden som oplysningerne bliver indsat i Conventus.
        /// </summary>
        public MedlemAendringStatusTyper Status {get; set;}

        /// <summary>
        /// Angiver hvornår denne ændringsanmodning er oprettet
        /// </summary>
        public DateTime Oprettet {get; set;}

        /// <summary>
        /// Angiver hvornår ændringen er gennemført.
        /// </summary>
        public DateTime BehandletDen {get; set;}

        /// <summary>
        /// Angiver hvem der har gennemført ændringen. Typisk af medlemsadministrator (her forudsættes det at de er oprettet som medlemmer)
        /// </summary>
        public virtual Medlem BehandletAf {get; set;}
        
    
        /// <summary>
        ///  En ændring er altid knyttet til et medlem
        /// </summary>
        public virtual Medlem Medlem {get; set;}
        
    }

    public enum SSOProviders {
        Google,
        Facebook,
        Yahoo
    }

    
    /// <summary>
    /// Indeholder oplysninger om de single-sign-on providere som medlemmet har gjort sig brug af.
    /// Oplysningerne gemmes af hensyn til tilfælde hvor en indledende validering har fundet sted (fx. i forhold til medlemsid).
    /// Et medlem kan benytte sig af flere SSO-providere.
    /// </summary>
    public class MedlemSSO {

        /// <summary>
        /// internt id på meldemssso. Hvert medlem kan have flere sso-udbydere tilknyttet
        /// </summary>
        public int MedlemSSOID {get; set;}

        /// <summary>
        /// Typen af SSOProvider der behandles i denne entitet
        /// </summary>
        public SSOProviders SSOProvider {get; set;}

        /// <summary>
        /// Unikt token fra SSO-provider
        /// </summary>
        public string SsoProviderToken {get; set;}
        
        /// <summary>
        /// Nogle SSO-providere tilbyder at sende brugerens emailadresse.
        /// </summary>
        public string Email {get; set;}

        /// <summary>
        /// Feltet bruges kun i tilfælde, hvor medlemmer ønsker et specifikt brugernavn / password til sitet)
        /// </summary>
        public string username {get; set;}

        /// <summary>
        /// Se kommentar omkring username. Feltet bør strengt taget gemmes kodet.
        /// </summary>
        public string password;

        /// <summary>
        /// Reference til <code>Medlem</code>
        /// </summary>
        public int MedlemId;

        /// <summary>
        /// Reference til Medlemet for dette MedlemSSO
        /// </summary>
        public virtual Medlem Medlem {get; set;}
    }
    
  
    /* Nedenstående er entiteter som med fordel kan oprettes i datamodellen */
    
    /* Rostatistik */
    public enum BaadTyper {
        Inrigger,
        Kajak_kap,
        Kajak_tur,
        Kajak_polo,
        Sculler
    }

    public enum BaadStatusVaerdier {
        Virker,
        Skadet,
        UdeAfDrift,
        Kasseret
    }

    public enum BaadKategori {
        Grøn,
        Gul,
        Rød
    }


    public class Baad
    {
        public int BaadId { get; set; }
        public string Navn { get; set; }
        public DateTime IndkoebsDato { get; set; }
        public BaadTyper BaadType { get; set; }
        /// <summary>
        /// Antal personer båden er beregnet til, inklusiv styrmand (1,2,3,4,5,8)
        /// </summary>
        public int bemanding { get; set; }

        public BaadStatusVaerdier BaadStatus { get; set; }

        public BaadKategori? BaadKategori { get; set; }

        public int SvaerhedsGrad { get; set; }

        public string Maerke { get; set; }

        public string Beskrivelse { get; set; }

        public string Historik { get; set; }
    }


    // public class Rotur (roturId, udskrevet, indskrevet, båd_id, distance, tid, destination, bemærkning)

    // public class RoturDeltager (roturId, MedlemId, evt. rolle som fx styrmand)

    /* Tilladelseslog */
    // Nedenstående entiteter kan være rare at have til at holde styr på hvem vi har frigivet. Vi får til tider forespørgsler fra andre klubber og ingen kan huske det efter lidt tid.
    
    // public class Frigivelser_og_tilladelser (id, medlemid, frigivelsestype (epp-1, epp2..., vinterroning, kajak, inrigger, sculler, styrmand)
    // public class FrigivelsesType (id, navn, beskrivelse (eksempel (epp1, instruktør Epp-2, vinterkajak, styrmand-kort, styrmand-lang)

    /* Når vi engang for et automatisk indkøbssystem til sauna poletter, køkkenkøb m.v. :-)
     * public class Forbrug (MedlemId, StartTid, SlutTid, Tekst, Beløb, varighed, type) */


}
