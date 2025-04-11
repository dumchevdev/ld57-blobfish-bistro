using System;
using Cysharp.Threading.Tasks;

namespace Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons
{
    public class FuncComponent : CMSComponent
    {
        public Func<UniTask> Func;
    }
}