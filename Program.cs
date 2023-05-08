using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;
using System.Xml;

namespace ProjetBDDFleurs
{
    class Program
    {
        const string RootConnection = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
        const string BozoConnection = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=bozo;PASSWORD=bozo;";

        static void Main(string[] args)
        {
            Menu();
            //UpdateElementStock();
            /*
             
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
            if (Request("select count(*) from client", BozoConnection) == "0")
            {
                InsertionTable("clients.txt", "client");
                InsertionTable("magasin.txt", "magasin");
                InsertionTable("bonCommande.txt", "bonCommande");
                InsertionTable("elementCommande.txt", "elementCommande");
                InsertionTable("elementStock.txt", "elementStock");
            }
            string res = "";
            do
            {
                Console.Clear();
                Console.WriteLine("Que faire ?\n" +
                                  "(1) Se connecter\n" +
                                  "(2) Créer un compte client\n" +
                                  "(3) Connection employés\n");
                do
                {
                    Console.WriteLine("Choisir une action :");
                    res = Console.ReadLine();
                } while (!(res == "1" || res == "2" || res == "3"));
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
                else if (res == "2") { NouvClient(); }
                else { LoginEmployes(); }
                Console.WriteLine("\nMenu principal");
                res = Continuer();
            } while (res == "O");
        }

        static void LoginEmployes()
        {
            Console.WriteLine("\nLogin employés");
            string id = StrNotNull("Identifiant : ");
            string mdp = StrNotNull("Mot de passe : ");
            if (id == "root" && mdp == "root")
            {
                string res = "";
                do
                {
                    MenuEmployes();
                    Console.WriteLine("\nMenu employés");
                    res = Continuer();
                } while (res == "O");
            }
        }

        static void MenuEmployes()
        {
            Console.Clear();
            string res = "";
            Console.WriteLine("Que faire ?\n" +
                              "(1) Voir les commandes à livrer\n" +
                              "(2) vérifier les stocks\n" +
                              "(3) Commandes à vérifier\n" +
                              "(4) Statistiques\n");
            do
            {
                Console.WriteLine("Choisir une action :");
                res = Console.ReadLine();
            } while (!(res == "1" || res == "2" || res == "3"));
            if (res == "1")
            {
                string[] tmp = Request("select numCommande,dateCommande,adresseM,adresseLivraison,message,produit,description from bonCommande where etat='CAL';", BozoConnection).Split(";");
                for (int i = 0; i < tmp.Length; i++)
                {
                    Console.Write(tmp[i] + ", ");
                }
                Console.WriteLine("\n");
                do
                {
                    Console.WriteLine("Une commande a été livrée ? O/N");
                    res = Console.ReadLine();
                } while (!(res == "O" || res == "N"));
                if (res == "O")
                {
                    res = StrNotNull("Entrer le numero de commande : ");
                    Request($"update bonCommande set etat='CL',dateLivraison='{DateTime.Now.ToString("yyyy-MM-dd")}' where numCommande={res};", RootConnection);
                }

            }
            else if (res == "2")
            {
                Console.WriteLine("Liste des articles en dessous de 10 unités");
                string[] tmp = Request("select nomM, nomES,quantiteES,prixES from elementStock where quantiteES<100 and nomES not in('Gros Merci','Lamoureux','L exotique','Maman','Vive la mariee') order by nomM;", BozoConnection).Split(";");
                for (int i = 0; i < tmp.Length; i++)
                {
                    Console.Write(tmp[i] + ", ");
                }
                Console.WriteLine("\n");
                Console.WriteLine("\nListe des bouquets en dessous de 10 unités :");
                tmp = Request("select nomM, nomES,quantiteES,prixES from elementStock where quantiteES<100 and nomES in('Gros Merci','Lamoureux','L exotique','Maman','Vive la mariee') order by nomM;", BozoConnection).Split(";");
                for (int i = 0; i < tmp.Length; i++)
                {
                    Console.Write(tmp[i] + ", ");
                }
                res = StrNotNull("Ajouter des elements au stock ? O/N : ");
                if (res == "O") { AjoutElementStock(); }
            }
            else if (res == "3")
            {
                Console.WriteLine("Que faire ?\n" +
                              "(1) Commandes standard à vérifier\n" +
                              "(2) Compléter les commandes personnalisées\n" +
                              "(3) Ajouter éléments commande personnalisée\n");
                do
                {
                    Console.WriteLine("Choisir une action :");
                    res = Console.ReadLine();
                } while (!(res == "1" || res == "2" || res == "3"));
                if (res == "1") { Console.WriteLine("t'as oublié de faire ça"); }
                else if (res == "2")
                {
                    string[] CC = Request("select numCommande,dateCommande from bonCommande where etat='CC';", BozoConnection).Split("\n");
                    int[] num=new int[CC.Length];
                    Console.WriteLine("numCommande,dateCommande");
                    for (int i = 0; i < CC.Length; i++)
                    {
                        string[] tmp = CC[i].Split(";");
                        for(int j=0;j<tmp.Length;j++)
                        {
                            if(j==0 && tmp[j]!="") { num[j] = Convert.ToInt32(tmp[j]); }
                            else { num[j] = -1; }
                            if (j < tmp.Length - 1) { Console.Write(tmp[j] + ", "); }
                            else { Console.Write(tmp[j]); }
                        }
                        if (num[i] != -1)
                        {
                            string[] mag = GetMagasin(num[i]);
                            if (mag[0] != "")
                            {
                                CCtoCAL(num[i], mag[0]);
                                Console.WriteLine($"Commande {num[i]} prête à être livrée");
                            }
                            else { Console.WriteLine($"Stock insuffisant pour la commade {num[i]}, vérifier les stocks"); }
                        }
                    }
                }
                else
                {
                    string[] CPAV = Request("select numCommande,dateCommande,message,description from bonCommande where etat='CPAV';", BozoConnection).Split(";");
                    for (int i = 0; i < CPAV.Length; i++)
                    {
                        Console.Write(CPAV[i] + ", ");
                    }
                    res = StrNotNull("Entrer le numero de commande : ");
                    CPAVtoCC(Convert.ToInt32(res));
                }
            }
        }

        static void Statistiques()
        {
            int nombreTotalDeCommandes = NombreTotalDeCommandes();
            Dictionary<string, int> commandesParEtat = CommandesParEtat();
            decimal montantTotalDesVentes = MontantTotalDesVentes();
            double prixMoyenBouquet = PrixMoyenBouquet();
            string meilleurClientMois = MeilleurClient("MONTH");
            string meilleurClientAnnee = MeilleurClient("YEAR");
            //string bouquetStandardPlusDeSucces = BouquetStandardPlusDeSucces();
            string magasinPlusDeChiffreAffaires = MagasinPlusDeChiffreAffaires();
            //string fleurExotiqueMoinsVendue = FleurExotiqueMoinsVendue();

            Console.WriteLine("Statistiques:");
            Console.WriteLine($"Nombre total de commandes: {nombreTotalDeCommandes}");
            Console.WriteLine("Commandes par état:");
            foreach (KeyValuePair<string, int> kvp in commandesParEtat)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }
            Console.WriteLine($"Montant total des ventes: {montantTotalDesVentes}");
            Console.WriteLine($"Prix moyen du bouquet acheté: {prixMoyenBouquet}");
            Console.WriteLine($"Meilleur client du mois: {meilleurClientMois}");
            Console.WriteLine($"Meilleur client de l'année: {meilleurClientAnnee}");
            //Console.WriteLine($"Bouquet standard le plus populaire: {bouquetStandardPlusDeSucces}");
            Console.WriteLine($"Magasin ayant généré le plus de chiffre d'affaires: {magasinPlusDeChiffreAffaires}");
            //Console.WriteLine($"Fleur exotique la moins vendue: {fleurExotiqueMoinsVendue}");
        }

        static int NombreTotalDeCommandes()
        {
            string req = "SELECT COUNT(*) FROM bonCommande;";
            string result = Request(req, RootConnection);
            return int.Parse(result);
        }

        static Dictionary<string, int> CommandesParEtat()
        {
            string req = "SELECT etat, COUNT(*) FROM bonCommande GROUP BY etat;";
            string result = Request(req, RootConnection);
            string[] lines = result.Split("\n");
            Dictionary<string, int> commandesParEtat = new Dictionary<string, int>();

            foreach (string line in lines)
            {
                string[] data = line.Split(";");
                commandesParEtat.Add(data[0], int.Parse(data[1]));
            }

            return commandesParEtat;
        }

        static decimal MontantTotalDesVentes()
        {
            string req = "SELECT SUM(prix) FROM bonCommande;";
            string result = Request(req, RootConnection);
            return decimal.Parse(result);
        }

        static double PrixMoyenBouquet()
        {
            string req = "SELECT AVG(prix) FROM bonCommande;";
            string result = Request(req, RootConnection);
            return double.Parse(result.Replace('.', ','));
        }

        static string MeilleurClient(string periode)
        {
            string req = "SELECT courriel, SUM(prix) as total FROM bonCommande WHERE dateCommande >= DATE_ADD(CURDATE(), INTERVAL -1 " + periode + ") GROUP BY courriel ORDER BY total DESC LIMIT 1;";
            string result = Request(req, RootConnection);
            return result.Split(';')[0];
        }

        static string BouquetStandardPlusDeSucces()
        {
            string req = "SELECT ec.nomEC, COUNT(*) as nb " +
                         "FROM elementCommande ec " +
                         "JOIN bonCommande bc ON ec.numCommande = bc.numCommande " +
                         "WHERE bc.produit = 'Standard' AND bc.etat = 'CC' " +
                         "GROUP BY ec.nomEC " +
                         "ORDER BY nb DESC LIMIT 1;";
            string result = Request(req, RootConnection);
            return result.Split(';')[0];
        }

        static string MagasinPlusDeChiffreAffaires()
        {
            string req = "SELECT m.adresseM, m.nomM, SUM(bc.prix) as total " +
                         "FROM bonCommande bc " +
                         "JOIN magasin m ON bc.adresseM = m.adresseM " +
                         "GROUP BY m.adresseM, m.nomM " +
                         "ORDER BY total DESC LIMIT 1;";
            string result = Request(req, RootConnection);
            string[] parts = result.Split(';');
            return parts[0] + ", " + parts[1];
        }

        static string FleurExotiqueMoinsVendue()
        {
            string req = "SELECT ec.nomEC, COUNT(*) as nb " +
                         "FROM elementCommande ec " +
                         "JOIN bonCommande bc ON ec.numCommande = bc.numCommande " +
                         "WHERE bc.produit = 'Exotique' " +
                         "GROUP BY ec.nomEC " +
                         "ORDER BY nb ASC LIMIT 1;";
            string result = Request(req, RootConnection);
            return result.Split(';')[0];
        }

        static void ExportTableToXml(string StringConnection, string tableName)
        {
            string selectQuery = $"SELECT * FROM {tableName}";
            MySqlConnection connection = new MySqlConnection(StringConnection);
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    // Création d'un DataTable pour stocker les données de la table
                    DataTable dataTable = new DataTable(tableName);
                    dataTable.Load(reader);
                    // Export des données du DataTable en XML
                    using (XmlTextWriter xmlWriter = new XmlTextWriter($"{tableName}.xml", System.Text.Encoding.UTF8))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        dataTable.WriteXml(xmlWriter);
                    }
                }
            }
            connection.Close();
        }

        static void BonCommande(string email)
        {
            Console.Clear();
            string tmp = Request("select max(numCommande) from bonCommande;", BozoConnection);
            if (tmp == "") { tmp = "0"; }
            int numCommande = Convert.ToInt32(tmp) + 1;
            string dateCommande = DateTime.Now.ToString("yyyy-MM-dd");
            string[] produit = choixProduit();
            string adresseLivraison = StrNotNull("Adresse de livraison : ");
            Console.Write("Message : ");
            double coeffReduc = Reduction(email);
            double prixFinal = Convert.ToInt32(produit[1]) * coeffReduc;
            string message = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Récapitulatif de la commande :" +
                            "\nChoix du produit : " + produit[0] +
                            "\nPrix : " + produit[1]);
            Console.Write("Réduction fidélité : ");
            switch (coeffReduc)
            {
                case 0.85:
                    Console.WriteLine("15%");
                    break;
                case 0.95:
                    Console.WriteLine("5%");
                    break;
                default:
                    Console.WriteLine("Non");
                    break;
            }
            Console.WriteLine("Prix final : " + prixFinal);
            if (produit[2] != "") { Console.WriteLine("Description du produit souhaité : " + produit[2]); }
            Console.WriteLine("Adresse de livraison :" + adresseLivraison +
                            "\nMessage :" + message + "\n");
            tmp = Continuer();
            if (tmp == "O")
            {
                string etat = "VINV";
                if (produit[2] != "") { etat = "CPAV"; }
                string req = $"insert into `Fleurs`.`bonCommande` values({numCommande},'{dateCommande}','{email}',null,'{adresseLivraison}','{message}',null,'{produit[0]}','{etat}',{prixFinal.ToString(CultureInfo.CreateSpecificCulture("en-GB"))},'{produit[2]}');";
                try
                {
                    Request(req, RootConnection);
                    Console.WriteLine("Commande enregistrée");
                    if (produit[2] == "")
                    {
                        VINVtoCAL(numCommande, produit[0], Convert.ToDouble(produit[1].Replace('.', ',')));
                    }
                }
                catch { Console.WriteLine("Erreur dans l'enregistrement de la commande\nVeuillez réessayer ultérieurement" + req); }
            }
            else { Console.WriteLine("Commande annulée"); }
        }

        static double Reduction(string email)
        {
            double coeff = 1.0;
            double moy = 0;
            try
            {
                moy = Convert.ToDouble(Request($"select avg(c) from(select count(*) as c, courriel from bonCommande where courriel = '{email}' group by month(dateCommande)) as a;", BozoConnection));
            }
            catch { moy = 0.0; }
            if (moy > 5) { coeff = 0.85; }
            else if (moy >= 1) { coeff = 0.95; }
            return coeff;
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
                    produit[0] = "Vive la mariee";
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
            string req = $"select count(*) from client where courriel='{email}' and motDePasse='{mdp}';";
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
            if (Request($"select count(*) from client where courriel='{courriel}';", RootConnection) == "0")
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
                string req = $"INSERT INTO `Fleurs`.`client` VALUES ('{courriel}','{nom}','{prenom}','{tel}','{mdp}','{adresse}','{carte}');";
                try { Request(req, RootConnection); }
                catch { Console.WriteLine("Erreur dans la création du compte"); }
            }
            else { Console.WriteLine("Email déjà utilisé !"); }
        }

        static void NouvElementStock(string nomMag, string adresseMag)
        {
            string nomES = StrNotNull("Nom de l'élément : ");
            if (Request($"select count(*) from elementStock where nomES='{nomES}' and nomM='{nomMag}';", BozoConnection) == "0")
            {
                string quantite = StrNotNull("Quantité : ");
                Console.Write("Disponibilité : ");
                string dispo = Console.ReadLine();
                string prix = StrNotNull("Prix : ");
                try
                {
                    Request($"insert into elementStock values('{nomMag}','{adresseMag}','{nomES}','{quantite}','{dispo}','{prix}');", RootConnection);
                }
                catch { Console.WriteLine("Erreur création element"); }
            }
            else { Console.WriteLine("Nom déjà utilisé !"); }
        }

        static void NouvElementCommande(int numCommande, string nomEC, int quantite, double prix)
        {
            try
            {
                Request($"insert into `Fleurs`.`elementCommande` values('{numCommande}','{nomEC}','{quantite}','{prix.ToString(CultureInfo.CreateSpecificCulture("en-GB"))}';", RootConnection);
            }
            catch { Console.WriteLine("Erreur enregistrement élément commande"); }
        }

        static string[] GetMagasin(int numCommande) //return magasin[adresse, nom]
        {
            string[] magasin;
            try
            {
                string a = Request($"SELECT m.adresseM, m.nomM FROM magasin m WHERE NOT EXISTS (SELECT 1 FROM elementCommande eC LEFT JOIN elementStock eS ON eC.nomEC = eS.nomES AND eS.adresseM = m.adresseM AND eC.quantiteEC < eS.quantiteES WHERE eC.numCommande = '{numCommande}' AND eS.nomES IS NULL);", BozoConnection);
                string[] tmp = a.Split("\n");
                magasin = tmp[0].Split(";");
            }
            catch { magasin = new string[2] { "", "" }; }
            return magasin;
        }

        static void VINVtoCAL(int numCommande, string nomEC, double prix)
        {
            NouvElementCommande(numCommande, nomEC, 1, prix);
            string[] mag = GetMagasin(numCommande);
            if (mag[1] != "")
            {
                Request($"update elementStock set quantiteES=quantiteES-1 where adresseM='{mag[1]}' and nomES='{nomEC}';", BozoConnection);
                Request($"update bonCommande set etat='CAL' where numCommande={numCommande};", RootConnection);
            }
        }

        static void CCtoCAL(int numCommande, string adresseM)
        {
            string[] nomEC = Request($"select nomEC from elementCommande where numCommande='{numCommande}';", BozoConnection).Split(";");
            string[] quantites = Request($"select quantiteEC from elementCommande where numCommande='{numCommande}';", BozoConnection).Split(";");
            for (int i = 0; i < nomEC.Length; i++)
            {
                Request($"update elementStock set quantiteES=quantiteES-{quantites[i]} where adresseM='{adresseM}' and nomES='{nomEC[i]}';", BozoConnection);
            }
        }

        static void CPAVtoCC(int numCommande)
        {
            string[] tmp = Request($"select description,prix from bonCommande where numCommande={numCommande};", BozoConnection).Split(";");
            string description = tmp[0];
            double prixTotal = Convert.ToDouble(tmp[1].Replace('.', ','));
            string res = "";
            string nomEC;
            int quantite;
            double prix;
            Console.WriteLine("Ajout éléments commande n°" + numCommande
                          + "\nDescription : " + description +
                          "\nprix total : " + prixTotal);
            do
            {
                nomEC = StrNotNull("Nom de l'élément : ");
                quantite = Convert.ToInt32(StrNotNull("Quantité : "));
                Console.Write("Prix : ");
                prix = Convert.ToDouble(Console.ReadLine());
                NouvElementCommande(numCommande, nomEC, quantite, prix);
                Console.WriteLine("\nCréation éléments commande");
                res = Continuer();
            } while (res == "O");
            Request("update bonCommande set etat='CC' where numCommande=" + numCommande + ";", RootConnection);
        }

        static void AjoutElementStock()
        {
            Console.WriteLine("Nom\t\tAdresse");
            string[] magasins = Request("select nomM, adresseM from magasin;", BozoConnection).Split("\n");
            for (int i = 0; i < magasins.Length; i++)
            {
                Console.WriteLine("(" + i + ") : " + magasins[i].Split(";")[0] + "   " + magasins[i].Split(";")[1]);
            }
            string res = StrNotNull("Choisir un magasin : ");
            string nomMag = magasins[Convert.ToInt32(res)].Split(";")[0];
            string adresseMag = magasins[Convert.ToInt32(res)].Split(";")[1];
            string[] stock = Request($"select nomES, quantiteES from elementStock where adresseM='{adresseMag}';", BozoConnection).Split("\n");
            Console.WriteLine("\nNom\t\tquantité");
            for (int i = 0; i < stock.Length; i++)
            {
                Console.WriteLine($"({i}) : " + stock[i].Split(";")[0] + "   " + stock[i].Split(";")[1]);
            }
            Console.WriteLine($"({stock.Length}) : Ajouter un nouvel élément");
            res = StrNotNull("Choisir un élément : ");
            if (Convert.ToInt32(res) >= stock.Length) { NouvElementStock(nomMag, adresseMag); }
            else
            {
                string nomES = stock[Convert.ToInt32(res)].Split(";")[0];
                Console.WriteLine();
                res = StrNotNull("Quantité à ajouter : ");
                try
                {
                    Request($"update elementStock set quantiteES=quantiteES+{res} where adresseM='{adresseMag}' and nomES='{nomES}';", RootConnection);
                }
                catch { Console.WriteLine("Erreur update quantité"); }
            }
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
                if (str.Length > 0) { str = str.Substring(0, str.Length - 1); }
            }
            connection.Close();
            return str;
        }
    }
}
