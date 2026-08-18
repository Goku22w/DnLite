using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterClass
{
    public class Character
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public string Description { get; set; }
        public char Token { get; set; }
        public int MaxHP { get; set; }
        public int CurHP { get; set; }
        public int Lvl { get; set; }
        public string ImgFileLocation { get; set; }
        public bool isLarge { get; set; }
        public int AC { get; set; } = 12; // Default AC value

        public Character(string Name, string Class, string Description, char Token, int MaxHP, int CurHP, int Lvl, string ImgFileLocation = "", bool isLarge = false, int AC = 12)
        {
            this.Name = Name;
            this.Class = Class;
            this.Description = Description;
            this.Token = Token;
            this.MaxHP = MaxHP + Lvl;
            this.CurHP = CurHP + Lvl;
            this.Lvl = Lvl;
            this.ImgFileLocation = ImgFileLocation;
            this.isLarge = isLarge;
            this.AC = AC + Lvl;
        }
    }
}
