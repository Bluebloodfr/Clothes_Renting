using MySql.Data.MySqlClient;
using System.Globalization;

namespace ProjetBDDFleurs
{
    class Program
    {
        const string RootConnection = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
        const string BozoConnection = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=bozo;PASSWORD=bozo;";

        static void Main(string[] args)
        {
            Menu();

            /*
             idées suite :
             * définir la date livraison : null au début ? --> dès que le designer a validé les produits, on cherche quel mag peut assembler la commande 
                                                           --> donne une date de livraison de dateCommande+3 max si un magasin est trouvé
             * faire un affichage console de toutes les commandes non livrées et validées (ya tout les composants dispo dans un mag) 
               --> l'ordre par numCommande croissant permet de donner un j de livraison (cb de livraisons par j ?)
             * pour le client possibilité de voir l'historique des anciennes commandes
             * 
             * XML : clients ayant commandé plusieurs fois durant le dernier mois, requete sql :
             select nomC, prenomC, client.courriel from client join bonCommande on client.courriel=bonCommande.courriel
             where dateCommande>date_sub(curdate(), INTERVAL 1 MONTH) group by client.courriel having count(*)>1;

             * JSON : clients n’ayant pas commandé depuis plus de 6 mois, requete sql :
             select nomC, prenomC, client.courriel from client join bonCommande on client.courriel=bonCommande.courriel
             where dateCommande>date_sub(curdate(), INTERVAL 6 MONTH) group by client.courriel having count(*)=0;

             */
        }

        static void Menu()
        {
            if (Request("select count(*) from client", BozoConnection) == "0") { InsertionTable("clients.txt", "client"); }
            if (Request("select count(*) from magasin", BozoConnection) == "0") { InsertionTable("magasin.txt", "magasin"); }
            if (Request("select count(*) from bonCommande", BozoConnection) == "0") { InsertionTable("bonCommande.txt", "bonCommande"); }
            if (Request("select count(*) from elementCommande", BozoConnection) == "0") { InsertionTable("elementCommande.txt", "elementCommande"); }
            if (Request("select count(*) from elementStock", BozoConnection) == "0") { InsertionTable("elementStock.txt", "elementStock"); }
            string res = "";
            do
            {
                Console.Clear();
                Console.WriteLine("Que faire ?\n" +
                                  "(1) Se connecter\n" +
                                  "(2) Créer un compte client\n");
                do
                {
                    Console.WriteLine("Choisir une action :");
                    res = Console.ReadLine();
                } while (!(res == "1" || res == "2"));
                if (res == "1")
                {
                    string[] login = Connection();
                    if (login[0] != "" && login[1] != "")
                    {
                        Console.WriteLine("Que faire ?\n" +
                                  "(1) Créer un bon de commande\n" +
                                  "(2) Voir les commandes précédentes\n");
                        do
                        {
                            Console.WriteLine("Choisir une action :");
                            res = Console.ReadLine();
                        } while (!(res == "1" || res == "2"));
                        if (res == "1") { BonCommande(login[0]); }
                        else { HistClient(login[0]); }
                    }
                }
                else { NouvClient(); }
                Console.WriteLine();
                res = Continuer();
            } while (res != "O");
        }

        static void BonCommande(string email) //note : créer un menu pour se connecter avant (email/mdp)
        {
            Console.Clear();
            string tmp = Request("select max(numCommande) from bonCommande;", BozoConnection);
            if (tmp == "") { tmp = "0"; }
            int numCommande = Convert.ToInt32(tmp) + 1;
            string dateCommande = DateTime.Now.ToString("yyyy-MM-dd");
            string[] produit = choixProduit();
            string adresseLivraison = StrNotNull("Adresse de livraison :");
            Console.WriteLine("Message :");
            string message = Console.ReadLine();
            //ici if reduc
            string dateLivraison = "2023-05-05"; //à définir
            Console.Clear();
            Console.WriteLine("Récapitulatif de la commande :\n" +
                              "Date de la commande : " + dateCommande +
                            "\nChoix du produit : " + produit[0] +
                            "\nPrix : " + produit[1]);
            //aff fid
            //prix avec reduc
            if (produit[2] != "") { Console.WriteLine("Description du produit souhaité : " + produit[2]); }
            Console.WriteLine("Adresse de livraison :" + adresseLivraison +
                            "\nMessage :" + message +
                            "\nDate Livraison : " + dateLivraison +
                            "\n");
            tmp = Continuer();
            if (tmp == "O")
            {
                string etat = "VINV";
                if (produit[2] != "") { etat = "CPAV"; }
                string req = "insert into `Fleurs`.`bonCommande` values(" + numCommande + ",'" + dateCommande + "','" + email + "',null,'" + adresseLivraison + "','" + message + "','" + dateLivraison + "','" + produit[0] + "','" + etat + "'," + produit[1] + ",'" + produit[2] + "');";
                try
                {
                    Request(req, RootConnection);
                    Console.WriteLine("Commande enregistrée");
                }
                catch { Console.WriteLine("Erreur dans l'enregistrement de la commande\nVeuillez réessayer ultérieurement"); }
            }
            else { Console.WriteLine("Commande annulée"); }
        }

        static double Reduction()
        {
            double reduc = 0;


            return reduc;
            /*
             * Fidélité OR si le client achète plus de 5 bouquets par mois --> 15% est offerte sur chaque bouquet
             * Fidélité Bronze si le client achète en moyenne un bouquet par mois --> 5%
              
             * requete :
             * select avg(c) from (select count(*) as c, courriel from bonCommande where courriel='EMAIL' group by month(dateCommande)) as a;
             *      moyenne (avg(c))   du nombre (count(*) as c )     de bonCommande         de EMAIL            par mois
             */
        }

        static string[] choixProduit() //return string[nom,prix,description]
        {
            string res = "";
            string[] produit = new string[] { "", "", "" };
            Console.WriteLine("    Nom              Composition                                                           Prix        Catégorie\n" +
                              "(1) Gros Merci :     Arrangement floral avec marguerites et verdure                         45 euros   Toute occasion\n" +
                              "(2) L'amoureux :     Arrangement floral avec roses blanches et roses rouges                 65 euros   St-Valentin\n" +
                              "(3) L’Exotique :     Arrangement floral avec ginger, oiseaux du paradis, roses et genet     40 euros   Toute occasion\n" +
                              "(4) Maman :          Arrangement floral avec gerbera, roses blanches, lys et alstroméria    80 euros   Fête des mères\n" +
                              "(5) Vive la mariée : Arrangement floral avec lys et orchidées                              120 euros   Mariage\n" +
                              "(6) Personnalisé :   Description à fournir                                                 A définir");
            do
            {
                Console.WriteLine("Choisir un produit :");
                res = Console.ReadLine();
            } while (!(res == "1" || res == "2" || res == "3" || res == "4" || res == "5" || res == "6" || res == "7"));
            switch (res)
            {
                case "1":
                    produit[0] = "Gros Merci";
                    produit[1] = "45";
                    break;
                case "2":
                    produit[0] = "L amoureux";
                    produit[1] = "65";
                    break;
                case "3":
                    produit[0] = "L exotique";
                    produit[1] = "40";
                    break;
                case "4":
                    produit[0] = "Maman";
                    produit[1] = "80";
                    break;
                case "5":
                    produit[0] = "Vive la mariée";
                    produit[1] = "120";
                    break;
                default:
                    produit[0] = "Personnalisé";
                    produit[1] = StrNotNull("Entrer le prix :");
                    produit[2] = StrNotNull("Descrition du produit souhaité :");
                    break;
            }
            return produit;
        }

        static string[] Connection()
        {
            Console.Clear();
            string[] res = new string[] { "", "" };
            Console.WriteLine("Login");
            string email = StrNotNull("Email : ");
            string mdp = StrNotNull("Mot de passe : ");
            string req = "select count(*) from client where courriel='" + email + "' and motDePasse='" + mdp + "';";
            if (Request(req, RootConnection) == "1")
            {
                Console.WriteLine("Connection réussie\n");
                res[0] = email;
                res[1] = mdp;
            }
            return res;
        }

        static void NouvClient()
        {
            Console.Clear();
            string res = "";
            string nom = "";
            string prenom = "";
            string tel = "";
            string mdp = "";
            string adresse = "";
            string carte = "";
            string courriel = StrNotNull("Enter l'email : ");
            if (Request("select count(*) from client where courriel='" + courriel + "';", RootConnection) == "0")
            {
                do
                {
                    nom = StrNotNull("Nom : ");
                    prenom = StrNotNull("Prénom : ");
                    tel = StrNotNull("Tel : ");
                    mdp = StrNotNull("Mot de passe : ");
                    adresse = StrNotNull("Adresse de facturation : ");
                    carte = StrNotNull("Carte de crédit : ");
                    Console.Clear();
                    Console.WriteLine("Création du compte\n" +
                                   "\nNom : " + nom +
                                   "\nPrénom : " + prenom +
                                   "\nTel : " + tel);
                    Console.Write("Mot de passe : ");
                    for (int i = 0; i < mdp.Length; i++) { Console.Write("*"); }
                    Console.WriteLine("\nAdresse de facturation : " + adresse);
                    Console.Write("Carte de crédit : ");
                    for (int i = 0; i < carte.Length; i++)
                    {
                        if (i < 4 || i > carte.Length - 3) { Console.Write(carte[i]); }
                        else { Console.Write("*"); }
                    }
                    Console.WriteLine("\n");
                    res = Continuer();
                } while (res != "O");
                string req = "INSERT INTO `Fleurs`.`client` VALUES ('" + courriel + "','" + nom + "','" + prenom + "','" + tel + "','" + mdp + "','" + adresse + "','" + carte + "');";
                try { Request(req, RootConnection); }
                catch { Console.WriteLine("Erreur dans la création du compte"); }
            }
            else { Console.WriteLine("Email déjà utilisé !"); }
        }

        static void HistClient(string email)
        {
            Console.WriteLine("En travaux"); //select * from bonCommande where 
            string req = "select dateCommande, adresseLivraison,message,dateLivraison,produit,prix,description from bonCommande where courriel='";
            try
            {
                string[] hist = Request(req + email + "';", BozoConnection).Split("\n");
                Console.WriteLine("date de la commande, adresse de livraison, message, date de livraison, produit, prix");
                for (int i = 0; i < hist.Length; i++)
                {
                    string[] tmp = hist[i].Split(";");
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        Console.Write(tmp[j] + "   ");
                    }
                    Console.WriteLine();
                }

            }
            catch { Console.WriteLine("Erreur affichage des précédentes commandes"); }
            Console.ReadKey();
        }

        static string Continuer()
        {
            string res = "";
            do
            {
                Console.WriteLine("Continuer ? O/N");
                res = Console.ReadLine();
            } while (!(res == "O" || res == "N"));
            return res;
        }

        static string StrNotNull(string message)
        {
            string res = "";
            do
            {
                Console.Write(message);
                res = Console.ReadLine();
            } while (res == "");
            return res;
        }

        static void AffichageCommande()
        {
            /*
             VINV Commande standard pour laquelle un employé doit vérifier l’inventaire.
             CC Commande complète. Tous les items de la commande ont été indiqués (pour les
                commandes personnalisées) et tous ces items se trouvent en stock.
             CPAV Commande personnalisée à vérifier. Lorsqu’un client passe une commande
                personnalisée, son état est « CPAV ». Un employé vérifie la commande et rajoute
                des items si nécessaire. Ensuite, il change l’état de la commande à « CC ».
             CAL Commande à livrer. La commande est prête !
             CL Commande livrée. La commande a été livrée à l’adresse indiquée par le client. 
             */
            string res = "";
            Console.WriteLine("    Nom                                   Prix        Code\n" +
                              "(1) Liste des commandes standard à vérifier\n" +
                              "(2) Liste des commandes personnalisées à vérifier\n" +
                              "(3) Liste des commandes complètes\n" +
                              "(4) Liste des commandes à livrer\n" +
                              "(5) Historique des commandes livrées");
            do
            {
                Console.WriteLine("Choisir qqch :");
                res = Console.ReadLine();
            } while (!(res == "1" || res == "2" || res == "3" || res == "4" || res == "5" || res == "6" || res == "7"));

            /*
             * 1    
             * 2    
             * 3    
             * 4
             * 
            */
        }

        static void InsertionTable(string path, string nomTable)
        {
            string[] table = File.ReadAllLines(path);
            foreach (string line in table)
            {
                string req = "insert into `Fleurs`.`" + nomTable + "` values (";
                string[] elements = line.Split(";");
                for (int i = 0; i < elements.Length - 1; i++)
                {
                    req += "'" + elements[i] + "',";
                }
                req += "'" + elements[elements.Length - 1] + "');";
                try { Request(req, RootConnection); }
                catch { Console.WriteLine("Erreur insertion table " + nomTable); }
            }
        }

        static void Sauvegarde() //à finir
        {
            string tableClient = Request("select * from client;", BozoConnection);
            CreateWrite("client.txt", tableClient);
            //CreateWrite("bonCommande.txt",tableCommande)
            Console.WriteLine("Sauvegarde effectuée");
        }

        static void CreateWrite(string path, string str)
        {
            StreamWriter sw;
            if (!File.Exists(path)) { sw = File.CreateText(path); }
            else
            {
                sw = new StreamWriter(path);
                sw.WriteLine(str);
                sw.Close();
            }
        }

        static string Request(string req, string StringConnection) //renvoie un string pour pouvoir traiter les données ou l'afficher
        {
            MySqlConnection connection = new MySqlConnection(StringConnection);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = req;
            string str = "";
            if (req.Split()[0].ToLower() == "insert" || req.Split()[0].ToLower() == "delete" || req.Split()[0].ToLower() == "update") //sépare les commandes qui renvoient rien des autres
            {
                command.ExecuteNonQuery();
            }
            else
            {
                MySqlDataReader reader;
                reader = command.ExecuteReader();
                while (reader.Read())                           // parcours ligne par ligne
                {
                    string currentRowAsString = "";
                    for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                    {
                        string tmp = reader.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                        if (tmp != "") { currentRowAsString += tmp.ToUpper()[0] + tmp.Substring(1); }
                        if (i < reader.FieldCount - 1) currentRowAsString += ";";
                    }
                    str += currentRowAsString + "\n"; //'\n' et ';' permettent de récupérer les données sous le format csv
                }
                str = str.Substring(0, str.Length - 1);
            }
            connection.Close();
            return str;
        }
    }
}
