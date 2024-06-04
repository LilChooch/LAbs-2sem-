using RpnLogic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RpnWPF3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CanvasGraph.Children.Clear();
            
            DrawCanva();
        }
        private void DrawCanva()
        {
            string input = expressoiN.Text;
            float scale = float.Parse(tbscale.Text);
            double start = double.Parse(tbstart.Text);
            double end = double.Parse(tbend.Text);
            double step = double.Parse(tbstep.Text);
            var canvsGraph = CanvasGraph;

            var canvDrawer = new CanvasDrawer(canvsGraph, start, end, step, scale);
            canvDrawer.DrawAxes();

            List<Point> points = new List<Point>();
            for(double x = start; x <= end; x+=step)
            {
                var y = new ExpRpn(input, x).Value;
                points.Add(new Point(x, y));
            }
            canvDrawer.DrawGraph(points);

        }

        private void CanvasGraph_MouseMove(object sender, MouseEventArgs e)
        {
            Point uiPoint = Mouse.GetPosition(CanvasGraph);
            float scale;
            if (string.IsNullOrWhiteSpace(tbscale.Text) || !float.TryParse(tbscale.Text, out scale))
            {
                scale = 1.0f;
                tbscale.Text = scale.ToString();
            }

            var mathPoint = uiPoint.ToMathCoordinates(CanvasGraph, scale);
            lbUiPoint.Content = $"{uiPoint.X:0.#};{uiPoint.Y:0.#}";
            lbMathPoint.Content = $"{mathPoint.X:0.#};{mathPoint.Y:0.#}";


        }

       
    }
}