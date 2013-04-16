using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace ThemedWindows
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public class ExpressionWindow : Window
    {
        static bool Loaded = false;
        const int TITLE_BAR_HEIGHT = 26;

        ResourceDictionary BaseTheme;
        ResourceDictionary CurrentTheme;
        public enum ThemeColors { Green, Blue, Yellow, Red, Orange, Purple, Pink, Grey, White }
        private static int X_BUTTON_NORMAL_WIDTH = 54;
        private static int X_BUTTON_MAXIMIZED_WIDTH = 58;
        bool ResizeInProcess = false;
        bool Maximized = false;
        private Rect _restoreLocation;
        bool FrameLoaded = false;
        bool BaseResourceLoaded = false;

        DispatcherTimer EpilepticTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 100) };

        ThemeColors themeColor;
        public ThemeColors ThemeColor
        {
            get { return themeColor; }
            set
            {
                themeColor = value;
                if (CurrentTheme is ResourceDictionary)
                    Application.Current.Resources.MergedDictionaries.Remove(CurrentTheme);
                else
                    CurrentTheme = new ResourceDictionary();

                string ColorS = Enum.GetName(typeof(ThemeColors), value);
                CurrentTheme.Source = new Uri("pack://application:,,,/ExpressionWindow;component/Themes/" + ColorS + "Colors.xaml");
                Application.Current.Resources.MergedDictionaries.Add(CurrentTheme);

                if (!BaseResourceLoaded)
                {
                    BaseTheme = new ResourceDictionary();
                    BaseTheme.Source = new Uri("pack://application:,,,/ExpressionWindow;component/Themes/ExpressionDarkBase.xaml");
                    Application.Current.Resources.MergedDictionaries.Add(BaseTheme);
                    BaseResourceLoaded = true;
                }
            }
        }

        private bool isColorPickerEnabled;
        public bool IsColorPickerEnabled 
        {
            get { return isColorPickerEnabled; }
            set
            {
                isColorPickerEnabled = value;
                if (value)
                {
                    Window_TitleGrid.ContextMenu = ColorPicker;
                }
                else
                {
                    Window_TitleGrid.ContextMenu = null;
                }
            }
        }

        private bool isEpileptic;
        public bool IsEpileptic
        {
            get { return isEpileptic; }
            set
            {
                isEpileptic = value;
                if (value)
                {
                    EpilepticTimer.Start();
                }
                else
                {
                    EpilepticTimer.Stop();
                }
            }
        }

        bool isModal;
        public bool IsModal
        {
            get { return isModal; }
            set
            {
                isModal = value;
                if (!value)
                {
                    Window_TitleGrid.Visibility = System.Windows.Visibility.Visible;
                    ContentPlaceHolder.Margin = new Thickness(0, TITLE_BAR_HEIGHT, 0, 0);
                }
                else
                {
                    Window_TitleGrid.Visibility = System.Windows.Visibility.Hidden;
                    ContentPlaceHolder.Margin = new Thickness(0);
                }
            }
        }

        private bool isResizable;
        public bool IsResizable
        {
            get { return isResizable; }
            set
            {
                isResizable = value;
                if (value)
                {
                    Window_ResizeLeft.Visibility = System.Windows.Visibility.Visible;
                    Window_ResizeRight.Visibility = System.Windows.Visibility.Visible;
                    Window_ResizeTop.Visibility = System.Windows.Visibility.Visible;
                    Window_ResizeBottom.Visibility = System.Windows.Visibility.Visible;
                    Window_ResizeTopLeft.Visibility = System.Windows.Visibility.Visible;
                    Window_ResizeTopRight.Visibility = System.Windows.Visibility.Visible;
                    Window_ResizeBottomLeft.Visibility = System.Windows.Visibility.Visible;
                    Window_ResizeBottomRight.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    Window_ResizeLeft.Visibility = System.Windows.Visibility.Hidden;
                    Window_ResizeRight.Visibility = System.Windows.Visibility.Hidden;
                    Window_ResizeTop.Visibility = System.Windows.Visibility.Hidden;
                    Window_ResizeBottom.Visibility = System.Windows.Visibility.Hidden;
                    Window_ResizeTopLeft.Visibility = System.Windows.Visibility.Hidden;
                    Window_ResizeTopRight.Visibility = System.Windows.Visibility.Hidden;
                    Window_ResizeBottomLeft.Visibility = System.Windows.Visibility.Hidden;
                    Window_ResizeBottomRight.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        //Expose some of the maintly utilised brushes
        static public Brush MainColorBrush
        {
            get { return (Brush)Application.Current.FindResource("NormalBrush"); }
        }
        static public Brush SecondaryColorBrush
        {
            get { return (Brush)Application.Current.FindResource("NormalBorderBrush"); }
        }
        static public Brush BackgroundColorBrush
        {
            get { return (Brush)Application.Current.FindResource("BackgroundBrush"); }
        }

        int NB_COLORS = Enum.GetValues(typeof(ThemeColors)).Length;

        public static IEnumerable<ThemeColors> AvailableThemeColors
        {
            get
            {
                return Enum.GetValues(typeof(ThemeColors)).Cast<ThemeColors>();
            }
        }

        #region UI

        Border Window_Border = new Border();

        Grid Window_Grid = new Grid();

        Thumb Window_ResizeLeft = new Thumb();
        Thumb Window_ResizeRight = new Thumb();
        Thumb Window_ResizeTop = new Thumb();
        Thumb Window_ResizeBottom = new Thumb();
        Thumb Window_ResizeTopLeft = new Thumb();
        Thumb Window_ResizeTopRight = new Thumb();
        Thumb Window_ResizeBottomLeft = new Thumb();
        Thumb Window_ResizeBottomRight = new Thumb();

        Grid Window_TitleGrid = new Grid();
        Label Window_TitleLabel = new Label();
        Button Window_Button_Close = new Button();
        Button Window_Button_Maximize = new Button();
        Button Window_Button_Minimize = new Button();

        protected Grid Window_Content_Grid = new Grid();
        ContentControl ContentPlaceHolder = new ContentControl();

        ContextMenu ColorPicker = new ContextMenu();

        #endregion

        public ExpressionWindow()
            : base()
        {
            this.MinWidth = 50;
            this.MinHeight = TITLE_BAR_HEIGHT;
            IsColorPickerEnabled = true;
            IsEpileptic = false;
            IsModal = false;
            Window_TitleLabel.SetBinding(Label.ContentProperty, new Binding() { Path = new PropertyPath("Title"), RelativeSource = new RelativeSource() { AncestorType = typeof(ExpressionWindow), Mode = RelativeSourceMode.FindAncestor }, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            this.Activated += new EventHandler(ExpressionWindow_Activated);
            this.Deactivated += new EventHandler(ExpressionWindow_Deactivated);
            if (Loaded)
                themeColor = ((ExpressionWindow)Application.Current.MainWindow).ThemeColor;
            else
            {
                ThemeColor = ThemeColors.Green;
                Loaded = true;
            }
            Initialize();
        }

        void ExpressionWindow_Deactivated(object sender, EventArgs e)
        {
            this.OpacityMask = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0));
        }

        void ExpressionWindow_Activated(object sender, EventArgs e)
        {
            this.OpacityMask = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
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

        void Initialize()
        {
            base.ResizeMode = System.Windows.ResizeMode.CanMinimize;
            base.WindowStyle = System.Windows.WindowStyle.None;

            Window_Border = new Border();
            this.Content = Window_Border;

            Window_Border.BorderThickness = new Thickness(1);
            Window_Border.Child = Window_Grid;

            Window_Grid.Background = new SolidColorBrush(Color.FromRgb(56, 56, 56));
            Window_Grid.Children.Add(Window_TitleGrid);
            Window_Content_Grid.Children.Add(ContentPlaceHolder);
            Window_Grid.Children.Add(Window_Content_Grid);
            Window_Grid.Children.Add(Window_ResizeLeft);
            Window_Grid.Children.Add(Window_ResizeRight);
            Window_Grid.Children.Add(Window_ResizeTop);
            Window_Grid.Children.Add(Window_ResizeBottom);
            Window_Grid.Children.Add(Window_ResizeTopLeft);
            Window_Grid.Children.Add(Window_ResizeTopRight);
            Window_Grid.Children.Add(Window_ResizeBottomLeft);
            Window_Grid.Children.Add(Window_ResizeBottomRight);

            #region RESIZE
            Window_ResizeLeft.Opacity = 0;
            Window_ResizeLeft.Width = 14;
            Window_ResizeLeft.Margin = new Thickness(-6, 10, 0, 10);
            Window_ResizeLeft.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Window_ResizeLeft.Cursor = Cursors.SizeWE;
            Window_ResizeLeft.DragDelta += Resize_Left;

            Window_ResizeRight.Opacity = 0;
            Window_ResizeRight.Width = 14;
            Window_ResizeRight.Margin = new Thickness(0, 10, -6, 10);
            Window_ResizeRight.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            Window_ResizeRight.Cursor = Cursors.SizeWE;
            Window_ResizeRight.DragDelta += Resize_Right;

            Window_ResizeTop.Opacity = 0;
            Window_ResizeTop.Height = 14;
            Window_ResizeTop.Margin = new Thickness(10, -6, 128, 0);
            Window_ResizeTop.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Window_ResizeTop.Cursor = Cursors.SizeNS;
            Window_ResizeTop.DragDelta += Resize_Top;

            Window_ResizeBottom.Opacity = 0;
            Window_ResizeBottom.Height = 14;
            Window_ResizeBottom.Margin = new Thickness(10, 0, 10, -6);
            Window_ResizeBottom.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Window_ResizeBottom.Cursor = Cursors.SizeNS;
            Window_ResizeBottom.DragDelta += Resize_Bottom;

            Window_ResizeTopLeft.Opacity = 0;
            Window_ResizeTopLeft.Width = 14;
            Window_ResizeTopLeft.Height = 14;
            Window_ResizeTopLeft.Margin = new Thickness(-6, -6, 0, 0);
            Window_ResizeTopLeft.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Window_ResizeTopLeft.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Window_ResizeTopLeft.Cursor = Cursors.SizeNWSE;
            Window_ResizeTopLeft.DragDelta += Resize_Left;
            Window_ResizeTopLeft.DragDelta += Resize_Top;

            Window_ResizeTopRight.Opacity = 0;
            Window_ResizeTopRight.Width = 14;
            Window_ResizeTopRight.Height = 14;
            Window_ResizeTopRight.Margin = new Thickness(-6, 0, -6, 0);
            Window_ResizeTopRight.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            Window_ResizeTopRight.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Window_ResizeTopRight.Cursor = Cursors.SizeNESW;
            Window_ResizeTopRight.DragDelta += Resize_Right;
            Window_ResizeTopRight.DragDelta += Resize_Top;

            Window_ResizeBottomLeft.Opacity = 0;
            Window_ResizeBottomLeft.Width = 14;
            Window_ResizeBottomLeft.Height = 14;
            Window_ResizeBottomLeft.Margin = new Thickness(-6, 0, 0, -6);
            Window_ResizeBottomLeft.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Window_ResizeBottomLeft.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Window_ResizeBottomLeft.Cursor = Cursors.SizeNESW;
            Window_ResizeBottomLeft.DragDelta += Resize_Left;
            Window_ResizeBottomLeft.DragDelta += Resize_Bottom;

            Window_ResizeBottomRight.Opacity = 0;
            Window_ResizeBottomRight.Width = 14;
            Window_ResizeBottomRight.Height = 14;
            Window_ResizeBottomRight.Margin = new Thickness(0, 0, -6, -6);
            Window_ResizeBottomRight.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            Window_ResizeBottomRight.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Window_ResizeBottomRight.Cursor = Cursors.SizeNWSE;
            Window_ResizeBottomRight.DragDelta += Resize_Right;
            Window_ResizeBottomRight.DragDelta += Resize_Bottom;
            #endregion

            #region TITLE_BAR

            Window_TitleGrid.Height = TITLE_BAR_HEIGHT;
            Window_TitleGrid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Window_TitleGrid.MouseDown += Window_Drag;
            Window_TitleGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            Window_TitleGrid.Children.Add(Window_TitleLabel);
            Window_TitleGrid.Children.Add(Window_Button_Close);
            Window_TitleGrid.Children.Add(Window_Button_Maximize);
            Window_TitleGrid.Children.Add(Window_Button_Minimize);
            

            Window_TitleLabel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Window_TitleLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Window_TitleLabel.Height = TITLE_BAR_HEIGHT - 2;
            Window_TitleLabel.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            Window_TitleLabel.Margin = new Thickness(6, 0, 140, 0);
            Window_TitleLabel.Foreground = Brushes.White;

            Window_Button_Close.MouseEnter += Window_Button_MouseEnter;
            Window_Button_Close.MouseLeave += Window_Button_MouseLeave;
            Window_Button_Close.Click += Window_Close;
            Window_Button_Close.Content = 'r';
            Window_Button_Close.FontFamily = new System.Windows.Media.FontFamily("Webdings");
            Window_Button_Close.FontSize = 11;
            Window_Button_Close.Height = 20;
            Window_Button_Close.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            Window_Button_Close.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Window_Button_Close.Padding = new Thickness(0, 3, 0, 0);
            Window_Button_Close.Foreground = Brushes.Black;
            Window_Button_Close.BorderThickness = new Thickness(1);
            Window_Button_Close.FontWeight = FontWeights.Bold;
            Window_Button_Close.Width = 48;
            Window_Button_Close.Margin = new Thickness(0, -3, 5, 0);
            Window_Button_Close.Focusable = false;

            Window_Button_Maximize.MouseEnter += Window_Button_MouseEnter;
            Window_Button_Maximize.MouseLeave += Window_Button_MouseLeave;
            Window_Button_Maximize.Click += Window_MaximizeRestore;
            Window_Button_Maximize.Content = '1';
            Window_Button_Maximize.FontFamily = new System.Windows.Media.FontFamily("Webdings");
            Window_Button_Maximize.FontSize = 11;
            Window_Button_Maximize.Height = 20;
            Window_Button_Maximize.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            Window_Button_Maximize.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Window_Button_Maximize.Padding = new Thickness(0, 3, 0, 0);
            Window_Button_Maximize.Foreground = Brushes.Black;
            Window_Button_Maximize.BorderThickness = new Thickness(1);
            Window_Button_Maximize.FontWeight = FontWeights.Bold;
            Window_Button_Maximize.Width = 30;
            Window_Button_Maximize.Margin = new Thickness(0, -3, 52, 0);
            Window_Button_Maximize.Focusable = false;

            Window_Button_Minimize.MouseEnter += Window_Button_MouseEnter;
            Window_Button_Minimize.MouseLeave += Window_Button_MouseLeave;
            Window_Button_Minimize.Click += Window_Minimize;
            Window_Button_Minimize.Content = '0';
            Window_Button_Minimize.FontFamily = new System.Windows.Media.FontFamily("Webdings");
            Window_Button_Minimize.FontSize = 11;
            Window_Button_Minimize.Height = 20;
            Window_Button_Minimize.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            Window_Button_Minimize.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Window_Button_Minimize.Padding = new Thickness(0, 3, 0, 0);
            Window_Button_Minimize.Foreground = Brushes.Black;
            Window_Button_Minimize.BorderThickness = new Thickness(1);
            Window_Button_Minimize.FontWeight = FontWeights.Bold;
            Window_Button_Minimize.Width = 30;
            Window_Button_Minimize.Margin = new Thickness(0, -3, 81, 0);
            Window_Button_Minimize.Focusable = false;

            #endregion

            #region COLOR_PICKER
            foreach (ThemeColors t in AvailableThemeColors)
            {
                MenuItem NewItem = new MenuItem();
                NewItem.Header = t;
                NewItem.Click += (o, e) =>
                {
                    ThemeColor = (ThemeColors)((MenuItem)e.Source).Header;
                };
                ColorPicker.Items.Add(NewItem);
            }
            //Window_TitleGrid.ContextMenu = ColorPicker;
            #endregion

            Window_Button_Close.Style = (Style)this.FindResource("Window_Button_Close");
            Window_Button_Maximize.Style = (Style)this.FindResource("Window_Button_Maximize");
            Window_Button_Minimize.Style = (Style)this.FindResource("Window_Button_Minimize");
            Window_TitleGrid.Style = (Style)this.FindResource("Window_Frame_Title_Bar");
            Window_Border.Style = (Style)this.FindResource("Window_Frame_Border");

            EpilepticTimer.Tick += new EventHandler(EpilepticTimer_Tick);
        }

        void EpilepticTimer_Tick(object sender, EventArgs e)
        {
            this.ThemeColor = (ThemeColors)((int)(this.ThemeColor + 1) % NB_COLORS);
        }


        private void Window_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Minimize(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void MaximizeWindow()
        {
            _restoreLocation = new Rect { Width = Width, Height = Height, X = Left, Y = Top };

            System.Windows.Forms.Screen currentScreen;
            currentScreen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            Height = currentScreen.WorkingArea.Height;
            Width = currentScreen.WorkingArea.Width;
            Left = currentScreen.WorkingArea.X;
            Top = currentScreen.WorkingArea.Y;

            //Prohib resize
            Window_ResizeLeft.Cursor = Cursors.Arrow;
            Window_ResizeRight.Cursor = Cursors.Arrow;
            Window_ResizeTop.Cursor = Cursors.Arrow;
            Window_ResizeBottom.Cursor = Cursors.Arrow;
            Window_ResizeTopLeft.Cursor = Cursors.Arrow;
            Window_ResizeTopRight.Cursor = Cursors.Arrow;
            Window_ResizeBottomLeft.Cursor = Cursors.Arrow;
            Window_ResizeBottomRight.Cursor = Cursors.Arrow;

            //Remove border
            Window_Border.BorderThickness = new Thickness(0);
            Window_Button_Close.Width = X_BUTTON_MAXIMIZED_WIDTH;
            Window_Button_Close.Margin = new Thickness(Window_Button_Close.Margin.Left, Window_Button_Close.Margin.Top, Window_Button_Close.Margin.Right - 4, Window_Button_Close.Margin.Bottom);
        }

        private void Restore()
        {
            Height = _restoreLocation.Height;
            Width = _restoreLocation.Width;
            Left = _restoreLocation.X;
            Top = _restoreLocation.Y;

            //Restore resize cursors
            Window_ResizeLeft.Cursor = Cursors.SizeWE;
            Window_ResizeRight.Cursor = Cursors.SizeWE;
            Window_ResizeTop.Cursor = Cursors.SizeNS;
            Window_ResizeBottom.Cursor = Cursors.SizeNS;
            Window_ResizeTopLeft.Cursor = Cursors.SizeNWSE;
            Window_ResizeTopRight.Cursor = Cursors.SizeNESW;
            Window_ResizeBottomLeft.Cursor = Cursors.SizeNESW;
            Window_ResizeBottomRight.Cursor = Cursors.SizeNWSE;

            //Restore border
            Window_Border.BorderThickness = new Thickness(1);
            Window_Button_Close.Width = X_BUTTON_NORMAL_WIDTH;
            Window_Button_Close.Margin = new Thickness(Window_Button_Close.Margin.Left, Window_Button_Close.Margin.Top, Window_Button_Close.Margin.Right + 4, Window_Button_Close.Margin.Bottom);
        }

        private void Window_MaximizeRestore(object sender, RoutedEventArgs e)
        {
            this.Focus();
            if (Maximized)
            {
                ((Button)sender).Content = 1;
                Restore();
            }
            else
            {
                ((Button)sender).Content = 2;
                MaximizeWindow();
            }
            Maximized = !Maximized;
            this.Focus();
        }

        private void Window_Drag(object sender, MouseButtonEventArgs e)
        {
            if (!Maximized)
            {
                try
                {
                    this.DragMove();
                }
                catch (Exception) { }
            }
        }

        private void Window_Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Foreground = new SolidColorBrush(Colors.White);
        }

        private void Window_Button_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Foreground = new SolidColorBrush(Colors.Black);
        }

        private void Form_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Resize_Left(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (!Maximized)
            {
                double width = Width;
                double left = Left;

                width -= e.HorizontalChange;
                left += e.HorizontalChange;
                if (MinWidth < width && width <= MaxWidth &&
                    left > 0)
                {
                    Width = width;
                    Left = left;
                }
            }
        }

        private void Resize_Right(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (!Maximized)
            {
                double width = Width;

                width += e.HorizontalChange;
                if (MinWidth < width && width <= MaxWidth)
                {
                    Width = width;
                }
            }
        }

        private void Resize_Top(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (!Maximized)
            {
                double height = Height;
                double top = Top;

                height -= e.VerticalChange;
                top += e.VerticalChange;
                if (MinWidth < height && height <= MaxWidth &&
                    top > 0)
                {
                    Height = height;
                    Top = top;
                }
            }
        }

        private void Resize_Bottom(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (!Maximized)
            {
                double height = Height;

                height += e.VerticalChange;
                if (MinWidth < height && height <= MaxWidth)
                {
                    Height = height;
                }
            }
        }
    }
}