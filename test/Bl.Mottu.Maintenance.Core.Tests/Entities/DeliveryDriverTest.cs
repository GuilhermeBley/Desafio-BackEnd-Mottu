using Bl.Mottu.Maintenance.Core.Entities;
using Bl.Mottu.Maintenance.Core.Primitive;
using Bl.Mottu.Maintenance.Core.Tests.Mock;
using Bl.Mottu.Maintenance.Core.ValueObjects;
using System.Numerics;

namespace Bl.Mottu.Maintenance.Core.Tests.Entities;

public class DeliveryDriverTest
{
    [Fact]
    public void Create_ShouldBeValid()
    {
        var entity = Create();

        Assert.NotNull(entity);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldBeInvalidName_WhenNameIsNullOrEmpty(string name)
    {
        var act = () => Create(name: name!);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Theory]
    [InlineData("A")] // Too short
    [InlineData(StringUtil.String251Chars)] // Too long
    public void Create_ShouldBeInvalidName_WhenNameLengthInvalid(string name)
    {
        var act = () => Create(name: name);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234567890123")] // 13 digits
    [InlineData("123456789012345")] // 15 digits
    [InlineData("1234567890abc")] // contains letters
    public void Create_ShouldBeInvalidCnpj_WhenCnpjInvalid(string cnpj)
    {
        var act = () => Create(cnpj: cnpj!);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Fact]
    public void Create_ShouldBeInvalidBirthDate_WhenYearBefore1900()
    {
        var birthDate = new DateOnly(1899, 12, 31);
        var act = () => Create(birthDate: birthDate);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234567890")] // 10 digits
    [InlineData("123456789012")] // 12 digits
    [InlineData("00000000000")] // all zeros
    [InlineData("123abc45678")] // contains letters
    public void Create_ShouldBeInvalidCnhNumber_WhenCnhNumberInvalid(string cnhNumber)
    {
        var act = () => Create(cnhNumber: cnhNumber!);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("C")] // not in allowed values
    [InlineData("D")] // not in allowed values
    [InlineData("ABC")] // not in allowed values
    public void Create_ShouldBeInvalidCnhType_WhenCnhTypeInvalid(string cnhKind)
    {
        var act = () => Create(cnhKind: cnhKind!);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("B")]
    [InlineData("AB")]
    public void Create_ShouldBeValid_WhenCnhTypeIsAllowed(string cnhKind)
    {
        var entity = Create(cnhKind: cnhKind);

        Assert.NotNull(entity);
        Assert.Equal(cnhKind, entity.CnhCategory);
    }

    [Theory]
    [InlineData("invalid-url")]
    public void Create_ShouldBeInvalidCnhImage_WhenCnhImageUrlInvalid(string cnhImageUrl)
    {
        var act = () => Create(cnhImageUrl: cnhImageUrl);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Fact]
    public void Create_ShouldSetCnhImgUrl_WhenValidUrlProvided()
    {
        var validUrl = "https://example.com/cnh-image.jpg";
        var entity = Create(cnhImageUrl: validUrl);

        Assert.NotNull(entity);
        Assert.NotNull(entity.CnhImgUrl);
        Assert.Equal(validUrl, entity.CnhImgUrl.ToString());
    }

    [Fact]
    public void Create_ShouldNormalizeCnpj_RemoveNonNumericCharacters()
    {
        var cnpjWithFormatting = "12.345.678/0001-90";
        var expectedCnpj = "12345678000190";

        var entity = Create(cnpj: cnpjWithFormatting);

        Assert.NotNull(entity);
        Assert.Equal(expectedCnpj, entity.Cnpj);
    }

    [Fact]
    public void Create_ShouldNormalizeCnhNumber_RemoveNonNumericCharacters()
    {
        var cnhWithFormatting = "123.456.789-01";
        var expectedCnh = "12345678901";

        var entity = Create(cnhNumber: cnhWithFormatting);

        Assert.NotNull(entity);
        Assert.Equal(expectedCnh, entity.CnhNumber);
    }

    [Fact]
    public void Create_ShouldTrimAndUpperCaseName()
    {
        var nameWithSpaces = "  john doe  ";
        var expectedName = "JOHN DOE";

        var entity = Create(name: nameWithSpaces);

        Assert.NotNull(entity);
        Assert.Equal(expectedName, entity.Name);
    }

    [Fact]
    public void Create_ShouldUpperCaseCnhKind()
    {
        var cnhKindLower = "ab";
        var expectedCnhKind = "AB";

        var entity = Create(cnhKind: cnhKindLower);

        Assert.NotNull(entity);
        Assert.Equal(expectedCnhKind, entity.CnhCategory);
    }

    [Fact]
    public void Create_ShouldNotSetCnhImgUrl_WhenNullUrlProvided()
    {
        var entity = Create(cnhImageUrl: null);

        Assert.NotNull(entity);
        Assert.Null(entity.CnhImgUrl);
    }

    [Fact]
    public void Equal_ShouldBeEqual()
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var entity1 = Create(id: id, createdAt: createdAt);
        var entity2 = Create(id: id, createdAt: createdAt);

        Assert.Equal(entity1, entity2);
    }

    [Fact]
    public void Equal_ShouldNotBeEqual()
    {
        var entity1 = Create();
        var entity2 = Create();

        Assert.NotEqual(entity1, entity2);
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameHashCode_ForEqualObjects()
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var entity1 = Create(id: id, createdAt: createdAt);
        var entity2 = Create(id: id, createdAt: createdAt);

        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }

    private static DeliveryDriver Create(
        string code = "DRV001",
        string name = "John Doe",
        string cnpj = "12345678000190",
        DateOnly? birthDate = null,
        string cnhNumber = "12345678901",
        string cnhKind = "AB",
        string? cnhImageUrl = "https://example.com/cnh-image.jpg",
        DateTime? createdAt = null,
        Guid? id = null)
    {
        var defaultBirthDate = new DateOnly(1990, 1, 1);

        return DeliveryDriver.Create(
            code: code,
            name: name,
            cnpj: cnpj,
            birthdate: birthDate ?? defaultBirthDate,
            cnhNumber: cnhNumber,
            cnhKind: cnhKind,
            cnhImageUrl: cnhImageUrl,
            createdAt: createdAt,
            id: id)
            .RequiredResult;
    }
}