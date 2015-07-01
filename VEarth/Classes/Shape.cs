using System;
using System.Collections.Generic;
using System.Text;

namespace VEarth.Shapes
{
    class Shape
    {
        public string ID;
        public string Title;

        public Shape()
        {
        }

        public Shape(string id, string title)
        {
            ID = id;
            Title = title;
        }
    }
}
