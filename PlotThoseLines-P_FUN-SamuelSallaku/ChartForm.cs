using ScottPlot.TickGenerators.Financial;
using ScottPlot.WinForms;
using System.Threading.Channels;
using System.Windows.Forms;

namespace PlotThoseLines_P_FUN_SamuelSallaku
{
    public partial class ChartForm : Form
    {
        // liste de jeux
        private List<GameData> gamesData = new List<GameData>();

        // classe des données
        private class GameData
        {
            public string Name;
            public int Year;
            public double Sales;
        }

        public ChartForm()
        {
            InitializeComponent();
        }

        private void LoadPlotForm(object sender, EventArgs e)
        {
            PlotForm.Plot.Clear(); // effacer
        }

        private void ImportCSV(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Import Data";
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(fileName);

                // parser le CSV en objets GameData
                gamesData = lines
                    .Skip(1) // skip la premiere ligne
                    .Where(line => !string.IsNullOrWhiteSpace(line)) // ignorer lignes vide
                    .Select(line => line.Split(',')) // separation des virgules
                    .Where(p => p.Length >= 3) // verifier quela ligne ait au moins 3 donnees
                    .Select(gamedata => new GameData
                    {
                        Name = gamedata[0].Trim(), //effacer espacements
                        Year = int.Parse(gamedata[1]), //transforme en numero le string
                        Sales = double.Parse(gamedata[2]) // " "
                    })
                    .ToList();

                // effacer la liste
                Games.Items.Clear();

                // ajouter le nom du jeu dans la liste
                gamesData
                    .Select(g => g.Name)
                    .Distinct()
                    .ToList()
                    .ForEach(name => Games.Items.Add(name, true));

                // appel methode pour ajouter les données
                PlotGames();

                PlotForm.Refresh();
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

            foreach (var name in selectedNames)
            {
                var games = gamesData
                    .Where(g => g.Name == name)
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
        }

        private void SelectYears(object sender, EventArgs e)
        {
            Years.Items.Clear();
            gamesData
            .Select(g => g.Year)
            .Distinct()
            .OrderBy(y => y)
            .ToList()
            .ForEach(year => Years.Items.Add(year, true));

        }
    }
}
