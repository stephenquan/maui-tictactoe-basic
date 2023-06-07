﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace TicTacToe.Pages
{
    public class GameViewModel : INotifyPropertyChanged
    {
        ObservableCollection<string> board = new ObservableCollection<string>(new List<string>() { "", "", "", "", "", "", "", "", "" });
        string winner = "";
        int frameWidth = 100;
        int frameHeight = 100;

        public event PropertyChangedEventHandler PropertyChanged;

        public GameViewModel()
        {
            MoveCommand = new Command<string>(
                execute: async (string arg) =>
                {
                    if (Winner != "") return;
                    int i = Int32.Parse(arg);
                    if (Board[i] != "") return;
                    PlaceMove(i, "X");
                    CheckWinner();
                    List<int> moves = AvailableMoves();
                    if (moves.Count == 0) return;
                    await Task.Delay(100);
                    Random r = new Random();
                    int m = moves[r.Next(moves.Count)];
                    PlaceMove(m, "O");
                    CheckWinner();
                }
                );

            NewGameCommand = new Command(
                execute: () =>
                {
                    Parallel.For(0, 9, i => Board[i] = "");
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Board)));
                    Winner = "";
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinnerText)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewGameEnabled)));
                }
                );
        }

        public List<int> AvailableMoves()
        {
            if (Winner != "") return new List<int>();
            return Board.Select((p, i) => p == "" ? i : -1).Where(i => i != -1).ToList();
        }

        public void PlaceMove(int pos, string mark)
        {
            Board[pos] = mark;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Board)));
        }

        public void CheckWinner(int a, int b, int c)
        {
            string tst = Board[a] + Board[b] + Board[c];
            if (tst == "XXX") Winner = "X";
            if (tst == "OOO") Winner = "O";
        }
        
        public void CheckWinner()
        {
            CheckWinner(0, 1, 2);
            CheckWinner(3, 4, 5);
            CheckWinner(6, 7, 8);
            CheckWinner(0, 3, 6);
            CheckWinner(1, 4, 7);
            CheckWinner(2, 5, 8);
            CheckWinner(0, 4, 8);
            CheckWinner(2, 4, 6);
            List<int> moves = AvailableMoves();
            if (moves.Count == 0)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinnerText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewGameEnabled)));
            }
        }

        public ObservableCollection<string> Board => board;

        public string Winner
        {
            get => winner;
            set
            {
                if (winner == value) return;
                winner = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Winner)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinnerText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewGameEnabled)));
            }
        }

        public string WinnerText
        {
            get
            {
                if (Winner != "")
                {
                    return Winner + " Wins";
                }
                List<int> moves = AvailableMoves();
                if (moves.Count == 0)
                {
                    return "Draw";
                }
                return "";
            }
        }

        public int FrameWidth
        {
            get => frameWidth;
            set
            {
                if (frameWidth == value) return;
                frameWidth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FrameWidth)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GameSize)));
            }
        }

        public int FrameHeight
        {
            get => frameHeight;
            set
            {
                if (frameHeight == value) return;
                frameHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FrameHeight)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GameSize)));
            }
        }

        public int GameSize
        {
            get => frameWidth < frameHeight ? frameWidth : frameHeight;
        }

        public ICommand MoveCommand { private set; get; }

        public ICommand NewGameCommand { private set; get; }

        public bool NewGameEnabled => AvailableMoves().Count == 0;

    }
}
