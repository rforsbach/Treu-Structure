using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Renderer
{
    public class StressAreaRenderer : AreaRenderer
    {
        public override void Render(Microsoft.DirectX.Direct3D.Device device, Canguro.Model.AreaElement area, RenderOptions options)
        {
            throw new System.NotImplementedException();
        }

        public override void Render(Microsoft.DirectX.Direct3D.Device device, Canguro.Model.Model model, IEnumerable<Canguro.Model.AreaElement> areas, RenderOptions options, List<Canguro.Model.Item> itemsInView)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
