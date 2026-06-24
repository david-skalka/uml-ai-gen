using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace TodoApp.Infrastructure;

public static class ObservableObjectExtensions
{
    public static IObservable<bool> ObservePropertyNotNull<TSource, TProperty>(
        this TSource source,
        Expression<Func<TSource, TProperty>> property)
        where TSource : INotifyPropertyChanged =>
        ObserveProperty(source, property, x => x != null);

    private static IObservable<TRet> ObserveProperty<TSource, TProperty, TRet>(
        this TSource source,
        Expression<Func<TSource, TProperty>> property,
        Func<TProperty, TRet> selector)
        where TSource : INotifyPropertyChanged
    {
        var propertyName = ((MemberExpression)property.Body).Member.Name;
        var getValue = property.Compile();

        return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h)
            .Where(e => e.EventArgs.PropertyName == propertyName)
            .Select(_ => selector(getValue(source)))
            .StartWith(selector(getValue(source)));
    }
}
