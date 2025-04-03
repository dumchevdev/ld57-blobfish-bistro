using System;
using Cysharp.Threading.Tasks;

namespace Game.Runtime.CMS.Commons
{
    public class FuncComponent : CMSComponent
    {
        public Func<UniTask> Func;
    }
}