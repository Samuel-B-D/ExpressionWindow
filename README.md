# ExpressionWindow #

A simple library providing themed window, dialog and messagebox based on the ExpressionDark theme.
Include a color picker and a demo application.

## Usage ##
1. Reference `ExpressionWindow.dll` and `Microsoft.Windows.Shell.dll` in your project.
2. Turn a Window into an Expression Window
	* Add the following namespace to the window you wish to turn into an Expression Window : `xmlns:EW="clr-namespace:ThemedWindows;assembly=ExpressionWindow"`
	* (Optional) Add the following using clause to your CodeBehind : `using ThemedWindows;`
	* Inherit `ExpressionWindow` instead of `Window` in the CodeBehind (Or `ThemedWindows.ExpressionWindow` if you didn't add the using clause)
	
	### Example XAML ###
	```
	<EW:ExpressionWindow xmlns:EW="clr-namespace:ThemedWindows;assembly=ExpressionWindow"  x:Class="TEST.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="MainWindow">
		<Grid>
		</Grid>
	</EW:ExpressionWindow>
	```
	
	### Example CodeBehind ###
	```
	//...
	using ThemedWindows;
	
	napespace EXAMPLE
	{
		public partial class MainWindow : ExpressionWindow
		{
			public MainWindow()
			{
				InitializeComponent();
			}
		}
	}
	```
3. Enable in-designer preview of the theme by adding the following snipped to your App.xaml :
	```
	<Application.Resources>
        <EW:DesignTimeResourceDictionary xmlns:EW="clr-namespace:ThemedWindows;assembly=ExpressionWindow" DesignTimeColor="Green"/>
    </Application.Resources>
	```

## TODO ##
Redesign the yellow and white theme since they look pretty weird.

## License ##
The project is licensed under the Microsoft Public License (Ms-PL), for the exact terms please see the [LICENSE file](https://github.com/kazelone/ExpressionWindow/blob/master/LICENCE).
