using System.Collections.Generic;

namespace WebApiCustomSerialization
{
	/// <summary>
	/// Конфигурация для CustomContractResolver, содержащяя некоторый набор правил для сериализации.
	/// </summary>
	public class CustomContractResolverConfig
	{
		/// <summary>
		/// Список свойств и полей для игнорирования сериализации, аналогично аттрибуту JsonIgnore.
		/// </summary>
		public readonly List<string> JsonIgnores;

		/// <summary>
		/// Список закрытых свойств и полей для сериализации, аналогично аттрибуту JsonProperty.
		/// </summary>
		public readonly List<string> JsonProperties;

		/// <summary>
		/// Создает класс конфигурации.
		/// </summary>
		public CustomContractResolverConfig()
		{
			JsonIgnores = new List<string>();
			JsonProperties = new List<string>();
		}
	}
}