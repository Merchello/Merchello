using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Chains;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Chains
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
    public class TaskChainConceptTests
    {
        private IEnumerable<AttemptChainTaskHandler<int>> _tasks;
        private int _taskCount = 10;


        [SetUp]
        public void Init()
        {
            var taskList = new List<AttemptChainTaskHandler<int>>();

            for (var i = 0; i < _taskCount; i++)
            {
                var newTask = new AttemptChainTaskHandler<int>(new DemoAttemptChainTask() { Index = i });
                taskList.Add(newTask);
            }

            foreach (var task in taskList.Where(task => taskList.IndexOf(task) != taskList.IndexOf(taskList.Last())))
            {
                task.RegisterNext(taskList[taskList.IndexOf(task) + 1]);
            }
          
            _tasks = taskList;

        }

        [Test]
        public void Can_Execute_All_Tasks_In_The_TaskChain()
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
}