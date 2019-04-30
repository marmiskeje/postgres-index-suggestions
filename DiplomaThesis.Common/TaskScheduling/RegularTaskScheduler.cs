using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DiplomaThesis.Common.TaskScheduling
{
    public class RegularTaskScheduler : IRegularTaskScheduler
    {
        private readonly ICommandProcessingQueue<IExecutableCommand> queue;
        private readonly List<InternalTask> registeredTasksAscSorted;
        private readonly Timer timer;
        private readonly object lockObject = new object();
        private bool isDisposed = false;
        private LinkedList<InternalTask> availableTasks;
        private InternalTask followingTask;

        private class InternalTask
        {
            public TimeSpan Time { get; set; }
            public IExecutableCommand Task { get; set; }
        }
        public RegularTaskScheduler(ICommandProcessingQueue<IExecutableCommand> queue, IDictionary<TimeSpan, IExecutableCommand> tasks)
        {
            this.queue = queue;
            this.registeredTasksAscSorted = new List<InternalTask>();
            foreach (var t in tasks)
            {
                this.registeredTasksAscSorted.Add(new InternalTask() { Time = t.Key, Task = t.Value });
            }
            this.registeredTasksAscSorted.Sort((x, y) => x.Time.CompareTo(y.Time));
            this.availableTasks = new LinkedList<InternalTask>(registeredTasksAscSorted);
            this.timer = new Timer(Timer_Job, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            Stop();
            PlanTimer();
        }

        public void Stop()
        {
            lock (lockObject)
            {
                this.timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void Timer_Job(object obj)
        {
            Stop();
            lock (lockObject)
            {
                if (followingTask != null)
                {
                    queue.Enqueue(followingTask.Task);
                    followingTask = null;
                }
            }
            if (!isDisposed)
            {
                PlanTimer();
            }
        }

        private void PlanTimer()
        {
            lock (lockObject)
            {
                followingTask = null;
                DateTime now = DateTime.Now;
                InternalTask next = null;
                TimeSpan addDay = TimeSpan.FromDays(0);
                while (next == null && registeredTasksAscSorted.Count > 0)
                {
                    var potentiallyNext = availableTasks.First?.Value;
                    if (potentiallyNext != null)
                    {
                        availableTasks.RemoveFirst();
                        if (potentiallyNext.Time.Add(addDay) > now.TimeOfDay)
                        {
                            next = potentiallyNext;
                        }
                    }
                    else
                    {
                        availableTasks = new LinkedList<InternalTask>(registeredTasksAscSorted);
                        addDay = addDay.Add(TimeSpan.FromDays(1));
                    }
                }
                if (next != null)
                {
                    var waitTime = next.Time.Add(addDay) - now.TimeOfDay;
                    if (!isDisposed)
                    {
                        followingTask = next;
                        timer.Change(waitTime, waitTime);
                    }
                }
            }
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    Stop();
                }
                timer.Dispose();
                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RegularTaskScheduler()
        {
            Dispose(false);
        }
    }
}