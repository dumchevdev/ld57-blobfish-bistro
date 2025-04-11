using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Runtime.Gameplay.Character
{
    public class UnitCommandManager : IDisposable
    {
        private readonly Queue<Func<UniTask>> _commandQueue = new();
        private CancellationTokenSource _cts = new();
        private bool _isProcessing;

        public void AddCommand(Func<UniTask> command)
        {
            _commandQueue.Enqueue(command);
            ProcessCommands().Forget();
        }

        private async UniTaskVoid ProcessCommands()
        {
            if (_isProcessing) return;
            _isProcessing = true;

            try
            {
                while (_commandQueue.Count > 0 && !_cts.IsCancellationRequested)
                {
                    var command = _commandQueue.Dequeue();
                    await command.Invoke().AttachExternalCancellation(_cts.Token).SuppressCancellationThrow();
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        public void ResetManager()
        {
            _commandQueue.Clear();
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _commandQueue.Clear();
        }
    }
}