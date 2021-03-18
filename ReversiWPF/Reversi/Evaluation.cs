using System;

namespace ReversiEngine.Reversi {

    /// <summary>評価法クラス</summary>
    public class Evaluator {
        static readonly int[,] ev_table_uncertain = new int[Board.Size, Board.Size]
            { {    0, -798, -10, -546, -546, -10, -798,    0 },
              { -798, -501, -40, -150, -150, -40, -501, -798 },
              {  -10,  -40,  26,   49,   49,  26,  -40,  -10 },
              { -546, -150,  49,  -50,  -50,  49, -150, -546 },
              { -546, -150,  49,  -50,  -50,  49, -150, -546 },
              {  -10,  -40,  26,   49,   49,  26,  -40,  -10 },
              { -798, -501, -40, -150, -150, -40, -501, -798 },
              {    0, -798, -10, -546, -546, -10, -798,    0 }};

        static readonly int[,] ev_table_interim = new int[Board.Size, Board.Size]
            { {    0, -532, -224, -111, -111, -224, -532,    0 },
              { -532, -494,  -32, -156, -156,  -32, -494, -532 },
              { -224,  -32,   27,   49,   49,   27,  -32, -224 },
              { -111, -156,   49,   22,   22,   49, -156, -111 },
              { -111, -156,   49,   22,   22,   49, -156, -111 },
              { -224,  -32,   27,   49,   49,   27,  -32, -224 },
              { -532, -494,  -32, -156, -156,  -32, -494, -532 },
              {    0, -532, -224, -111, -111, -224, -532,    0 }};
        
        static readonly int[,] ev_table_definite = new int[Board.Size, Board.Size]
            { { 2500, 767, 607, 642, 642, 607, 767, 2500 },
              {  767, 344, 284, 103, 103, 284, 344,  767 },
              {  607, 284, 114, 182, 182, 114, 284,  607 },
              {  642, 103, 182, 202, 202, 182, 103,  642 },
              {  642, 103, 182, 202, 202, 182, 103,  642 },
              {  607, 284, 114, 182, 182, 114, 284,  607 },
              {  767, 344, 284, 103, 103, 284, 344,  767 },
              { 2500, 767, 607, 642, 642, 607, 767, 2500 }};
        
        /// <summary>コンストラクタ</summary>
        public Evaluator() { }

        /// <summary>評価</summary>
        /// <param name="board">盤面</param>
        /// <param name="evaluate_color">評価する石</param>
        /// <param name="finale_mode">終局読みモードを有効にするか</param>
        /// <returns>評価値</returns>
        public double Evaluate(Board board, Stone evaluate_color, bool finale_mode = false) {
            if(evaluate_color == Stone.None) {
                throw new ArgumentException();
            }

            if(finale_mode) {
                return board.CountStone(evaluate_color);
            }

            if(!board.IsExistStone(evaluate_color)) {
                return double.MinValue;
            }

            if(!board.IsExistStone((evaluate_color != Stone.Black) ? Stone.Black : Stone.White)) {
                return double.MaxValue;
            }
            
            int ev = 0, evaluate_stone_cnt = 0, opponent_stone_cnt = 0;
            bool isexist_uncertain_evaluate_stone = false, isexist_uncertain_opponent_stone = false;

            for(int x, y = 0; y < Board.Size; y++) {
                for(x = 0; x < Board.Size; x++) {
                    if(board[x, y] == Stone.None) {
                        continue;
                    }

                    if(board[x, y] == evaluate_color) {
                        evaluate_stone_cnt++;

                        var stone_reliability = board.JudgeReliability(x, y);

                        switch(stone_reliability) {
                            case StoneReliability.Uncertain:
                                isexist_uncertain_evaluate_stone = true;
                                ev += ev_table_uncertain[x, y];
                                break;
                            case StoneReliability.Interim:
                                ev += ev_table_interim[x, y];
                                break;
                            case StoneReliability.Definite:
                                ev += ev_table_definite[x, y];
                                break;
                        }
                    }
                    else{
                        opponent_stone_cnt++;

                        var stone_reliability = board.JudgeReliability(x, y);

                        switch(stone_reliability) {
                            case StoneReliability.Uncertain:
                                isexist_uncertain_opponent_stone = true;
                                ev -= ev_table_uncertain[x, y];
                                break;
                            case StoneReliability.Interim:
                                ev -= ev_table_interim[x, y];
                                break;
                            case StoneReliability.Definite:
                                ev -= ev_table_definite[x, y];
                                break;
                        }
                    }
                }
            }

            if(evaluate_stone_cnt > opponent_stone_cnt && !isexist_uncertain_evaluate_stone) {
                return ev + 1.0e+6;
            }
            if(evaluate_stone_cnt < opponent_stone_cnt && !isexist_uncertain_opponent_stone) {
                return ev - 1.0e+6;
            }
            
            return ev;
        }
    }
}
