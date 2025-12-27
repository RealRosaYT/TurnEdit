using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TurnEdit
{
    using System.Collections;
    using System.Resources;
    public static class IntelliSense
    {
        public static List<string> IterateIntelliSenseResx(string language)
        {
            using (ResXResourceReader reader = new ResXResourceReader(language))
            {
                IDictionaryEnumerator dict = reader.GetEnumerator();
                List<string>? completions = null;
                while (dict.MoveNext())
                {
                    object value = dict.Value!;
                    completions.Add((string)value);
                }
                return completions!;
            }
        }
    }
}