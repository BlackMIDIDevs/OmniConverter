using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OmniConverter
{
    public enum MoveDirection
    {
        Up = -1,
        Down = 1
    }

    public static class DebugExtensions
    {
        public static string WhoThis([CallerMemberName] string func = "")
        {
            return func;
        }
    }

    internal static class ListExtensions
    {
        public static int Move<T>(this IList<T> LINQList, int ITM, MoveDirection D)
        {
            // If there are few than 2 items, ignore
            if (LINQList.Count <= 1) return -1;

            // Store the expected new index for the item
            int FinalDir = ITM + (int)D;

            // Do some logic to prevent crashes
            int wIndexToMove = ((D == MoveDirection.Up) ? ((FinalDir < 0) ? LINQList.Count : ITM) : ((FinalDir >= LINQList.Count)) ? -1 : ITM) + (int)D;

            // Beep boop it should be all good, take a copy of the item
            var OldItem = LINQList[wIndexToMove];

            // If the item to move is at the end, move it back to the top
            if (ITM == LINQList.Count)
            {
                LINQList[wIndexToMove] = LINQList[0];
                LINQList[0] = OldItem;
            }
            // Else it's all good
            else
            {
                LINQList[wIndexToMove] = LINQList[ITM];
                LINQList[ITM] = OldItem;
            }

            // Return the new index value
            return wIndexToMove;
        }
    }

    public static class InputExtensions
    {
        public static int LimitToRange(
            this int value, int inclusiveMinimum, int inclusiveMaximum)
        {
            if (value < inclusiveMinimum) { return inclusiveMinimum; }
            if (value > inclusiveMaximum) { return inclusiveMaximum; }
            return value;
        }
    }

    public static class Parallel
    {
        public static void For(int from, int to, ParallelOptions parallelOptions, Action<int> func)
        {
            var cancel = parallelOptions.CancellationToken;
            var threads = parallelOptions.MaxDegreeOfParallelism;
            var tasks = new Dictionary<int, Task>();
            var completed = new BlockingCollection<int>();

            void RunTask(int i)
            {
                if (!cancel.IsCancellationRequested)
                {
                    var t = new Task(() =>
                    {
                        try
                        {
                            func(i);
                        }
                        finally
                        {
                            completed.Add(i);
                        }
                    });
                    tasks.Add(i, t);
                    t.Start();
                }
            }

            void TryTake()
            {
                var t = completed.Take(cancel);
                tasks[t].Wait();
                tasks.Remove(t);
            }

            for (int i = from; i < to && !cancel.IsCancellationRequested; i++)
            {
                RunTask(i);
                if (tasks.Count >= threads) TryTake();
            }

            while ((completed.Count > 0 || tasks.Count > 0) && !cancel.IsCancellationRequested) TryTake();
        }
    }

    public class CustomSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback action, object state)
        {
            SendOrPostCallback actionWrap = (object state2) =>
            {
                SynchronizationContext.SetSynchronizationContext(new CustomSynchronizationContext());
                action.Invoke(state2);
            };
            var callback = new WaitCallback(actionWrap.Invoke);
            ThreadPool.QueueUserWorkItem(callback, state);
        }
        public override SynchronizationContext CreateCopy()
        {
            return new CustomSynchronizationContext();
        }
        public override void Send(SendOrPostCallback d, object state)
        {
            base.Send(d, state);
        }
        public override void OperationStarted()
        {
            base.OperationStarted();
        }
        public override void OperationCompleted()
        {
            base.OperationCompleted();
        }

        public static TaskScheduler GetSynchronizationContext()
        {
            TaskScheduler taskScheduler = null;

            try
            {
                taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }
            catch { }

            if (taskScheduler == null)
            {
                try
                {
                    taskScheduler = TaskScheduler.Current;
                }
                catch { }
            }

            if (taskScheduler == null)
            {
                try
                {
                    var context = new CustomSynchronizationContext();
                    SynchronizationContext.SetSynchronizationContext(context);
                    taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                }
                catch { }
            }

            return taskScheduler;
        }
    }
}
