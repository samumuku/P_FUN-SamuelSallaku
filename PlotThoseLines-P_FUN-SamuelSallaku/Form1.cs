using ScottPlot.WinForms;
using System.Threading.Channels;

namespace PlotThoseLines_P_FUN_SamuelSallaku
{
    public partial class Form1 : Form
    {
        private ScottPlot.Plottables.DataLogger Logger1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            createChart();
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

                foreach (var line in lines)
                {
                    //continue mme si cest un espace vide
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // diviser par la virgule
                    string[] parts = line.Split(',');

                    // toutes les colonnes se transforment en double
                    List<double> rowValues = new List<double>();
                    foreach (var part in parts)
                    {
                        if (double.TryParse(part, out double val))
                            rowValues.Add(val);
                    }

                    // ajouter si ya une valeur detecté
                    if (rowValues.Count > 0)
                        Logger1.Add(rowValues.ToArray());
                }


                // refresh des que la boucle est finie
                formsPlot1.Refresh();
                formsPlot1.Plot.XLabel("Year");
                formsPlot1.Plot.YLabel("Sales");
            }
        }

        private void createChart()
        {
            Logger1 = formsPlot1.Plot.Add.DataLogger();
            Logger1.LineWidth = 2;
        }

        private void Years_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
