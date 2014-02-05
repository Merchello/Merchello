using Merchello.Core.Chains;
using Umbraco.Core;

namespace Merchello.Tests.UnitTests.Chains
{
    internal class DemoAttemptChainTask : IAttemptChainTask<int>
    {
        public int Index { get; set; }

        public Attempt<int> PerformTask(int arg)
        {
            var addOne = arg + 1;
            return Attempt.Succeed(addOne);
        }
    }
}