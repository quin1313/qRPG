using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Monster : LivingCreature
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MaxDmg { get; set; }
        public int RewardExp { get; set; }
        public int RewardGold { get; set; }
        public List<LootItem> LootTable { get; set; }



        public Monster(int id, string name, int maxDmg, int rewexp, int rewgold, int currHP, int maxHP) : base(currHP, maxHP)
        {
            ID = id;
            Name = name;
            MaxDmg = maxDmg;
            RewardExp = rewexp;
            RewardGold = rewgold;
            LootTable = new List<LootItem>();
        }
    }
}
