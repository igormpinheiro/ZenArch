using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedKernel.Helpers;

/// <summary>
/// Utilitário para operações JSON com configurações otimizadas e seguras
/// </summary>
public static class JsonHelper
{
    // public JsonSerializerOptions DefaultOptions { get; set; } = new();
    public static readonly JsonSerializerOptions DefaultOptions = CreateDefaultOptions();
    private static readonly JsonSerializerOptions _prettyOptions = CreatePrettyOptions();

    /// <summary>
    /// Serializa um objeto para JSON usando as configurações padrão
    /// </summary>
    /// <typeparam name="T">Tipo do objeto</typeparam>
    /// <param name="value">Objeto a ser serializado</param>
    /// <returns>String JSON ou null se o valor for null</returns>
    public static string? Serialize<T>(T? value)
    {
        return value is null ? null : JsonSerializer.Serialize(value, DefaultOptions);
    }

    /// <summary>
    /// Serializa um objeto para JSON com formatação legível (indentado)
    /// </summary>
    /// <typeparam name="T">Tipo do objeto</typeparam>
    /// <param name="value">Objeto a ser serializado</param>
    /// <returns>String JSON formatada ou null se o valor for null</returns>
    public static string? SerializePretty<T>(T? value)
    {
        return value is null ? null : JsonSerializer.Serialize(value, _prettyOptions);
    }

    /// <summary>
    /// Desserializa uma string JSON para o tipo especificado
    /// </summary>
    /// <typeparam name="T">Tipo de destino</typeparam>
    /// <param name="json">String JSON</param>
    /// <returns>Objeto desserializado ou default(T) se json for null/empty</returns>
    /// <exception cref="JsonException">Quando o JSON é inválido</exception>
    /// <exception cref="NotSupportedException">Quando o tipo não é suportado</exception>
    public static T? Deserialize<T>(string? json)
    {
        return string.IsNullOrWhiteSpace(json)
            ? default
            : JsonSerializer.Deserialize<T>(json, DefaultOptions);
    }

    /// <summary>
    /// Tenta desserializar uma string JSON de forma segura
    /// </summary>
    /// <typeparam name="T">Tipo de destino</typeparam>
    /// <param name="json">String JSON</param>
    /// <param name="result">Objeto desserializado (out parameter)</param>
    /// <returns>True se a desserialização foi bem-sucedida, false caso contrário</returns>
    public static bool TryDeserialize<T>(string? json, out T? result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            result = JsonSerializer.Deserialize<T>(json, DefaultOptions);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    /// <summary>
    /// Valida se uma string é um JSON válido
    /// </summary>
    /// <param name="json">String a ser validada</param>
    /// <returns>True se for um JSON válido, false caso contrário</returns>
    public static bool IsValidJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Formata uma string JSON (adiciona indentação)
    /// </summary>
    /// <param name="json">String JSON a ser formatada</param>
    /// <returns>String JSON formatada ou a string original se inválida</returns>
    public static string FormatJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return string.Empty;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(document, _prettyOptions);
        }
        catch (JsonException)
        {
            return json;
        }
    }

    /// <summary>
    /// Minifica uma string JSON (remove espaços e quebras de linha desnecessárias)
    /// </summary>
    /// <param name="json">String JSON a ser minificada</param>
    /// <returns>String JSON minificada ou a string original se inválida</returns>
    public static string MinifyJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return string.Empty;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(document, DefaultOptions);
        }
        catch (JsonException)
        {
            return json;
        }
    }

    /// <summary>
    /// Clona um objeto através de serialização/desserialização JSON
    /// </summary>
    /// <typeparam name="T">Tipo do objeto</typeparam>
    /// <param name="source">Objeto a ser clonado</param>
    /// <returns>Clone do objeto ou default(T) se source for null</returns>
    public static T? DeepClone<T>(T? source)
    {
        if (source is null)
        {
            return default;
        }

        var json = JsonSerializer.Serialize(source, DefaultOptions);
        return JsonSerializer.Deserialize<T>(json, DefaultOptions);
    }

    /// <summary>
    /// Obtém um valor aninhado de um JSON usando notação de ponto
    /// </summary>
    /// <param name="json">String JSON</param>
    /// <param name="path">Caminho usando notação de ponto (ex: "user.address.city")</param>
    /// <returns>Valor encontrado ou null se não encontrado</returns>
    public static string? GetNestedValue(string? json, string path)
    {
        if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            var current = document.RootElement;

            var pathParts = path.Split('.');

            foreach (var part in pathParts)
            {
                if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(part, out var property))
                {
                    current = property;
                }
                else if (current.ValueKind == JsonValueKind.Array && int.TryParse(part, out var index))
                {
                    if (index >= 0 && index < current.GetArrayLength())
                    {
                        current = current[index];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            return current.ValueKind switch
            {
                JsonValueKind.String => current.GetString(),
                JsonValueKind.Number => current.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => null,
                _ => current.GetRawText()
            };
        }
        catch (JsonException)
        {
            return null;
        }
    }

    #region Configurações privadas

    private static JsonSerializerOptions CreateDefaultOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new DateTimeConverter(),
                new DateTimeOffsetConverter()
            }
        };
    }

    private static JsonSerializerOptions CreatePrettyOptions()
    {
        var options = CreateDefaultOptions();
        options.WriteIndented = true;
        return options;
    }

    #endregion

    #region Converters customizados

    private sealed class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TryGetDateTime(out var dateTime))
            {
                return dateTime;
            }

            var value = reader.GetString();
            return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result)
                ? result
                : throw new JsonException($"Unable to convert \"{value}\" to DateTime.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("O", CultureInfo.InvariantCulture));
        }
    }

    private sealed class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TryGetDateTimeOffset(out var dateTimeOffset))
            {
                return dateTimeOffset;
            }

            var value = reader.GetString();
            return DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind,
                out var result)
                ? result
                : throw new JsonException($"Unable to convert \"{value}\" to DateTimeOffset.");
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("O", CultureInfo.InvariantCulture));
        }
    }

    #endregion
}
