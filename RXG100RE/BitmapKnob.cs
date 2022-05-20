using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RXG100RE
{
    internal class BitmapKnob : BitmapControl
    {
        public static readonly DependencyProperty NumFramesProperty = DependencyProperty.Register("NumFrames", typeof(int), typeof(BitmapKnob), new PropertyMetadata(1, UpdateImageHandler));
        public int NumFrames
        {
            get => (int)GetValue(NumFramesProperty);
            set => SetValue(NumFramesProperty, value);
        }

        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(BitmapKnob), new PropertyMetadata(0.005));
        public double Speed
        {
            get => (double)GetValue(SpeedProperty);
            set => SetValue(SpeedProperty, value);
        }

        public BitmapKnob() : base()
        {
            Cursor = Cursors.Hand;

            PreviewMouseLeftButtonDown += BitmapKnob_MouseLeftButtonDown;
            PreviewMouseLeftButtonUp += BitmapKnob_PreviewMouseLeftButtonUp;
            PreviewMouseMove += BitmapKnob_PreviewMouseMove;
        }

        bool editing;
        Point lastPos;

        private void BitmapKnob_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            editing = true;
            lastPos = e.GetPosition(this);

            Mouse.Capture(this);
        }

        private void BitmapKnob_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            editing = false;

            Mouse.Capture(null);
        }

        private void BitmapKnob_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (editing)
            {
                Point currentPos = e.GetPosition(this);
                double dx = lastPos.Y - currentPos.Y;

                if (dx == 0)
                    return;

                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    dx *= 0.1;

                double value = Value;
                value += dx * Speed;

                if (value < 0.0)
                    value = 0.0;
                if (value > 1.0)
                    value = 1.0;

                Value = value;

                lastPos = currentPos;
                e.Handled = true;
            }
        }

        public override int GetFrameIndex()
        {
            int index = (int)Math.Floor(Value * NumFrames);
            while (index >= NumFrames)
            {
                index--;
            }
            return index;
        }
    }
}
