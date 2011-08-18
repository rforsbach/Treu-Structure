using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to hide all Internal Forces Diagrams
    /// </summary>
    class HideDiagrams : ViewCommand
    {
        private HideDiagrams() { }
        public static HideDiagrams Instance = new HideDiagrams();

        /// <summary>
        /// This command is not interactive. Returns false.
        /// </summary>
        public override bool IsInteractive
        {
            get { return false; }
        }

        /// <summary>
        /// Sets the RenderOptions.InternalForcesShown property to None
        /// </summary>
        /// <param name="activeView">The Active View object</param>
        public override void  Run(Canguro.View.GraphicView activeView)
        {
            if (activeView.ModelRenderer != null && activeView.ModelRenderer.ForcesRenderer != null)
                activeView.ModelRenderer.RenderOptions.InternalForcesShown = Canguro.View.Renderer.RenderOptions.InternalForces.None;
        }
    }
}
