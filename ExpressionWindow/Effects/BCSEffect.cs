using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows;

namespace ThemedWindows.Effects
{
    public class BCSEffect : ShaderEffect
    {
        private static PixelShader pixelShader =
            new PixelShader() { UriSource = ShaderHelper.MakeShaderURI(typeof(BCSEffect)) };

        public BCSEffect()
        {
            PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(BrightnessProperty);
            UpdateShaderValue(ContrastProperty);
            UpdateShaderValue(SaturationProperty);
        }

        ///////////////////////////////////////////////////////////////////////
        #region Input dependency property

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(BCSEffect), 0);

        #endregion

        ///////////////////////////////////////////////////////////////////////
        #region Brightness dependency property

        /// <summary>
        /// Brightness modifier (between -1 and 1).
        /// </summary>
        public double Brightness
        {
            get { return (double)GetValue(BrightnessProperty); }
            set { SetValue(BrightnessProperty, value); }
        }

        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(BCSEffect),
                    new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        #endregion

        ///////////////////////////////////////////////////////////////////////
        #region Contrast dependency property

        /// <summary>
        /// Contrast modifier (between 0 and a lot).
        /// </summary>
        public double Contrast
        {
            get { return (double)GetValue(ContrastProperty); }
            set { SetValue(ContrastProperty, value); }
        }

        public static readonly DependencyProperty ContrastProperty =
            DependencyProperty.Register("Contrast", typeof(double), typeof(BCSEffect),
                    new UIPropertyMetadata(1.0, PixelShaderConstantCallback(1)));

        #endregion

        ///////////////////////////////////////////////////////////////////////
        #region Saturation dependency property

        /// <summary>
        /// Saturation modifier (between 0 (normal) and 1 (greyscale)).
        /// </summary>
        public double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }

        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register("Saturation", typeof(double), typeof(BCSEffect),
                    new UIPropertyMetadata(0.0, PixelShaderConstantCallback(2)));

        #endregion
    }
}