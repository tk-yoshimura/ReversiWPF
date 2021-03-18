using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiEngine.Reversi {

    /// <summary>対戦</summary>
    public class BattleEngine : ICloneable{
        readonly Board board;

        /// <summary>コンストラクタ</summary>
        /// <param name="player_black">先攻プレイヤー</param>
        /// <param name="player_white">後攻プレイヤー</param>
        public BattleEngine(Player player_black, Player player_white) : this(Board.Init(), player_black, player_white) {
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="board">盤面状態</param>
        /// <param name="player_black">先攻プレイヤー</param>
        /// <param name="player_white">後攻プレイヤー</param>
        public BattleEngine(Board board, Player player_black, Player player_white) {
            if(player_black == null || player_white == null) {
                throw new ArgumentNullException();
            }

            if(player_black.Color != Stone.Black || player_white.Color != Stone.White) {
                throw new ArgumentException();
            }

            this.board = (Board)board.Clone();
            this.PlayerBlack = player_black;
            this.PlayerWhite = player_white;
            this.NextColor = Stone.Black;
        }

        /// <summary>次のプレイヤー</summary>
        public Stone NextColor {
            get; private set;
        }

        /// <summary>現在の盤面（クローン）</summary>
        public Board CurrentBoard {
            get {
                return (Board)board.Clone();
            }
        }

        /// <summary>先攻プレーヤー</summary>
        public Player PlayerBlack {
            get; private set;
        }

        /// <summary>後攻プレーヤー</summary>
        public Player PlayerWhite {
            get; private set;
        }

        /// <summary>次の手</summary>
        public void Next() {
            if(!board.IsEndGame) {
                if(NextColor == Stone.Black) {
                    Decision decision = PlayerBlack.Play(board);
                    if(decision != null && !decision.IsPass) {
                        board.Locate(decision.X, decision.Y, Stone.Black);
                    }

                    NextColor = Stone.White;
                }
                else {
                    Decision decision = PlayerWhite.Play(board);
                    if(decision != null && !decision.IsPass) {
                        board.Locate(decision.X, decision.Y, Stone.White);
                    }

                    NextColor = Stone.Black;
                }
            }
        }

        /// <summary>ゲームセットか否か</summary>
        public bool IsEndGame => board.IsEndGame;
        
        /// <summary>石の個数をカウント</summary>
        /// <param name="stone">石の色</param>
        /// <returns>石の個数</returns>
        public int CountStone(Stone stone) {
            return board.CountStone(stone);
        }

        /// <summary>優勢となっている石の色</summary>
        /// <remarks>引き分けならばNoneを返す</remarks>
        public Stone SuperiorityColor {
            get {
                if(CountStone(Stone.Black) > CountStone(Stone.White)) {
                    return Stone.Black;
                }
                else if(CountStone(Stone.Black) < CountStone(Stone.White)) {
                    return Stone.White;
                }
                else {
                    return Stone.None;
                }
            }
        }

        /// <summary>可視化</summary>
        public override string ToString() {
            return board.ToString();
        }

        /// <summary>クローン</summary>
        /// <returns>盤面と次の手を複製したオブジェクト</returns>
        public object Clone() {
            BattleEngine ret = new BattleEngine(CurrentBoard, PlayerBlack, PlayerWhite);
            ret.NextColor = NextColor;

            return ret;
        }
    }
}
