using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ThemedWindows.Controls
{
    public class MonoPictureButton : Button
    {
        public MonoPictureButton() : base() { }

        public ImageSource Image
        {
            get { return (ImageSource)this.GetValue(ImageProperty); }
            set { this.SetValue(ImageProperty, value); }
        }
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(MonoPictureButton));
    }
}
