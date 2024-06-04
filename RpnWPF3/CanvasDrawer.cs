using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RpnLogic;
using System.Windows.Documents;
using System.CodeDom;
using System.Xml.Linq;





namespace RpnWPF3
{

    static class PointExtensions
    {
        public static Point ToMathCoordinates(this Point point, Canvas canvas, float zoom)
        {
            return new Point((point.X - canvas.ActualWidth / 2) / zoom, (canvas.ActualHeight / 2 - point.Y) / zoom);
        }

        public static Point ToUiCoordinates(this Point point, Canvas canvas, float zoom)
        {
            return new Point((point.X * zoom + canvas.ActualWidth / 2), (canvas.ActualHeight / 2 - point.Y * zoom));
        }
    }



    public class CanvasDrawer
    {
        private readonly Canvas _canvas;
        private readonly double _start;
        private readonly double _end;
        private readonly double dashSize = 5;
        private readonly double _step;
        private readonly float _scale;
        private readonly Brush _lineGraph = Brushes.Blue;
        private readonly Brush _pointBrush = Brushes.Red;
        private readonly Point _lineStartX, _lineEndX, _lineStartY, _lineEndY;

        public CanvasDrawer(Canvas canvas, double startX, double endX, double step, float scale)
        {
            _canvas = canvas;

            _lineStartX = new Point(_canvas.ActualWidth / 2, 0);
            _lineEndX = new Point(_canvas.ActualWidth / 2, _canvas.ActualHeight);

            _lineStartY = new Point(0, _canvas.ActualHeight / 2);
            _lineEndY = new Point(_canvas.ActualWidth, _canvas.ActualHeight / 2);

            _scale = scale;
            _step = step;
            _start = startX;
            _end = endX;
        }

        public void DrawLine(Point startPoint, Point endPoint, Brush stroke = null)
        {
            stroke ??= _lineGraph;
            Line line = new Line
            {
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y,
                Stroke = stroke,
                StrokeThickness = 1
            };

            _canvas.Children.Add(line);
        }

        public void DrawAxes()
        {
            DrawLine(_lineStartX, _lineEndX, _lineGraph);
            DrawLine(_lineStartY, _lineEndY, _lineGraph);
        }

        public void DrawGraph(List<Point> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                var uiPoint1 = points[i].ToUiCoordinates(_canvas, _scale);
                var uiPoint2 = points[i + 1].ToUiCoordinates(_canvas, _scale);
                DrawLine(uiPoint1, uiPoint2);
                DrawPoint(uiPoint1);
                DrawDashLine(points[i]);
            }
            if (points.Count > 0)
            {
                var lastUiPoint = points[points.Count-1].ToUiCoordinates(_canvas, _scale);
                DrawPoint(lastUiPoint);
                DrawDashLine(points[points.Count - 1]);
            }
        }

        public void DrawDashLine(Point point)
        {
            
            var uiPoint = point.ToUiCoordinates(_canvas, _scale);
            DrawLine(new Point(uiPoint.X, _canvas.ActualHeight / 2 - dashSize), new Point(uiPoint.X, _canvas.ActualHeight / 2 + dashSize));
            DrawLine(new Point(_canvas.ActualWidth / 2 - dashSize, uiPoint.Y), new Point(_canvas.ActualWidth / 2 + dashSize, uiPoint.Y));
        }
        public void DrawPoint(Point point, double size = 4)
        {
            Ellipse ellipse = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = _pointBrush
            };

            Canvas.SetLeft(ellipse, point.X - size / 2);
            Canvas.SetTop(ellipse, point.Y - size / 2);

            _canvas.Children.Add(ellipse);
        }
       

       


    }
}







