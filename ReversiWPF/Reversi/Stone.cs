namespace ReversiEngine.Reversi {
    ///<summary>石 列挙型</summary>
    public enum Stone : int{
        /// <summary>石が配置されていない</summary>
        None = 0 ,
        /// <summary>白</summary>
        White = +1,
        /// <summary>黒</summary>
        Black = -1
    };

    ///<summary>石 信頼性</summary>
    public enum StoneReliability : int{
        ///<summary>未配置</summary>
        None = 0, 
        ///<summary>不確実 - 次の手で返される可能性アリ</summary> 
        Uncertain,
        ///<summary>暫定 - 次の手で返される可能性ナシ</summary>
        Interim,
        ///<summary>確定 - 今後返される可能性ナシ</summary>
        Definite
    };
}
