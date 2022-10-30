using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Snake_Wave
{
    class Snake
    {
        public static int[,] _map;  // карта
        private int _cellSize;  // Размер клетки
        private int _imgWidth, _imgHeight;  // Game field size pixels
        private int _mapWidth; // _numCellsX // Количество клеток по Х
        private int _mapHeight; // _numCellsY   // Количество клеток по Y
        private int _snakeStep; // Snake step
        private Point _snakeDir;    // Snake movement direction
        private List<Point> _snake = new List<Point>() { }; // Snake list of (x, y) positions
        private Point _food = new Point();  // Food
        private bool _drawingAllow = true; // Разрешение на отрисовку
        private Random _rnd = new Random(); // Random generator

        /// <summary>
        /// КОНСТРУКТОР
        /// </summary>
        public Snake(int cellSize, int imgWidth, int imgHeight)
        {
            ///
            /// ИНИЦИАЛИЗАЦИЯ
            /// 
            _cellSize = cellSize;
            _snakeStep = _cellSize;
            _imgWidth = imgWidth;   _imgHeight = imgHeight;

            // Количество клеток
            _mapWidth = _imgWidth / _cellSize;
            _mapHeight = _imgHeight / _cellSize;


            Reset();

            // Generate an initial random position for the food
            GenerateFood();

            // Создание числовых полей еды
            GenerateFoodDist();
        }

        /// <summary>
        /// УПРАВЛЕНИЕ
        /// </summary>
        public void Update(System.Windows.Threading.DispatcherTimer timer)
        {
            // Calc a new position of the head
            Point newHeadPosition = new Point(
                _snake[0].X + _snakeDir.X,
                _snake[0].Y + _snakeDir.Y
            );

            // Insert new position in the beginning of the snake list
            _snake.Insert(0, newHeadPosition);

            // Remove the last element
            _snake.RemoveAt(_snake.Count - 1);

            // Check collision with the food
            if (_snake[0].X == _food.X &&
                _snake[0].Y == _food.Y)
            {
                // Add new element in the snake
                _snake.Add(new Point(_food.X, _food.Y));

                GenerateFood();

                for (int y = 0; y < _mapHeight; y++)
                    for (int x = 0; x < _mapWidth; x++)
                        _map[y, x] = -1;

                GenerateFoodDist(); // Generate a new food position
            }

            // Check collision with the walls
            if ((_snake[0].X < 0) || (_snake[0].X + _cellSize) > _imgWidth || (_snake[0].Y < 0) || (_snake[0].Y + _cellSize) > _imgHeight)
            {
                timer.Stop();
                _drawingAllow = false; // запрещает отрисовывать графику при столкновении
            }

            // После столкновения перерисовываем квадрат у стенки, чтобы он не уходил за пределы поля
            if (_snake[0].X < 0) _snake[0] = new Point(0, _snake[0].Y);
            else if ((_snake[0].X + _cellSize) > _imgWidth) _snake[0] = new Point(_snake[0].X - _cellSize, _snake[0].Y);
            else if (_snake[0].Y < 0) _snake[0] = new Point(_snake[0].X, 0);
            else if ((_snake[0].Y + _cellSize) > _imgHeight) _snake[0] = new Point(_snake[0].X, _snake[0].Y - _cellSize);

            int X = (int)_snake[0].X / _cellSize;
            int Y = (int)_snake[0].Y / _cellSize;
            ChangeSnakeDir(X, Y);  // Меняем направление змейки
        }

        /// <summary>
        /// ОТРИСОВКА
        /// </summary>
        public void Draw(Canvas g)
        {
            if (!_drawingAllow) return;

            g.Children.Clear();
            DrawMap(g);
            DrawFood(g);
            DrawSnake(g);
        }

        /// <summary>
        /// СБРОС
        /// </summary>
        public void Reset()
        {
            _drawingAllow = true;
            _snakeDir = new Point(_cellSize, 0);
            _snake.Add(new Point(20, 20)); // ПЕРЕДЕЛАТЬ НА НОМЕР ЯЧЕЙКИ или КООРДИНАТУ

            // Формирование массива с исходными ячейками
            _map = new int[_mapHeight, _mapWidth];
            for (int y = 0; y < _mapHeight; y++)
                for (int x = 0; x < _mapWidth; x++)
                    _map[y, x] = -1;
        }



        /// <summary>
        /// PRIVATE
        /// </summary>
               
        // Generate an initial random position for the food
        private void GenerateFood()
        {
            _food.X = 20 * _rnd.Next(0, _mapWidth - 1);
            _food.Y = 20 * _rnd.Next(0, _mapHeight - 1);
        }

        // Создание числовых полей еды
        private void GenerateFoodDist()
        {
            int totalCells = _mapWidth * _mapHeight - 1;
            int step = 0;
            int xm1, xp1, ym1, yp1;

            //_food = new Point(60, 280);
            int X = (int)_food.X / _cellSize;
            int Y = (int)_food.Y / _cellSize;

            _map[Y, X] = 0;

            while (totalCells > 0)
            {
                for (int y = 0; y < _mapHeight; y++)
                    for (int x = 0; x < _mapWidth; x++)
                    {
                        if (_map[y, x] == step)
                        {
                            xm1 = (x - 1 < 0) ? 0 : x - 1;
                            xp1 = (x + 1 > _mapWidth - 1) ? _mapWidth - 1 : x + 1;
                            ym1 = (y - 1 < 0) ? 0 : y - 1;
                            yp1 = (y + 1 > _mapHeight - 1) ? _mapHeight - 1 : y + 1;

                            //Ставим значение шага+1 в соседние ячейки (если они проходимы)
                            if (_map[ym1, x] < 0) _map[ym1, x] = step + 1; 
                            if (_map[yp1, x] < 0) _map[yp1, x] = step + 1; 
                            if (_map[y, xm1] < 0) _map[y, xm1] = step + 1; 
                            if (_map[y, xp1] < 0) _map[y, xp1] = step + 1;
                        }
                    }
                step++;
                totalCells -= 1; // Каждый раз уменьшаем на единицу
            }
        }

        // Меняем направление змейки
        private void ChangeSnakeDir(int x, int y)
        {
            List<int> p = new List<int> { };

            if (y - 1 >= 0) p.Add(_map[y - 1, x]); else p.Add(99999);
            if (y + 1 < _mapHeight) p.Add(_map[y + 1, x]); else p.Add(99999);
            if (x - 1 >= 0) p.Add(_map[y, x - 1]); else p.Add(99999);
            if (x + 1 < _mapWidth) p.Add(_map[y, x + 1]); else p.Add(99999);

            int indx = p.IndexOf(p.Min()); // Находим индекс минимального элемента из 4-х сторон

            if (indx == 0) _snakeDir = new Point(0, -_cellSize);  // up
            if (indx == 1) _snakeDir = new Point(0, _cellSize);  // down
            if (indx == 2) _snakeDir = new Point(-_cellSize, 0);  // left
            if (indx == 3) _snakeDir = new Point(_cellSize, 0); // right
        }


        private void DrawMap(Canvas g)
        {
            int offs_y = 0;
            int offs_x = 0;
            //return;

            for (int y = 0; y < _mapHeight; y++)
            {
                offs_y = y * _cellSize;
                for (int x = 0; x < _mapWidth; x++)
                {
                    offs_x = x * _cellSize;

                    Rectangle cellMap = new Rectangle()
                    {
                        Width = _cellSize,
                        Height = _cellSize,
                        Fill = Brushes.White,
                        Stroke = Brushes.Black,
                        StrokeThickness = 0.1,
                    };
                    Canvas.SetLeft(cellMap, offs_x);
                    Canvas.SetTop(cellMap, offs_y);
                    g.Children.Add(cellMap);

                    TextBlock cellText = new TextBlock()
                    {
                        Text = _map[y, x].ToString(),
                        FontSize = 8
                    };
                    Canvas.SetLeft(cellText, offs_x);
                    Canvas.SetTop(cellText, offs_y);
                    g.Children.Add(cellText);
                }
            }

        }

        private void DrawSnake(Canvas g)
        {
            foreach (var cell in _snake)
            {
                DrawRect(g, cell.X, cell.Y, Brushes.Green);
            }
        }

        private void DrawFood(Canvas g)
        {
            DrawRect(g, _food.X, _food.Y, Brushes.OrangeRed);
        }

        private void DrawRect(Canvas g, double x, double y, Brush color)
        {
            Rectangle rec = new Rectangle()
            {
                Width = _cellSize,
                Height = _cellSize,
                Fill = color,
            };
            Canvas.SetLeft(rec, x);
            Canvas.SetTop(rec, y);
            g.Children.Add(rec);
        }

    }
}
