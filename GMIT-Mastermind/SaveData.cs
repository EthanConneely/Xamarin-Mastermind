
namespace GMIT_Mastermind
{
    public class SaveData
    {
        public int round;
        public int[] target;
        public int[,] board;
        public int[,] pegs;

        public SaveData()
        {
            target = new int[4];
            board = new int[8, 4];
            pegs = new int[8, 2];

            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    board[x, y] = -1;
                }
            }
        }
    }
}
