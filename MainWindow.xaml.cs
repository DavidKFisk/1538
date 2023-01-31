using _38puzzle;
using System;
using System.Security.Cryptography.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace _15ways38
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int _numOfTiles = 19;

        Tile[] TileGrid = new Tile[19];
        TileBase[] _tileBase = new TileBase[_numOfTiles];

 
        int[] _tileXLocationGrid = new int[] {-400,	-300,-200,-450,-350	,-250,-150,	-500,-400,-300,-200	,-100,-450,	-350,-250,-150,-400	,-300,-200 };
        int[] _tileYLocationGrid = new int[] { -50,-50,-50,25,25,25,25,100,100,100,100,100,175,175,175,175,250,250, 250};

        int[] _tileXLocationStart = new int[] { -700, -600, -500, -400, -300, -200, -100,    0,  100, -700, -600, -500, -400, -300, -200, -100,    0,  100, -300 };
        int[] _tileYLocationStart = new int[] { -400, -400, -400, -400, -400, -400, -400, -400, -400, -300, -300, -300, -300, -300, -300, -300, -300, -300, -200 };

        public MainWindow()
        {
            InitializeComponent();

            SetupBaseTiles();
            SetupTiles();

            MainViewModel vm = new MainViewModel(TileGrid, this, _tileXLocationGrid, _tileYLocationGrid, _tileXLocationStart, _tileYLocationStart);
            DataContext = vm;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }

        private void SetupTiles()
        {
            Grid WinGrid = this.GridForTiles;

            for (int i = 18; i >= 0; i--)
            {
                TileGrid[i] = new Tile();
                TileGrid[i].TileColor = Brushes.IndianRed;
                TileGrid[i].TileNum = (i + 1).ToString();
                TileGrid[i].Margin = new Thickness(_tileXLocationStart[i], _tileYLocationStart[i], 0, 0);
                TileGrid[i].Width = 50;
                TileGrid[i].Height = 50;
                TileGrid[i].MouseUp += TileMouseUp;
                TileGrid[i].MouseDown += TileCheckPosition;

                WinGrid.Children.Add(TileGrid[i]);
                Grid.SetZIndex(_tileBase[i], 1);
            }
        }

        private void SetupBaseTiles()
        {
            Grid WinGrid = this.GridForTiles;
           
            for (int i = 0; i < _tileBase.Length; i++)
            {
                _tileBase[i] = new TileBase();
                _tileBase[i].Margin = new Thickness(_tileXLocationGrid[i], _tileYLocationGrid[i], 0, 0);
                _tileBase[i].Width = 50;
                _tileBase[i].Height = 50;
                WinGrid.Children.Add(_tileBase[i]);
                Grid.SetZIndex(_tileBase[i], 0);
            }
        }


        private void TileMouseUp(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm != null)
            {
                var tile = sender as Tile;
                vm.MouseUpCommand.Execute(tile);
            }
        }

        private void TileCheckPosition(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm != null)
            {
                var tile = sender as Tile;
                vm.TileCheckPosition.Execute(tile);
            }
        }
    }

   
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = (p) => execute();
            _canExecute = (p) => canExecute == null || canExecute();
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }



    public class MainViewModel
    {
        public ICommand ResetGame { get; set; }
        public ICommand SolveGame { get; set; }
        public ICommand TileCheckPosition { get; set; }
        public ICommand MouseUpCommand { get; set; }
        public ICommand CloseGame { get; set; }


        const int _numOfTiles = 19;

        MainWindow _mainWindow;

        Tile[] TileGrid = new Tile[15];

        int[] _tileXLocationSolved;
        int[] _tileYLocationSolved;

        int[] _tileXLocationStart;
        int[] _tileYLocationStart;

        bool[] _TileBaseUsed = new bool[_numOfTiles];
        int[] _TileBaseUsedValues = new int[_numOfTiles];

        public MainViewModel(Tile[] tileGrid, MainWindow window, int[] _tileXLocSolved, int[] _tileYLocSolved, int[] _tileXLocStart, int[] _tileYLocStart)
        {
            ResetGame = new RelayCommand(ExecuteResetGame);
            SolveGame = new RelayCommand(ExecuteSolveGame);
            CloseGame = new RelayCommand(CloseWindow);

            TileCheckPosition = new RelayCommand((param) => TileMouseDown(param));
            MouseUpCommand = new RelayCommand((param) => TileMouseUP(param));

            _tileXLocationSolved = _tileXLocSolved;
            _tileYLocationSolved = _tileYLocSolved;

            _tileXLocationStart = _tileXLocStart;
            _tileYLocationStart = _tileYLocStart;

            TileGrid = tileGrid;
            _mainWindow = window;

            //Mark positions that have been used true
            for (int i = 0; i < _numOfTiles; i++) { _TileBaseUsed[i] = false; }
            for (int i = 0; i < _numOfTiles; i++) { _TileBaseUsedValues[i] = 0; }
        }

        private void ExecuteTileMoving()
        {
            int Position = 700;

            for (int i = 18; i >= 0; i--)
            {
                TileGrid[i].TileColor = Brushes.Yellow;
                TileGrid[i].Margin = new Thickness(Position, -400, 0, 0);

                Position -= 100;
            }
        }

        private void ExecuteResetGame()
        {
            for (int i = 18; i >= 0; i--)
            {
                TileGrid[i].TileColor = Brushes.IndianRed;
                TileGrid[i].Margin = new Thickness(_tileXLocationStart[i], _tileYLocationStart[i], 0, 0);
                _TileBaseUsed[i] = false;
                _TileBaseUsedValues[i] = 0;
                SumRows();
            }
        }

        private void CloseWindow()
        {
           _mainWindow.Close();
        }

        private void ExecuteSolveGame()
        {
            /*~~ Sum Positions ~~~~~~~~~~~~~~~~~~~~~~~~~Solutions~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ 
                 	0   1    2       ||       15  14  9            15  13  10          3  17  18            9  11  18
                 3   4    5    6     ||     13   8   6  11        14  8  4  12       19  7  1  11        14   6   1  17
               7   8   9   10   11   ||   10   4   5   1   18    9  6  5  2  16    16  2   5  6  9     15  8   5   7  3
                 12  13  14  15      ||      12  2   7   17       11  1  7  19       12  4   8   14      13  4   2  19
                   16  17   18       ||        16 19  3             18  17  3          10 13  15          10  12  16
            ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
            int[] _tileXSolved = new int[19];
            int[] _tileYSolved = new int[19];
                 int[] _tileLocation = new int[19];

            if (Convert.ToBoolean(_mainWindow.Solution1.IsChecked)) {
                _tileXSolved = new int[] { -400, -300, -200, -450, -350, -250, -150, -500, -400, -300, -200, -100, -450, -350, -250, -150, -400, -300, -200 };
                _tileYSolved = new int[] { -50, -50, -50, 25, 25, 25, 25, 100, 100, 100, 100, 100, 175, 175, 175, 175, 250, 250, 250 };
                _tileLocation = new int[] { 8, 10, 17, 13, 5, 0, 16, 14, 7, 4, 6, 2, 12, 3, 1, 18, 9, 11, 15 };
            }
            else
            {
                _tileXSolved = new int[] { -400, -300, -200, -450, -350, -250, -150, -500, -400, -300, -200, -100, -450, -350, -250, -150, -400, -300, -200 };
                _tileYSolved = new int[] { -50, -50, -50, 25, 25, 25, 25, 100, 100, 100, 100, 100, 175, 175, 175, 175, 250, 250, 250 };
                _tileLocation = new int[] { 14, 12, 9, 13, 7, 3, 11, 8, 5, 4, 1, 15, 10, 0, 6, 18, 17, 16, 2 };
            }

            Brush brush = Brushes.LightGreen;

            for (int i = 0; i < _numOfTiles; i++)
            {
                TileGrid[_tileLocation[i]].TileColor = brush;
                TileGrid[_tileLocation[i]].Margin = new Thickness(_tileXSolved[i], _tileYSolved[i], 0, 0);
                _TileBaseUsed[i] = true;
                _TileBaseUsedValues[i] = Convert.ToInt32(_tileLocation[i] + 1);
                Grid.SetZIndex(TileGrid[_tileLocation[i]], 1);
            }

            SumRows();
        }

        private void TileMouseUP(Object param)
        {
            var tile = param as Tile;
            Boolean _foundLocationToDrop = false;
            if (tile != null)
            {
                tile.TileColor = Brushes.LightGreen;

                for (int i = 0; i < TileGrid.Length; i++)
                {
                    if ((tile.CurrentUPX >= _tileXLocationSolved[i] - 25 && tile.CurrentUPX <= (_tileXLocationSolved[i] + 50)) &&
                        (tile.CurrentUPY >= _tileYLocationSolved[i] - 25 && tile.CurrentUPY <= (_tileYLocationSolved[i] + 50)) && !_TileBaseUsed[i])
                    {
                        if (!_TileBaseUsed[i])
                        {
                            tile.Margin = new Thickness(_tileXLocationSolved[i], _tileYLocationSolved[i], 0, 0);
                            _foundLocationToDrop = true;
                            Grid.SetZIndex(tile, 1);

                            _TileBaseUsed[i] = true;
                            _TileBaseUsedValues[i] = Convert.ToInt32(tile.TileNum);

                            SumRows();
                        }
                        break;
                    }
                }

                if (!_foundLocationToDrop)
                {
                    tile.Margin = new Thickness(_tileXLocationStart[Convert.ToInt32(tile.TileNum) - 1], _tileYLocationStart[Convert.ToInt32(tile.TileNum) - 1], 0, 0);
                    tile.TileColor = Brushes.LightGoldenrodYellow; ;
                }
            }
        }

        private void SumRows()
        {
            /*~~ Sum Positions ~~~~~~~~~~~~~~~~~~~~~~~~~Solutions~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ 
                 	0   1    2       ||       15  14  9             5  13  10          3  17  18            9  11  18
                 3   4    5    6     ||     13   8   6  11        14  8  4  12       19  7  1  11        14   6   1  17
               7   8   9   10   11   ||   10   4   5   1   18    9  6  5  2  16    16  2   5  6  9     15  8   5   7  3
                 12  13  14  15      ||      12  2   7   17       11  1  7  19       12  4   8  14      13  4   2  19
                   16  17   18       ||        16 19  3             18  17  3          10 13  15          10  12  16
            ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/


            _mainWindow.LR1_label.Content = (_TileBaseUsedValues[7] + _TileBaseUsedValues[12] + _TileBaseUsedValues[16]).ToString();
            _mainWindow.LR2_label.Content = (_TileBaseUsedValues[3] + _TileBaseUsedValues[8] + _TileBaseUsedValues[13] + _TileBaseUsedValues[17]).ToString();
            _mainWindow.LR3_label.Content = (_TileBaseUsedValues[0] + _TileBaseUsedValues[4] + _TileBaseUsedValues[9] + _TileBaseUsedValues[14] + _TileBaseUsedValues[18]).ToString();
            _mainWindow.LR4_label.Content = (_TileBaseUsedValues[1] + _TileBaseUsedValues[5] + _TileBaseUsedValues[10] + _TileBaseUsedValues[15]).ToString();
            _mainWindow.LR5_label.Content = (_TileBaseUsedValues[2] + _TileBaseUsedValues[6] + _TileBaseUsedValues[11]).ToString();

            _mainWindow.RL1_label.Content = (_TileBaseUsedValues[11] + _TileBaseUsedValues[15] + _TileBaseUsedValues[18]).ToString();
            _mainWindow.RL2_label.Content = (_TileBaseUsedValues[6] + _TileBaseUsedValues[10] + _TileBaseUsedValues[14] + _TileBaseUsedValues[17]).ToString();
            _mainWindow.RL3_label.Content = (_TileBaseUsedValues[2] + _TileBaseUsedValues[5] + _TileBaseUsedValues[9] + _TileBaseUsedValues[13] + _TileBaseUsedValues[16]).ToString();
            _mainWindow.RL4_label.Content = (_TileBaseUsedValues[1] + _TileBaseUsedValues[4] + _TileBaseUsedValues[8] + _TileBaseUsedValues[12]).ToString();
            _mainWindow.RL5_label.Content = (_TileBaseUsedValues[0] + _TileBaseUsedValues[3] + _TileBaseUsedValues[7]).ToString();

            _mainWindow.H1_label.Content = (_TileBaseUsedValues[0] + _TileBaseUsedValues[1] + _TileBaseUsedValues[2]).ToString();
            _mainWindow.H2_label.Content = (_TileBaseUsedValues[3] + _TileBaseUsedValues[4] + _TileBaseUsedValues[5] + _TileBaseUsedValues[6]).ToString();
            _mainWindow.H3_label.Content = (_TileBaseUsedValues[7] + _TileBaseUsedValues[8] + _TileBaseUsedValues[9] + _TileBaseUsedValues[10] + _TileBaseUsedValues[11]).ToString();
            _mainWindow.H4_label.Content = (_TileBaseUsedValues[12] + _TileBaseUsedValues[13] + _TileBaseUsedValues[14] + _TileBaseUsedValues[15]).ToString();
            _mainWindow.H5_label.Content = (_TileBaseUsedValues[16] + _TileBaseUsedValues[17] + _TileBaseUsedValues[18]).ToString();


            if (_mainWindow.LR1_label.Content.ToString() == "38") { _mainWindow.LR1_label.Background = Brushes.LightGreen; } else { _mainWindow.LR1_label.Background = Brushes.PaleVioletRed; }
            if (_mainWindow.LR2_label.Content.ToString() == "38") { _mainWindow.LR2_label.Background = Brushes.LightGreen; } else { _mainWindow.LR2_label.Background = Brushes.PaleVioletRed; }
            if (_mainWindow.LR3_label.Content.ToString() == "38") { _mainWindow.LR3_label.Background = Brushes.LightGreen; } else { _mainWindow.LR3_label.Background = Brushes.PaleVioletRed; }
            if (_mainWindow.LR4_label.Content.ToString() == "38") { _mainWindow.LR4_label.Background = Brushes.LightGreen; } else { _mainWindow.LR4_label.Background = Brushes.PaleVioletRed; }
            if (_mainWindow.LR5_label.Content.ToString() == "38") { _mainWindow.LR5_label.Background = Brushes.LightGreen; } else { _mainWindow.LR5_label.Background = Brushes.PaleVioletRed; }

            if (_mainWindow.RL1_label.Content.ToString() == "38") { _mainWindow.RL1_label.Background = Brushes.LightGreen; } else { _mainWindow.RL1_label.Background = Brushes.PaleVioletRed; }
            if (_mainWindow.RL2_label.Content.ToString() == "38") { _mainWindow.RL2_label.Background = Brushes.LightGreen; } else { _mainWindow.RL2_label.Background = Brushes.PaleVioletRed; }
            if (_mainWindow.RL3_label.Content.ToString() == "38") { _mainWindow.RL3_label.Background = Brushes.LightGreen; } else { _mainWindow.RL3_label.Background = Brushes.PaleVioletRed; }
            if (_mainWindow.RL4_label.Content.ToString() == "38") { _mainWindow.RL4_label.Background = Brushes.LightGreen; } else { _mainWindow.RL4_label.Background = Brushes.PaleVioletRed; }
            if (_mainWindow.RL5_label.Content.ToString() == "38") { _mainWindow.RL5_label.Background = Brushes.LightGreen; } else { _mainWindow.RL5_label.Background = Brushes.PaleVioletRed; }

            if (_mainWindow.H1_label.Content.ToString() == "38") { _mainWindow.H1_label.Background = Brushes.LightGreen; } else { _mainWindow.H1_label.Background = Brushes.PaleVioletRed; }
            if (_mainWindow.H2_label.Content.ToString() == "38") { _mainWindow.H2_label.Background = Brushes.LightGreen; } else { _mainWindow.H2_label.Background = Brushes.PaleVioletRed; }
            if (_mainWindow.H3_label.Content.ToString() == "38") { _mainWindow.H3_label.Background = Brushes.LightGreen; } else { _mainWindow.H3_label.Background = Brushes.PaleVioletRed; }
            if (_mainWindow.H4_label.Content.ToString() == "38") { _mainWindow.H4_label.Background = Brushes.LightGreen; } else { _mainWindow.H4_label.Background = Brushes.PaleVioletRed; }
            if (_mainWindow.H5_label.Content.ToString() == "38") { _mainWindow.H5_label.Background = Brushes.LightGreen; } else { _mainWindow.H5_label.Background = Brushes.PaleVioletRed; }

        }

        private void TileMouseDown(Object param)
        {
            var tile = param as Tile;
            if (tile != null)
            {
                Panel.SetZIndex(tile, 2);
                tile.TileColor = Brushes.Yellow;

                for (int i = 0; i < TileGrid.Length; i++)
                {
                    if ((tile.CurrentUPX >= _tileXLocationSolved[i] - 25 && tile.CurrentUPX <= (_tileXLocationSolved[i] + 50)) &&
                       (tile.CurrentUPY >= _tileYLocationSolved[i] - 25 && tile.CurrentUPY <= (_tileYLocationSolved[i] + 50)) && _TileBaseUsed[i])
                    {
                        if (_TileBaseUsed[i])
                        {
                            _TileBaseUsed[i] = false;
                            _TileBaseUsedValues[i] = 0;
                            SumRows();
                        }
                    }
                    else
                    { }
                }
            }
        }
    }
}
