using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Pipelines;
using NUnit.Framework;
using Umbraco.Core;

namespace Merchello.Tests.UnitTests.Pipeline
{
    // Generate Invoice (method in checkoutbase)
    // 
    //  Create a new Invoice copying line items for basket -> add shipping line item(s) -> calculate taxes -> apply discounts -> end of chain
    // 
    // ---> persist invoice (complete checkout) 
    // -----> process payment -> persist payment -> apply payment (payment provider should do all of this) and return Approval to generate an order  
    //
    // Order Pipeline
    // Generate order( invoice ) -> Generate shipment(s) (invoice)

    [TestFixture]
    public class ConceptPipelineTests
    {
        private IEnumerable<TestPipelineTaskHandler> _tasks;
        private int _taskCount = 10;


        [SetUp]
        public void Init()
        {         
            var taskList = new List<TestPipelineTaskHandler>();

            for (var i = 0; i < _taskCount; i++)
            {
                var newTask = new TestPipelineTaskHandler(new DemoPipelineTask() {Index = i});
                taskList.Add(newTask);
            }

            foreach (var task in taskList.Where(task => taskList.IndexOf(task) != taskList.IndexOf(taskList.Last())))
            {
                task.RegisterNext(taskList[taskList.IndexOf(task) + 1]);
            }
          
            _tasks = taskList;

        }

        [Test]
        public void Can_Execute_All_Tasks_In_The_Pipeline()
        {
            //// Arrange
            var first = _tasks.FirstOrDefault();
            var expected = 10;
            Assert.NotNull(first);

            //// Act
            var attempt = first.Execute(0);

            //// Assert
            Assert.IsTrue(attempt.Success);
            Console.Write(attempt.Result);
            Assert.AreEqual(expected, attempt.Result);
        }

       

    }

    internal class TestPipelineTaskHandler : PipelineTaskHandlerBase<int>
    {
        public TestPipelineTaskHandler(IPipelineTask<int> task) : base(task)
        {
        }
    }

    internal class DemoPipelineTask : IPipelineTask<int>
    {
        public int Index { get; set; }

        public Attempt<int> PerformTask(int arg)
        {
            var addOne = arg + 1;
            return Attempt.Succeed(addOne);
        }
    }
}