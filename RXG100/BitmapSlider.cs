using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RXG100
{
    internal class BitmapSlider : BitmapControl
    {
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(BitmapSlider), new PropertyMetadata(0.005));
        public double Speed
        {
            get => (double)GetValue(SpeedProperty);
            set => SetValue(SpeedProperty, value);
        }

        public BitmapSlider() : base()
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
                double dx = currentPos.X - lastPos.X;

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

        ImageSource lastImageSource;

        public override ImageSource GetFrameImage()
        {
            if(lastImageSource != Source)
            {
                BackgorundImageSource = new CroppedBitmap(Source, new Int32Rect(OffsetX, OffsetY, Width, Height));
                lastImageSource = Source;
            }

            int pixelLen = (int)(Width * Value);

            if(pixelLen == 0) return null;

            Int32Rect sourceRect = new Int32Rect(OffsetX + StrideX, OffsetY + StrideY, OffsetX + pixelLen, Height);
            return new CroppedBitmap(Source, sourceRect);
        }
    }
}
