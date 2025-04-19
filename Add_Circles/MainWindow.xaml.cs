using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Add_Circles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Ellipse draggedEllipse = null;
        private Point mouseOffset;
        private Ellipse selectedCircle = null;

        private bool isDragging = false;
        private double offsetX;

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += MainWindow_KeyDown;
            myCanvas.MouseLeftButtonDown += MyCanvas_MouseLeftButtonDown;
            myCanvas.MouseRightButtonDown += DeleteCircle_MouseRightButtonDown;

            // Keep the line on top of everything else
            Canvas.SetZIndex(verticalLine, int.MaxValue);
        }

        #region Vertical Line Movement Code
        private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            Point mousePosition = e.GetPosition(myCanvas);

            // Store the offset between the mouse click and the line's X1
            offsetX = mousePosition.X - verticalLine.X1;

            // Capture mouse input so we continue getting move events even if mouse leaves the line
            verticalLine.CaptureMouse();
        }

        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging) return;

            Point mousePosition = e.GetPosition(myCanvas);
            double newX = mousePosition.X - offsetX;

            // Clamp line within canvas bounds
            newX = Math.Max(0, Math.Min(myCanvas.ActualWidth, newX));

            // Only update X1 and X2 to keep it vertical
            verticalLine.X1 = newX;
            verticalLine.X2 = newX;
        }

        private void Line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            verticalLine.ReleaseMouseCapture();
        }
        #endregion

        private void AddCircle_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(CircleDiameter.Text, out double diameter) || diameter <= 0)
            {
                MessageBox.Show("Enter a valid diameter.");
                return;
            }

            Ellipse circle = new Ellipse
            {
                Width = diameter,
                Height = diameter,
                Fill = Brushes.SkyBlue,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            double x = 50 + new Random().Next(0, (int)(myCanvas.ActualWidth - 100));
            double y = 50 + new Random().Next(0, (int)(myCanvas.ActualHeight - 100));

            Canvas.SetLeft(circle, x);
            Canvas.SetTop(circle, y);

            circle.MouseLeftButtonDown += Circle_MouseLeftButtonDown;
            circle.MouseMove += Circle_MouseMove;
            circle.MouseLeftButtonUp += Circle_MouseLeftButtonUp;         

            myCanvas.Children.Add(circle);
        }

        #region Circle Movement
        private void Circle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // Prevent canvas click event

            draggedEllipse = sender as Ellipse;

            // Deselect previous
            DeselectCircle();

            selectedCircle = draggedEllipse;
            ApplyGlowEffect(selectedCircle);

            mouseOffset = e.GetPosition(myCanvas);
            mouseOffset.X -= Canvas.GetLeft(draggedEllipse);
            mouseOffset.Y -= Canvas.GetTop(draggedEllipse);

            SelectCircle(draggedEllipse);
            draggedEllipse.CaptureMouse();
        }

        private void Circle_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedEllipse != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePos = e.GetPosition(myCanvas);
                double newX = mousePos.X - mouseOffset.X;
                double newY = mousePos.Y - mouseOffset.Y;

                double radius = draggedEllipse.Width / 2;

                // Boundary Check
                if (newX < 0 || newY < 0 ||
                    newX + draggedEllipse.Width > myCanvas.ActualWidth ||
                    newY + draggedEllipse.Height > myCanvas.ActualHeight)
                    return;

                // Collision Check
                foreach (UIElement element in myCanvas.Children)
                {
                    if (element is Ellipse ellipse && ellipse != draggedEllipse)
                    {
                        double otherX = Canvas.GetLeft(ellipse);
                        double otherY = Canvas.GetTop(ellipse);
                        double dx = (newX + radius) - (otherX + ellipse.Width / 2);
                        double dy = (newY + radius) - (otherY + ellipse.Height / 2);
                        double distance = Math.Sqrt(dx * dx + dy * dy);

                        if (distance < radius + ellipse.Width / 2)
                            return;
                    }
                }

                Canvas.SetLeft(draggedEllipse, newX);
                Canvas.SetTop(draggedEllipse, newY);
            }
        }

        private void Circle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedEllipse != null)
            {
                draggedEllipse.ReleaseMouseCapture();
                draggedEllipse = null;
            }
        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Deselect on canvas click
            SelectCircle(null);
        }

        private void DeleteCircle_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Ellipse ellipse)
            {
                myCanvas.Children.Remove(ellipse);
            }
        }

        private void SelectCircle(Ellipse circle)
        {
            if (selectedCircle != null)
                selectedCircle.Effect = null;

            selectedCircle = circle;

            if (selectedCircle != null)
            {
                selectedCircle.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Yellow,
                    BlurRadius = 20,
                    ShadowDepth = 0
                };
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (selectedCircle == null) return;

            double moveStep = 2;
            double left = Canvas.GetLeft(selectedCircle);
            double top = Canvas.GetTop(selectedCircle);
            double radius = selectedCircle.Width / 2;

            double newX = left, newY = top;

            if (e.Key == Key.Left) newX -= moveStep;
            if (e.Key == Key.Right) newX += moveStep;
            if (e.Key == Key.Up) newY -= moveStep;
            if (e.Key == Key.Down) newY += moveStep;

            // Boundary Check
            if (newX < 0 || newY < 0 ||
                newX + selectedCircle.Width > myCanvas.ActualWidth ||
                newY + selectedCircle.Height > myCanvas.ActualHeight)
                return;

            // Collision Check
            foreach (UIElement element in myCanvas.Children)
            {
                if (element is Ellipse ellipse && ellipse != selectedCircle)
                {
                    double otherX = Canvas.GetLeft(ellipse);
                    double otherY = Canvas.GetTop(ellipse);
                    double dx = (newX + radius) - (otherX + ellipse.Width / 2);
                    double dy = (newY + radius) - (otherY + ellipse.Height / 2);
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    if (distance < radius + ellipse.Width / 2)
                        return;
                }
            }

            Canvas.SetLeft(selectedCircle, newX);
            Canvas.SetTop(selectedCircle, newY);
        }
        #endregion

        #region Canvas Zoom In Zoom Out Code
        private void MyCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomSpeed = 0.001;
            double scale = e.Delta > 0 ? (1 + zoomSpeed * e.Delta) : (1 + zoomSpeed * e.Delta);

            double newScaleX = canvasScale.ScaleX * scale;
            double newScaleY = canvasScale.ScaleY * scale;

            // Limit scale between 0.2 and 5.0
            if (newScaleX < 0.2 || newScaleX > 5.0) return;

            // Zoom around mouse position
            Point mousePos = e.GetPosition(myCanvas);
            double absX = mousePos.X * canvasScale.ScaleX + canvasTranslate.X;
            double absY = mousePos.Y * canvasScale.ScaleY + canvasTranslate.Y;

            canvasScale.ScaleX = newScaleX;
            canvasScale.ScaleY = newScaleY;

            canvasTranslate.X = absX - mousePos.X * newScaleX;
            canvasTranslate.Y = absY - mousePos.Y * newScaleY;
        }

        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            canvasScale.ScaleX = 1;
            canvasScale.ScaleY = 1;
            canvasTranslate.X = 0;
            canvasTranslate.Y = 0;
        }
        #endregion

        #region Highlight Selected Circle
        private void ApplyGlowEffect(Ellipse circle)
        {           
            var glow = new DropShadowEffect
            {
                Color = Colors.Yellow,
                BlurRadius = circle.Width + 20,
                ShadowDepth = 0,
                Opacity = 1
            };
            circle.Effect = glow;
        }
        private void DeselectCircle()
        {
            if (selectedCircle != null)
            {
                selectedCircle.Effect = null;
                selectedCircle = null;
            }
        }
        #endregion

        #region Get Circle Details
        private void GetCircleCoordinates_Click(object sender, RoutedEventArgs e)
        {
            string result = "Line Coordinates:\n";
            result += $"line X Position: {verticalLine.X1:F1}\n";

            result += "Circle Coordinates:\n";

            foreach (UIElement element in myCanvas.Children)
            {
                if (element is Ellipse ellipse)
                {
                    double x = Canvas.GetLeft(ellipse) + (ellipse.Width / 2);
                    double y = Canvas.GetTop(ellipse) + (ellipse.Width / 2);

                    double transformedX = GetCircle_X_CoordinateWithRefLine(x);

                    // Transform Y-coordinate to make (0,0) the bottom-left
                    double transformedY = myCanvas.ActualHeight - y;

                    result += $"(X: {x:F1}, X Trans: {transformedX:F1}, Y: {transformedY:F1}, Dia: {ellipse.Width:F1})\n";
                }
            }
            MessageBox.Show(result, "Coordinates");
        }

        private double GetCircle_X_CoordinateWithRefLine(double circle_X)
        {
            double result_X = 0;

            if (circle_X < verticalLine.X1)
                result_X = circle_X - verticalLine.X1;
            else
                result_X = circle_X - verticalLine.X1;

            return result_X;
        }
        #endregion

    }
}
