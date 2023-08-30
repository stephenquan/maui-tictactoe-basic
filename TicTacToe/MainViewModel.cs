﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Localization;
using System.Globalization;
using TicTacToe.Resources.Strings;

namespace TicTacToe;

public partial class MainViewModel : ObservableObject
{
    private Random random = new Random();

    [ObservableProperty]
    private IStringLocalizer _localizer = ServiceHelper.GetService<IStringLocalizer<AppStrings>>();

    public FlowDirection FlowDirection => Culture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

    [ObservableProperty]
    private Board _board = new Board();

    [ObservableProperty]
    private int _countWinX = 0;

    [ObservableProperty]
    private int _countWinO = 0;

    [ObservableProperty]
    private int _countTie = 0;

    [ObservableProperty]
    private int _maxLevel = 100;

    [ObservableProperty]
    private int _level = 75;

    private void IncrementScores()
    {
        if (Board.WinX.Count > 0)
            CountWinX++;
        else if (Board.WinO.Count > 0)
            CountWinO++;
        else if (Board.AvailableMoves.Count == 0)
            CountTie++;
    }

    private void ResetOpacity()
    {
        foreach (var c in Board.Cells) c.O = 1.0;
    }

    private void SetOpacity()
    {
        foreach (var c in Board.Cells) c.O = 0.2;
        foreach (var w in Board.WinX)
        {
            Board.Cells[w.A].O = 1.0;
            Board.Cells[w.B].O = 1.0;
            Board.Cells[w.C].O = 1.0;
        }
        foreach (var w in Board.WinO)
        {
            Board.Cells[w.A].O = 1.0;
            Board.Cells[w.B].O = 1.0;
            Board.Cells[w.C].O = 1.0;
        }
    }

    public CultureInfo Culture
    {
        get => CultureInfo.CurrentUICulture;
        set
        {
            if (value == null) return;
            if (value.Name == CultureInfo.CurrentUICulture.Name) return;
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = value;
            OnPropertyChanged(nameof(Culture));
            OnPropertyChanged(nameof(Localizer));
            OnPropertyChanged(nameof(FlowDirection));
        }
    }

    private List<CultureInfo> Languages = new List<CultureInfo>()
    {
        new CultureInfo("en-US"),
        new CultureInfo("fr-FR"),
        new CultureInfo("de-DE"),
        new CultureInfo("zh-CN"),
    };

    [ObservableProperty]
    private int _languageIndex = 0;

    [RelayCommand]
    private void ChangeLanguage()
        => Culture = Languages[LanguageIndex = (LanguageIndex + 1) % Languages.Count];

    [RelayCommand]
    private async Task Click(int Index)
    {
        if (Board.IsGameOver) Board.Clear();
        if (Board.Cells[Index].V != String.Empty) return;
        Board.PlayMove(Index, "X");
        if (Board.IsGameOver)
        {
            IncrementScores();
            SetOpacity();
            return;
        }

        await Task.Delay(250);
        var BestMoves = Board.AvailableMoves.OrderBy(M => -Board.TryMove(M, "O").Score).ToList();
        int Move = random.Next(MaxLevel) < Level ? BestMoves[0] : BestMoves[random.Next(BestMoves.Count)];
        Board.PlayMove(Move, "O");
        if (Board.IsGameOver)
        {
            IncrementScores();
            SetOpacity();
            return;
        }
    }
}