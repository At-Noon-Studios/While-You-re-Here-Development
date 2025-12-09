using System;
using System.Collections.Generic;

namespace gamestate
{
    [Serializable]
    public class GamestateFlag
    {
        public string name;
        public bool value;
    }

    [Serializable]
    public class GamestateFlagList
    {
        public List<GamestateFlag> flags;

        public GamestateFlag GetFlagWithName(string name)
        {
            foreach (GamestateFlag flag in flags)
            {
                if (flag.name == name) return flag;
            }
            return null;
        }
    }
}