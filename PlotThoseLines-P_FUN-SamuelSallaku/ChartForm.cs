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

        // classe des donn�es
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

                // effacer la liste
                Games.Items.Clear();

                // ajouter le nom du jeu dans la liste
                gamesData
                    .Select(g => g.Name)
                    .Distinct()
                    .ToList()
                    .ForEach(name => Games.Items.Add(name, true));

                // appel methode pour ajouter les donn�es
                PlotGames();

                formsPlot1.Refresh();
            }
        }

        /// <summary>
        /// m�thode s�par�e pour tracer les jeux s�lectionn�s
        /// </summary>
        private void PlotGames()
        {
            formsPlot1.Plot.Clear(); // vider avant de re-tracer

            // r�cup�rer les jeux s�lectionn�s
            var selectedNames = Games.CheckedItems.Cast<string>().ToList();

            foreach (var name in selectedNames)
            {
                var games = gamesData
                    .Where(g => g.Name == name)
                    .OrderBy(g => g.Year)
                    .ToArray();

                if (games.Length > 0)
                {
                    // convertir les ann�es en DateTime OADate pour ScottPlot
                    double[] yearsAsDates = games.Select(g => new DateTime(g.Year, 1, 1).ToOADate()).ToArray();
                    double[] sales = games.Select(g => g.Sales).ToArray();

                    // ajouter la courbe
                    formsPlot1.Plot.Add.Scatter(yearsAsDates, sales);
                }
            }

            // personnalisation
            formsPlot1.Plot.Title("Video game sales by year"); // titre
            formsPlot1.Plot.XLabel("Years");                   // label axe X
            formsPlot1.Plot.YLabel("Sales");                   // label axe Y
            formsPlot1.Plot.Axes.DateTimeTicksBottom();        // afficher les ann�es correctement

            formsPlot1.Refresh(); // refresh le graphique
        }

        /// <summary>
        /// declench� quand on change la selection des jeux
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Years_SelectedIndexChanged(object sender, EventArgs e)
        {
            PlotGames(); // appel m�thode qui ajoute les jeux dans la liste
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
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
