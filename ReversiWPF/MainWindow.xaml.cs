using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Xml;
using System.Reflection;

using ReversiEngine.Reversi;

namespace ReversiWPF {
    /// <summary>MainWindow.xaml の相互作用ロジック</summary>
    public partial class MainWindow : Window {
        Dictionary<string, string> player_dictionary;
        Reversi.GameEngine game_engine;

        /// <summary>コンストラクタ</summary>
        public MainWindow() {
            InitializeComponent();

            player_dictionary = new Dictionary<string, string>();
            game_engine = new Reversi.GameEngine(this);

            this.DataContext = game_engine;

            DisableAllButtons();
            InitializeComboBoxSelectAI();
        }

        /// <summary>対戦相手のAI選択リストの初期化</summary>
        public void InitializeComboBoxSelectAI() {
            Assembly asm = Assembly.GetExecutingAssembly();

            using(Stream stream = asm.GetManifestResourceStream("ReversiWPF.Resources.players.xml")) {
                XmlDocument xml_document = new XmlDocument();
                xml_document.Load(stream);

                foreach(XmlElement elem in xml_document.DocumentElement) {
                    string player_name = elem.GetAttribute("name");
                    string player_class_name = elem.GetAttribute("className");

                    player_dictionary.Add(player_name, player_class_name);
                }
            }

            var select_list = player_dictionary.Select(x => x.Key).ToList();

            select_list.Insert(0, "対戦相手のAI");

            combobox_select_AI.ItemsSource = select_list;
            combobox_select_AI.SelectedIndex = 0;
        }

        /// <summary>盤面および石の数の描画更新 *即時*</summary>
        public void ReflashBoard() {
            const double stone_margin = 4;
            double stone_w = canvas_board.Width / Board.Size, stone_h = canvas_board.Height / Board.Size;
            
            for(int i = canvas_board.Children.Count - 1; i >= 0; i--) {
                if(canvas_board.Children[i].GetType() == typeof(Ellipse)) {
                    canvas_board.Children.RemoveAt(i);
                }
            }
            
            for(int x, y = 0, index = 0; y < Board.Size; y++) {
                for(x = 0; x < Board.Size; x++, index++) {
                    if(game_engine.CurrentBoard[x, y] == Stone.None) {
                        continue;
                    }

                    var stone = new Ellipse();

                    stone.Margin = new Thickness(x * stone_w + stone_margin, y * stone_h + stone_margin, 0, 0);
                    stone.Width = stone_w - stone_margin * 2;
                    stone.Height = stone_h - stone_margin * 2;

                    if(game_engine.CurrentBoard[x, y] == Stone.Black) {
                        stone.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
                    }
                    else {
                        stone.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                        stone.Stroke = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
                        stone.StrokeThickness = 1.5;
                    }

                    canvas_board.Children.Add(stone);
                }
            }

            label_blackcount.Content = game_engine.CurrentBoard.CountStone(Stone.Black);
            label_whitecount.Content = game_engine.CurrentBoard.CountStone(Stone.White);

            DoRenderEvents();
        }
        
        /// <summary>盤面キャンバスのマウス押下イベント</summary>
        private void OnMouseDownCanvasBoard(object sender, MouseButtonEventArgs e) {
            if(SelectPlayer == null) {
                return;
            }

            Point mouse_point = e.GetPosition(canvas_board);
            int decision_x = (int)(mouse_point.X * Board.Size / canvas_board.Width);
            int decision_y = (int)(mouse_point.Y * Board.Size / canvas_board.Height);

            if(decision_x < 0 || decision_x >= Board.Size) {
                return;
            }
            if(decision_y < 0 || decision_y >= Board.Size) {
                return;
            }

            Decision decision = Decision.Place(decision_x, decision_y);

            if(game_engine.IsValidManualNextDecision(decision)) {
                game_engine.ManualNextDecision = decision;
                game_engine.Next();
            }
        }

        /// <summary>パスボタンのクリックイベント</summary>
        private void OnClickButtonPass(object sender, RoutedEventArgs e) {
            Decision decision = Decision.Pass;

            if(game_engine.IsValidManualNextDecision(decision)) {
                game_engine.ManualNextDecision = decision;
                game_engine.Next();
            }
        }

        /// <summary>指し直しボタンのクリックイベント</summary>
        private void OnClickButtonReset(object sender, RoutedEventArgs e) {
            game_engine.InitializeGameEngine(SelectPlayer);
        }
        
        /// <summary>対戦相手のAI選択リストの選択変更イベント</summary>
        private void OnSelectionChangedComboboxSelectAI(object sender, SelectionChangedEventArgs e) {
            IsEnableButtonReset = (SelectPlayer != null);
            game_engine.InitializeGameEngine(SelectPlayer);
        }
        
        /// <summary>先攻ラジオボタンのクリックイベント</summary>
        private void OnClickRadiobuttonPrev(object sender, RoutedEventArgs e) {
            game_engine.InitializeGameEngine(SelectPlayer);
        }

        /// <summary>後攻ラジオボタンのクリックイベント</summary>
        private void OnClickRadiobuttonPost(object sender, RoutedEventArgs e) {
            game_engine.InitializeGameEngine(SelectPlayer);
        }

        /// <summary>指し直しボタンのクリックイベント</summary>
        private void OnClickButtonUndo(object sender, RoutedEventArgs e) {
            game_engine.Undo();
        }

        /// <summary>選択されている対戦相手のAIの完全修飾クラス名</summary>
        public string SelectPlayer {
            get {
                string player_name = (string)combobox_select_AI.SelectedValue;

                if(!player_dictionary.ContainsKey(player_name)) {
                    return null;
                }

                return player_dictionary[player_name];
            }
        }
        
        /// <summary>対戦相手のAI選択リストの有効無効</summary>
        public bool IsEnableComboBoxSelectAI {
            set {
                combobox_select_AI.IsEnabled = value;
            }
        }

        /// <summary>パスボタンの有効無効</summary>
        public bool IsEnableButtonPass {
            set {
                button_pass.IsEnabled = value;
            }
        }
        
        /// <summary>最初からボタンの有効無効</summary>
        public bool IsEnableButtonReset {
            set {
                button_reset.IsEnabled = value;
            }
        }

        /// <summary>指し直しボタンの有効無効</summary>
        public bool IsEnableButtonUndo {
            set {
                button_undo.IsEnabled = value;
            }
        }

        /// <summary>ボタンUI要素を全て無向化</summary>
        public void DisableAllButtons() {
            button_reset.IsEnabled = false;
            button_pass.IsEnabled = false;
            button_undo.IsEnabled = false;
        }

        /// <summary>マニュアルプレイヤーの先攻・後攻</summary>
        public Stone ManualPlayerStone {
            get {
                if(radiobutton_prev.IsChecked.Value) {
                    return Stone.Black;
                }
                else {
                    return Stone.White;
                }
            }
        }

        /// <summary>現在メッセージ待ち行列の中にある描画UIメッセージの処理 *非同期 ディスパッチャ使用*</summary>
        private void DoRenderEvents() {
            DispatcherFrame frame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(obj => {
                ((DispatcherFrame)obj).Continue = false;
                return null;
            });
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, callback, frame);
            Dispatcher.PushFrame(frame);
        }        
    }
}
