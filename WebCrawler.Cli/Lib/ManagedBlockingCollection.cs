using System.Collections;
using System.Collections.Concurrent;

namespace WebCrawler.Cli.Lib;

public class ManagedBlockingCollection<T> : IDisposable, IEnumerable<T>
{
    private readonly BlockingCollection<T> _queue;
    private volatile bool _autoCompleteStarted;
    private volatile int _intervalMilliseconds = 2000;
    private readonly Barrier _barrier;
    
    public ManagedBlockingCollection()
    {
        _queue = new BlockingCollection<T>();
        _barrier = new(0, _ => _queue.CompleteAdding());
    }
    
    public bool IsCompleted => _queue.IsCompleted;
    
    public void BeginObservingAutoComplete() => _autoCompleteStarted = true;
    
    public void Add(T item, CancellationToken cancellationToken = default)
        => _queue.Add(item, cancellationToken);
    
    public IEnumerable<T> GetConsumingEnumerable(
        CancellationToken cancellationToken = default)
    {
        _barrier.AddParticipant();
        try
        {
            while (true)
            {
                if (!_autoCompleteStarted)
                {
                    if (_queue.TryTake(out var item, _intervalMilliseconds,
                            cancellationToken))
                        yield return item;
                }
                else
                {
                    if (_queue.TryTake(out var item, 0, cancellationToken))
                        yield return item;
                    else if (_barrier.SignalAndWait(_intervalMilliseconds,
                                 cancellationToken))
                        break;
                }
            }
        }
        finally { _barrier.RemoveParticipant(); }
    }
    
    
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_queue).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<T>)_queue).GetEnumerator();
    }
    
    public void Dispose()
    {
        _queue.Dispose();
        _barrier.Dispose();
    }
}