using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Local.JS.Extension.BatchedMultiTask
{
    public class TaskGroup:IDisposable
    {
        int Limit = -1;
        TaskGroup(int MaxParallelTasks)
        {
            Limit = MaxParallelTasks;

        }
        ConcurrentQueue<Delegate> Tasks = new();
        int CurrentTaskCount = 0;
        Action<Exception> ExceptionHandler = null;
        public void Batch(Delegate task)
        {
            if (Limit == -1) StartTask(task);
            else
            {
                if (CurrentTaskCount > Limit)
                {
                    Tasks.Enqueue(task);
                    //Console.WriteLine("Add into Waitlist:" + task.GetType());
                }
                else
                {
                    StartTask(task);
                }
            }
        }
        public void BatchAsyncTask(Func<Task> func)
        {
            Batch(new Action(() => { func().Wait(); }));
        }
        void StartTask(Delegate task)
        {
            CurrentTaskCount++;
            Task.Run(() =>
            {
                try
                {
                    if (Disposed) return;
                    var t=task.DynamicInvoke();
                    if(t is Task)
                    {
                        (t as Task).Wait();
                    }
                    if (Disposed) return;
                    CurrentTaskCount--;
                }
                catch (Exception e)
                {
                    if (ExceptionHandler is not null)
                        ExceptionHandler(e);
                }
                if (Tasks.IsEmpty is false)
                {
                    Delegate action;
                    while (Tasks.TryDequeue(out action) == false)
                    {
                        if (Tasks.IsEmpty == true) return;
                    }
                    StartTask(action);
                }
            });
        }
        public static TaskGroup CreateTaskGroup(int MaxParallelTasks, Action<Exception> ExceptionHandler)
        {
            int Count = MaxParallelTasks;
            if (MaxParallelTasks == 0)
            {
                Count = Environment.ProcessorCount;
            }
            TaskGroup taskGroup = new TaskGroup(Count);
            taskGroup.ExceptionHandler = ExceptionHandler;
            return taskGroup;
        }
        bool Disposed=false;
        public void Dispose()
        {
            Disposed = true;
            this.Tasks.Clear();

        }
    }
}
