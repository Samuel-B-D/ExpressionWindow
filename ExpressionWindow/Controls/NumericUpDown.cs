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
        const double DOUBLE_ZERO_OFFSET = 0.0001;

        public event EventHandler ValueChanged;

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
                txt.PreviewMouseLeftButtonDown += (o, e) =>
                {
                    TextBox tb = o as TextBox;
                    if (tb != null)
                        tb.SelectAll();

                    e.Handled = true;
                    tb.Focus();
                };
                txt.TextChanged += (o, e) =>
                {
                    if (txt != null)
                    {
                        var binding = txt.GetBindingExpression(TextBox.TextProperty);
                        if (binding != null)
                            binding.UpdateSource();
                    }
                    RaiseValueChanged();
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
                var Min = (double)e.NewValue;
                var NUD = d as NumericUpDown;
                if (Min > NUD.Maximum) Min = NUD.Maximum;
                d.SetCurrentValue(MinimumProperty, Min);
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
                var Max = (double)e.NewValue;
                var NUD = d as NumericUpDown;
                if (Max < NUD.Minimum) Max = NUD.Minimum;
                d.SetCurrentValue(MaximumProperty, Max);
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
                var Value = (double)e.NewValue;
                var NUD = d as NumericUpDown;
                if (Value < NUD.Minimum) Value = NUD.Minimum;
                if (Value > NUD.Maximum) Value = NUD.Maximum;
                d.SetCurrentValue(ValueProperty, Value);
                d.SetCurrentValue(DisplayValueProperty, Value.ToString());
                NUD.RaiseValueChanged();
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
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender, (d, e) =>
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
            new FrameworkPropertyMetadata(17, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        [BindableAttribute(true)]
        public string NeutralCaption
        {
            get { return (string)GetValue(NeutralCaptionProperty); }
            set { SetCurrentValue(NeutralCaptionProperty, value); }
        }

        public static readonly DependencyProperty NeutralCaptionProperty =
            DependencyProperty.Register("NeutralCaption", typeof(string), typeof(NumericUpDown), 
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender, (d, e) =>
            {
                var NUD = d as NumericUpDown;
                string NewCaption = (string)e.NewValue;
                if (NewCaption.Length > 0 && IsZero(NUD.Value))
                    d.SetCurrentValue(DisplayValueProperty, NewCaption);
            })
            );

        [BindableAttribute(true)]
        private string DisplayValue
        {
            get { return (string)GetValue(DisplayValueProperty); }
            set { SetCurrentValue(DisplayValueProperty, value); }
        }

        private static readonly DependencyProperty DisplayValueProperty =
            DependencyProperty.Register("DisplayValue", typeof(string), typeof(NumericUpDown),
            new FrameworkPropertyMetadata("0", FrameworkPropertyMetadataOptions.AffectsRender, (d, e) =>
            {
                var NUD = d as NumericUpDown;
                string ValueStr = (string)e.NewValue;
                if (ValueStr[0] == '.')
                    ValueStr = "0" + ValueStr;
                double Value;
                if (!double.TryParse(ValueStr, out Value))
                    if (NUD.NeutralCaption.Length > 0 && ValueStr == NUD.NeutralCaption)
                        Value = 0;
                    else
                    {
                        if (!double.TryParse((string)e.OldValue, out Value))
                            Value = 0;
                        ValueStr = Value.ToString();
                    }
                else
                    if (IsZero(Value) && NUD.NeutralCaption.Length > 0)
                        ValueStr = NUD.NeutralCaption;

                //double Value;
                //double.TryParse(ValueStr, out Value);

                d.SetCurrentValue(DisplayValueProperty, ValueStr);
                d.SetCurrentValue(ValueProperty, Value);
            })
            );


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


        private void RaiseValueChanged()
        {
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }

        private static bool IsZero(double value)
        {
            return value + DOUBLE_ZERO_OFFSET > 0 && value - DOUBLE_ZERO_OFFSET < 0;
        }
    }
}