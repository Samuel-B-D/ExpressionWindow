using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;

namespace ThemedWindows
{
    public class ExpressionDialog : Window
    {
        public static RoutedCommand CloseCommand = new RoutedCommand();
        public static RoutedCommand PrimaryCommand = new RoutedCommand();

        /// <summary>
        /// Occur when the "Save" option is used in a save/cancel dialog.
        /// </summary>
        static public event EventHandler DialogSaved;

        Border Window_Border = new Border();
        Grid Window_Content_Grid = new Grid();
        ContentControl ContentPlaceHolder = new ContentControl();

        String[] Names = null;

        public enum DialogTypes { None, SaveCancel, Ok, Cancel, SaveDiscardCancel };
        public DialogTypes DialogType { get; private set; }

        private Grid Footer;

        public  enum StatusTypes { None, Ok, Save, Cancel, Discard };
        public StatusTypes Status { get; private set; }

        public ExpressionDialog(DialogTypes Type)
            : base()
        {
            Initialize(Type);
        }

        public ExpressionDialog(DialogTypes Type, params String[] Names)
            : base()
        {
            this.Names = Names;
            Initialize(Type);
        }

        public ExpressionDialog(DialogTypes Type, Window Owner, params String[] Names)
            : base()
        {
            this.Names = Names;
            Initialize(Type, Owner);
        }

        public void Initialize(DialogTypes Type) { Initialize(Type, Application.Current.MainWindow); }
        public void Initialize(DialogTypes Type, Window Owner)
        {
            CloseCommand.InputGestures.Add(new KeyGesture(Key.Escape, ModifierKeys.None));
            PrimaryCommand.InputGestures.Add(new KeyGesture(Key.Enter, ModifierKeys.None));

            Window_Border.BorderThickness = new Thickness(1);
            Window_Border.Background = new SolidColorBrush(Color.FromRgb(56, 56, 56));
            Window_Border.Child = Window_Content_Grid;
            this.Content = Window_Border;
            Window_Content_Grid.Children.Add(ContentPlaceHolder);
            Window_Border.Style = (Style)this.FindResource("Window_Frame_Border");

            Status = StatusTypes.None;
            try
            {
                base.Owner = Owner;
                base.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            catch (Exception)
            {
                base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            base.SizeToContent = SizeToContent.WidthAndHeight;
            base.ShowInTaskbar = false;
            base.ResizeMode = System.Windows.ResizeMode.CanMinimize;
            base.WindowStyle = System.Windows.WindowStyle.None;

            if (Type != DialogTypes.None)
            {
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
                        Button Button_Ok = new Button()
                        {
                            Content = Names == null || Names.Length <= 0 ? "Ok" : Names[0],
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            Padding = new Thickness(10, 5, 10, 5),
                            Margin = new Thickness(5)
                        };
                        Button_Ok.Click += ButtonOk_Click;
                        Footer.Children.Add(Button_Ok);
                        this.CommandBindings.Add(new CommandBinding(CloseCommand, ButtonOk_Click));
                        this.CommandBindings.Add(new CommandBinding(PrimaryCommand, ButtonOk_Click));
                        break;
                    case DialogTypes.Cancel:
                        Button Button_Cancel = new Button()
                        {
                            Content = Names == null || Names.Length <= 0 ? "Cancel" : Names[0],
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            Padding = new Thickness(10, 5, 10, 5),
                            Margin = new Thickness(5)
                        };
                        Button_Cancel.Click += ButtonCancel_Click;
                        Footer.Children.Add(Button_Cancel);
                        this.CommandBindings.Add(new CommandBinding(CloseCommand));
                        this.CommandBindings.Add(new CommandBinding(PrimaryCommand));
                        break;
                    case DialogTypes.SaveCancel:
                        Footer.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                        Footer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                        Footer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                        Button Button_Cancel2 = new Button()
                        {
                            Content = Names == null || Names.Length <= 1 ? "Cancel" : Names[1],
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            Padding = new Thickness(10, 5, 10, 5),
                            Margin = new Thickness(5)
                        };
                        Button_Cancel2.Click += ButtonCancel_Click;
                        Grid.SetColumn(Button_Cancel2, 1);
                        Footer.Children.Add(Button_Cancel2);

                        Button Button_Save = new Button()
                        {
                            Content = Names == null || Names.Length <= 0 ? "Save" : Names[0],
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            Padding = new Thickness(10, 5, 10, 5),
                            Margin = new Thickness(5)
                        };
                        Button_Save.Click += ButtonSave_Click;
                        Footer.Children.Add(Button_Save);
                        this.CommandBindings.Add(new CommandBinding(CloseCommand, ButtonCancel_Click));
                        this.CommandBindings.Add(new CommandBinding(PrimaryCommand, ButtonSave_Click));
                        break;

                    case DialogTypes.SaveDiscardCancel:
                        Footer.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                        Footer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                        Footer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                        Footer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                        Button Button_Save3 = new Button()
                        {
                            Content = Names == null || Names.Length <= 0 ? "Save" : Names[0],
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            Padding = new Thickness(10, 5, 10, 5),
                            Margin = new Thickness(5)
                        };
                        Button_Save3.Click += ButtonSave_Click;
                        Footer.Children.Add(Button_Save3);

                        Button Button_Discard3 = new Button()
                        {
                            Content = Names == null || Names.Length <= 1 ? "Discard" : Names[1],
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            Padding = new Thickness(10, 5, 10, 5),
                            Margin = new Thickness(5)
                        };
                        Button_Discard3.Click += ButtonDiscard_Click;
                        Grid.SetColumn(Button_Discard3, 1);
                        Footer.Children.Add(Button_Discard3);

                        Button Button_Cancel3 = new Button()
                        {
                            Content = Names == null || Names.Length <= 2 ? "Cancel" : Names[2],
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            Padding = new Thickness(10, 5, 10, 5),
                            Margin = new Thickness(5)
                        };
                        Button_Cancel3.Click += ButtonCancel_Click;
                        Grid.SetColumn(Button_Cancel3, 2);
                        Footer.Children.Add(Button_Cancel3);

                        this.CommandBindings.Add(new CommandBinding(CloseCommand, ButtonCancel_Click));
                        this.CommandBindings.Add(new CommandBinding(PrimaryCommand, ButtonSave_Click));
                        break;
                }
            }

            this.Activated += ExpressionDialog_Activated;
            this.Deactivated += ExpressionDialog_Deactivated;
        }

        public virtual void ExpressionDialog_Deactivated(object sender, EventArgs e)
        {
            foreach (var w in Application.Current.Windows.OfType<ExpressionWindow>())
                w.OpacityMask = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        }

        public virtual void ExpressionDialog_Activated(object sender, EventArgs e)
        {
            foreach (var w in Application.Current.Windows.OfType<ExpressionWindow>())
                w.OpacityMask = new SolidColorBrush(Color.FromArgb(120, 0, 0, 0));
        }

        protected virtual void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            this.Status = StatusTypes.Ok;
            this.Close();
        }

        protected virtual void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Status = StatusTypes.Cancel;
            this.Close();
        }

        protected virtual void ButtonDiscard_Click(object sender, RoutedEventArgs e)
        {
            this.Status = StatusTypes.Discard;
            this.Close();
        }

        protected virtual void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            this.Status = StatusTypes.Save;
            if (DialogSaved != null)
                DialogSaved(this, EventArgs.Empty);
            this.Close();
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
