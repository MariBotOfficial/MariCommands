using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MariGlobals.Extensions;

namespace MariCommands.Features
{
    internal sealed class DisposablesFeature : IDisposablesFeature
    {
        private List<IDisposable> _disposables;
        private List<IAsyncDisposable> _asyncDisposables;

        public void RegisterForDispose(IDisposable disposable)
        {
            if (_disposables == null)
                _disposables = new List<IDisposable>();

            _disposables.Add(disposable);
        }

        public void RegisterForDisposeAsync(IAsyncDisposable asyncDisposable)
        {
            if (_asyncDisposables == null)
                _asyncDisposables = new List<IAsyncDisposable>();

            _asyncDisposables.Add(asyncDisposable);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposables.HasContent())
            {
                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
            }

            if (_asyncDisposables.HasContent())
            {
                foreach (var asyncDisposable in _asyncDisposables)
                {
                    await asyncDisposable.DisposeAsync();
                }
            }

            _disposables?.Clear();
            _asyncDisposables?.Clear();
        }
    }
}