﻿using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Xamarin.Forms;

namespace GMIT_Mastermind
{
    public partial class MainPage : ContentPage
    {
        private const int Rounds = 8;
        private SaveData save = new SaveData();

        private int selectedColor;
        private static readonly string[] ColorNames = { "Red", "Green", "Blue", "Purple", "Yellow", "Turquoise" };
        private readonly List<Image> boardPegs = new List<Image>();
        private readonly List<Grid> scoreGridPegs = new List<Grid>();

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            GenerateBoardUI();
            GenerateSelectionPegsUI();

            NewGame();

            base.OnAppearing();
        }

        private void NewGame()
        {
            save = new SaveData();

            Random rng = new Random();

            List<int> duplicates = new List<int>();

            for (int i = 0; i < boardPegs.Count; i++)
            {
                boardPegs[i].Source = "White.png";
            }

            for (int i = 0; i < scoreGridPegs.Count; i++)
            {
                foreach (Image item in scoreGridPegs[i].Children)
                {
                    item.Source = "";
                }
            }

            EnableCurrentRow();

            for (int i = 0; i < 4; i++)
            {
                int num;

                // Make sure its not a duplicate
                do
                {
                    num = rng.Next(0, ColorNames.Length);
                } while (duplicates.Contains(num));

                duplicates.Add(num);

                // Cheat
                Debug.Write(ColorNames[num] + ", ");

                save.target[i] = num;
            }

            Debug.WriteLine("");

        }

        #region UI Generation
        private void GenerateSelectionPegsUI()
        {
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += SelectPeg_Tapped;

            for (int i = 0; i < 6; i++)
            {
                Image peg = new Image();
                peg.StyleId = i.ToString();
                peg.Source = ColorNames[i] + ".png";

                if (i == 0)
                {
                    peg.BackgroundColor = new Color(1f, 1f, .25f);
                }

                peg.GestureRecognizers.Add(tapGestureRecognizer);
                PegsContainer.Children.Add(peg);
            }
        }

        private void GenerateBoardUI()
        {
            for (int row = 0; row < Rounds; row++)
            {
                // Vertical Rounds list
                StackLayout roundLayout = new StackLayout();
                roundLayout.Orientation = StackOrientation.Horizontal;

                // Label number for each row
                GenerateRowLabel(row, roundLayout);

                // Guess Grid Square
                GenerateGuessPegs(roundLayout);

                // Board rows
                GeneratePegGuessRow(row, roundLayout);


                RoundsContainer.Children.Add(roundLayout);
            }
        }

        private static void GenerateRowLabel(int row, StackLayout roundLayout)
        {
            Label numberLabel = new Label();
            numberLabel.Margin = new Thickness(6, 0);
            numberLabel.VerticalTextAlignment = TextAlignment.Center;
            numberLabel.Text = (Rounds - row).ToString();
            numberLabel.FontSize = 32;
            numberLabel.FontAttributes = FontAttributes.Bold;
            numberLabel.TextColor = Color.Black;
            roundLayout.Children.Add(numberLabel);
        }

        private void GeneratePegGuessRow(int row, StackLayout roundLayout)
        {
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += PlacePeg_Tapped;

            Frame pegChoiceFrame = new Frame();
            pegChoiceFrame.Margin = new Thickness(4, 0);
            pegChoiceFrame.CornerRadius = 10;
            pegChoiceFrame.BackgroundColor = Color.FromHex("#B7733C");
            pegChoiceFrame.Padding = new Thickness(0);


            StackLayout pegsLayout = new StackLayout();
            pegsLayout.Padding = new Thickness(Rounds, 0);
            pegsLayout.Orientation = StackOrientation.Horizontal;

            pegsLayout.StyleId = (Rounds - 1 - row).ToString();

            for (int j = 0; j < 4; j++)
            {
                Image peg = new Image();
                peg.Source = "White.png";
                if (row != 7)
                {
                    peg.Opacity = .5f;
                    peg.IsEnabled = false;
                }
                peg.GestureRecognizers.Add(tapGestureRecognizer);
                peg.StyleId = j.ToString();

                boardPegs.Add(peg);
                pegsLayout.Children.Add(peg);
            }

            pegChoiceFrame.Content = pegsLayout;
            pegChoiceFrame.CornerRadius = 30;
            roundLayout.Children.Add(pegChoiceFrame);

        }

        private void GenerateGuessPegs(StackLayout roundLayout)
        {
            Frame guessFrame = new Frame();
            guessFrame.BackgroundColor = Color.FromHex("#B7733C");
            guessFrame.Padding = new Thickness(4);

            Grid grid = new Grid();
            scoreGridPegs.Add(grid);

            grid.Margin = new Thickness(4);

            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    Image peg = new Image();

                    peg.WidthRequest = 24;
                    peg.HeightRequest = 24;

                    peg.HorizontalOptions = LayoutOptions.Center;
                    peg.VerticalOptions = LayoutOptions.Center;

                    peg.SetValue(Grid.RowProperty, x);
                    peg.SetValue(Grid.ColumnProperty, y);

                    grid.Children.Add(peg);
                }
            }

            guessFrame.Content = grid;
            roundLayout.Children.Add(guessFrame);
        }
        #endregion

        private void PlacePeg_Tapped(object sender, EventArgs e)
        {
            Image pegImg = (Image)sender;

            pegImg.Source = ColorNames[selectedColor] + ".png";

            save.board[save.round, int.Parse(pegImg.StyleId)] = selectedColor;

            EnableCheckButton();
        }

        private void EnableCheckButton()
        {
            bool isValid = true;

            for (int i = 0; i < 4; i++)
            {
                if (save.board[save.round, i] == -1)
                {
                    isValid = false;
                }
            }

            CheckButton.IsEnabled = isValid;
        }

        private void SelectPeg_Tapped(object sender, EventArgs e)
        {
            Image img = (Image)sender;

            // Clear selection
            foreach (Image child in PegsContainer.Children)
            {
                child.BackgroundColor = Color.Transparent;
            }

            img.BackgroundColor = new Color(1f, 1f, .25f);

            selectedColor = int.Parse(img.StyleId);
        }

        private void Checked_Clicked(object sender, EventArgs e)
        {
            int whitePegs = 0;
            int blackPegs = 0;

            // Check number of black/white pegs to give
            for (int i = 0; i < 4; i++)
            {
                bool whiteMatched = false;
                for (int j = 0; j < 4; j++)
                {
                    if (save.target[i] == save.board[save.round, j])
                    {
                        if (whiteMatched == false)
                        {
                            whitePegs++;
                            whiteMatched = true;
                        }

                        if (i == j)
                        {
                            blackPegs++;
                        }
                    }
                }
            }

            save.pegs[save.round, 0] = whitePegs;
            save.pegs[save.round, 1] = blackPegs;

            // The game was won
            if (blackPegs == 4)
            {
                WinGame();
                return;
            }

            UpdateGuessPegs();

            // Move on to next round
            save.round++;

            if (save.round >= Rounds)
            {
                LoseGame();
                return;
            }

            EnableCurrentRow();

            EnableCheckButton();
        }

        private void UpdateGuessPegs()
        {
            for (int i = 0; i < scoreGridPegs.Count; i++)
            {
                int index = 0;
                foreach (Image peg in scoreGridPegs[Rounds - 1 - i].Children)
                {
                    if (index < save.pegs[i, 1])
                    {
                        peg.Source = "Black.png";
                    }
                    else if (index < save.pegs[i, 0])
                    {
                        peg.Source = "White.png";
                    }
                    else
                    {
                        peg.Source = "";
                    }
                    index++;
                }
            }
        }

        private void EnableCurrentRow()
        {
            // Enable next row
            foreach (Image peg in boardPegs)
            {
                if (peg.Parent.StyleId == save.round.ToString())
                {
                    peg.Opacity = 1f;
                    peg.IsEnabled = true;
                }
                else
                {
                    peg.Opacity = 0.5f;
                    peg.IsEnabled = false;
                }
            }
        }

        private async void WinGame()
        {
            await DisplayAlert("You Win!", $"Congrats you won the game on Round {save.round + 1}", "New Game");
            NewGame();
        }

        private async void LoseGame()
        {
            await DisplayAlert("You Lose!", $"Better luck next Time\n", "New Game");
            NewGame();
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string filePath = Path.Combine(appFolder, "Save.json");

            string text = JsonConvert.SerializeObject(save);
            File.WriteAllText(filePath, text);

            await DisplayAlert("Saved!", "Game has been saved", "Close");
        }

        private async void Restart_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Are you sure?", "Are you sure you want to restart the game?", "Yes", "No");

            if (answer == false)
            {
                return;
            }

            save = new SaveData();

            EnableCheckButton();

            LoadFromSave();
        }

        private async void Load_Clicked(object sender, EventArgs e)
        {
            string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string filePath = Path.Combine(appFolder, "Save.json");

            if (File.Exists(filePath) == false)
            {
                await DisplayAlert("No save game found!", "Save a game to load it later", "Close");
                return;
            }

            string text = File.ReadAllText(filePath);
            save = JsonConvert.DeserializeObject<SaveData>(text);

            LoadFromSave();

            EnableCheckButton();

            await DisplayAlert("Loaded!", "Game has been loaded", "Play");
        }

        private void LoadFromSave()
        {
            for (int i = 0; i < save.target.Length; i++)
            {
                // Cheat
                Debug.Write(save.target[i] + ", ");
            }

            for (int i = 0; i < boardPegs.Count; i++)
            {
                int y = int.Parse(boardPegs[i].StyleId);
                int x = int.Parse(boardPegs[i].Parent.StyleId);

                int value = save.board[x, y];

                if (value == -1)
                {
                    boardPegs[i].Source = "White.png";
                }
                else
                {
                    boardPegs[i].Source = ColorNames[value] + ".png";
                }
            }

            EnableCurrentRow();

            UpdateGuessPegs();
        }
    }
}
