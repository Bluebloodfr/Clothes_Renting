using MySql.Data.MySqlClient;

namespace ProjetBDDFleurs
{
    class Program
    {
        const string RootConnection = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
        const string BozoConnection = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=bozo;PASSWORD=bozo;";

        static void Main(string[] args)
        {
            InsertionTable("clients.txt", "Client"); //à exécuter qu'une fois/creation table
            BonCommande("scooby.doo@gmail.com", "ouaf");

            /*
             idées suite :
             * définir la date livraison : null au début ? --> dès que le designer a validé les produits, on cherche quel mag peut assembler la commande 
                                                           --> donne une date de livraison de dateCommande+3 max si un magasin est trouvé
             * faire un affichage console de toutes les commandes non livrées et validées (ya tout les composants dispo dans un mag) 
               --> l'ordre par numCommande croissant permet de donner un j de livraison (cb de livraisons par j ?)
             */
        }

        static void BonCommande(string email, string mdp) //note : créer un menu pour se connecter avant (email/mdp)
        {
            Console.Clear();
            string tmp = Request("select max(numCommande) from bonCommande;", BozoConnection);
            if (tmp == "")
            {
                tmp = "0";
            }
            int numCommande = Convert.ToInt32(tmp) + 1;
            string dateCommande = DateTime.Now.ToString("yyyy-MM-dd");
            string[] produit = choixProduit();
            string adresseLivraison = StrNotNull("Adresse de livraison :");
            Console.WriteLine("Message :");
            string message = Console.ReadLine();
            string dateLivraison = "2023-05-05"; //à définir
            Console.Clear();
            Console.WriteLine("Récapitulatif de la commande :\n" +
                              "Date de la commande : " + dateCommande +
                            "\nChoix du produit : " + produit[0] +
                            "\nPrix : " + produit[1]);
            if (produit[2] != "") { Console.WriteLine("Description du produit souhaité : " + produit[2]); }
            Console.WriteLine("Adresse de livraison :" + adresseLivraison +
                            "\nMessage :" + message +
                            "\nDate Livraison : " + dateLivraison +
                            "\n\nContinuer ? O/N");
            do
            {
                tmp = Console.ReadLine();
            } while (!(tmp == "O" || tmp == "N"));
            if (tmp == "O")
            {
                string etat = "VINV";
                if (produit[2] != "") { etat = "CPAV"; }
                string req = "insert into `Fleurs`.`bonCommande` values(" + numCommande + ",'" + dateCommande + "','" + email + "',null,'" + adresseLivraison + "','" + message + "','" + dateLivraison + "','" + produit[0] + "','" + etat + "'," + produit[1] + ",'" + produit[2] + "');";
                Console.WriteLine(req);
                try
                {
                    Request(req, RootConnection);
                    Console.WriteLine("Commande enregistrée");
                }
                catch
                {
                    Console.WriteLine("Erreur dans l'enregistrement de la commande\nVeuillez réessayer ultérieurement");
                }
            }
            else { Console.WriteLine("Commande annulée"); }
        }

        static string[] choixProduit() //return string[nom,prix,description]
        {
            string res = "";
            string[] produit = new string[] { "", "", "" };
            Console.WriteLine("    Nom              Composition                                                           Prix        Catégorie\n" +
                              "(1) Gros Merci :     Arrangement floral avec marguerites et verdure                         45 euros   Toute occasion\n" +
                              "(2) L’amoureux :     Arrangement floral avec roses blanches et roses rouges                 65 euros   St-Valentin\n" +
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
                    produit[0] = "L'amoureux";
                    produit[1] = "65";
                    break;
                case "3":
                    produit[0] = "L'exotique";
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
            string[] res = new string[] { "", "" };
            Console.Clear();
            Console.WriteLine("Login");
            string email = StrNotNull("Email :");
            string mdp = StrNotNull("Mot de passe :");
            string req = "select count(*) from client where courriel='" + email + "' and motDePasse='" + mdp + "';";
            if (Request(req, RootConnection) == "1")
            {
                Console.WriteLine("Connection done");
                res[0] = email;
                res[1] = mdp;
            }
            return res;
        }

        static void NouvClient()
        {
            Console.Write("Entrer l'email : ");
            string courriel = StrNotNull("Email :");
            if (Request("select count(*) from client where courriel='" + courriel + "';", RootConnection) == "0")
            {
                string nom = StrNotNull("Nom :");
                string prenom = StrNotNull("Prenom");
                Console.WriteLine("Tel : ");
                string tel = Console.ReadLine();
                Console.WriteLine("Mot de passe : ");
                string mdp = StrNotNull("Mot de passe :");
                Console.WriteLine("Adresse de facturation : ");
                string adresse = Console.ReadLine();
                Console.WriteLine("Carte de Crédit : ");
                string carte = Console.ReadLine();
                string req = "INSERT INTO `Fleurs`.`client` VALUES ('"
                    + courriel + "','" + nom + "','" + prenom + "','" + tel + "','" + mdp + "','" + adresse + "','" + carte + "');";
                Request(req, RootConnection);
            }
            else
            {
                Console.WriteLine("Email déjà utilisé !");
            }
        }

        static string StrNotNull(string message)
        {
            string res = "";
            do
            {
                Console.WriteLine(message);
                res = Console.ReadLine();
            } while (res == "");
            return res;
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
                try
                {
                    Request(req, RootConnection);
                }
                catch
                {
                    Console.WriteLine("Erreur insertion table " + nomTable);
                }

            }
        }

        static void Sauvegarde()
        {
            string tableClient = Request("select * from client;", BozoConnection);
            CreateWrite("client.txt", tableClient);
            //CreateWrite("bonCommande.txt",tableCommande)
            Console.WriteLine("Sauvegarde effectuée");
        }

        static void CreateWrite(string path, string str)
        {
            StreamWriter sw;
            if (!File.Exists(path))
            {
                sw = File.CreateText(path);
            }
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
                        if (tmp != "") currentRowAsString += tmp.ToUpper()[0] + tmp.Substring(1);
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
