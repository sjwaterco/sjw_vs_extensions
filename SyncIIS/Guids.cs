// Guids.cs
// MUST match guids.h
using System;

namespace SanJoseWaterCompany.SyncIIS
{
    static class GuidList
    {
        public const string guidSyncIISPkgString = "a74f763c-9598-4209-901b-d7e3b65c2a43";
        public const string guidSyncIISCmdSetString = "25c9e3c7-8e6d-4252-8500-fcbd097ba107";
        public const string guidToolWindowPersistanceString = "eff74664-7a12-44d5-a586-e32dc4906732";

        public static readonly Guid guidSyncIISCmdSet = new Guid(guidSyncIISCmdSetString);
    };
}