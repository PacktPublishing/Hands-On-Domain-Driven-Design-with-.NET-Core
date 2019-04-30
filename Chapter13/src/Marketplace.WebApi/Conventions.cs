using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Marketplace.WebApi
{
    /// <summary>
    /// The code for this convention is based on the original work
    /// of Tugberk Ugurlu
    /// https://gist.github.com/tugberkugurlu/4bcb7af3682771ba9c18828329f04920
    /// </summary>
    public class CommandConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            foreach (var parameter in action.Parameters)
            {
                var paramType = parameter.ParameterInfo.ParameterType;

                if (parameter.BindingInfo != null ||
                    IsSimpleType(paramType) ||
                    IsSimpleUnderlyingType(paramType))
                    continue;

                parameter.BindingInfo = new BindingInfo
                {
                    BindingSource = BindingSource.Body
                };
            }

            static bool IsSimpleType(Type type)
                => type.IsPrimitive ||
                   type == typeof(string) ||
                   type == typeof(DateTime) ||
                   type == typeof(decimal) ||
                   type == typeof(Guid) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan);

            static bool IsSimpleUnderlyingType(Type type)
            {
                var underlyingType = Nullable.GetUnderlyingType(type);

                if (underlyingType != null)
                    type = underlyingType;

                return IsSimpleType(type);
            }
        }
    }
}