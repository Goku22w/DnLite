using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoClass
{
    public class Decoration
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string ImgFileLocation { get; set; }

        public Decoration(string Name, string Description, int Height, int Width, string ImgFileLocation)
        {
            this.Name = Name;
            this.Description = Description;
            this.Height = Height;
            this.Width = Width;
            this.ImgFileLocation = ImgFileLocation;
        }
    }
}
