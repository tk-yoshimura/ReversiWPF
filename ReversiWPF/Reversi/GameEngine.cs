using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

using ReversiEngine.Reversi;

namespace ReversiWPF.Reversi {
    public class GameEngine : INotifyPropertyChanged {
        readonly MainWindow presentation_window;

        Player player_ai;
        ManualPlayer player_manual;

        BattleEngine battle_engine;
        Stack<BattleEngine> battle_engine_history;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>コンストラクタ</summary>
        /// <param name="presentation_window">プレゼンテーション層</param>
        public GameEngine(MainWindow presentation_window) {
            this.presentation_window = presentation_window;
            this.player_manual = new ManualPlayer(Stone.Black);
            this.player_ai = new RandomPlayer(Stone.White);
            this.battle_engine = new BattleEngine(player_manual, player_ai);
            this.battle_engine_history = new Stack<BattleEngine>();
        }
        
        /// <summary>初期化</summary>
        /// <param name="player_class_name">対戦相手のAIの完全修飾クラス名</param>
        public void InitializeGameEngine(string player_class_name) {
            battle_engine_history.Clear();
            presentation_window.IsEnableButtonUndo = false;
            
            if(player_class_name == null) {
                StatusText = "対戦相手のAIを選択してください";
                NotifyPropertyChanged();

                player_manual = new ManualPlayer(Stone.Black);
                player_ai = new RandomPlayer(Stone.White);
                battle_engine = new BattleEngine(player_manual, player_ai);
                ReflashBoard();
                return;
            }

            Type type = Type.GetType(player_class_name);

            if(type == null) {
                throw new TypeLoadException();
            }

            Stone player_ai_stone = presentation_window.ManualPlayerStone == Stone.Black ? Stone.White : Stone.Black;

            player_ai = (Player)Activator.CreateInstance(type, player_ai_stone);

            if(player_ai == null) {
                throw new TypeLoadException();
            }

            if(presentation_window.ManualPlayerStone == Stone.White) {
                player_manual = new ManualPlayer(Stone.White);
                battle_engine = new BattleEngine(player_ai, player_manual);
                Next();
            }
            else {
                player_manual = new ManualPlayer(Stone.Black);
                battle_engine = new BattleEngine(player_manual, player_ai);

                StatusText = "あなたのターンです";
                NotifyPropertyChanged();
            }
            
            ReflashBoard();

            presentation_window.IsEnableButtonPass = false;
        }

        /// <summary>盤面を次の状態にする</summary>
        public void Next() {
            if(battle_engine.IsEndGame) {
                return;
            }

            StatusText = "対戦相手が次の一手を考えています...";
            NotifyPropertyChanged();
            
            if(battle_engine.NextColor == player_manual.Color){
                battle_engine_history.Push((BattleEngine)battle_engine.Clone());
                presentation_window.IsEnableButtonUndo = true;

                battle_engine.Next();
                ReflashBoard();
                Thread.Sleep(50);
            }

            battle_engine.Next();
            ReflashBoard();
            
            if(battle_engine.IsEndGame) {
                Stone superiority_color = battle_engine.SuperiorityColor;

                if(superiority_color == player_manual.Color) {
                    StatusText = "あなたの勝ちです";
                    NotifyPropertyChanged();
                }
                else if(superiority_color == Stone.None) {
                    StatusText = "引き分けです";
                    NotifyPropertyChanged();
                }
                else {
                    StatusText = "あなたの負けです";
                    NotifyPropertyChanged();
                }

                presentation_window.IsEnableButtonPass = false;
            }
            else {
                StatusText = "あなたのターンです";
                NotifyPropertyChanged();

                presentation_window.IsEnableButtonPass = !battle_engine.CurrentBoard.IsLocatable(player_manual.Color);
            }
        }

        /// <summary>盤面を差し戻す</summary>
        public void Undo() {
            if(battle_engine_history.Count <= 0) {
                return;
            }

            battle_engine = battle_engine_history.Pop();

            StatusText = "あなたのターンです";
            NotifyPropertyChanged();

            presentation_window.IsEnableButtonPass = !battle_engine.CurrentBoard.IsLocatable(player_manual.Color);
            if(battle_engine_history.Count <= 0) {
                presentation_window.IsEnableButtonUndo = false;
            }

            ReflashBoard();
        }

        public void ReflashBoard() {
            presentation_window.ReflashBoard();

            BlackStoneCount = battle_engine.CurrentBoard.CountStone(Stone.Black);
            WhiteStoneCount = battle_engine.CurrentBoard.CountStone(Stone.White);

            NotifyPropertyChanged();
        }

        /// <summary>マニュアルプレイヤーの次の手</summary>
        public Decision ManualNextDecision {
            set {
                if(value == null) {
                    return;
                }

                player_manual.NextDecision = value;
            }
        }

        /// <summary>マニュアルプレイヤーの次の手が現在の盤面に対して受理され得るか</summary>
        public bool IsValidManualNextDecision(Decision decision) {
            if(decision == null) {
                return false;
            }

            if(battle_engine.IsEndGame) {
                return false;
            }

            if(!decision.IsPass) {
                if(battle_engine.CurrentBoard.IsLocatable(decision.X, decision.Y, player_manual.Color)) {
                    return true;
                }
            }
            else {
                if(!battle_engine.CurrentBoard.IsLocatable(player_manual.Color)) {
                    return true;
                }
            }

            return false;
        }       

        /// <summary>現在の盤面</summary>
        public Board CurrentBoard {
            get {
                return battle_engine.CurrentBoard;
            }
        }

        /// <summary>StatusTextのバインディング</summary>
        public string StatusText {
            get;
            set;
        }

        /// <summary>BlackStoneCountのバインディング</summary>
        public int BlackStoneCount {
            get; set;
        }

        /// <summary>WhiteStoneCountのバインディング</summary>
        public int WhiteStoneCount {
            get; set;
        }

        /// <summary>プロパティが更新されたことをクライアントに通知 *WPFバインディング*</summary>
        /// <param name="property_name">プロパティ名　指定しなければ包含する全てのプロパティが対象</param>
        private void NotifyPropertyChanged(string property_name = "") {
            if(PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(property_name));
            }
        }
    }
}
