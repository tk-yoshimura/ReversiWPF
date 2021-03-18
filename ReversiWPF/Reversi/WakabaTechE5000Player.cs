using GameTreeSearch;

namespace ReversiEngine.Reversi {
    ///<summary>吉村プレイヤー (ゲーム木のノード評価数 5000)</summary>
    public class WakabaTechE5000Player : Player {
        Stone player_color;
        Evaluator evaluator;
        
        /// <summary>コンストラクタ</summary>
        public WakabaTechE5000Player(Stone player_color) {
            this.player_color = player_color;

            evaluator = new Evaluator();
        }

        /// <summary>次の手を打つ</summary>
        /// <returns>決定オブジェクトを返す</returns>
        public Decision Play(Board current_board) {
            if(current_board.CountStone(Stone.None) > 10) {
                State root_state = new State(current_board, evaluator, player_color, player_color);
                return DiscontinuableIDDFSMethod<State, Decision>.Search(root_state, 5, 12, 5000, Decision.Pass); 
            }
            else {
                State root_state = new State(current_board, evaluator, player_color, player_color, is_finale_mode: true);
                return CompleteSearchMethod<State, Decision>.Search(root_state, Decision.Pass);
            }
        }

        /// <summary>プレイヤーの色</summary>
        public Stone Color {
            get {
                return player_color;
            }
        }
    }
}
