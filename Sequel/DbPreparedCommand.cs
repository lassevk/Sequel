using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using JetBrains.Annotations;

namespace Sequel
{
    internal class DbPreparedCommand
    {
        [NotNull]
        private readonly IDbCommand _Command;

        [CanBeNull, ItemNotNull]
        private readonly List<Tuple<PropertyInfo, IDbDataParameter>> _PropertiesAndParameters;

        public DbPreparedCommand([NotNull] IDbConnection connection, [CanBeNull] IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters)
        {
            _Command = connection.CreateCommand();
            _Command.Transaction = transaction;
            _Command.CommandText = sql;
            _PropertiesAndParameters = CreateParameterMapping(_Command, parameters);
            AssignParameters(parameters);
            _Command.Prepare();
        }

        private List<Tuple<PropertyInfo, IDbDataParameter>> CreateParameterMapping([NotNull] IDbCommand command, [CanBeNull] object parameters)
        {
            if (parameters == null)
                return null;

            var result = new List<Tuple<PropertyInfo, IDbDataParameter>>();
            foreach (var property in parameters.GetType().GetProperties())
            {
                if (!property.CanRead)
                    continue;
                if (property.GetIndexParameters().Length > 0)
                    continue;

                IDbDataParameter parameter = command.CreateParameter();
                parameter.ParameterName = property.Name;
                command.Parameters.Add(parameter);
                result.Add(Tuple.Create(property, parameter));
            }
            return result;
        }

        public void Dispose()
        {
            _Command.Dispose();
        }

        protected IDbCommand Command
        {
            get
            {
                return _Command;
            }
        }

        protected void AssignParameters([CanBeNull] object parameterValues)
        {
            if (parameterValues == null)
                return;

            if (_PropertiesAndParameters == null)
                throw new InvalidOperationException("Parameters have to specified when the command is prepared");

            foreach (var propertyAndParameter in _PropertiesAndParameters)
                propertyAndParameter.Item2.Value = propertyAndParameter.Item1.GetValue(parameterValues, null);
        }
    }
}