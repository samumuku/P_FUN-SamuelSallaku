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

        //initializer la DB
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
    }
}
