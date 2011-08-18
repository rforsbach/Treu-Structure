using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to show the Torsional component of the internal forces in elements.
    /// </summary>
    public class ColorLinesByLayer : Canguro.Commands.ViewCommand
    {
        private ColorLinesByLayer() { }
        public static ColorLinesByLayer Instance = new ColorLinesByLayer();

        /// <summary>
        /// This command is not interactive. Returns false.
        /// </summary>
        public override bool IsInteractive
        {
            get { return false; }
        }

        /// <summary>
        /// Executes the Non-Interactive Command.
        /// Sets the InternalForcesShown property of RenderOptions to Mx
        /// </summary>
        /// <param name="activeView">The Current Active View object</param>
        public override void Run(Canguro.View.GraphicView activeView)
        {
            Canguro.View.Renderer.ModelRenderer mr = activeView.ModelRenderer;
            if (mr != null)
                mr.RenderOptions.LineColoredBy = Canguro.View.Renderer.RenderOptions.LineColorBy.Layer;
        }
    }
}
