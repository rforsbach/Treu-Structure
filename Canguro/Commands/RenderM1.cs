using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to show the Torsional component of the internal forces in elements.
    /// </summary>
    public class RenderM1 : Canguro.Commands.ViewCommand
    {
        private RenderM1() { }
        public static RenderM1 Instance = new RenderM1();

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
        public override void  Run(Canguro.View.GraphicView activeView)
        {
            Canguro.View.Renderer.ModelRenderer mr = activeView.ModelRenderer;
            if (mr != null && mr.ForcesRenderer != null)
            {
                if (mr.RenderOptions.InternalForcesShown == Canguro.View.Renderer.RenderOptions.InternalForces.Mx)
                    mr.RenderOptions.InternalForcesShown = Canguro.View.Renderer.RenderOptions.InternalForces.None;
                else
                    mr.RenderOptions.InternalForcesShown = Canguro.View.Renderer.RenderOptions.InternalForces.Mx;
            }
        }
    }
}
