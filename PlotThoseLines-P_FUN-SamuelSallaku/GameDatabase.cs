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
    }
}
