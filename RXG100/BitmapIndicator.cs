using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RXG100
{
    internal class BitmapIndicator : BitmapControl
    {
        public BitmapIndicator() : base()
        {
            
        }

        public override int GetFrameIndex()
        {
            return (int)Value;
        }
    }
}
