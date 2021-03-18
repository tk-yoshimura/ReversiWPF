using System;
using System.Linq;

namespace ReversiEngine.Reversi {
    ///<summary>ランダムプレイヤー</summary>
    public class RandomPlayer : Player {
        System.Random rd;
        Stone player_color;
                
        /// <summary>コンストラクタ</summary>
        /// <param name="player_color">石の色</param>
        public RandomPlayer(Stone player_color){
            this.rd = new Random();
            
            this.player_color = player_color;
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="player_color">石の色</param>
        /// <param name="rd">次の手をランダムに決定する乱数生成器</param>
        public RandomPlayer(Stone player_color, Random rd){
            this.rd = rd;
            
            this.player_color = player_color;
        }

        /// <summary>次の手を打つ</summary>
        /// <returns>決定オブジェクトを返す</returns>
        public Decision Play(Board current_board) {
            if(current_board.IsLocatable(0, 0, player_color)) {
                return Decision.Place(0, 0);
            }
            if(current_board.IsLocatable(0, Board.Size - 1, player_color)) {
                return Decision.Place(0, Board.Size - 1);
            }
            if(current_board.IsLocatable(Board.Size - 1, 0, player_color)) {
                return Decision.Place(Board.Size - 1, 0);
            }
            if(current_board.IsLocatable(Board.Size - 1, Board.Size - 1, player_color)) {
                return Decision.Place(Board.Size - 1, Board.Size - 1);
            }

            State root_state = new State(current_board, new Evaluator(), player_color, player_color);

            var decisions = root_state.NextDecisions().ToArray();

            return decisions[rd.Next(decisions.Length)];
        }

        /// <summary>プレイヤーの色</summary>
        public Stone Color {
            get {
                return player_color;
            }
        }
    }
}
