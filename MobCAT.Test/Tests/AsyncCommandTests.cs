using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.MobCAT.MVVM;
using NUnit.Framework;

[Parallelizable(ParallelScope.None)]
public class AsyncCommandTests
{
    const int CoalescingTestParallelExecutions = 5;

    /// <summary>
    /// Validates that CanExecute returns a value based on the custom CanExecute action when one is provided.
    /// </summary>
    [Test, TestCase(TestName = "CanExecute Uses Custom Action Test")]
    public void CanExecuteUsesCustomActionTest()
    {
        bool canExecute = false;
        ICommand asyncCommand = new AsyncCommand(() => Task.Run(() => { }), () => canExecute);
        Assert.IsFalse(asyncCommand.CanExecute(null));
        canExecute = true;
        Assert.IsTrue(asyncCommand.CanExecute(null));
    }

    /// <summary>
    /// Validates that the default behavior for the <see cref="AsyncCommand"/> command is to return true for CanExecute where no explicit action is set for this.
    /// </summary>
    /// <remarks>This applies to situations where coalescing is not enabled.</remarks>
    [Test, TestCase(TestName = "CanExecute Default Action When Coalescing Disabled Test")]
    public void CanExecuteDefaultActionNoCoalescingTest()
    {
        ICommand asyncCommand = new AsyncCommand(() => Task.Run(async () => { await Task.Delay(10); }));
        Assert.IsTrue(asyncCommand.CanExecute(null));
        asyncCommand.Execute(null);
        Assert.IsTrue(asyncCommand.CanExecute(null));
    }

    /// <summary>
    /// Validates that the default behavior for the <see cref="AsyncCommand"/> command is to return true for CanExecute where no explicit action is set for this and the action is not currently being executed.
    /// </summary>
    /// <remarks>This applies to situations where coalescing is enabled.</remarks>
    [Test, TestCase(TestName = "CanExecute Default Action When Coalescing Enabled Test")]
    public void CanExecuteDefaultActionCoalescingTest()
    {
        ICommand asyncCommand = new AsyncCommand(() => Task.Run(async () => { await Task.Delay(10); }), true);
        Assert.IsTrue(asyncCommand.CanExecute(null));
        asyncCommand.Execute(null);
        Assert.IsFalse(asyncCommand.CanExecute(null));
    }

    /// <summary>
    /// Ensures that the Execute action is invoked when the value of CanExecute is true.
    /// </summary>
    [Test, TestCase(TestName = "Handle Execute When CanExecute Is True Test")]
    public void HandleExecuteWhenCanExecuteIsTrueTest()
        => TestExecuteOnCanExecuteBehavior(true);

    /// <summary>
    /// Ensures that the Execute action is not invoked when the value of CanExecute is false.
    /// </summary>
    [Test, TestCase(TestName = "Handle Execute When CanExecute Is False Test")]
    public void HandleExecuteWhenCanExecuteIsFalseTest()
        => TestExecuteOnCanExecuteBehavior(false);

    /// <summary>
    /// Ensures that only a single task is run at any given time when invoking the <see cref="AsyncCommand"/> command multiple times during the original task execution. 
    /// </summary>
    [Test, TestCase(TestName = "Coalescing Enabled Test")]
    public void CoalescingEnabledTest()
        => TestCoalescingOption(true, CoalescingTestParallelExecutions);

    /// <summary>
    /// Ensures that multiple executions of the underlying task are possible when the <see cref="AsyncCommand"/> command is configured to do so.
    /// </summary>
    [Test, TestCase(TestName = "Coalescing Disabled Test")]
    public void CoalescingDisabledTest()
        => TestCoalescingOption(false, CoalescingTestParallelExecutions, CoalescingTestParallelExecutions);

    void TestExecuteOnCanExecuteBehavior(bool shouldExecute)
    {
        bool executed = false;
        ICommand asyncCommand = new AsyncCommand(() => Task.Run(() => { executed = true; }), () => shouldExecute);
        asyncCommand.Execute(null);
        Assert.That(executed == shouldExecute, Is.EqualTo(executed == shouldExecute).After(delayInMilliseconds: 100, pollingInterval: 10));
    }

    void TestCoalescingOption(bool enableCoalescing, int parallelExecutions, int expectedExecutions = 1)
    {
        int executions = 0;

        ICommand asyncCommand = new AsyncCommand(() => Task.Run(async () =>
        {
            executions++;
            await Task.Delay(10);
        }), enableCoalescing);

        var tasks = new List<Task>();

        for (var i = 0; i < parallelExecutions; i++)
            tasks.Add(Task.Run(() => asyncCommand.Execute(null)));

        Assert.That(expectedExecutions == executions, Is.EqualTo(expectedExecutions == executions).After(delayInMilliseconds: 100, pollingInterval: 10));

        Task.Run(() => asyncCommand.Execute(null)).GetAwaiter().GetResult();

        Assert.That(expectedExecutions + 1 == executions, Is.EqualTo(expectedExecutions + 1 == executions).After(delayInMilliseconds: 100, pollingInterval: 10));
    }
}