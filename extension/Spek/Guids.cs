// Guids.cs
// MUST match guids.h
using System;

namespace ElectricSky.Spek
{
    static class GuidList
    {
        public const string guidSpekPkgString = "5487e730-81ec-4bee-b65e-4bc297a5fccc";
        public const string guidSpekCmdSetString = "53790482-537b-44ef-8ab0-8af8f0d44903";

        public static readonly Guid guidSpekCmdSet = new Guid(guidSpekCmdSetString);
    };
}