using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiEngine.Reversi {

    ///<summary>プレイヤー</summary>
    public interface Player {

        /// <summary>次の手を打つ</summary>
        /// <returns>決定オブジェクトを返す</returns>
        Decision Play(Board current_board);

        /// <summary>プレイヤーの色</summary>
        Stone Color { get; }
    }
}
