using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace TicTacToeCSharpPlayground.Api.Configs
{
    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            var valueAsString = value?.ToString();
            if (valueAsString is not null)
            {
                var cultureInvariant = RegexOptions.CultureInvariant;
                var matchTimeout = TimeSpan.FromMilliseconds(100);
                var pattern = "([a-z])([A-Z])";
                var replacement = "$1-$2";

                var result = Regex.Replace(valueAsString, pattern, replacement, cultureInvariant, matchTimeout);

                return result.ToLowerInvariant();
            }

            return null;
        }
    }
}
