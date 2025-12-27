using System;
using System.Collections.Generic;
using System.Text;

namespace TurnEdit
{
    public class LanguageInfo
    {
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public override string ToString() => DisplayName;
    }
}
