using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThemedWindows;
using ThemedWindows.Effects;

namespace TEST
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindows.ExpressionWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ColorPicker_Click(object sender, RoutedEventArgs e)
        {
            MainApplicationWindow.ThemeColor = (ThemeColors)((MenuItem)e.OriginalSource).Header;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            button1.Effect = new BCSEffect() { Brightness = -0.1 };
            ExpressionMessageBox.Show("Hello");
        }
    }
}
