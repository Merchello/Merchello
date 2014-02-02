using System.Collections.Generic;
using Umbraco.Core;

namespace Merchello.Core.Pipelines
{
    /// <summary>
    /// Represents a PipeLine base class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class PipeLineBase<T>  : IPipelineBase<T> 
        where T : IPipelineTask
    {
        private readonly Queue<T> _pipelineTaskQueue = new Queue<T>(); 

        internal PipeLineBase()
        {
        }

        public void RegisterTask(T task)
        {
            throw new System.NotImplementedException();
        }

        public Attempt<IPipelineEndOfChain> Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}