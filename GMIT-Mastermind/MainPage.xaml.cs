using System;
using System.Collections.Generic;
using System.Diagnostics;

using Xamarin.Forms;

namespace GMIT_Mastermind
{
    public partial class MainPage : ContentPage
    {
        SaveData save = new SaveData();

        private int selectedColor;
        Image selectedImage;

        static readonly string[] ColorNames = { "Red", "Green", "Blue", "Purple", "Yellow", "Turquoise" };

        List<Image> boardPegs = new List<Image>();
        List<Grid> scoreGridPegs = new List<Grid>();

        public MainPage()
        {
            InitializeComponent();

            GenerateBoardUI();
            GeneratePegsUI();

            NewGame();
        }

        private void NewGame()
        {
            save = new SaveData();

            Random rng = new Random();

            List<int> duplicates = new List<int>();

            for (int i = 0; i < 4; i++)
            {
                int r;

                // Make sure its not a duplicate
                do
                {
                    r = rng.Next(0, ColorNames.Length);
                } while (duplicates.Contains(r));

                duplicates.Add(r);

                // Cheat
                Debug.WriteLine(ColorNames[r]);

                save.target[i] = r;
            }
        }

        /// <summary>
        /// Generate the UI for the pegs at the bottom
        /// </summary>
        private void GeneratePegsUI()
        {
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += SelectPeg_Tapped;

            for (int i = 0; i < 6; i++)
            {
                Image peg = new Image();
                peg.StyleId = i.ToString();
                peg.Source = ColorNames[i] + ".png";
                peg.GestureRecognizers.Add(tapGestureRecognizer);
                PegsContainer.Children.Add(peg);
            }
        }

        /// <summary>
        /// Generate the complex main board UI
        /// </summary>
        private void GenerateBoardUI()
        {
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += PlacePeg_Tapped;

            for (int i = 0; i < 8; i++)
            {
                // Vertical Rounds list
                StackLayout roundLayout = new StackLayout();
                roundLayout.Orientation = StackOrientation.Horizontal;
                roundLayout.Padding = new Thickness(0, 8);

                Label numberLabel = new Label();
                numberLabel.Margin = new Thickness(6, 0);
                numberLabel.VerticalTextAlignment = TextAlignment.Center;
                numberLabel.Text = (8 - i).ToString();
                numberLabel.FontSize = 32;
                numberLabel.FontAttributes = FontAttributes.Bold;
                numberLabel.TextColor = Color.Black;
                roundLayout.Children.Add(numberLabel);

                // Guess Grid Square
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
                        Image bv = new Image();

                        bv.Opacity = 0.5f;

                        bv.HeightRequest = 20;
                        bv.WidthRequest = 20;

                        bv.SetValue(Grid.RowProperty, x);
                        bv.SetValue(Grid.ColumnProperty, y);

                        grid.Children.Add(bv);
                    }
                }

                guessFrame.Content = grid;
                roundLayout.Children.Add(guessFrame);

                // Board
                Frame pegChoiceFrame = new Frame();
                pegChoiceFrame.Margin = new Thickness(4, 0);
                pegChoiceFrame.CornerRadius = 10;
                pegChoiceFrame.BackgroundColor = Color.FromHex("#B7733C");
                pegChoiceFrame.Padding = new Thickness(0);


                StackLayout pegsLayout = new StackLayout();
                pegsLayout.Padding = new Thickness(8, 0);
                pegsLayout.Orientation = StackOrientation.Horizontal;

                pegsLayout.StyleId = (7 - i).ToString();

                for (int j = 0; j < 4; j++)
                {
                    Image peg = new Image();
                    peg.Source = "White.png";
                    if (i != 7)
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
                roundLayout.Children.Add(pegChoiceFrame);

                pegChoiceFrame.CornerRadius = 30;

                RoundsContainer.Children.Add(roundLayout);
            }
        }

        private void PlacePeg_Tapped(object sender, EventArgs e)
        {
            Image pegImg = (Image)sender;

            pegImg.Source = ColorNames[selectedColor] + ".png";

            save.board[int.Parse(pegImg.StyleId), save.round] = selectedColor;

            bool isValid = true;

            for (int i = 0; i < 4; i++)
            {
                if (save.board[i, save.round] == -1)
                {
                    isValid = false;
                }
            }

            CheckButton.IsEnabled = isValid;

            // Clear selection
            foreach (Image child in PegsContainer.Children)
            {
                child.BackgroundColor = Color.Transparent;
            }
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
            selectedImage = img;
        }

        private void Checked_Clicked(object sender, EventArgs e)
        {
            int whitePegs = 0;
            int blackPegs = 0;

            // Check number of black/white pegs to give
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (save.target[i] == save.board[j, save.round])
                    {
                        whitePegs++;

                        if (i == j)
                        {
                            blackPegs++;
                        }
                        break;
                    }
                }
            }

            int index = 0;

            foreach (Image peg in scoreGridPegs[7 - save.round].Children)
            {
                if (index < blackPegs)
                {
                    peg.Opacity = 1;
                    peg.Source = "Black.png";
                }
                else if (index < whitePegs)
                {
                    peg.Opacity = 1;
                    peg.Source = "White.png";
                }

                index++;
            }

            // Move on to next round
            save.round++;

            if (save.round >= 8)
            {
                // TODO end game
                return;
            }

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

            CheckButton.IsEnabled = false;
        }
    }
}
