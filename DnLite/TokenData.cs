namespace DnLite
{
    // Simple container for token metadata editable in the admin panel
    public class TokenData
    {
        public string Name { get; set; }
        public int MaxHP { get; set; }
        public int CurHP { get; set; }
        public int Lvl { get; set; }

        public TokenData() { }
        public TokenData(string name, int maxHP, int curHP, int lvl = 0)
        {
            Name = name;
            MaxHP = maxHP;
            CurHP = curHP;
            Lvl = lvl;
        }
    }
}
