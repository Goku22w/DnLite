using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenDataClass;

namespace GridPreset
{
    public class GridPresetClass
    {
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
        public string PlacematFileLocation { get; set; }
        // Map of coordinate string ("col,row") to TokenData for easy JSON serialization
        public Dictionary<string, TokenData> TokenCoordDictionary { get; set; }
        // Map of coordinate string ("col,row") to DecorationData for decoration tokens
        public Dictionary<string, DecorationData> DecorationCoordDictionary { get; set; }

        public GridPresetClass(int gridWidth, int gridHeight, string placematFileLocation, Dictionary<string, TokenData> tokenCoordDictionary, Dictionary<string, DecorationData> decorationCoordDictionary = null)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            PlacematFileLocation = placematFileLocation;
            TokenCoordDictionary = tokenCoordDictionary;
            DecorationCoordDictionary = decorationCoordDictionary ?? new Dictionary<string, DecorationData>();
        }
    }
}
