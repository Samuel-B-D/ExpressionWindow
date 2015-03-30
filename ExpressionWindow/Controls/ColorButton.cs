using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ThemedWindows.Controls
{
    public class ColorButton : Button
    {
        const int BorderBrightness = 0;
        const int HoverBrightness = 20;
        const int HoverBorderBrightness = 20;

        const int BorderContrast = 90;
        const int HoverContrast = 120;
        const int HoverBorderContrast = 120;

        public ColorButton() : base() { }

        private static byte ByteClamp(int value)
        {
            if (value < 0)
                return 0;

            if (value > 255)
                return 255;

            return (byte)value;
        }
        private static byte ByteClamp(double value)
        {
            return ByteClamp((int)value);
        }

        private static SolidColorBrush BrushFromColorBrightnessContrast(Color color, int Brightness, int Contrast)
        {
            return new SolidColorBrush(Color.FromRgb(
                ByteClamp((color.R * Contrast / 100) + Brightness),
                ByteClamp((color.G * Contrast / 100) + Brightness),
                ByteClamp((color.B * Contrast / 100) + Brightness)
            ));
        }
        private static SolidColorBrush BrushFromColorBrightnessContrast(SolidColorBrush color, int Brightness, int Contrast)
        {
            return color == null ? null : BrushFromColorBrightnessContrast(color.Color, Brightness, Contrast);
        }

        private new Brush Background { get; set; }
        private new Brush BorderBrush { get; set; }
        public SolidColorBrush BackgroundColor 
        {
            get { return (SolidColorBrush)this.GetValue(BackgroundColorProperty); }
            set { this.SetValue(BackgroundColorProperty, value); }
        }
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(SolidColorBrush), typeof(ColorButton), new PropertyMetadata(OnBackgroundColorChanged));
        private static void OnBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var Button = d as ColorButton;
            if (Button != null)
            {
                Button.BorderColor = BrushFromColorBrightnessContrast(Button.BackgroundColor, BorderBrightness, BorderContrast);
                Button.HoverColor = BrushFromColorBrightnessContrast(Button.BackgroundColor, HoverBrightness, HoverContrast);
                Button.HoverBorderColor = BrushFromColorBrightnessContrast(Button.BackgroundColor, HoverBorderBrightness, HoverBorderContrast);
            }
        }

        public SolidColorBrush BorderColor
        {
            get { return (SolidColorBrush)this.GetValue(BorderColorProperty); }
            protected set { SetValue(BorderColorPropertyKey, value); }
        }
        private static DependencyPropertyKey BorderColorPropertyKey =
            DependencyProperty.RegisterReadOnly("BorderColor", typeof(SolidColorBrush), typeof(ColorButton), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty BorderColorProperty = BorderColorPropertyKey.DependencyProperty;

        public SolidColorBrush HoverColor
        {
            get { return (SolidColorBrush)this.GetValue(HoverColorProperty); }
            protected set { SetValue(HoverColorPropertyKey, value); }
        }
        private static DependencyPropertyKey HoverColorPropertyKey =
            DependencyProperty.RegisterReadOnly("HoverColor", typeof(SolidColorBrush), typeof(ColorButton), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty HoverColorProperty = HoverColorPropertyKey.DependencyProperty;

        public SolidColorBrush HoverBorderColor
        {
            get { return (SolidColorBrush)this.GetValue(HoverBorderColorProperty); }
            protected set { SetValue(HoverBorderColorPropertyKey, value); }
        }
        private static DependencyPropertyKey HoverBorderColorPropertyKey =
            DependencyProperty.RegisterReadOnly("HoverBorderColor", typeof(SolidColorBrush), typeof(ColorButton), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty HoverBorderColorProperty = HoverBorderColorPropertyKey.DependencyProperty;
    }
}
