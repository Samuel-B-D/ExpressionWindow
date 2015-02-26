using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ThemedWindows.Effects
{
    class ShaderHelper
    {
        public static Uri MakeShaderURI(Type effect)
        {
            Assembly a = effect.Assembly;

            // Extract the short name. 
            string assemblyShortName = a.ToString().Split(',')[0];

            string uriString = "pack://application:,,,/" +
                assemblyShortName +
                ";component/Effects/" +
                effect.Name + ".ps";

            return new Uri(uriString);
        }
    }
}
