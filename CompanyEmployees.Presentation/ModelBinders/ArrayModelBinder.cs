using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CompanyEmployees.Presentation.ModelBinders;

public class ArrayModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (!IsEnumerableType(bindingContext.ModelMetadata))
        {
            SetFailedBindingResult(bindingContext);

            return Task.CompletedTask;
        }

        var providedValue = GetProvidedValue(bindingContext);

        if (string.IsNullOrEmpty(providedValue))
        {
            SetEmptyArrayBindingResult(bindingContext);
            return Task.CompletedTask;
        }

        var genericType = GetGenericType(bindingContext.ModelType);

        var objectArray = ConvertProvidedValueToArray(providedValue, genericType);

        var finalArray = CreateArrayOfType(genericType, objectArray.Length);
        CopyObjectsToArray(objectArray, finalArray);

        SetModelAndBindingResultSuccess(bindingContext, finalArray);

        return Task.CompletedTask;
    }

    // Проверяет, является ли тип модели перечислимым типом
    private static bool IsEnumerableType(ModelMetadata modelMetadata)
    {
        return modelMetadata.IsEnumerableType;
    }

    // Устанавливает результат привязки модели как неудачный
    private static void SetFailedBindingResult(ModelBindingContext bindingContext)
    {
        bindingContext.Result = ModelBindingResult.Failed();
    }

    // Получает предоставленное значение из ValueProvider (то есть просто значение параметра)
    private static string GetProvidedValue(ModelBindingContext bindingContext)
    {
        return bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();
    }

    // Устанавливает результат привязки модели как успешный с пустым массивом
    private static void SetEmptyArrayBindingResult(ModelBindingContext bindingContext)
    {
        bindingContext.Result = ModelBindingResult.Success(null);
    }

    // Получает Generic тип элементов массива
    private static Type GetGenericType(Type modelType)
    {
        return modelType.GetTypeInfo().GenericTypeArguments[0];
    }

    // Разделяет предоставленное значение на отдельные элементы, преобразует их в объекты заданного типа и создает массив
    private static object?[] ConvertProvidedValueToArray(string providedValue, Type genericType)
    {
        var converter = TypeDescriptor.GetConverter(genericType);

        return providedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => converter.ConvertFromString(x.Trim()))
            .ToArray();
    }

    // Создает массив заданного типа с заданной длиной
    private static Array CreateArrayOfType(Type genericType, int length)
    {
        return Array.CreateInstance(genericType, length);
    }

    // Копирует объекты из одного массива в другой
    private static void CopyObjectsToArray(object?[] sourceArray, Array destinationArray)
    {
        sourceArray.CopyTo(destinationArray, 0);
    }

    // Устанавливает привязанную модель и результат привязки модели в контексте
    private static void SetModelAndBindingResultSuccess(ModelBindingContext bindingContext, Array model)
    {
        bindingContext.Model = model;
        bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
    }
}