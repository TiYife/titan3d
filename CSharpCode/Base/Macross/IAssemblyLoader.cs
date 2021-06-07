﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EngineNS.Macross
{
    public interface IAssemblyLoader
    {
        List<string> IncludeAssemblies { get; }
        System.Reflection.Assembly LoadAssembly(string assemblyPath);
        void TryUnload();
        object GetInnerObject();
    }
}