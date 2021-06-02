﻿namespace UniModules.UniCore.Runtime.Rx.Extensions
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using ObjectPool.Runtime;
    using Rx;
    using UniGame.Core.Runtime.Interfaces.Rx;
    using UniRx;

    public static class RxExtension
    {
        public static IObservable<bool> Any<T>(this IEnumerable<IObservable<T>> source, Predicate<T> predicate)
        {
            return source.CombineLatest().Select(x => x.Any(m => predicate(m)));
        }

        public static IObservable<bool> All<T>(this IEnumerable<IObservable<T>> source, Predicate<T> predicate)
        {
            return source.CombineLatest().Select(x => x.All(m => predicate(m)));
        }
        
        public static IObservable<TResult> CombineLatestFunc<TValue,TValue2, TResult>(
            this IObservable<TValue> source, 
            Func<IObservable<TValue>,IObservable<TValue2>> func,
            Func<TValue,TValue2,TResult> resultFunc)
        {
            var funcObservable = func(source);
            return source.CombineLatest(funcObservable, resultFunc);
        }
        
        public static IDisposable Cancel(this IDisposable disposable, bool clearValue = true)
        {
            disposable?.Dispose();
            return clearValue ? null : disposable;
        }

        public static void Cancel(this object target, ref IDisposable disposable)
        {
            disposable.Cancel();
            disposable = null;
        }
        
        public static void Cancel<TItem>(this List<TItem> disposables)
            where TItem : IDisposable
        {
            if (disposables == null)
                return;
            for (var i = 0; i < disposables.Count; i++)
            {
                disposables[i]?.Dispose();
            }
            
            disposables.Clear();
        }

        public static void Cancel(this List<IDisposable> disposables)
        {
            if (disposables == null)
                return;
            for (var i = 0; i < disposables.Count; i++)
            {
                disposables[i]?.Dispose();
            }
            
            disposables.Clear();
        }

        public static IRecycleObserver<T> CreateRecycleObserver<T>(this object _, 
            Action<T> onNext, 
            Action onComplete = null,
            Action<Exception> onError = null)
        {
            
            var observer = ClassPool.Spawn<RecycleActionObserver<T>>();
            
            observer.Initialize(onNext,onComplete,onError);

            return observer;

        }

        public static IObservable<T> When<T>(this IObservable<T> source, Predicate<T> predicate, Action<T> action)
        {
            return source.Do(x =>
            {
                if (predicate(x))
                {
                    action(x);
                }
            });
        }

        public static IObservable<T> When<T>(this IObservable<T> source, Predicate<T> predicate, Action<T> actionIfTrue, Action<T> actionIfFalse)
        {
            return source.Do(x =>
            {
                if (predicate(x))
                {
                    actionIfTrue(x);
                }
                else
                {
                    actionIfFalse(x);
                }
            });
        }

        public static IObservable<T> WhenTrue<T>(this IObservable<T> source, Action<T> action)
        {
            return source.When<T>(x => x, action);
        }

        public static IObservable<T> WhenFalse<T>(this IObservable<T> source, Action<T> action)
        {
            return source.When<T>(x => !x, action);
        }
    }
}
