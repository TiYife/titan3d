﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCooker
{

    class UCookSettings
    {
    }

    class UCookCommand
    {
        public virtual async System.Threading.Tasks.Task ExecuteCommand(string[] args)
        {
            await EngineNS.Thread.AsyncDummyClass.DummyFunc();
        }
        public static string FindArgument(string[] args, string startWith)
        {
            foreach (var i in args)
            {
                if (i.StartsWith(startWith))
                {
                    return i.Substring(startWith.Length);
                }
            }
            return null;
        }
        public static string[] GetArguments(string[] args, string startWith, char split='+')
        {
            var types = FindArgument(args, startWith);
            if (types != null)
            {
                return types.Split(split);
            }
            return null;
        }
    }
}