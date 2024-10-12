using System;
using System.Collections.Generic;

namespace HeurekaGames.AssetHunterPRO
{
    [Serializable]
    public class AH_WrapperList
    {
        public List<string> list = new();

        public AH_WrapperList(List<string> value)
        {
            this.list = value;
        }
    }
}