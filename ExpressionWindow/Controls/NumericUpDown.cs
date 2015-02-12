using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ThemedWindows.Controls
{
    public class NumericUpDown : Control
    {
        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var txt = this.GetTemplateChild("NUD_TextBox") as TextBox;
            var Up = this.GetTemplateChild("NUD_Up") as RepeatButton;
            var Down = this.GetTemplateChild("NUD_Down") as RepeatButton;
            var Border = this.GetTemplateChild("NUD_Border") as Border;

            if (txt != null && Up != null && Down != null && Border != null)
            {
                txt.GotFocus += (o, e) => { txt.SelectAll(); };
                txt.MouseLeftButtonDown += (o, e) => { txt.SelectAll(); };
                txt.MouseLeftButtonUp += (o, e) => { txt.SelectAll(); };
                txt.TextChanged += (o, e) =>
                {
                    if (txt != null)
                    {
                        var binding = txt.GetBindingExpression(TextBox.TextProperty);
                        if (binding != null)
                            binding.UpdateSource();
                    }
                };
                Up.GotFocus += (o, e) => { Border.Focus(); };
                Down.GotFocus += (o, e) => { Border.Focus(); };
            }
        }

        [BindableAttribute(true)]
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetCurrentValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, (d, e) =>
            {

            }
            , (d, v) =>
            {
                var Min = (double)v;
                var NUD = d as NumericUpDown;
                if (Min > NUD.Maximum) Min = NUD.Maximum;
                return Min;
            }
            ));

        [BindableAttribute(true)]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetCurrentValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(double.MaxValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, (d, e) =>
            {

            }, (d, v) =>
            {
                var Max = (double)v;
                var NUD = d as NumericUpDown;
                if (Max < NUD.Minimum) Max = NUD.Minimum;
                return Max;
            }
            ));

        [BindableAttribute(true)]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetCurrentValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, (d, e) =>
            {

            }
            , (d, v) =>
            {
                var Value = (double)v;
                var NUD = d as NumericUpDown;
                if (Value < NUD.Minimum) Value = NUD.Minimum;
                if (Value > NUD.Maximum) Value = NUD.Maximum;
                return Value;
            }
            ));

        [BindableAttribute(true)]
        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetCurrentValue(SmallChangeProperty, value); }
        }

        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register("SmallChange", typeof(double), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, (d, e) =>
            {

            }
            ));

        [BindableAttribute(true)]
        public int ChangeInterval
        {
            get { return (int)GetValue(ChangeIntervalProperty); }
            set { SetCurrentValue(ChangeIntervalProperty, value); }
        }

        public static readonly DependencyProperty ChangeIntervalProperty =
            DependencyProperty.Register("ChangeInterval", typeof(int), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(17, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, (d, e) =>
            {
            }
            ));



        public ICommand Decrease
        {
            get
            {
                return new Utilities.RelayCommand(() =>
                {
                    Value -= SmallChange;
                });
            }
        }

        public ICommand Increase
        {
            get
            {
                return new Utilities.RelayCommand(() =>
                {
                    Value += SmallChange;
                });
            }
        }
    }
}