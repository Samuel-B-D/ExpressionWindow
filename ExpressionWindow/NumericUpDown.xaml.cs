using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace ThemedWindows
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        bool TextChangedProgramatically = false;

        private decimal? min;
        public decimal? Min
        {
            get { return min; }
            set
            {
                min = value;
            }
        }

        private decimal? max;
        public decimal? Max
        {
            get { return max; }
            set
            {
                max = value;
            }
        }

        private decimal valValue;
        public decimal Value
        {
            get
            {
                return valValue;
            }
            set
            {
                valValue = value;
                if (Min != null && value < Min) valValue = (decimal)Min;
                if (Max != null && value > Max) valValue = (decimal)Max;
                TextChangedProgramatically = true;
                if (NeutralCaptation != null && valValue == 0)
                    TBX_Value.Text = NeutralCaptation;
                else
                    TBX_Value.Text = valValue.ToString();
                TextChangedProgramatically = false;
            }
        }

        private string neutralCaptation;
        public string NeutralCaptation 
        {
            get { return neutralCaptation; }
            set
            {
                neutralCaptation = value;
                if (NeutralCaptation != null && Value == 0)
                    TBX_Value.Text = NeutralCaptation;
            }
        }

        public decimal Tick { get; set; }

        public event TextChangedEventHandler ValueChanged;

        public NumericUpDown()
        {
            NeutralCaptation = null; 
            InitializeComponent();

            Min = null;
            Max = null;

            Tick = 1;

            ScrollbarValue.Minimum = -1;
            ScrollbarValue.Value = 0;
            ScrollbarValue.Maximum = 1;
            ScrollbarValue.SmallChange = 1;

            if (ScrollbarValue.ContextMenu != null)
            {
                foreach (Control i in ScrollbarValue.ContextMenu.Items)
                {
                    i.IsEnabled = false;
                }
                ScrollbarValue.ContextMenu.IsEnabled = false;
            }
        }

        private void ScrollBar_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            switch ((int)ScrollbarValue.Value)
            {
                case -1:
                    Value += Tick;
                    break;
                case 1:
                    Value -= Tick;
                    break;
            }
            ScrollbarValue.Value = 0;
        }

        private void TBX_Value_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (!TextChangedProgramatically)
            {
                try
                {
                    string temp = Regex.Replace(TBX_Value.Text, "[^0-9-" + System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "]", "");
                    valValue = decimal.Parse(temp);
                    e.Handled = true;
                    TBX_Value.Text = temp;
                }
                catch (Exception)
                {
                    Value = Min != null && Min > 0 ? (decimal)Min : 0;
                }
                finally
                {
                    if (ValueChanged != null)
                        ValueChanged(this, e);
                }
            }
        }
    }
}
