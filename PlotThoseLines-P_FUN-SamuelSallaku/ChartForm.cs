using ScottPlot.TickGenerators.Financial;
using ScottPlot.WinForms;
using System.Drawing.Text;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Channels;
using System.Windows.Forms;

namespace PlotThoseLines_P_FUN_SamuelSallaku
{
    public partial class ChartForm : Form
    {
        // liste de jeux
        private List<GameData> gamesData = new List<GameData>();

        public ChartForm()
        {
            InitializeComponent();
        }

        private void LoadPlotForm(object sender, EventArgs e)
        {
            PlotForm.Plot.Clear(); // effacer
            LoadData(); // charger le fichier 
        }
        private Func<string[], int, int, int, GameData?> formatGameData = (gamedata, nameIndex, yearIndex, salesIndex) =>
        {
            // utiliser TryParse pour éviter les exceptions
            bool yearParsed = int.TryParse(gamedata[yearIndex], out int year); //convertir l'année en int
            bool salesParsed = double.TryParse(gamedata[salesIndex], // texte à convertir
                System.Globalization.NumberStyles.Any, // definit quel formats de numero a utiliser durant le parse, dans ce cas, tout
                System.Globalization.CultureInfo.InvariantCulture, // certain pays utilisent des numéros différents, comme en France on fait 123,45, au lieu de 123.45
                out double sales);

            if (!yearParsed || !salesParsed)
                return null; // ignorer les lignes invalides

            return new GameData
            {
                Name = gamedata[nameIndex].Trim(), // effacer espacements
                Year = year, // transformer en numero le string
                Sales = sales // " "
            };
        };
        private void ImportCSV(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Import Data";
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                try
                {
                    string[] lines = File.ReadAllLines(fileName);

                    if (lines.Length <= 1)
                    {
                        MessageBox.Show("Le fichier est vide, ou bien il n'a pas de lignes dedans", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // première ligne, nettoyer les espaces inutiles
                    string[] headers = lines[0].Split(',').Select(h => h.Trim()).ToArray();

                    // détecter les colonnes correspondantes, donc
                    // il va chercher pour la première ligne et si par exemple nameIndex est premier alors la valeur sera 0, et 1 pour yearIndex, etc
                    int nameIndex = Array.FindIndex(headers, h => h.Equals("Game", StringComparison.OrdinalIgnoreCase)); //StringComparison.OrdinalIgnoreCase va comparer
                    int yearIndex = Array.FindIndex(headers, h => h.Equals("Year", StringComparison.OrdinalIgnoreCase)); // les strings, mais va ignorer la casse.
                    int salesIndex = Array.FindIndex(headers, h => h.Equals("Sales", StringComparison.OrdinalIgnoreCase)); // si "Sales" était "sales" dans le CSV = erreur

                    // vérifier que les colonnes existent
                    if (nameIndex == -1 || yearIndex == -1 || salesIndex == -1)
                    {
                        // si les colonnes voulues n'existent pas alors on affiche une erreur
                        MessageBox.Show(
                            "Le format CSV est invalide. Les colonnes requises: 'Game', 'Year', et 'Sales'.",
                            "Format CSV invalide",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }

                    
                    // parser le CSV en objets GameData
                    var newGames = lines
                        .Skip(1) // skip la premiere ligne
                        .Where(line => !string.IsNullOrWhiteSpace(line)) // ignorer lignes vide
                        .Select(line => line.Split(',')) // separation des virgules
                        .Where(p => p.Length > Math.Max(nameIndex, Math.Max(yearIndex, salesIndex))) // verifier que la ligne ait assez de données (colonnes)
                        .Select(g => formatGameData(g, nameIndex, yearIndex, salesIndex))
                        .Where(g => g != null)
                        .Select(g => g!) // on sait que g n'est pas null grace au Where avant
                        .ToList()!;

                    // vérifier qu’il y a bien des données valides
                    if (!newGames.Any())
                    {
                        MessageBox.Show("Aucune donnée valide a été trouvée dans le fichier.", "Aucune donnée", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    this.gamesData = this.gamesData
                        .Concat(newGames)
                        .GroupBy(g => (Name: g.Name.Trim().ToLowerInvariant(), g.Year))
                        .Select(grp => grp.Last()) // garde la dernière occurrence du groupe
                        .OrderBy(g => g.Year)
                        .ThenBy(g => g.Name, StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    // effacer la liste
                    Games.Items.Clear();

                    // ajouter le nom du jeu dans la liste
                    this.gamesData
                        .Select(g => g.Name)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList()
                        .ForEach(name => Games.Items.Add(name, true));

                    SaveData(); // sauvegarder les donnees
                    SelectYears();

                    // appel methode pour ajouter les données
                    PlotGames();

                    PlotForm.Refresh();

                    MessageBox.Show("Données importées et fusionnées avec succès !", "Importation réussie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // afficher une erreur claire plutôt que afficher une exception
                    MessageBox.Show(
                        "Erreur pendant le chargement du fichier CSV:\n\n" + ex.Message,
                        "Erreur d'importation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        /// <summary>
        /// méthode séparée pour tracer les jeux sélectionnés
        /// </summary>
        private void PlotGames()
        {
            PlotForm.Plot.Clear(); // vider avant de re-tracer

            // récupérer les jeux sélectionnés
            var selectedNames = Games.CheckedItems.Cast<string>().ToList();
            // récupérer les années selectionnées
            var selectedYears = Years.CheckedItems.Cast<int>().ToList();

            foreach (var name in selectedNames)
            {
                var games = gamesData
                    .Where(g => g.Name == name && selectedYears.Contains(g.Year))
                    .OrderBy(g => g.Year)
                    .ToArray();

                if (games.Length > 0)
                {
                    // convertir les années en DateTime OADate pour ScottPlot
                    double[] yearsAsDates = games.Select(g => new DateTime(g.Year, 1, 1).ToOADate()).ToArray();
                    double[] sales = games.Select(g => g.Sales).ToArray();

                    // ajouter la courbe
                    PlotForm.Plot.Add.Scatter(yearsAsDates, sales);
                }
            }

            // personnalisation
            PlotForm.Plot.Title("Video game sales by year"); // titre
            PlotForm.Plot.XLabel("Years");                   // label axe X
            PlotForm.Plot.YLabel("Sales");                   // label axe Y
            PlotForm.Plot.Axes.DateTimeTicksBottom();        // afficher les années correctement

            PlotForm.Refresh(); // refresh le graphique
        }

        /// <summary>
        /// declenché quand on change la selection des jeux
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectGames(object sender, EventArgs e)
        {
            PlotGames(); // appel méthode qui ajoute les jeux dans la liste
            SelectYears();
        }

        private void SelectYears()
        {
            SelectYears(this, EventArgs.Empty);
        }

        private void SelectYears(object sender, EventArgs e)
        {
            PlotGames();

            Years.Items.Clear(); // effacer

            gamesData
            .Select(g => g.Year)
            .Distinct()
            .OrderBy(y => y)
            .ToList()
            .ForEach(year => Years.Items.Add(year, true));

        }
        //sauvegarder les données
        private void SaveData()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("gamesData.txt"))
                {
                    foreach (var game in gamesData)
                    {
                        writer.WriteLine($"{game.Name},{game.Year},{game.Sales}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la sauvegarde : " + ex.Message);
            }
        }

        // charger les données lors du lancement
        private void LoadData()
        {
            if (!File.Exists("gamesData.txt")) return;

            //refaire la lecture des données et les afficher
            try
            {
                gamesData.Clear();
                string[] lines = File.ReadAllLines("gamesData.txt");

                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 3)
                    {
                        bool yearParsed = int.TryParse(parts[1], out int year);
                        bool salesParsed = double.TryParse(parts[2],
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out double sales);

                        if (yearParsed && salesParsed)
                        {
                            gamesData.Add(new GameData
                            {
                                Name = parts[0].Trim(), // effacer tous les espaces inutiles
                                Year = year,
                                Sales = sales
                            });
                        }
                    }
                }

                // populer la liste de jeux
                Games.Items.Clear();
                foreach (var name in gamesData.Select(g => g.Name).Distinct())
                    Games.Items.Add(name, true);

                // populer la list des annees
                Years.Items.Clear();
                foreach (var year in gamesData.Select(g => g.Year).Distinct().OrderBy(y => y))
                    Years.Items.Add(year, true);

                PlotGames();
            }
            catch (Exception ex)
            {
                // afficher lors d'une exception
                MessageBox.Show("Erreur lors du chargement : " + ex.Message);
            }
        }
    }
}
