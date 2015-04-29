using System;
using JetBrains.Annotations;

namespace Sequel
{
    [PublicAPI]
    public interface IDbPreparedCommand<out T> : IDisposable
    {
        T Execute([CanBeNull] object parameterValues = null);
    }
}