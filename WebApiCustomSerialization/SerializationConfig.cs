using System.Web.Http;

namespace WebApiCustomSerialization
{
	/// <summary>
	/// Конфигурирование сесриализации.
	/// </summary>
	public class SerializationConfig
	{
		/// <summary>
		/// Регистрируем настройки сериализации.
		/// </summary>
		public static void Register(HttpConfiguration config)
		{
			// Создаем сериализатор с нужными для нас правилами сериализации.

			var customResolver = new CustomContractResolver();

			// Сериализуем как объект и игнорируем свойство Hidden.
			// customResolver.JsonObject<SomeClass>(c => c.Ignore(x => x.Hidden));
			// Сериализуем как объект и сериализуем закрытое поле values
			// customResolver.JsonObject<SomeClass2>(c => c.JsonProperty("values"));
			// Сериализуем как объект, игнорируем свойство Hidden и сериализуем закрытое поле list
			// customResolver.JsonObject<SomeClass3>(c => c.Ignore(x => x.Hidden).JsonProperty("list"));

			// Настраиваем сериализацию WebApi.

			config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = customResolver;
			config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
		}
	}
}