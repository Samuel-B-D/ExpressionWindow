using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace ThemedWindows
{
    class ExpressionDialog : Window
    {
        Border Window_Border = new Border();
        Grid Window_Content_Grid = new Grid();
        ContentControl ContentPlaceHolder = new ContentControl();

        public enum DialogTypes { SaveCancel, Ok, Cancel };
        private DialogTypes dialogType;
        public DialogTypes DialogType 
        {
            get { return dialogType; }
            set
            {
                dialogType = value;
                switch (value)
                {
                    case DialogTypes.Ok:
                        Button BTN_Ok = new Button()
                        {
                            Content = "Ok",
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            Padding = new Thickness(10, 5, 10, 5),
                            Margin = new Thickness(5)
                        };
                        break;
                }
            }
        }

        private Grid Footer;

        public  enum StatusTypes { None, Ok, Save, Cancel };
        public StatusTypes Status { get; private set; }

        public ExpressionDialog(DialogTypes Type)
            : base()
        {
            Window_Border.BorderThickness = new Thickness(1);
            Window_Border.Background = new SolidColorBrush(Color.FromRgb(56, 56, 56));
            Window_Border.Child = Window_Content_Grid;
            this.Content = Window_Border;
            Window_Content_Grid.Children.Add(ContentPlaceHolder);
            Window_Border.Style = (Style)this.FindResource("Window_Frame_Border");

            Status = StatusTypes.None;
            base.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            base.SizeToContent = SizeToContent.WidthAndHeight;
            base.ShowInTaskbar = false;
            base.ResizeMode = System.Windows.ResizeMode.CanMinimize;
            base.WindowStyle = System.Windows.WindowStyle.None;

            //Create Content Grid
            Window_Content_Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            Window_Content_Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            Grid FooterFill = new Grid()
            {
                Height = 36,
                Background = ExpressionWindow.BackgroundColorBrush
            };
            Footer = new Grid();
            Grid.SetRow(FooterFill, 1);
            Grid.SetColumnSpan(FooterFill, 2);
            FooterFill.Children.Add(Footer);
            Window_Content_Grid.Children.Add(FooterFill);

            switch (Type)
            {
                case DialogTypes.Ok:
                    Button BTN_Ok = new Button()
                    {
                        Content = "Ok",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = new Thickness(10, 5, 10, 5),
                        Margin = new Thickness(5)
                    };
                    BTN_Ok.Click += (e, o) =>
                    {
                        this.Status = StatusTypes.Ok;
                        this.Close();
                    };
                    Footer.Children.Add(BTN_Ok);
                    break;
                case DialogTypes.Cancel:
                    Button BTN_Cancel = new Button()
                    {
                        Content = "Cancel",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = new Thickness(10, 5, 10, 5),
                        Margin = new Thickness(5)
                    };
                    BTN_Cancel.Click += (e, o) =>
                    {
                        this.Status = StatusTypes.Cancel;
                        this.Close();
                    };
                    Footer.Children.Add(BTN_Cancel);
                    break;
                case DialogTypes.SaveCancel:
                    Footer.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    Footer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                    Footer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                    Button BTN_sCancel = new Button()
                    {
                        Content = "Cancel",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = new Thickness(10, 5, 10, 5),
                        Margin = new Thickness(5)
                    };
                    BTN_sCancel.Click += (e, o) =>
                    {
                        this.Status = StatusTypes.Cancel;
                        this.Close();
                    };
                    Grid.SetColumn(BTN_sCancel, 1);
                    Footer.Children.Add(BTN_sCancel);

                    Button BTN_sSave = new Button()
                    {
                        Content = "Save",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = new Thickness(10, 5, 10, 5),
                        Margin = new Thickness(5)
                    };
                    BTN_sSave.Click += (e, o) =>
                    {
                        this.Status = StatusTypes.Save;
                        this.Close();
                    };
                    Footer.Children.Add(BTN_sSave);
                    break;
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            // REQUIRED TO KEEP DESIGNER SUPPORT
            if (oldContent == Window_Border && newContent != Window_Border)
            {
                object Backup = newContent;
                newContent = Window_Border;
                ContentPlaceHolder.Content = Backup;
                this.Content = Window_Border;
            }
            else
            {
                base.OnContentChanged(oldContent, newContent);
            }
        }
    }
}
