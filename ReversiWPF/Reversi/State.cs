using System;
using System.Collections.Generic;

using GameTreeSearch;

namespace ReversiEngine.Reversi {

    ///<summary>プレイ状態クラス</summary>
    public class State : IState<Decision>, ICloneable {

        /// <summary>コンストラクタ</summary>
        public State(Board board, Evaluator evaluator, Stone PlayerColor, Stone NextColor, bool is_finale_mode = false) {
            if(board == null || evaluator == null) {
                throw new ArgumentNullException();
            }

            if(PlayerColor == Stone.None || NextColor == Stone.None) {
                throw new ArgumentException();
            }

            this.Board = (Board)board.Clone();
            this.Evaluator = evaluator;
            this.PlayerColor = PlayerColor;
            this.NextColor = NextColor;
            this.IsFinaleMode = is_finale_mode;
        }

        /// <summary>盤面状態</summary>
        public Board Board {
            get; private set;
        }

        /// <summary>評価法</summary>
        public Evaluator Evaluator {
            get; private set;
        }

        /// <summary>プレイヤーの石の色</summary>
        public Stone PlayerColor {
            get; private set;
        }

        /// <summary>次に打つ石の色</summary>
        public Stone NextColor {
            get; private set;
        }

        /// <summary>終局読みモードを有効にするか</summary>
        public bool IsFinaleMode {
            get; private set;
        }

        /// <summary>盤面の評価</summary>
        public double Evaluation {
            get {
                return Evaluator.Evaluate(Board, PlayerColor, IsFinaleMode);
            }
        }

        /// <summary>ゲームセットか否か</summary>
        public bool IsEndGame => Board.IsEndGame;

        /// <summary>次の手をコレクションとして返す</summary>
        public IEnumerable<Decision> NextDecisions() {
            bool exist_locatable = false;

            for(int i, j = 0; j < Board.Size / 2; j++) {
                if(Board.IsLocatable(j, j, NextColor)) {
                    exist_locatable = true;
                    yield return Decision.Place(j, j);
                }
                if(Board.IsLocatable(Board.Size - j - 1, j, NextColor)) {
                    exist_locatable = true;
                    yield return Decision.Place(Board.Size - j - 1, j);
                }
                if(Board.IsLocatable(j, Board.Size - j - 1, NextColor)) {
                    exist_locatable = true;
                    yield return Decision.Place(j, Board.Size - j - 1);
                }
                if(Board.IsLocatable(Board.Size - j - 1, Board.Size - j - 1, NextColor)) {
                    exist_locatable = true;
                    yield return Decision.Place(Board.Size - j - 1, Board.Size - j - 1);
                }

                for(i = j + 1; i < Board.Size / 2; i++) {
                    if(Board.IsLocatable(i, j, NextColor)) {
                        exist_locatable = true;
                        yield return Decision.Place(i, j);
                    }
                    if(Board.IsLocatable(Board.Size - i - 1, j, NextColor)) {
                        exist_locatable = true;
                        yield return Decision.Place(Board.Size - i - 1, j);
                    }
                    if(Board.IsLocatable(i, Board.Size - j - 1, NextColor)) {
                        exist_locatable = true;
                        yield return Decision.Place(i, Board.Size - j - 1);
                    }
                    if(Board.IsLocatable(Board.Size - i - 1, Board.Size - j - 1, NextColor)) {
                        exist_locatable = true;
                        yield return Decision.Place(Board.Size - i - 1, Board.Size - j - 1);
                    }
                    if(Board.IsLocatable(j, i, NextColor)) {
                        exist_locatable = true;
                        yield return Decision.Place(j, i);
                    }
                    if(Board.IsLocatable(j, Board.Size - i - 1, NextColor)) {
                        exist_locatable = true;
                        yield return Decision.Place(j, Board.Size - i - 1);
                    }
                    if(Board.IsLocatable(Board.Size - j - 1, i, NextColor)) {
                        exist_locatable = true;
                        yield return Decision.Place(Board.Size - j - 1, i);
                    }
                    if(Board.IsLocatable(Board.Size - j - 1, Board.Size - i - 1, NextColor)) {
                        exist_locatable = true;
                        yield return Decision.Place(Board.Size - j - 1, Board.Size - i - 1);
                    }
                }
            }

            if(!exist_locatable) {
                yield return Decision.Pass;
            }
        }

        /// <summary>次の盤面状態を生成する</summary>
        public IState<Decision> NextState(Decision decision) {
            State ret = (State)Clone();

            if(!decision.IsPass) {
                ret.Board.Locate(decision.X, decision.Y, NextColor);
            }

            if(NextColor == Stone.Black) {
                ret.NextColor = Stone.White;
            }
            else if(NextColor == Stone.White) {
                ret.NextColor = Stone.Black;
            }

            return ret;
        }

        /// <summary>次に隅が配置可能ならばその打つ手を返す</summary>
        /// <remarks>隅に配置不能ならばnullを返す</remarks>
        public Decision NextCornerDecision() {
            double ev, max_ev = double.NegativeInfinity;
            Decision decision, max_ev_decision = null;

            if(Board.IsLocatable(0, 0, NextColor)) {
                decision = Decision.Place(0, 0);
                ev = NextState(decision).Evaluation;
                if(max_ev < ev) {
                    max_ev = ev;
                    max_ev_decision = decision;
                }
            }
            if(Board.IsLocatable(0, Board.Size - 1, NextColor)) {
                decision = Decision.Place(0, Board.Size - 1);
                ev = NextState(decision).Evaluation;
                if(max_ev < ev) {
                    max_ev = ev;
                    max_ev_decision = decision;
                }
            }
            if(Board.IsLocatable(Board.Size - 1, 0, NextColor)) {
                decision = Decision.Place(Board.Size - 1, 0);
                ev = NextState(decision).Evaluation;
                if(max_ev < ev) {
                    max_ev = ev;
                    max_ev_decision = decision;
                }
            }
            if(Board.IsLocatable(Board.Size - 1, Board.Size - 1, NextColor)) {
                decision = Decision.Place(Board.Size - 1, Board.Size - 1);
                ev = NextState(decision).Evaluation;
                if(max_ev < ev) {
                    max_ev = ev;
                    max_ev_decision = decision;
                }
            }

            return max_ev_decision;
        }

        /// <summary>石の個数をカウント</summary>
        public int CountStone(Stone stone) {
            return Board.CountStone(stone);
        }

        /// <summary>クローン</summary>
        public object Clone() {
            return new State(Board, Evaluator, PlayerColor, NextColor, IsFinaleMode);
        }
    }
}
