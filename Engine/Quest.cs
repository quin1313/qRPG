using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Quest
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int RewardExp { get; set; }
        public int RewardGold { get; set; }
        public Item RewardItem { get; set; }
        public List<QuestCompletionItem> QuestCompletionItems { get; set; }
        public Quest(int id, string name, string desc, int rewexp, int rewgold, Item rewItem = null)
        {
            ID = id;
            Name = name;
            Desc = desc;
            RewardExp = rewexp;
            RewardGold = rewgold;
            RewardItem = rewItem;
            QuestCompletionItems = new List<QuestCompletionItem>();
        }
    }
}
