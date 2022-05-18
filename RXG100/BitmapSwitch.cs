using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RXG100
{
    internal class BitmapSwitch : BitmapControl
    {
        public BitmapSwitch() : base()
        {
            Cursor = Cursors.Hand;

            PreviewMouseLeftButtonDown += BitmapKnob_MouseLeftButtonDown;
        }

        private void BitmapKnob_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Value = 1 - Value;
        }

        public override int GetFrameIndex()
        {
            return (int)Value;
        }
    }
}
