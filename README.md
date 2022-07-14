# TimerSink.NET

[![Nuget](https://buildstats.info/nuget/TimerSink.NET)](https://www.nuget.org/packages/TimerSink.NET)

Uses a `Stopwatch` and `PrecisionTimer` to track a pool of `Tasks` and complete them at regular `Intervals`

The `Resolution` for events is `1 Millisecond` and the `TimeSink` is capable of honoring it.

`TimingSinkItems` can be set to wait for the Synchronization Context, The default is `ConfigureAwait(false)`

# Usage 

Create a `TimingSink`

```cs
TimingSink tSink = new TimingSink();
```

Create a `TimingSinkItem` with `ConfigureAwait(false)` that will run every `200ms`

```cs
private void TaskMethod() { // Your Code Here }
TimingSinkItem TaskToTime = new TimingSinkItem(TaskMethod, 200);
```

Add the `TimingSinkItem` to the `TimingSink`

```cs
tSink.TimingSinkItems.Add(TaskToTime);
```

Start the Sink.

```cs
tSink.Start();
```

# Examples

Create a `TimingSinkItem` with `ConfigureAwait(false)` that will run every `200ms`

```cs
TimingSinkItem TaskToTime = new TimingSinkItem(TaskMethod, 200);
private void TaskMethod() { }
```

Create a `TimingSinkItem` with `ConfigureAwait(true)`

```cs
TimingSinkItem TaskToTimeAwaitUI = new TimingSinkItem(TaskMethodUI, 200, true);
private void TaskMethodUI() { }
```

Create a `TimingSinkItem` with `ConfigureAwait(true)` and `throwOnError` to catch exceptions

```cs
TimingSinkItem TaskToTimeThrowError = new TimingSinkItem(TaskMethodUI, 200, true, true);
```

Check the `Health` of the `Sink`
```cs
if(tSink.SinkFaulted){ }
```

You can view a [Full Example on Github](https://github.com/HypsyNZ/Timer-Sink.NET/tree/main/TestSink/)

# Warning

Your Tasks will execute at the Interval and Execution time is Excluded.

If you place a long running activity into a `TimerSinkItem` and set the `Interval` to be shorter than the execution time you will end up with multiple running `Tasks`

You can use [LockedTask.NET](https://www.nuget.org/packages/LockedTask.NET) to ensure there is only ever one of your `Task` for long running methods
