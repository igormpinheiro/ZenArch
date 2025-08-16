using System.Text.Json;
using SharedKernel.Helpers;
using Shouldly;

namespace UnitTests.SharedKernel.Helpers;

public class JsonHelperTests
{
    private readonly TestObject _testObject = new()
    {
        Id = 1,
        Name = "Test Name",
        Email = "test@example.com",
        CreatedAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
        Status = TestStatus.Active,
        Tags = ["tag1", "tag2"],
        Metadata = new Dictionary<string, object>
        {
            { "key1", "value1" },
            { "key2", 42 }
        }
    };

    #region Serialize Tests

    [Fact]
    public void Serialize_WithValidObject_ShouldReturnJsonString()
    {
        // Act
        var result = JsonHelper.Serialize(_testObject);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldContain("\"id\":1");
        result.ShouldContain("\"name\":\"Test Name\"");
        result.ShouldNotContain("\n"); // Não deve estar indentado
    }

    [Fact]
    public void Serialize_WithNullObject_ShouldReturnNull()
    {
        // Act
        var result = JsonHelper.Serialize<TestObject>(null);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void SerializePretty_WithValidObject_ShouldReturnIndentedJson()
    {
        // Act
        var result = JsonHelper.SerializePretty(_testObject);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldContain("  \"id\": 1"); // Deve estar indentado
        result.ShouldContain("\n");
    }

    [Fact]
    public void Serialize_ShouldUseCamelCaseByDefault()
    {
        // Act
        var result = JsonHelper.Serialize(_testObject);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldContain("\"id\":");
        result.ShouldContain("\"name\":");
        result.ShouldContain("\"createdAt\":");
        result.ShouldContain("\"status\":\"active\""); // Enum em camelCase
    }

    #endregion

    #region Deserialize Tests

    [Fact]
    public void Deserialize_WithValidJson_ShouldReturnObject()
    {
        // Arrange
        var json = JsonHelper.Serialize(_testObject);

        // Act
        var result = JsonHelper.Deserialize<TestObject>(json);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(_testObject.Id);
        result.Name.ShouldBe(_testObject.Name);
        result.Email.ShouldBe(_testObject.Email);
        result.Status.ShouldBe(_testObject.Status);
    }

    [Fact]
    public void Deserialize_WithNullOrEmptyJson_ShouldReturnDefault()
    {
        // Act & Assert
        JsonHelper.Deserialize<TestObject>(null).ShouldBeNull();
        JsonHelper.Deserialize<TestObject>("").ShouldBeNull();
        JsonHelper.Deserialize<TestObject>("   ").ShouldBeNull();
    }

    [Fact]
    public void Deserialize_WithInvalidJson_ShouldThrowJsonException()
    {
        // Arrange
        var invalidJson = "{ invalid json }";

        // Act & Assert
        Should.Throw<JsonException>(() => JsonHelper.Deserialize<TestObject>(invalidJson));
    }

    #endregion

    #region TryDeserialize Tests

    [Fact]
    public void TryDeserialize_WithValidJson_ShouldReturnTrueAndObject()
    {
        // Arrange
        var json = JsonHelper.Serialize(_testObject);

        // Act
        var success = JsonHelper.TryDeserialize<TestObject>(json, out var result);

        // Assert
        success.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(_testObject.Id);
        result.Name.ShouldBe(_testObject.Name);
    }

    [Fact]
    public void TryDeserialize_WithInvalidJson_ShouldReturnFalseAndDefault()
    {
        // Arrange
        var invalidJson = "{ invalid json }";

        // Act
        var success = JsonHelper.TryDeserialize<TestObject>(invalidJson, out var result);

        // Assert
        success.ShouldBeFalse();
        result.ShouldBeNull();
    }

    [Fact]
    public void TryDeserialize_WithNullOrEmptyJson_ShouldReturnFalseAndDefault()
    {
        // Act & Assert
        JsonHelper.TryDeserialize<TestObject>(null, out var result1).ShouldBeFalse();
        result1.ShouldBeNull();

        JsonHelper.TryDeserialize<TestObject>("", out var result2).ShouldBeFalse();
        result2.ShouldBeNull();

        JsonHelper.TryDeserialize<TestObject>("   ", out var result3).ShouldBeFalse();
        result3.ShouldBeNull();
    }

    #endregion

    #region Validation Tests

    [Theory]
    [InlineData("{\"name\":\"test\"}", true)]
    [InlineData("[1,2,3]", true)]
    [InlineData("\"simple string\"", true)]
    [InlineData("123", true)]
    [InlineData("true", true)]
    [InlineData("null", true)]
    [InlineData("{invalid json}", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("   ", false)]
    public void IsValidJson_ShouldReturnExpectedResult(string? json, bool expected)
    {
        // Act
        var result = JsonHelper.IsValidJson(json);

        // Assert
        result.ShouldBe(expected);
    }

    #endregion

    #region Format and Minify Tests

    [Fact]
    public void FormatJson_WithValidJson_ShouldReturnFormattedJson()
    {
        // Arrange
        var compactJson = "{\"name\":\"test\",\"value\":123}";

        // Act
        var result = JsonHelper.FormatJson(compactJson);

        // Assert
        result.ShouldContain("  \"name\": \"test\"");
        result.ShouldContain("\n");
    }

    [Fact]
    public void FormatJson_WithInvalidJson_ShouldReturnOriginalString()
    {
        // Arrange
        var invalidJson = "{ invalid }";

        // Act
        var result = JsonHelper.FormatJson(invalidJson);

        // Assert
        result.ShouldBe(invalidJson);
    }

    [Fact]
    public void FormatJson_WithNullOrEmpty_ShouldReturnEmptyString()
    {
        // Act & Assert
        JsonHelper.FormatJson(null).ShouldBe(string.Empty);
        JsonHelper.FormatJson("").ShouldBe(string.Empty);
        JsonHelper.FormatJson("   ").ShouldBe(string.Empty);
    }

    [Fact]
    public void MinifyJson_WithFormattedJson_ShouldReturnCompactJson()
    {
        // Arrange
        var formattedJson = "{\n  \"name\": \"test\",\n  \"value\": 123\n}";

        // Act
        var result = JsonHelper.MinifyJson(formattedJson);

        // Assert
        result.ShouldNotContain("\n");
        result.ShouldNotContain("  ");
        result.ShouldContain("\"name\":\"test\"");
    }

    [Fact]
    public void MinifyJson_WithInvalidJson_ShouldReturnOriginalString()
    {
        // Arrange
        var invalidJson = "{ invalid }";

        // Act
        var result = JsonHelper.MinifyJson(invalidJson);

        // Assert
        result.ShouldBe(invalidJson);
    }

    #endregion

    #region DeepClone Tests

    [Fact]
    public void DeepClone_WithValidObject_ShouldReturnClonedObject()
    {
        // Act
        var cloned = JsonHelper.DeepClone(_testObject);

        // Assert
        cloned.ShouldNotBeNull();
        cloned.ShouldNotBeSameAs(_testObject); // Diferentes instâncias
        cloned.Id.ShouldBe(_testObject.Id);
        cloned.Name.ShouldBe(_testObject.Name);
        cloned.Email.ShouldBe(_testObject.Email);
        cloned.Status.ShouldBe(_testObject.Status);
        cloned.CreatedAt.ShouldBe(_testObject.CreatedAt);

        // Verificar que arrays são clonados
        cloned.Tags.ShouldNotBeSameAs(_testObject.Tags);
        cloned.Tags.ShouldBe(_testObject.Tags);
    }

    [Fact]
    public void DeepClone_WithNullObject_ShouldReturnDefault()
    {
        // Act
        var result = JsonHelper.DeepClone<TestObject>(null);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void DeepClone_ModifyingClone_ShouldNotAffectOriginal()
    {
        // Arrange
        var original = new TestObject { Id = 1, Name = "Original" };

        // Act
        var cloned = JsonHelper.DeepClone(original);
        cloned!.Name = "Modified";

        // Assert
        original.Name.ShouldBe("Original");
        cloned.Name.ShouldBe("Modified");
    }

    #endregion

    #region GetNestedValue Tests

    [Fact]
    public void GetNestedValue_WithValidPath_ShouldReturnValue()
    {
        // Arrange
        var json = """
                   {
                       "user": {
                           "profile": {
                               "name": "John Doe",
                               "age": 30
                           },
                           "addresses": [
                               {"city": "New York"},
                               {"city": "Los Angeles"}
                           ]
                       }
                   }
                   """;

        // Act & Assert
        JsonHelper.GetNestedValue(json, "user.profile.name").ShouldBe("John Doe");
        JsonHelper.GetNestedValue(json, "user.profile.age").ShouldBe("30");
        JsonHelper.GetNestedValue(json, "user.addresses.0.city").ShouldBe("New York");
        JsonHelper.GetNestedValue(json, "user.addresses.1.city").ShouldBe("Los Angeles");
    }

    [Fact]
    public void GetNestedValue_WithInvalidPath_ShouldReturnNull()
    {
        // Arrange
        var json = """{"user": {"name": "John"}}""";

        // Act & Assert
        JsonHelper.GetNestedValue(json, "user.invalid").ShouldBeNull();
        JsonHelper.GetNestedValue(json, "invalid.path").ShouldBeNull();
        JsonHelper.GetNestedValue(json, "user.name.invalid").ShouldBeNull();
    }

    [Theory]
    [InlineData(null, "user.name")]
    [InlineData("", "user.name")]
    [InlineData("   ", "user.name")]
    [InlineData("{\"user\":\"John\"}", null)]
    [InlineData("{\"user\":\"John\"}", "")]
    [InlineData("{\"user\":\"John\"}", "   ")]
    public void GetNestedValue_WithInvalidInput_ShouldReturnNull(string? json, string? path)
    {
        // Act
        var result = JsonHelper.GetNestedValue(json, path!);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void GetNestedValue_WithArrayIndexOutOfBounds_ShouldReturnNull()
    {
        // Arrange
        var json = """{"items": [1, 2, 3]}""";

        // Act & Assert
        JsonHelper.GetNestedValue(json, "items.5").ShouldBeNull();
        JsonHelper.GetNestedValue(json, "items.-1").ShouldBeNull();
    }

    #endregion

    #region DateTime Handling Tests

    [Fact]
    public void Serialize_WithDateTime_ShouldUseIso8601Format()
    {
        // Arrange
        var obj = new { CreatedAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc) };

        // Act
        var json = JsonHelper.Serialize(obj);

        // Assert
        json.ShouldContain("\"2024-01-01T12:00:00.0000000Z\"");
    }

    [Fact]
    public void Deserialize_WithIso8601DateTime_ShouldReturnCorrectDateTime()
    {
        // Arrange
        var json = """{"createdAt": "2024-01-01T12:00:00.0000000Z"}""";

        // Act
        var result = JsonHelper.Deserialize<DateTimeTestObject>(json);

        // Assert
        result.ShouldNotBeNull();
        result.CreatedAt.ShouldBe(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));
    }

    #endregion

    #region Enum Handling Tests

    [Fact]
    public void Serialize_WithEnum_ShouldUseCamelCaseString()
    {
        // Arrange
        var obj = new { Status = TestStatus.Active };

        // Act
        var json = JsonHelper.Serialize(obj);

        // Assert
        json.ShouldContain("\"status\":\"active\"");
    }

    [Fact]
    public void Deserialize_WithEnumString_ShouldReturnCorrectEnum()
    {
        // Arrange
        var json = """{"status": "active"}""";

        // Act
        var result = JsonHelper.Deserialize<EnumTestObject>(json);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(TestStatus.Active);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void Serialize_WithCircularReference_ShouldThrowJsonException()
    {
        // Arrange
        var obj1 = new CircularTestObject { Name = "Object1" };
        var obj2 = new CircularTestObject { Name = "Object2" };
        obj1.Reference = obj2;
        obj2.Reference = obj1; // Referência circular

        // Act & Assert
        Should.Throw<JsonException>(() => JsonHelper.Serialize(obj1));
    }

    [Fact]
    public void TryDeserialize_WithUnsupportedType_ShouldReturnFalse()
    {
        // Arrange
        var json = """{"value": "test"}""";

        // Act
        var success = JsonHelper.TryDeserialize<System.IO.FileStream>(json, out var result);

        // Assert
        success.ShouldBeFalse();
        result.ShouldBeNull();
    }

    #endregion

    #region Performance Tests

    [Fact]
    public void Serialize_WithLargeObject_ShouldComplete()
    {
        // Arrange
        var largeObject = new
        {
            Items = Enumerable.Range(0, 1000).Select(i => new
            {
                Id = i,
                Name = $"Item {i}",
                Description = new string('x', 100),
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            }).ToArray()
        };

        // Act
        var json = JsonHelper.Serialize(largeObject);

        // Assert
        json.ShouldNotBeNull();
        json.Length.ShouldBeGreaterThan(100000); // Deve ser uma string bem grande
    }

    [Fact]
    public void DeepClone_WithComplexObject_ShouldMaintainAllProperties()
    {
        // Arrange
        var complex = new ComplexTestObject
        {
            SimpleProperty = "test",
            NestedObject = new TestObject
            {
                Id = 1,
                Name = "Nested",
                Tags = new[] { "tag1", "tag2" }
            },
            ListProperty = new List<string> { "item1", "item2" },
            DictionaryProperty = new Dictionary<string, int>
            {
                { "key1", 1 },
                { "key2", 2 }
            }
        };

        // Act
        var cloned = JsonHelper.DeepClone(complex);

        // Assert
        cloned.ShouldNotBeNull();
        cloned.ShouldNotBeSameAs(complex);
        cloned.SimpleProperty.ShouldBe(complex.SimpleProperty);
        cloned.NestedObject.ShouldNotBeSameAs(complex.NestedObject);
        cloned.NestedObject.Name.ShouldBe(complex.NestedObject.Name);
        cloned.ListProperty.ShouldNotBeSameAs(complex.ListProperty);
        cloned.ListProperty.ShouldBe(complex.ListProperty);
        cloned.DictionaryProperty.ShouldNotBeSameAs(complex.DictionaryProperty);
        cloned.DictionaryProperty.ShouldBe(complex.DictionaryProperty);
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void GetNestedValue_WithSpecialCharacters_ShouldWork()
    {
        // Arrange
        var json = """
                   {
                       "user-info": {
                           "first_name": "John",
                           "special@key": "special_value"
                       }
                   }
                   """;

        // Act & Assert
        JsonHelper.GetNestedValue(json, "user-info.first_name").ShouldBe("John");
        JsonHelper.GetNestedValue(json, "user-info.special@key").ShouldBe("special_value");
    }

    [Fact]
    public void Serialize_WithNullableTypes_ShouldHandleCorrectly()
    {
        // Arrange
        var obj = new NullableTestObject
        {
            NullableInt = null,
            NullableDateTime = DateTime.UtcNow,
            NullableEnum = null,
            NullableString = "not null"
        };

        // Act
        var json = JsonHelper.Serialize(obj);
        var deserialized = JsonHelper.Deserialize<NullableTestObject>(json);

        // Assert
        json.ShouldNotBeNull();
        deserialized.ShouldNotBeNull();
        deserialized.NullableInt.ShouldBeNull();
        deserialized.NullableDateTime.ShouldNotBeNull();
        deserialized.NullableEnum.ShouldBeNull();
        deserialized.NullableString.ShouldBe("not null");
    }

    [Theory]
    [InlineData("{ \"value\": true }", "value", "true")]
    [InlineData("{ \"value\": false }", "value", "false")]
    [InlineData("{ \"value\": null }", "value", null)]
    [InlineData("{ \"number\": 42.5 }", "number", "42.5")]
    [InlineData("{ \"array\": [1,2,3] }", "array", "[1,2,3]")]
    public void GetNestedValue_WithDifferentValueTypes_ShouldReturnCorrectString(
        string json, string path, string? expected)
    {
        // Act
        var result = JsonHelper.GetNestedValue(json, path);

        // Assert
        result.ShouldBe(expected);
    }

    #endregion
}

#region Test Classes

public class TestObject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public TestStatus Status { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class DateTimeTestObject
{
    public DateTime CreatedAt { get; set; }
}

public class EnumTestObject
{
    public TestStatus Status { get; set; }
}

public class CircularTestObject
{
    public string Name { get; set; } = string.Empty;
    public CircularTestObject? Reference { get; set; }
}

public class ComplexTestObject
{
    public string SimpleProperty { get; set; } = string.Empty;
    public TestObject NestedObject { get; set; } = new();
    public List<string> ListProperty { get; set; } = new();
    public Dictionary<string, int> DictionaryProperty { get; set; } = new();
}

public class NullableTestObject
{
    public int? NullableInt { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public TestStatus? NullableEnum { get; set; }
    public string? NullableString { get; set; }
}

public enum TestStatus
{
    Inactive = 0,
    Active = 1,
    Pending = 2,
    Suspended = 3
}

#endregion
