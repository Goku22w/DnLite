using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPCClass
{
    public class NPC
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public char Token { get; set; }
        public int MaxHP { get; set; }
        public int CurHP { get; set; }
        public int Lvl { get; set; }
        public int AC { get; set; }
        public bool IsHostile { get; set; }
        public bool IsLarge { get; set; }

        public NPC(string Name, string Description, char Token, int MaxHP, int CurHP, int Lvl, int AC, bool IsHostile, bool IsLarge)
        {
            this.Name = Name;
            this.Description = Description;
            this.Token = Token;
            this.MaxHP = MaxHP;
            this.CurHP = CurHP;
            this.Lvl = Lvl;
            this.AC = AC;
            this.IsHostile = IsHostile;
            this.IsLarge = IsLarge;
        }
    }
}
