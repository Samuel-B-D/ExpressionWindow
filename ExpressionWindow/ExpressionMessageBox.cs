using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThemedWindows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Media;

namespace ThemedWindows
{
    public class ExpressionMessageBox
    {
        public enum IconType { Warning, Question }

        static public void Show(string Content)
        {
            Show(Content, Application.Current.MainWindow);
        }
        static public ExpressionDialog.StatusTypes Show(string Content, Window Parent, IconType icon = IconType.Warning, ExpressionDialog.DialogTypes type = ExpressionDialog.DialogTypes.Ok, params string[] ButtonNames)
        {
            //Create the dialog
            ExpressionDialog dialog = new ExpressionDialog(type, ButtonNames);
            dialog.Owner = Parent;

            //Fill the dialog
            Grid ContentGrid = new Grid();
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            string ColorS = "_";
            if (Application.Current.MainWindow is ExpressionWindow)
                ColorS += Enum.GetName(typeof(ExpressionWindow.ThemeColors), ((ExpressionWindow)Application.Current.MainWindow).ThemeColor);
            BitmapImage IconBmp = null;
            switch (icon)
                {
                    case IconType.Warning:
                        IconBmp = new BitmapImage(new Uri("pack://application:,,,/ExpressionWindow;component/icons/warning" + ColorS + ".png"));
                        break;
                    case IconType.Question:
                        IconBmp = new BitmapImage(new Uri("pack://application:,,,/ExpressionWindow;component/Icons/Question" + ColorS + ".png"));
                        break;
                }
            Image Icon = new Image()
            {
                Source = IconBmp,
                Stretch = System.Windows.Media.Stretch.None,
                Margin = new Thickness(10)
            };
            Grid.SetColumn(Icon, 0);
            ContentGrid.Children.Add(Icon);

            Label lb = new Label()
            {
                Margin = new Thickness(10),
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Content = Content
            };
            Grid.SetColumn(lb, 1);
            ContentGrid.Children.Add(lb);

            dialog.Content = ContentGrid;

            //Show the dialog
            dialog.ShowDialog();

            return dialog.Status;
        }
    }
}
