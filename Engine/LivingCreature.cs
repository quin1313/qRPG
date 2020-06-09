using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class LivingCreature
    {
        public int CurrHP { get; set; }
        public int MaxHP { get; set; }
        public LivingCreature(int currHP, int maxHP)
        {
            CurrHP = currHP;
            MaxHP = maxHP;
        }
    }
}
