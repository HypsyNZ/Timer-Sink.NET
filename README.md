# TimerSink.NET

[![Nuget](https://img.shields.io/nuget/v/TimerSink.NET)](https://www.nuget.org/packages/TimerSink.NET)

Timer Sink uses a `Stopwatch` to track a pool of `Tasks` and complete `Actions` at regular `Intervals`

TimingSinkItems don't suffer from Timer Drift.

TimingSinkItems can be set to wait for the UI Context, The default is `ConfigureAwait(false)`

# PrecisionTimer.NET

You will need [PrecisionTimer.NET](https://github.com/HypsyNZ/Precision-Timer.NET) to use this, Each `TimingSinkItem` will have a `PrecisionTimer` created for it automatically, 

You just need to `Start()` the `Sink` and the `PrecisionTimers` will also start

# Usage 

Create a `TimingSink`

```cs
TimingSink tSink = new TimingSink();
```

Create a `TimingSinkItem` with `ConfigureAwait(false)` that will run every 200ms

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

Create a `TimingSinkItem` with `ConfigureAwait(false)` that will run every 200ms

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


# Warning

Your Tasks will execute at the Interval and Execution time is Excluded.

If you place a long running activity into a `TimerSinkItem` and set the `Interval` to be shorter than the execution time you will end up with multiple running `Tasks`

You should be aware of this.

# Tips

If you found this useful pleases consider leaving a tip

- [x] BTC: 1NXUg88UvRWYn1WTnikVNn2fbbEtuTeXzm
- [x] ETH: 0x50740d132481be4721b1742670031baee3655ec2
- [x] DOGE: DS6orKQwdK4sBTmwoS9NVqvWCKA5ksGPfa
- [x] LTC: Lbd3oMKeokyXUQaxBDJpMMNVUws5wYhQES
