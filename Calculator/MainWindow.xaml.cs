using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected int dragMoveThresh = 20;
        private string expression;
        private Storyboard storyboard;

        public MainWindow()
        {
            InitializeComponent();
        }

        /*
         * ExitButton events
         */

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        /*
         * MainWindow1 events
         */

        private void MainWindow1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Point mPoints = Mouse.GetPosition(MainWindow1);
                if (mPoints.Y <= dragMoveThresh)
                    this.DragMove();
            }
        }

        /******************
         * Computing logic
         *****************/

        private void AppendSymbolFromButton(object sender, RoutedEventArgs e)
        {
            Button sourceBtn = (Button)sender;
            string symbol = sourceBtn.Content.ToString();

            expression = expression == "0" ? symbol : $"{expression}{symbol}";
            this.SyncExprDisplay();
        }

        private void SyncExprDisplay()
        {
            int fullExprLen = expression.Length;
            Debug.WriteLine(expression);
            if (fullExprLen < 10)
            {
                ExprField.Text = expression;
                this.AnimateMoreText("out");
            }
            else
            {
                string partialExpr = expression.Substring(expression.Length - 9);
                ExprField.Text = partialExpr;
                this.AnimateMoreText("in");
            }
        }

        private void EvaluateFromBtn(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            var result = dt.Compute(expression, "");
            expression = result.ToString();
            this.SyncExprDisplay();
        }

        private void RemoveLastSymbolFromExp(object sender, RoutedEventArgs e)
        {
            expression = expression.Remove(expression.Length - 1);
            if (expression.Length == 0) expression = "0";
            this.SyncExprDisplay();
        }

        /******************
         * Animators
         *****************/

        private void AnimateMoreText(string type, double animDuration = 500)
        {
            var opacityAnimation = new DoubleAnimation();
            var rightAnimation = new DoubleAnimation();

            if(type == "in")
            {
                opacityAnimation.To = 1.0d;
            } else if (type == "out")
            {
                opacityAnimation.To = 0.0d;
            } else
            {
                throw new Exception("Unknown animation state: " + type);
            }

            opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animDuration));

            storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);
            Storyboard.SetTargetName(opacityAnimation, MoreTextInExpr.Name);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(TextBlock.OpacityProperty));
            storyboard.Begin(this);
        }
    }
}
