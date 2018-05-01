using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

//JMMB
namespace Banistmo.Sax.Common
{
    public static class Extension
    {

        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

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

        public static ExpandoObject Init(this ExpandoObject expando, dynamic obj)
        {
            var dict = (IDictionary<string, object>)expando;
            foreach (PropertyInfo propInfo in obj.GetType().GetProperties())
            {
                dict[propInfo.Name] = propInfo.GetValue(obj, null);
            }
            return expando;
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
            {
                return value.Substring(0, maxLength);
            }

            return value;
        }


        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
                expando.Add(property.Name, property.GetValue(value));
            return expando as ExpandoObject;
        }

        public static DataTable ToDataTable<T>(this List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);

            }
            return dataTable;
        }

        public static DataTable AnonymousToDataTable(this IList source)
        {
            if (source == null) throw new ArgumentNullException();
            var table = new DataTable();
            if (source.Count == 0) return table;
            Type itemType = source[0].GetType();
            table.TableName = "anonymoustable";
            List<string> names = new List<string>();
            foreach (var prop in itemType.GetProperties())
            {
                if (prop.CanRead && prop.GetIndexParameters().Length == 0)
                {
                    names.Add(prop.Name);
                    table.Columns.Add(prop.Name, prop.PropertyType);
                }
            }
            names.TrimExcess();
            PropertyInfo[] Props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var row in source)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(row, null);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public static List<TDestination> CustomMapIgnoreICollection<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            List<TDestination> listDestination = new List<TDestination>();
            foreach (TSource itemSource in source)
            {
                TDestination destination = CustomMapIgnoreICollection<TSource, TDestination>(itemSource);
                listDestination.Add(destination);
            }
            return listDestination;
        }

        /// <summary>
        /// Evalua que las propiedades a mapear no sean colecciones y mapea los valores entre la clase origen y destino
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns>Retorna la clase destino con los valores asignados.</returns>
        public static TDestination CustomMapIgnoreICollection<TSource, TDestination>(this TSource source)
        {
            Type typeICollection = typeof(ICollection<>);
            TDestination destination = Activator.CreateInstance<TDestination>();
            PropertyInfo[] destinationProperties = destination.GetType().GetProperties();
            source.GetType().GetProperties().ToList().ForEach(sourceProperty =>
            {
                bool notTypeICollection = true;
                Type sourcePropertyType = sourceProperty.PropertyType;
                if (sourcePropertyType.IsGenericType)
                {
                    notTypeICollection = !(sourcePropertyType.GetGenericTypeDefinition() == typeICollection);
                }
                if (notTypeICollection)
                {
                    PropertyInfo property = destinationProperties.FirstOrDefault(c => c.Name.ToUpper().Equals(sourceProperty.Name.ToUpper()));
                    if (property != null && sourcePropertyType.IsAssignableFrom(property.PropertyType))
                    {
                        object obj = sourceProperty.GetValue(source);
                        property.SetValue(destination, obj);
                    }
                }
            });
            return destination;
        }


        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }
            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            return null;
        }



    }
}
