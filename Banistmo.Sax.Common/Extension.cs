using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//JMMB
namespace Banistmo.Sax.Common
{
    public static class Extension
    {
        public static DbContext BulkInsert<T>(this DbContext context, T entity, int count, int batchSize) where T : class
        {
            context.Set<T>().Add(entity);

            if (count % batchSize == 0)
            {
                context.SaveChanges();
                context.Dispose();
                context = new DbContext("DefaultConnection");

                // This is optional
                context.Configuration.AutoDetectChangesEnabled = false;
            }
            return context;
        }

        public static object ChangeType(this object value, Type convertToType)
        {
            if (convertToType == null)
            {
                throw new ArgumentNullException("convertToType");
            }

            // return null if the value is null or DBNull
            if (value == null || value is DBNull)
            {
                return null;
            }

            // non-nullable types, which are not supported by Convert.ChangeType(),
            // unwrap the types to determine the underlying time
            if (convertToType.IsGenericType &&
                convertToType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                convertToType = Nullable.GetUnderlyingType(convertToType);
            }

            // deal with conversion to enum types when input is a string
            if (convertToType.IsEnum && value is string)
            {
                return Enum.Parse(convertToType, value as string);
            }

            // deal with conversion to enum types when input is a integral primitive
            if (value != null && convertToType.IsEnum && value.GetType().IsPrimitive &&
                !(value is bool) && !(value is char) &&
                !(value is float) && !(value is double))
            {
                return Enum.ToObject(convertToType, value);
            }

            // use Convert.ChangeType() to do all other conversions
            return Convert.ChangeType(value, convertToType, CultureInfo.InvariantCulture);
        }

        public static Task<M> ConvertMapper<T, M>(this Task<T> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var tcs = new TaskCompletionSource<M>();

            task.ContinueWith(t => tcs.TrySetCanceled(), TaskContinuationOptions.OnlyOnCanceled);
            task.ContinueWith(t =>
            {
                tcs.TrySetResult(Mapper.Map<T, M>(t.Result));
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(t => tcs.TrySetException(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

            return tcs.Task;
        }


        public static Task<ICollection<M>> ConvertEachMapper<T, M>(this Task<ICollection<T>> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var tcs = new TaskCompletionSource<ICollection<M>>();

            task.ContinueWith(t => tcs.TrySetCanceled(), TaskContinuationOptions.OnlyOnCanceled);
            task.ContinueWith(t =>
            {
                tcs.TrySetResult(t.Result.Select(Mapper.Map<T, M>).ToList());
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(t => tcs.TrySetException(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            return tcs.Task;
        }
    }
}
