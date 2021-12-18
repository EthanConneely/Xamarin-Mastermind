using System;

using Xamarin.Forms;

namespace GMIT_Mastermind
{
    public partial class MainPage : ContentPage
    {
        SaveData save;

        private int selectedColor;
        Image selectedImage;

        static readonly string[] ColorNames = { "Red", "Green", "Blue", "Purple", "Yellow", "Orange" };


        public MainPage()
        {
            InitializeComponent();

            GenerateBoardUI();

            GeneratePegsUI();
        }

        /// <summary>
        /// Generate the UI for the pegs at the bottom
        /// </summary>
        private void GeneratePegsUI()
        {
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += SelectPeg;

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
            tapGestureRecognizer.Tapped += PlacePeg;

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
                        bv.Source = "Black.png";

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
                    pegsLayout.Children.Add(peg);
                }

                pegChoiceFrame.Content = pegsLayout;
                roundLayout.Children.Add(pegChoiceFrame);

                pegChoiceFrame.CornerRadius = 30;

                RoundsContainer.Children.Add(roundLayout);
            }
        }

        private async void PlacePeg(object sender, EventArgs e)
        {
            Image img = (Image)sender;

            img.Source = ColorNames[selectedColor] + ".png";
        }

        private void SelectPeg(object sender, EventArgs e)
        {
            Image img = (Image)sender;

            foreach (Image child in PegsContainer.Children)
            {
                child.BackgroundColor = Color.Transparent;
            }

            img.BackgroundColor = new Color(1f, 1f, .25f);

            selectedColor = int.Parse(img.StyleId);
            selectedImage = img;
        }
    }
}
