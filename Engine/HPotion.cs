using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class HPotion : Item
    {
        public int AmountToHeal { get; set; }
        public HPotion(int id, string name, string namePlural, int amountToHeal) : base(id, name, namePlural)
            {
            AmountToHeal = amountToHeal;
            }
    }
}
