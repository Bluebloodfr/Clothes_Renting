using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace ProjetBDDFleurs
{
    class Program
    {
        const string RootConnection = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
        const string BozoConnection = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=bozo;PASSWORD=bozo;";

        static void Main(string[] args)
        {
            //NouvClient();
            //Sauvegarde();
            //InsertionTable("client.txt", "Client");
            //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd"));
            choixProduit();
        }

        static void BonCommande(string email, string mdp)
        {
            string req = "";
            /*
             numCommande,dateCommande,adresseLivraison,message,dateLivraison,produit
             */
            string numCommande = "";
            string dateCommande = DateTime.Now.ToString("yyyy-MM-dd");
            string adresseLivraison = "";
            string message = "";
            string dateLivraison = "";
            string produit = "";

        }

        static string choixProduit()
        {
            string res = "";
            Console.WriteLine("(1) Nom              Composition                                                           Prix        Catégorie\n" +
                              "(2) Gros Merci :     Arrangement floral avec marguerites et verdure                         45 euros   Toute occasion\n" +
                              "(3) L’amoureux :     Arrangement floral avec roses blanches et roses rouges                 65 euros   St-Valentin\n" +
                              "(4) L’Exotique :     Arrangement floral avec ginger, oiseaux du paradis, roses et genet     40 euros   Toute occasion\n" +
                              "(5) Maman :          Arrangement floral avec gerbera, roses blanches, lys et alstroméria    80 euros   Fête des mères\n" +
                              "(6) Vive la mariée : Arrangement floral avec lys et orchidées                              120 euros   Mariage\n" +
                              "(7) Personnalisé :   Description à fournir                                                 A définir");
            do
            {
                Console.WriteLine("Choisir un produit :");
                res = Console.ReadLine();
            } while (!(res == "1" || res == "2" || res == "3" || res == "4" || res == "5" || res == "6" || res == "7"));
            return res;
            /* 
            Gros Merci Arrangement floral avec marguerites et verdure 45 € Toute occasion
            L’amoureux Arrangement floral avec roses blanches et roses rouges 65 € St-Valentin
            L’Exotique Arrangement floral avec ginger, oiseaux du paradis, roses et genet 40 € Toute occasion
            Maman Arrangement floral avec gerbera, roses blanches, lys et alstroméria 80 € Fête des mères
            Vive la mariée Arrangement floral avec lys et orchidées 120 $ Mariage 
            Personnalisé      description à fournir      prix à définir
*/
        }

        static string[] Connection()
        {
            string[] res = new string[] { "", "" };
            Console.Clear();
            Console.WriteLine("Login");
            string email = "";
            do
            {
                Console.Write("Email : ");
                email = Console.ReadLine();
            } while (email == "");
            Console.Write("Mot de passe : ");
            string mdp = Console.ReadLine();
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
                string req = "INSERT INTO `Fleurs`.`client` (`courriel`,`nomC`,`prenomC`,`numTel`,`motDePasse`,`adresseFacturation`,`carteCredit`) VALUES ('"
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
                string req = "insert into `Fleurs`.`" + nomTable + "`( `courriel`,`nomC`,`prenomC`,`numTel`,`motDePasse`,`adresseFacturation`,`carteCredit`) values (";
                string[] elements = line.Split(";");
                for (int i = 0; i < elements.Length - 1; i++)
                {
                    req += "'" + elements[i] + "',";
                }
                req += "'" + elements[elements.Length - 1] + "');";
                Console.WriteLine(req);
                Request(req, RootConnection);
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
                        currentRowAsString += tmp.ToUpper()[0] + tmp.Substring(1);
                        if (i < reader.FieldCount - 1) currentRowAsString += ";";
                    }
                    str += currentRowAsString + "\n";    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
                }
                str = str.Substring(0, str.Length - 1);
            }
            connection.Close();
            return str;
        }
    }
}
