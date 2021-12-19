
namespace GMIT_Mastermind
{
    public class SaveData
    {
        public int round;
        public int[] target;
        public int[,] board;

        public SaveData()
        {
            target = new int[4];
            board = new int[4, 8];

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
