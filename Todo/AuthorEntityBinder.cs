using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo
{
    public class AuthorEntityBinder : IModelBinder
    {
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException(nameof(bindingContext));
			}

			var modelName = bindingContext.ModelName;

			// Try to fetch the value of the argument by name
			var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

			if (valueProviderResult == ValueProviderResult.None)
			{
				return Task.CompletedTask;
			}

			bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

			var value = valueProviderResult.FirstValue;

			// Check if the argument value is null or empty
			if (string.IsNullOrEmpty(value))
			{
				return Task.CompletedTask;
			}

			// 以下是資料轉換重點，將取得的JSON字串轉成對應的類別型態
			try
			{
				//JSON用以下的Function
				object model = JsonConvert.DeserializeObject(value, bindingContext.ModelType);
				bindingContext.Result = ModelBindingResult.Success(model);
			}
			catch
			{
				bindingContext.Result = ModelBindingResult.Failed();
			}

			return Task.CompletedTask;
		}
	}
}
