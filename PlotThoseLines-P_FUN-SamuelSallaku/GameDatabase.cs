using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace PlotThoseLines_P_FUN_SamuelSallaku
{
    internal class GameDatabase
    {
        private static string connectionDb = "Data Source=games.db";   // indiquer la source de la DB

        /// <summary>
        /// initialiser la DB sqlite
        /// </summary>
        public static void initializeDb()
        {
            // connexion à la db
            using var connection = new SqliteConnection(connectionDb);
            connection.Open();

            // spécifications de la table
            string table = @"
            CREATE TABLE IF NOT EXISTS t_games (
                Name TEXT NOT NULL,
                Year INTEGER NOT NULL,
                Sales REAL NOT NULL
            );";

            //création de la table t_games en spécifiant la commande et connexion
            using var createTable = new SqliteCommand(table, connection);

            //permet d'écrire les données dans la table
            createTable.ExecuteNonQuery();
        }

        /// <summary>
        /// méthode qui va sauvegarder les données dans la DB avec le paramètre de la liste des jeux
        /// </summary>
        /// <param name="games">liste des jeux</param>
        public static void saveGames(List<GameData> games)
        {
            using var connection = new SqliteConnection(connectionDb); //connexion
            connection.Open(); //faire la connexion a la db

            // effacer les données pour éviter les doublons
            using var clearData = new SqliteCommand("DELETE FROM t_games;", connection);
            clearData.ExecuteNonQuery(); //écrire

            // boucle foreach car dans ce cas on ne peut pas faire du LinQ
            foreach (var game in games)
            {
                var cmd = connection.CreateCommand(); // fonctionne comme une transaction en mysql, on commence une commande et elle s'éxecute quand on le précise
                cmd.CommandText = "INSERT INTO t_games (Name, Year, Sales) VALUES (@name, @year, @sales)"; // ajouter données dans la table
                cmd.Parameters.AddWithValue("@name", game.Name); // paramètre qui va prendre la valeur de "game.Name" et va le remplacer dans la requête ci dessus le "@name"
                cmd.Parameters.AddWithValue("@year", game.Year); // même chose
                cmd.Parameters.AddWithValue("@sales", game.Sales); // même chose
                cmd.ExecuteNonQuery(); // écrire
            }
        }

        /// <summary>
        /// méthode qui va charger et lire les données depuis la DB
        /// </summary>
        /// <returns>retourne la liste des jeux</returns>
        public static List<GameData> loadGames()
        {
            // créer une nouvelle liste de GameData vide 
            var gameList = new List<GameData>();

            //connexion
            using var connection = new SqliteConnection(connectionDb);
            connection.Open();

            // préparation de la requête SELECT
            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT Name, Year, Sales FROM t_games ORDER BY Year;"; // requête SELECT pour récupérer les données des jeux, ordonner par la colonne Year

            //éxecute la requête SQL puis retourne un objet reader qui sera utilisé pour lire les résultats de chaque ligne
            using var reader = selectCommand.ExecuteReader();

            //tant qu il a une ligne qui existe il va ajouter l'objet GameData avec les données de la ligne correspondante dans la liste des jeux
            while (reader.Read())
            {
                //ajouter les données à la liste
                gameList.Add(new GameData
                {
                    Name = reader.GetString(0), //lire la première colonne 
                    Year = reader.GetInt32(1),  //lire la deuxième colonne
                    Sales = reader.GetDouble(2),//lire la troisième colonne
                });
            }
            //retourner la liste des jeux
            return gameList;
        }
    }
}
