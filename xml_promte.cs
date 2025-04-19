/*
<Canvas Name="myCanvas" Background="LightGray" Width="580" Height="320" Margin="0,-60,0,0"/>

<Button Content="Add Circle" Click="AddCircle_Click" Width="100" Height="30"
        HorizontalAlignment="Left" VerticalAlignment="Bottom" Margin="190,0,0,20"/>

<Button Content="Delete Circle" Click="DeleteCircle_Click" Width="100" Height="30"
        HorizontalAlignment="center" VerticalAlignment="Bottom" Margin="0,0,0,20" />

<Button Content="Circle Co-Ordinates" Click="GetCircleCoordinates_Click" Width="130" Height="30"
        HorizontalAlignment="Right" VerticalAlignment="Bottom" Margin="0,0,160,20"/>
given to me code for C# for given xml for WPF to create circle on clicking add buttom,
user can able to move on the screen using mouse, if circles touches each on any side of them, 
it should not allow the user to move over circles and include this condition to code for those
circles should not cross the myCanvas get display the circles cooridinates, and 
add left, right, top, bottom key movements for selected circles for fine adjustment
*/

<Canvas Name="myCanvas" Background="LightGray" Width="580" Height="350"/>
<Button Content="Add Circle" Click="AddCircle_Click" Width="100" Height="30" VerticalAlignment="Top"/>
<Button Content="Circle Co-Ordinates" Click="GetCircleCoordiates_Click" Width="100" Height="30" VerticalAlignment="Bottom"/>

# region Code for Circle Collision from Chat GPT
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
        private const double CircleRadius = 30;
        private Ellipse selectedCircle = null;
        private Point lastPosition;
        private List<Ellipse> circles = new List<Ellipse>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddCircle_Click(object sender, RoutedEventArgs e)
        {
            Ellipse circle = new Ellipse
            {
                Width = CircleRadius * 2,
                Height = CircleRadius * 2,
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            Canvas.SetLeft(circle, 50 + circles.Count * 70);
            Canvas.SetTop(circle, 50);
            myCanvas.Children.Add(circle);
            circles.Add(circle);

            circle.MouseLeftButtonDown += Circle_MouseLeftButtonDown;
            circle.MouseMove += Circle_MouseMove;
            circle.MouseLeftButtonUp += Circle_MouseLeftButtonUp;
        }

        private void Circle_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedCircle != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPosition = e.GetPosition(myCanvas);
                double offsetX = currentPosition.X - lastPosition.X;
                double offsetY = currentPosition.Y - lastPosition.Y;

                double left = Canvas.GetLeft(selectedCircle) + offsetX;
                double top = Canvas.GetTop(selectedCircle) + offsetY;

                Rect newRect = new Rect(left, top, selectedCircle.Width, selectedCircle.Height);

                bool isColliding = false;

                foreach (var circle in circles)
                {
                    if (circle != selectedCircle)
                    {
                        Rect otherRect = new Rect(
                            Canvas.GetLeft(circle),
                            Canvas.GetTop(circle),
                            circle.Width,
                            circle.Height
                        );
                        if (newRect.IntersectsWith(otherRect))
                        {
                            isColliding = true;
                            break;
                        }
                    }
                }
                if (!isColliding)
                {
                    Canvas.SetLeft(selectedCircle, left);
                    Canvas.SetTop(selectedCircle, top);
                    lastPosition = currentPosition;
                }
            }
        }
        private void Circle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedCircle = sender as Ellipse;
            lastPosition = e.GetPosition(myCanvas);
            selectedCircle.CaptureMouse();
        }
        private void Circle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedCircle != null)
            {
                selectedCircle.ReleaseMouseCapture();
                selectedCircle = null;
            }
        }
    }
}
#endregion

#region Code from BING Co
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace CircleMovementApp
{
    public partial class MainWindow : Window
    {
        private Ellipse selectedCircle = null;
        private Point lastPosition;
        private List<Ellipse> circles = new List<Ellipse>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddCircle_Click(object sender, RoutedEventArgs e)
        {
            Ellipse circle = new Ellipse
            {
                Width = 50,
                Height = 50,
                Fill = Brushes.Blue
            };

            Canvas.SetLeft(circle, 50);
            Canvas.SetTop(circle, 50);

            circle.MouseLeftButtonDown += Circle_MouseLeftButtonDown;
            circle.MouseMove += Circle_MouseMove;
            circle.MouseLeftButtonUp += Circle_MouseLeftButtonUp;

            circles.Add(circle);
            myCanvas.Children.Add(circle);
        }

        private void Circle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedCircle = sender as Ellipse;
            lastPosition = e.GetPosition(myCanvas);
            selectedCircle.CaptureMouse();
        }

        private void Circle_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedCircle != null)
            {
                Point newPosition = e.GetPosition(myCanvas);
                double offsetX = newPosition.X - lastPosition.X;
                double offsetY = newPosition.Y - lastPosition.Y;

                double newLeft = Canvas.GetLeft(selectedCircle) + offsetX;
                double newTop = Canvas.GetTop(selectedCircle) + offsetY;

                // Check collision
                if (!IsColliding(newLeft, newTop, selectedCircle))
                {
                    Canvas.SetLeft(selectedCircle, newLeft);
                    Canvas.SetTop(selectedCircle, newTop);
                    lastPosition = newPosition;
                }
            }
        }

        private void Circle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedCircle != null)
            {
                selectedCircle.ReleaseMouseCapture();
                selectedCircle = null;
            }
        }

        private bool IsColliding(double x, double y, Ellipse movingCircle)
        {
            Rect movingRect = new Rect(x, y, movingCircle.Width, movingCircle.Height);

            foreach (var circle in circles)
            {
                if (circle == movingCircle) continue;

                Rect circleRect = new Rect(Canvas.GetLeft(circle), Canvas.GetTop(circle), circle.Width, circle.Height);
                if (movingRect.IntersectsWith(circleRect))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

#endregion

#region In VS Code
 public partial class MainWindow : Window
    {
        private const double CircleRadius = 30;
        private Ellipse selectedCircle = null;
        private Point lastPosition;
        private List<Ellipse> circles = new List<Ellipse>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddCircle_Click(object sender, RoutedEventArgs e)
        {
            Ellipse circle = new Ellipse
            {
                Width = CircleRadius * 2,
                Height = CircleRadius * 2,
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            Canvas.SetLeft(circle, 50 + circles.Count * 70);
            Canvas.SetTop(circle, 50);
            myCanvas.Children.Add(circle);
            circles.Add(circle);

            circle.MouseLeftButtonDown += Circle_MouseLeftButtonDown;
            circle.MouseMove += Circle_MouseMove;
            circle.MouseLeftButtonUp += Circle_MouseLeftButtonUp;
        }

        private void Circle_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedCircle != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPosition = e.GetPosition(myCanvas);
                double offsetX = currentPosition.X - lastPosition.X;
                double offsetY = currentPosition.Y - lastPosition.Y;

                double left = Canvas.GetLeft(selectedCircle) + offsetX;
                double top = Canvas.GetTop(selectedCircle) + offsetY;

                Rect newRect = new Rect(left, top, selectedCircle.Width, selectedCircle.Height);

                bool isColliding = false;

                foreach (var circle in circles)
                {
                    if (circle != selectedCircle)
                    {
                        Rect otherRect = new Rect(
                            Canvas.GetLeft(circle),
                            Canvas.GetTop(circle),
                            circle.Width,
                            circle.Height
                        );
                        if (newRect.IntersectsWith(otherRect))
                        {
                            isColliding = true;
                            break;
                        }
                    }
                }
                if (!isColliding)
                {
                    Canvas.SetLeft(selectedCircle, left);
                    Canvas.SetTop(selectedCircle, top);
                    lastPosition = currentPosition;
                }
            }
        }
        private void Circle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedCircle = sender as Ellipse;
            lastPosition = e.GetPosition(myCanvas);
            selectedCircle.CaptureMouse();
        }
        private void Circle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedCircle != null)
            {
                selectedCircle.ReleaseMouseCapture();
                selectedCircle = null;
            }
        }
    }
#endregion

#region circles not out of Canvas

<Canvas Name="myCanvas" Background="LightGray" Width="580" Height="320" Margin="0,-60,0,0"/>

<Button Content="Add Circle" Click="AddCircle_Click" Width="100" Height="30"
        HorizontalAlignment="Left" VerticalAlignment="Bottom" Margin="260,0,0,20"/>

<Button Content="Circle Co-Ordinates" Click="GetCircleCoordinates_Click" Width="130" Height="30"
        HorizontalAlignment="Right" VerticalAlignment="Bottom" Margin="0,0,260,20"/>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;

namespace CircleApp
{
    public partial class MainWindow : Window
    {
        private Ellipse draggedEllipse = null;
        private Point mouseOffset;
        private const double CircleRadius = 30;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddCircle_Click(object sender, RoutedEventArgs e)
        {
            Ellipse circle = new Ellipse
            {
                Width = CircleRadius * 2,
                Height = CircleRadius * 2,
                Fill = Brushes.Blue,
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

        private void Circle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedEllipse = sender as Ellipse;
            mouseOffset = e.GetPosition(myCanvas);
            mouseOffset.X -= Canvas.GetLeft(draggedEllipse);
            mouseOffset.Y -= Canvas.GetTop(draggedEllipse);
            draggedEllipse.CaptureMouse();
        }

        private void Circle_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedEllipse != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePos = e.GetPosition(myCanvas);
                double newX = mousePos.X - mouseOffset.X;
                double newY = mousePos.Y - mouseOffset.Y;

                // Boundary Check
                if (newX < 0 || newY < 0 || newX + CircleRadius * 2 > myCanvas.ActualWidth || newY + CircleRadius * 2 > myCanvas.ActualHeight)
                    return;

                // Collision Check
                foreach (UIElement element in myCanvas.Children)
                {
                    if (element is Ellipse ellipse && ellipse != draggedEllipse)
                    {
                        double otherX = Canvas.GetLeft(ellipse);
                        double otherY = Canvas.GetTop(ellipse);

                        double dx = (newX + CircleRadius) - (otherX + CircleRadius);
                        double dy = (newY + CircleRadius) - (otherY + CircleRadius);
                        double distance = Math.Sqrt(dx * dx + dy * dy);

                        if (distance < CircleRadius * 2)
                            return; // collision, do not allow move
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

        private void GetCircleCoordinates_Click(object sender, RoutedEventArgs e)
        {
            string result = "Circle Coordinates:\n";
            foreach (UIElement element in myCanvas.Children)
            {
                if (element is Ellipse ellipse)
                {
                    double x = Canvas.GetLeft(ellipse);
                    double y = Canvas.GetTop(ellipse);
                    result += $"Circle at (X: {x:F1}, Y: {y:F1})\n";
                }
            }

            MessageBox.Show(result, "Coordinates");
        }
    }
}


# endregion

#region Keyboard movement Code

<Window x:Class="CircleApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Circle App" Height="400" Width="600"
        KeyDown="Window_KeyDown"
        FocusManager.FocusedElement="{Binding RelativeSource={RelativeSource Self}}">
    <Grid>
        <Canvas Name="myCanvas" Background="LightGray" Width="580" Height="320" Margin="0,-60,0,0"/>
        
        <Button Content="Add Circle" Click="AddCircle_Click" Width="100" Height="30"
                HorizontalAlignment="Left" VerticalAlignment="Bottom" Margin="190,0,0,20"/>

        <Button Content="Delete Circle" Click="DeleteCircle_Click" Width="100" Height="30"
                HorizontalAlignment="Center" VerticalAlignment="Bottom" Margin="0,0,0,20"/>

        <Button Content="Circle Co-Ordinates" Click="GetCircleCoordinates_Click" Width="130" Height="30"
                HorizontalAlignment="Right" VerticalAlignment="Bottom" Margin="0,0,160,20"/>
    </Grid>
</Window>



using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;

namespace CircleApp
{
    public partial class MainWindow : Window
    {
        private Ellipse selectedCircle = null;
        private Point clickPosition;
        private bool isDragging = false;
        private const double CircleDiameter = 50;
        private List<Ellipse> circles = new List<Ellipse>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddCircle_Click(object sender, RoutedEventArgs e)
        {
            Ellipse circle = new Ellipse
            {
                Width = CircleDiameter,
                Height = CircleDiameter,
                Fill = Brushes.SkyBlue,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            Canvas.SetLeft(circle, 10);
            Canvas.SetTop(circle, 10);

            circle.MouseLeftButtonDown += Circle_MouseLeftButtonDown;
            circle.MouseMove += Circle_MouseMove;
            circle.MouseLeftButtonUp += Circle_MouseLeftButtonUp;

            circles.Add(circle);
            myCanvas.Children.Add(circle);
        }

        private void DeleteCircle_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCircle != null)
            {
                myCanvas.Children.Remove(selectedCircle);
                circles.Remove(selectedCircle);
                selectedCircle = null;
            }
        }

        private void GetCircleCoordinates_Click(object sender, RoutedEventArgs e)
        {
            foreach (var circle in circles)
            {
                double x = Canvas.GetLeft(circle);
                double y = Canvas.GetTop(circle);
                MessageBox.Show($"Circle at X: {x}, Y: {y}");
            }
        }

        private void Circle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedCircle = sender as Ellipse;
            clickPosition = e.GetPosition(myCanvas);
            isDragging = true;
            selectedCircle.CaptureMouse();
        }

        private void Circle_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && selectedCircle != null)
            {
                Point currentPosition = e.GetPosition(myCanvas);
                double offsetX = currentPosition.X - clickPosition.X;
                double offsetY = currentPosition.Y - clickPosition.Y;

                double newX = Canvas.GetLeft(selectedCircle) + offsetX;
                double newY = Canvas.GetTop(selectedCircle) + offsetY;

                if (CanMoveTo(selectedCircle, newX, newY))
                {
                    Canvas.SetLeft(selectedCircle, newX);
                    Canvas.SetTop(selectedCircle, newY);
                    clickPosition = currentPosition;
                }
            }
        }

        private void Circle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            if (selectedCircle != null)
                selectedCircle.ReleaseMouseCapture();
        }

        private bool CanMoveTo(Ellipse movingCircle, double x, double y)
        {
            Rect newRect = new Rect(x, y, CircleDiameter, CircleDiameter);

            // Check canvas bounds
            if (x < 0 || y < 0 || x + CircleDiameter > myCanvas.ActualWidth || y + CircleDiameter > myCanvas.ActualHeight)
                return false;

            // Check collision with other circles
            foreach (var circle in circles)
            {
                if (circle == movingCircle) continue;

                double otherX = Canvas.GetLeft(circle);
                double otherY = Canvas.GetTop(circle);
                Rect otherRect = new Rect(otherX, otherY, CircleDiameter, CircleDiameter);

                if (newRect.IntersectsWith(otherRect))
                    return false;
            }

            return true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (selectedCircle == null) return;

            double currentX = Canvas.GetLeft(selectedCircle);
            double currentY = Canvas.GetTop(selectedCircle);

            double moveStep = 2;

            double newX = currentX;
            double newY = currentY;

            switch (e.Key)
            {
                case Key.Left:
                    newX -= moveStep;
                    break;
                case Key.Right:
                    newX += moveStep;
                    break;
                case Key.Up:
                    newY -= moveStep;
                    break;
                case Key.Down:
                    newY += moveStep;
                    break;
                default:
                    return;
            }

            if (CanMoveTo(selectedCircle, newX, newY))
            {
                Canvas.SetLeft(selectedCircle, newX);
                Canvas.SetTop(selectedCircle, newY);
            }
        }
    }
}
#endregion