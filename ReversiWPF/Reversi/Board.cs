using System;

namespace ReversiEngine.Reversi {

    /// <summary>盤面</summary>
    public class Board : ICloneable{
        private int stone_count;
        private readonly Stone[,] board;

        /// <summary>コンストラクタ</summary>
        public Board() {
            this.stone_count = 0;
            this.board = new Stone[Size, Size];
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="board">石配列</param>
        public Board(Stone[,] board) {
            if(board == null) {
                throw new ArgumentNullException();
            }

            if(Size != board.GetLength(0) || Size != board.GetLength(1)) {
                throw new ArgumentException();
            }

            this.board = (Stone[,])board.Clone();

            for(int x, y = 0; y < Size; y++) {
                for(x = 0; x < Size; x++) {
                    if(this.board[x, y] != Stone.None) {
                        stone_count++;
                    }
                }
            }
        }

        /// <summary>コンストラクタ</summary>
        public Board(Board board) {
            if(board == null) {
                throw new ArgumentNullException();
            }

            this.board = (Stone[,])board.board.Clone();
            this.stone_count = board.stone_count;
        }

        /// <summary>盤面のサイズ</summary>
        public const int Size = 8;

        /// <summary>盤面インデクサ</summary>
        public Stone this [int x, int y]{
            get {
                return board[x, y];
            }
            set {
                if(board[x, y] == Stone.None && value != Stone.None) {
                    stone_count++;
                }
                else if(board[x, y] != Stone.None && value == Stone.None) {
                    stone_count--;
                }

                board[x, y] = value;
            }
        }

        /// <summary>石を配置可能か判定</summary>
        public bool IsLocatable(int x, int y, Stone color) {
            if(color == Stone.None) {
                throw new ArgumentException();
            }

            if(board[x, y] != Stone.None) {
                return false;
            }

            Func<int, int, bool> is_locatable_dxy = (dx, dy) => {
                int px = x + dx, py = y + dy;

                if(px < 0 || px >= Size || py < 0 || py >= Size) {
                    return false;
                }

                if(board[px, py] == color || board[px, py] == Stone.None) {
                    return false;
                }

                for(;;) {
                    px += dx;
                    py += dy;

                    if(px < 0 || px >= Size || py < 0 || py >= Size) {
                        return false;
                    }
                    
                    if(board[px, py] == Stone.None) {
                        return false;
                    }
                    if(board[px, py] == color) {
                        return true;
                    }
                }
            };

            if(is_locatable_dxy(-1, -1)) {
                return true;
            }
            if(is_locatable_dxy(-1,  0)) {
                return true;
            }
            if(is_locatable_dxy(-1, +1)) {
                return true;
            }
            if(is_locatable_dxy( 0, -1)) {
                return true;
            }
            if(is_locatable_dxy( 0, +1)) {
                return true;
            }
            if(is_locatable_dxy(+1, -1)) {
                return true;
            }
            if(is_locatable_dxy(+1,  0)) {
                return true;
            }
            if(is_locatable_dxy(+1, +1)) {
                return true;
            }

            return false;
        }

        /// <summary>石を配置可能か判定</summary>
        public bool IsLocatable(Stone color) {
            for(int x, y = 0; y < Size; y++) {
                for(x = 0; x < Size; x++) {
                    if(IsLocatable(x, y, color)) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>石を隅に配置可能か判定</summary>
        public bool IsCornerLocatable(Stone color) {
            return IsLocatable(0, 0, color) || IsLocatable(Size - 1, 0, color) || IsLocatable(0, Size - 1, color) || IsLocatable(Size - 1, Size - 1, color);
        }

        /// <summary>石を配置</summary>
        public void Locate(int x, int y, Stone color) {
            bool flip_stone = false;

            Action<int, int> locate_dxy = (dx, dy) => {
                int px = x, py = y;

                for(;;) {
                    px += dx;
                    py += dy;

                    if(px < 0 || px >= Size || py < 0 || py >= Size) {
                        return;
                    }

                    if(board[px, py] == Stone.None) {
                        return;
                    }

                    if(board[px, py] == color) {
                        break;
                    }
                }

                px -= dx;
                py -= dy;

                while(x != px || y != py) {
                    board[px, py] = color;
                    flip_stone = true;

                    px -= dx;
                    py -= dy;
                }
            };
            
            locate_dxy(-1, -1);
            locate_dxy(-1,  0);
            locate_dxy(-1, +1);
            locate_dxy( 0, -1);
            locate_dxy( 0, +1);
            locate_dxy(+1, -1);
            locate_dxy(+1,  0);
            locate_dxy(+1, +1);

            if(flip_stone) {
                board[x, y] = color;
                stone_count++;
            }
        }

        /// <summary>石の信頼性を判定</summary>
        public StoneReliability JudgeReliability(int x, int y) {
            Stone color = board[x, y];
            StoneReliability reliability = StoneReliability.Definite;

            if(color == Stone.None) {
                return StoneReliability.None;
            }
            
            Func<int, int, bool> isexist_opposite_dxy = (dx, dy) => {
                int px = x, py = y;

                for(;;) {
                    px += dx;
                    py += dy;

                    if(px < 0 || px >= Size || py < 0 || py >= Size) {
                        return false;
                    }
                    
                    if(board[px, py] == Stone.None) {
                        return false;
                    }

                    if(board[px, py] != color) {
                        return true;
                    }
                }
            };

            Func<int, int, bool> is_edge_dxy = (dx, dy) => {
                int px = x, py = y;

                for(;;) {
                    px += dx;
                    py += dy;

                    if(px < 0 || px >= Size || py < 0 || py >= Size) {
                        return true;
                    }

                    if(board[px, py] == Stone.None) {
                        return false;
                    }
                }
            };

            Action<int, int> renovate_reliability = (dx, dy) => {
                bool wall_a, wall_b, stone_a, stone_b;

                wall_a = is_edge_dxy(dx, dy);
                wall_b = is_edge_dxy(-dx, -dy);

                if(wall_a && wall_b) {
                    return;
                }

                stone_a = isexist_opposite_dxy(dx, dy);
                stone_b = isexist_opposite_dxy(-dx, -dy);

                if(wall_a && !wall_b && !stone_a) {
                    return;
                }
                if(!wall_a && wall_b && !stone_b) {
                    return;
                }

                if(stone_a && !stone_b && !wall_b) {
                    reliability = StoneReliability.Uncertain;
                    return;
                }
                if(!stone_a && stone_b && !wall_a) {
                    reliability = StoneReliability.Uncertain;
                    return;
                }

                reliability = StoneReliability.Interim;
            };

            renovate_reliability(1, 0);
            if(reliability == StoneReliability.Uncertain) {
                return reliability;
            }
            
            renovate_reliability(0, 1);
            if(reliability == StoneReliability.Uncertain) {
                return reliability;
            }

            renovate_reliability(1, 1);
            if(reliability == StoneReliability.Uncertain) {
                return reliability;
            }

            renovate_reliability(1, -1);
            if(reliability == StoneReliability.Uncertain) {
                return reliability;
            }

            return reliability;
        }

        /// <summary>石の個数</summary>
        public int StoneCount {
            get{
                return stone_count;
            }
        }

        /// <summary>石の個数をカウント</summary>
        public int CountStone(Stone stone) {
            int cnt = 0;

            for(int x, y = 0; y < Size; y++) {
                for(x = 0; x < Size; x++) {
                    if(board[x, y] == stone) {
                        cnt++;
                    }
                }
            }

            return cnt;
        }

        /// <summary>初期配置</summary>
        public static Board Init() {
            Board ret = new Board();

            ret[3, 3] = Stone.White;
            ret[3, 4] = Stone.Black;
            ret[4, 3] = Stone.Black;
            ret[4, 4] = Stone.White;

            return ret;
        }

        /// <summary>盤面が埋まっているか否か</summary>
        public bool IsFill {
            get {
                for(int x, y = 0; y < Size; y++) {
                    for(x = 0; x < Size; x++) {
                        if(board[x, y] == Stone.None) {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>ゲームセットか否か</summary>
        public bool IsEndGame {
            get {
                if(IsFill || !IsExistStone(Stone.White) || !IsExistStone(Stone.Black)) {
                    return true;
                }

                if(!IsLocatable(Stone.White) && !IsLocatable(Stone.Black)) {
                    return true;
                }

                return false;
            }
        }

        /// <summary>石が存在するか否か</summary>
        public bool IsExistStone(Stone stone) {
            for(int x, y = 0; y < Size; y++) {
                for(x = 0; x < Size; x++) {
                    if(board[x, y] == stone) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>可視化</summary>
        public override string ToString() {
            string str = "";
            for(int x, y = 0; y < Size; y++) {
                for(x = 0; x < Size; x++) {
                    if(board[x, y] == Reversi.Stone.White) {
                        str += "□";
                    }
                    else if(board[x, y] == Reversi.Stone.Black) {
                        str += "■";
                    }
                    else {
                        str += "  ";
                    }
                }

                str += "\n";
            }

            return str;
        }

        /// <summary>クローン</summary>
        public object Clone() {
            return new Board(this);
        }
    }
}
