using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace Snake_Wave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        System.Windows.Threading.DispatcherTimer _timer;
        Snake _snake;


        public MainWindow()
        {
            InitializeComponent();

            _snake = new Snake(20, (int)g.Width, (int)g.Height);

            // Create a timer for the GameLoop method
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Tick += new EventHandler(GameLoop);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
        }


        private void GameLoop(object sender, System.EventArgs e)
        {
            _snake.Update(_timer);
            _snake.Draw(g);
        }


        /// <summary>
        /// СОБЫТИЯ
        /// </summary>

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            //_snake.Reset();  // Инициализация переменных для повторного старта
            _timer.Start();
        }
    }
}
