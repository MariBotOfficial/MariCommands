using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MariCommands.Results;
using Xunit;

namespace MariCommands.Tests
{
    public class VoidTaskExecutor
    {
        [Fact]
        public async Task CanExecute()
        {
            await Task.Delay(0);
        }
    }


    public class VoidTaskCommand
    {
        public Task ExecuteAsync()
            => Task.CompletedTask;
    }
}