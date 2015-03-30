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
    public class ColorizeEffect : ShaderEffect
    {
        private static PixelShader pixelShader =
            new PixelShader() { UriSource = ShaderHelper.MakeShaderURI(typeof(ColorizeEffect)) };

        public ColorizeEffect()
        {
            PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(ColorProperty);
        }

        ///////////////////////////////////////////////////////////////////////
        #region Input dependency property

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ColorizeEffect), 0);

        #endregion

        ///////////////////////////////////////////////////////////////////////
        #region Color dependency property

        /// <summary>
        /// Color tint.
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorizeEffect),
                    new UIPropertyMetadata(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), PixelShaderConstantCallback(0)));

        #endregion
    }
}