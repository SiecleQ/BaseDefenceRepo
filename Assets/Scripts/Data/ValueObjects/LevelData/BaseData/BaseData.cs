using System;

namespace Data.ValueObjects
{
    [Serializable]
    public class BaseData
    {
        public BaseRoomData BaseRoomData;
        public MineBaseData MineBaseData;
        public MilitaryBaseData MilitaryBaseData;
        public BuyablesData BuyablesData;
    }
}