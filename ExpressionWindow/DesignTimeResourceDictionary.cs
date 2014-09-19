using System;
using System.ComponentModel;
using System.Windows;

namespace ThemedWindows
{
    public class DesignTimeResourceDictionary : ResourceDictionary
    {
        /// <summary>
        /// Local field storing info about designtime source.
        /// </summary>
        private string designTimeSource;

        /// <summary>
        /// Gets or sets the design time source.
        /// </summary>
        /// <value>
        /// The design time source.
        /// </value>
        public string DesignTimeColor
        {
            get
            {
                return this.designTimeSource;
            }

            set
            {
                this.designTimeSource = "pack://application:,,,/ExpressionWindow;component/Themes/" + value + "Colors.xaml";
                if ((bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)
                {
                    base.Source = new Uri(designTimeSource);
                }
            }
        }

        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) to load resources from.
        /// </summary>
        /// <returns>The source location of an external resource dictionary. </returns>
        public new Uri Source
        {
            get
            {
                throw new Exception("Use DesignTimeSource instead Source!");
            }

            set
            {
                throw new Exception("Use DesignTimeSource instead Source!");
            }
        }
    }
}