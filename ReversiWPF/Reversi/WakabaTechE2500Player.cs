﻿using GameTreeSearch;

namespace ReversiEngine.Reversi {
    ///<summary>吉村プレイヤー (ゲーム木のノード評価数 2500)</summary>
    public class WakabaTechE2500Player : Player {
        Stone player_color;
        Evaluator evaluator;
        
        /// <summary>コンストラクタ</summary>
        /// <param name="player_color">石の色</param>
        public WakabaTechE2500Player(Stone player_color) {
            this.player_color = player_color;

            evaluator = new Evaluator();
        }

        /// <summary>次の手を打つ</summary>
        /// <returns>決定オブジェクトを返す</returns>
        public Decision Play(Board current_board) {
            if(current_board.CountStone(Stone.None) > 10) {
                State root_state = new State(current_board, evaluator, player_color, player_color);
                return DiscontinuableIDDFSMethod<State, Decision>.Search(root_state, 4, 12, 2500, Decision.Pass); 
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
