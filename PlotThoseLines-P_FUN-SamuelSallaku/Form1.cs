using ScottPlot.TickGenerators.Financial;
using ScottPlot.WinForms;
using System.Threading.Channels;
using System.Windows.Forms;

namespace PlotThoseLines_P_FUN_SamuelSallaku
{
    public partial class Form1 : Form
    {
        // liste de jeux
        private List<GameData> gamesData = new List<GameData>();

        // classe des donnes
        private class GameData
        {
            public string Name;
            public int Year;
            public double Sales;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            formsPlot1.Plot.Clear(); // effacer
        }

        private void button1_Click(object sender, EventArgs e)
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

                // ajouter les noms des jeux à la liste (CheckedListBox)
                Years.Items.Clear();

                gamesData
                    .Select(g => g.Name)
                    .Distinct()
                    .ToList()
                    .ForEach(name => Years.Items.Add(name, true));

                // plotter les données importees
                PlotGames();

                formsPlot1.Refresh();
            }
        }

        // méthode séparée pour tracer les jeux sélectionnés
        private void PlotGames()
        {
            formsPlot1.Plot.Clear(); // vider avant de re-tracer

            // récupérer les jeux sélectionnés
            var selectedNames = Years.CheckedItems.Cast<string>().ToList();

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
                    formsPlot1.Plot.Add.Scatter(yearsAsDates, sales);
                }
            }

            // configurer le graphique
            formsPlot1.Plot.Title("Video game sales by year"); // titre
            formsPlot1.Plot.XLabel("Years");                   // label axe X
            formsPlot1.Plot.YLabel("Sales");                   // label axe Y
            formsPlot1.Plot.Axes.DateTimeTicksBottom();        // afficher les années correctement

            formsPlot1.Refresh(); // rafraîchir le graphique
        }

        // event déclenché quand on change les jeux sélectionnés
        private void Years_SelectedIndexChanged(object sender, EventArgs e)
        {
            PlotGames(); // utiliser la méthode commune pour tracer
        }
    }
}
