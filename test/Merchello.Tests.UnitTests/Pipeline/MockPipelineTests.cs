using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Pipelines;
using NUnit.Framework;
using Umbraco.Core;

namespace Merchello.Tests.UnitTests.Pipeline
{
    [TestFixture]
    public class MockPipelineTests
    {
        private IEnumerable<TestPipelineTaskHandler> _tasks;
        private int _taskCount = 10;

        [SetUp]
        public void Init()
        {
            var taskList = new List<TestPipelineTaskHandler>();
            //for (int i = 0; i < _taskCount; i++)
            //{
            //    var newTask = new TestPipelineTaskHandler(new DemoPipelineTask() { Index = i }); 
            //    taskList.Add(newTask);
            //    if (i >= 1) taskList[i - i].RegisterNext(newTask);
            //}

            var task1 = new TestPipelineTaskHandler(new DemoPipelineTask() {Index = 1});
            var task2 = new TestPipelineTaskHandler(new DemoPipelineTask() { Index = 2 });
            var task3 = new TestPipelineTaskHandler(new DemoPipelineTask() { Index = 3 });
            var task4 = new TestPipelineTaskHandler(new DemoPipelineTask() { Index = 4 });
            var task5 = new TestPipelineTaskHandler(new DemoPipelineTask() { Index = 5 });
            var task6 = new TestPipelineTaskHandler(new DemoPipelineTask() { Index = 6 });
            var task7 = new TestPipelineTaskHandler(new DemoPipelineTask() { Index = 7 });
            var task8 = new TestPipelineTaskHandler(new DemoPipelineTask() { Index = 8 });
            var task9 = new TestPipelineTaskHandler(new DemoPipelineTask() { Index = 9 });

            task1.RegisterNext(task2);
            task2.RegisterNext(task3);
            task3.RegisterNext(task4);
            task4.RegisterNext(task5);
            task5.RegisterNext(task6);
            task6.RegisterNext(task7);
            task7.RegisterNext(task8);
            task8.RegisterNext(task9);

            taskList.Add(task1);
            taskList.Add(task2);
            taskList.Add(task3);
            taskList.Add(task4);
            taskList.Add(task5);
            taskList.Add(task6);
            taskList.Add(task7);
            taskList.Add(task8);
            taskList.Add(task9);

            _tasks = taskList;

        }

        [Test]
        public void Can_Execute_All_Tasks_In_The_Pipeline()
        {
            //// Arrange
            var first = _tasks.FirstOrDefault();
            Assert.NotNull(first);

            //// Act
            var attempt = first.Execute(0);

            //// Assert
            Assert.IsTrue(attempt.Success);
            Console.Write(attempt.Result);
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