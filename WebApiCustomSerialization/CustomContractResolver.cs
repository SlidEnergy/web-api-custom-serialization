﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebApiCustomSerialization
{
	/// <summary>
	/// Разрешает JsonContract для переданных типов, по определенным правилам.
	/// </summary>
	public class CustomContractResolver : CamelCasePropertyNamesContractResolver
	{
		/// <summary>
		/// Словарь типов, которые должны быть сериализованы как объект, с набором правил для этих типов.
		/// </summary>
		private readonly Dictionary<Type, CustomContractResolverConfig> _jsonObjectRules = new Dictionary<Type, CustomContractResolverConfig>();

		/// <summary>
		/// Словарь закэшированных контрактов.
		/// </summary>
		private readonly Dictionary<Type, JsonContract> cachedContracts = new Dictionary<Type, JsonContract>();

		/// <summary>
		/// Помечает тип для сериализации как объект, аналогично атрибуту JsonObject. 
		/// Позволяет задать дополнительные правила для объекта.
		/// </summary>
		public void JsonObject<T>(Func<CustomContractResolverConfig<T>, CustomContractResolverConfig<T>> func)
		{
			var config = func.Invoke(new CustomContractResolverConfig<T>());

			if (!_jsonObjectRules.ContainsKey(typeof(T)))
				_jsonObjectRules[typeof(T)] = config;
		}

		/// <summary>
		/// Помечает тип для сериализации как объект, аналогично атрибуту JsonObject. 
		/// </summary>
		public void JsonObject<T>()
		{
			if (!_jsonObjectRules.ContainsKey(typeof(T)))
				_jsonObjectRules[typeof(T)] = new CustomContractResolverConfig<T>();
		}

		/// <summary>
		/// Разрешает JsonContract для переданного типа
		/// </summary>
		public override JsonContract ResolveContract(Type type)
		{
			if (type == (Type)null)
				throw new ArgumentNullException(nameof(type));

			if (_jsonObjectRules.ContainsKey(type))
			{
				if (cachedContracts.ContainsKey(type))
					return cachedContracts[type];

				var contract = this.CreateObjectContract(type);

				cachedContracts.Add(type, contract);
				return contract;
			}

			return base.ResolveContract(type);
		}

		/// <summary>
		/// Создает набор свойств для сериализации для переданного типа
		/// </summary>
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			var list = base.CreateProperties(type, memberSerialization);

			if (_jsonObjectRules.ContainsKey(type))
			{
				foreach (string propertyName in _jsonObjectRules[type].JsonProperties)
				{
					MemberInfo field = type.GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);

					var property = this.CreateProperty(field, memberSerialization);
					property.Readable = true;

					// Если свойство с таким же именем уже есть, оно будет конфликтовать с новым.
					if (list.Any(x => x.PropertyName == property.PropertyName))
						throw new Exception($"В классе {type.Name} или его родителе уже содержится свойство с таким же наименованием, как мы пытаемся добавить.");

					list.Add(property);
				}
			}

			return list;
		}

		/// <summary>
		/// Создает свойство для переданного члена типа.
		/// </summary>
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);

			// Удаляем свойства из коллекции, если нам нужно его игнорировать.
			if (_jsonObjectRules.ContainsKey(member.DeclaringType))
			{
				foreach (string propertyName in _jsonObjectRules[member.DeclaringType].JsonIgnores)
				{
					if (propertyName.ToLower() == property.PropertyName.ToLower())
					{
						// Есть несколько способов:
						// - выставить флаг Ignore = true
						// - задать делегат ShouldSerialize = i => false
						// - не возвращать свойство, тогда оно не будет добавлено в коллекцию для сериализации.
						// Возвращаем null, т.к. мы можем добавить другое свойство, которое при преобразовании 
						// в camel case будет иметь такое-же наименование, и оно будет конфликтовать со старым.
						return null;
					}
				}
			}

			return property;
		}
	}
}