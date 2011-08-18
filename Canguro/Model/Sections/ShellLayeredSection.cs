using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public class ShellLayeredSection : AreaSection
    {
        public ShellLayeredSection(string name, string shape, Material.Material material) : base(name, shape, material) { }
    }
}
