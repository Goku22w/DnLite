using System.Drawing;

namespace DnLite
{
    // Simple container for token metadata editable in the admin panel
    public class TokenData
    {
        public string Name { get; set; }
        public int MaxHP { get; set; }
        public int CurHP { get; set; }
        public int Lvl { get; set; }
        public int AC { get; set; }
        public bool IsPlayer { get; set; }
        public bool IsHostile { get; set; }
        public bool IsLarge { get; set; }

        // Store the unique initiative name (with numbering if needed, e.g., "Goblin -1-")
        public string InitiativeName { get; set; }

        // Store the base color (not affected by size/hostility modifiers) as ARGB int for JSON serialization
        public int BaseColorArgb { get; set; }

        public TokenData() { }
        public TokenData(string name, int maxHP, int curHP, int lvl = 0, int ac = 10, bool isPlayer = false, bool isHostile = false, bool isLarge = false, Color? baseColor = null)
        {
            Name = name;
            MaxHP = maxHP;
            CurHP = curHP;
            Lvl = lvl;
            AC = ac;
            IsPlayer = isPlayer;
            IsHostile = isHostile;
            IsLarge = isLarge;
            BaseColorArgb = baseColor.HasValue ? baseColor.Value.ToArgb() : Color.Gray.ToArgb();
        }

        // Helper property to get/set BaseColor as a Color object
        public Color GetBaseColor()
        {
            return Color.FromArgb(BaseColorArgb);
        }

        public void SetBaseColor(Color color)
        {
            BaseColorArgb = color.ToArgb();
        }
    }
}
