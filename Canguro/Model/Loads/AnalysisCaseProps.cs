using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Interfaz que agrupa las propiedades de un AnalysisCase.
    /// </summary>
    public interface AnalysisCaseProps : INamed
    {
        /// <summary>
        /// If the AnalysisCase depends on another one, it's returned by this property.
        /// </summary>
        AnalysisCase DependsOn
        {
            get;
        }
    }
}
