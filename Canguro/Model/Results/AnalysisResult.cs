using System;
using System.Data;
using System.Configuration;

namespace Canguro.Model.Results
{
    public class AnalysisResult
    {
        private AnalysisState state;
        private int stateInfo;
        private AnalysisManifest manifest;
        public static readonly AnalysisResult UnkownStateResult = new AnalysisResult(null, AnalysisState.UnknownState, 0);

        public AnalysisManifest Manifest
        {
            get { return manifest; }
            set { manifest = value; }
        }

        public AnalysisState State
        {
            get { return state; }
            set { state = value; }
        }

        public int StateInfo
        {
            get { return stateInfo; }
            set { stateInfo = value; }
        }

        public AnalysisResult() { }
        public AnalysisResult(AnalysisManifest manifest, AnalysisState state, int stateInfo)
        {
            this.manifest = manifest;
            this.state = state;
            this.stateInfo = stateInfo;
        }

        public enum AnalysisState
        {
            Analyzing,
            Finished,
            UnknownState,
            Error
        }
    }
}
