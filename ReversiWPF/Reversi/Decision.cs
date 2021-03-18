using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiEngine.Reversi {

    /// <summary>棋譜</summary>
    public class Decision {

        /// <summary>X座標</summary>
        public int X {
            get; private set;
        } = 0;

        /// <summary>Y座標</summary>
        public int Y {
            get; private set;
        } = 0;

        /// <summary>パスか否か</summary>
        public bool IsPass {
            get; private set;
        } = false;

        /// <summary>配置</summary>
        public static Decision Place(int x, int y) {
            return new Decision { X = x, Y = y };
        }

        /// <summary>パス</summary>
        public static Decision Pass {
            get {
                return new Decision { IsPass = true };
            }
        }

        /// <summary>文字列化</summary>
        public override string ToString() {
            return $"{X},{Y},{IsPass}";
        }
    }
}
