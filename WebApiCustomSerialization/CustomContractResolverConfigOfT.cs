using System;
using System.Linq.Expressions;

namespace WebApiCustomSerialization
{
	/// <summary>
	/// Конфигурация для CustomContractResolver, содержащяя некоторый набор правил для сериализации.
	/// </summary>
	public class CustomContractResolverConfig<T> : CustomContractResolverConfig
	{
		/// <summary>
		/// Добавляет правило игнорирования для переданного поля или свойства указанного типа, аналогично атрибуту JsonIgnore.
		/// </summary>
		public CustomContractResolverConfig<T> Ignore(Expression<Func<T, object>> expression)
		{
			var propertyName = ((MemberExpression)expression.Body).Member.Name;

			return Ignore(propertyName);
		}

		/// <summary>
		/// Добавляет правило игнорирования для переданного поля или свойства указанного типа, аналогично атрибуту JsonIgnore.
		/// </summary>
		public CustomContractResolverConfig<T> Ignore(string propertyName)
		{
			if (!JsonIgnores.Contains(propertyName))
				JsonIgnores.Add(propertyName);

			return this;
		}

		/// <summary>
		/// Добавляет правило сериализации для переданного поля или свойства указанного типа, аналогично атрибуту JsonProperty.
		/// </summary>
		public CustomContractResolverConfig<T> JsonProperty(string propertyName)
		{
			if (!JsonProperties.Contains(propertyName))
				JsonProperties.Add(propertyName);

			return this;
		}
	}
}