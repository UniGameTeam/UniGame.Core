﻿namespace UniModules.UniCore.Runtime.Common
{
    using System;
    using ObjectPool.Runtime.Interfaces;
    using UniGame.Core.Runtime.Interfaces;

    public class CompletionConditionSource : ICompletionStatus , IPoolable
    {
    
        private Func<bool> _competionFunc;

        public bool IsComplete
        {
            get { return _competionFunc == null || _competionFunc(); }
        }

        public void Initialize(Func<bool> competionFunc) {
            _competionFunc = competionFunc;
        }

        public void Dispose()
        {
            Release();
        }
    
        public void Release()
        {
            _competionFunc = null;
        }
    }
}
