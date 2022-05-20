using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RXG100RE
{
    /// <summary>
    /// Interaction logic for BitmapControl.xaml
    /// </summary>
    public abstract partial class BitmapControl : UserControl
    {
        public delegate void ValueChangedHandler(BitmapControl sender, double value);
        public event ValueChangedHandler ValueChanged;

        public BitmapControl()
        {
            InitializeComponent();

            Loaded += BitmapKnob_Loaded;
        }

        private void BitmapKnob_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateImage();
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(BitmapSource), typeof(BitmapControl), new PropertyMetadata(UpdateImageHandler));
        public BitmapSource Source
        {
            get => (BitmapSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty StrideXProperty = DependencyProperty.Register("StrideX", typeof(int), typeof(BitmapControl), new PropertyMetadata(0, UpdateImageHandler));
        public int StrideX
        {
            get => (int)GetValue(StrideXProperty);
            set => SetValue(StrideXProperty, value);
        }

        public static readonly DependencyProperty StrideYProperty = DependencyProperty.Register("StrideY", typeof(int), typeof(BitmapControl), new PropertyMetadata(0, UpdateImageHandler));
        public int StrideY
        {
            get => (int)GetValue(StrideYProperty);
            set => SetValue(StrideYProperty, value);
        }

        public static new readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(int), typeof(BitmapControl), new PropertyMetadata(100, UpdateImageHandler));
        public new int Width
        {
            get => (int)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public static new readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(int), typeof(BitmapControl), new PropertyMetadata(100, UpdateImageHandler));
        public new int Height
        {
            get => (int)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register("OffsetX", typeof(int), typeof(BitmapControl), new PropertyMetadata(0, UpdateImageHandler));
        public int OffsetX
        {
            get => (int)GetValue(OffsetXProperty);
            set => SetValue(OffsetXProperty, value);
        }

        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register("OffsetY", typeof(int), typeof(BitmapControl), new PropertyMetadata(0, UpdateImageHandler));
        public int OffsetY
        {
            get => (int)GetValue(OffsetYProperty);
            set => SetValue(OffsetYProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(BitmapControl), new PropertyMetadata(0.0, ValueUpdatedHandler));
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty BackgorundImageSourceProperty = DependencyProperty.Register("BackgorundImageSource", typeof(ImageSource), typeof(BitmapControl));
        public ImageSource BackgorundImageSource
        {
            get => (ImageSource)GetValue(BackgorundImageSourceProperty);
            set => SetValue(BackgorundImageSourceProperty, value);
        }

        private static readonly DependencyProperty CroppedImageSourceProperty = DependencyProperty.Register("CroppedImageSource", typeof(ImageSource), typeof(BitmapControl));
        private ImageSource CroppedImageSource
        {
            get => (ImageSource)GetValue(CroppedImageSourceProperty);
            set => SetValue(CroppedImageSourceProperty, value);
        }


        public static void ValueUpdatedHandler(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            BitmapControl instance = (BitmapControl)sender;
            instance.OnValueUpdated();
        }

        public static void UpdateImageHandler(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            BitmapControl instance = (BitmapControl)sender;
            instance.UpdateImage();
        }

        private void OnValueUpdated()
        {
            UpdateImage();
            ValueChanged?.Invoke(this, Value);
        }

        private void UpdateImage()
        {
            if (!IsLoaded)
            {
                return;
            }

            base.Width = Width;
            base.Height = Height;

            CroppedImageSource = GetFrameImage();
        }

        public virtual ImageSource GetFrameImage()
        {
            int index = GetFrameIndex();

            Int32Rect sourceRect = new Int32Rect(OffsetX + (index * StrideX), OffsetY + (index * StrideY), Width, Height);
            return new CroppedBitmap(Source, sourceRect);
        }

        public virtual int GetFrameIndex()
        {
            return 0;
        }
    }
}
