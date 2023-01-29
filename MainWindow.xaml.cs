using _38puzzle;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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

        int[] _tileXLocationSolved = new int[] { -250, -250, -100, -350, -300, -350, -200, -400, -400, -400, -300, -300, -450, -450, -500, -200, -150, -200, -150 };
        int[] _tileYLocationSolved = new int[] {   25,  175,  100,  175,  100,   25,  100,  100,  -50,  250,  -50,  250,  175,   25,  100,  250,   25,  -50,  175 };

        int[] _tileXLocationStart = new int[] { -700, -600, -500, -400, -300, -200, -100,   -0,  100, -700, -600, -500, -400, -300, -200, -100,    0,  100, -300 };
        int[] _tileYLocationStart = new int[] { -400, -400, -400, -400, -400, -400, -400, -400, -400, -300, -300, -300, -300, -300, -300, -300, -300, -300, -200 };

        public MainWindow()
        {
            InitializeComponent();

            SetupBaseTiles();
            SetupTiles();

            MainViewModel vm = new MainViewModel(TileGrid, this, _tileXLocationSolved, _tileYLocationSolved, _tileXLocationStart, _tileYLocationStart);
            DataContext = vm;
        }

        private void SetupTiles()
        {
            Grid WinGrid = this.GridForTiles;

            for (int i = 18; i >= 0; i--)
            {
                TileGrid[i] = new Tile();
                TileGrid[i].TileColor = Brushes.Red;
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
                _tileBase[i].Margin = new Thickness(_tileXLocationSolved[i], _tileYLocationSolved[i], 0, 0);
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
        public ICommand MouseUpCommand { get;  set; }

        const int _numOfTiles = 19;

        MainWindow _mainWindow;

        Tile[] TileGrid = new Tile[15];
       
        int[] _tileXLocationSolved;
        int[] _tileYLocationSolved;

        int[] _tileXLocationStart;
        int[] _tileYLocationStart;

        bool[] _TileBaseUsed = new bool[_numOfTiles];
        


        public MainViewModel(Tile[] tileGrid, MainWindow window, int[] _tileXLocSolved, int[] _tileYLocSolved, int[] _tileXLocStart, int[] _tileYLocStart)
        {
            ResetGame = new RelayCommand(ExecuteResetGame);
            SolveGame = new RelayCommand(ExecuteSolveGame);
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
                TileGrid[i].TileColor = Brushes.Red;
                TileGrid[i].Margin = new Thickness(_tileXLocationStart[i], _tileYLocationStart[i], 0, 0);
            }
        }

        private void ExecuteSolveGame()
        {
            Brush brush = Brushes.LightGreen;
          
            for (int i = 0; i < _numOfTiles; i++)
            {
                TileGrid[i].TileColor = brush;
                TileGrid[i].Margin = new Thickness(_tileXLocationSolved[i], _tileYLocationSolved[i], 0, 0);
                Grid.SetZIndex(TileGrid[i], 1);
            }
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
                    if ((tile.CurrentUPX >= _tileXLocationSolved[i] -25 && tile.CurrentUPX <= (_tileXLocationSolved[i] + 50)) &&
                        (tile.CurrentUPY >= _tileYLocationSolved[i] -25 && tile.CurrentUPY <= (_tileYLocationSolved[i] + 50)) && !_TileBaseUsed[i])
                    {
                        tile.Margin = new Thickness(_tileXLocationSolved[i], _tileYLocationSolved[i], 0, 0);
                        _foundLocationToDrop = true;
                        Grid.SetZIndex(tile, 1);
                        SumRows(tile.TileNum);
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

        private void SumRows(string tileNum)
        {
            _mainWindow.LR1_label.Content= tileNum;
        }

        private void TileMouseDown(Object param)
        {
            var tile = param as Tile;
            if (tile != null)
            {
                Panel.SetZIndex(tile, 2);
                tile.TileColor = Brushes.DeepPink;
            }
        }
    }

}
