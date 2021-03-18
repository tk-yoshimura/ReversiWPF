using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReversiEngine.Reversi;

namespace ReversiWPF.Reversi {
    /// <summary>マニュアルプレイヤー</summary>
    public class ManualPlayer : Player {
        Stone player_color;
        Decision next_decision;

        /// <summary>コンストラクタ</summary>
        /// <param name="player_color">石の色</param>
        public ManualPlayer(Stone player_color) {
            this.player_color = player_color;
        }

        /// <summary>次の手を決定</summary>
        /// <param name="current_board">盤面</param>
        /// <returns>決定オブジェクト</returns>
        public Decision Play(Board current_board) {
            if(next_decision == null) {
                throw new InvalidOperationException();
            }

            if(!next_decision.IsPass) {
                if(!current_board.IsLocatable(next_decision.X, next_decision.Y, player_color)) {
                    throw new ArgumentException();
                }
            }

            var ret = next_decision;
            next_decision = null;

            return ret;
        }

        /// <summary>次の手</summary>
        public Decision NextDecision{
            set {
                this.next_decision = value;
            }
        }

        /// <summary>石の色</summary>
        public Stone Color {
            get {
                return player_color;
            }
        }
    }
}
