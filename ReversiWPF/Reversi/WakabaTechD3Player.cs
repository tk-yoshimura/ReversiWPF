using GameTreeSearch;

namespace ReversiEngine.Reversi {
    ///<summary>吉村プレイヤー (ゲーム木の深さ 3)</summary>
    public class WakabaTechD3Player : Player {
        Stone player_color;
        Evaluator evaluator;
        
        /// <summary>コンストラクタ</summary>
        /// <param name="player_color">石の色</param>
        public WakabaTechD3Player(Stone player_color) {
            this.player_color = player_color;

            evaluator = new Evaluator();
        }

        /// <summary>次の手を打つ</summary>
        /// <returns>決定オブジェクトを返す</returns>
        public Decision Play(Board current_board) {
            State root_state = new State(current_board, evaluator, player_color, player_color);

            return IterativeDeepeningDepthFirstSearchMethod<State, Decision>.Search(root_state, 3, Decision.Pass);
        }

        /// <summary>プレイヤーの色</summary>
        public Stone Color {
            get {
                return player_color;
            }
        }
    }
}
