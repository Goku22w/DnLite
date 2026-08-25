using System;

namespace TokenDataClass
{
    // Container for decoration token metadata for saving/loading in GridPresets
    public class DecorationData
    {
        public string ImagePath { get; set; }
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }

        public DecorationData() { }

        public DecorationData(string imagePath, int gridWidth = 1, int gridHeight = 1)
        {
            ImagePath = imagePath;
            GridWidth = Math.Max(1, gridWidth);
            GridHeight = Math.Max(1, gridHeight);
        }

        // Create a deep copy of this DecorationData instance
        public DecorationData Clone()
        {
            return new DecorationData(ImagePath, GridWidth, GridHeight);
        }
    }
}
