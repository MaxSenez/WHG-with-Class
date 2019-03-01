using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHGwoClass
{
    public class GameArea
    {
        public int x, y, width, height;
        public string type;
        public Color color;

        public GameArea(int _x, int _y, int _width, int _height, string _type, Color _color)
        {
            x = _x;
            y = _y;
            width = _width;
            height = _height;
            type = _type;
            color = _color;
        }

        public GameArea(int _x, int _y, int _width, int _height, string _type)
        {
            x = _x;
            y = _y;
            width = _width;
            height = _height;
            type = _type;
        }
    }
}
