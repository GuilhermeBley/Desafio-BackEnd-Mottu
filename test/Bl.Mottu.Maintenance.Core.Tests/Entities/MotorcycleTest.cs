using Bl.Mottu.Maintenance.Core.Entities;
using Bl.Mottu.Maintenance.Core.Primitive;
using System.Numerics;

namespace Bl.Mottu.Maintenance.Core.Tests.Entities;

public class MotorcycleTest
{
    [Fact]
    public void Create_ShoulBeValid()
    {
        var entity = Create();

        Assert.NotNull(entity);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldBeInvalidModel(string? model)
    {
        var act = () => Create(model: model!);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldBeInvalidPlate(string placa)
    {
        var act = () => Create(placa: placa);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1899)]
    public void Create_ShouldBeInvalidYear(int year)
    {
        var act = () => Create(year: year);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Fact]
    public void Equal_ShouldBeEqual()
    {
        var entity = Create();
        
        Assert.Equal(entity, entity);
    }

    [Fact]
    public void Equal_ShouldNotBeEqual()
    {
        var entity1 = Create();
        var entity2 = Create();
        
        Assert.NotEqual(entity1, entity2);
    }

    private static Motorcycle Create(
        string placa = "ABC1234",
        string model = "MODEL1",
        string code = "123",
        int year = 2025,
        DateTime? createdAt = null,
        Guid? id = null)
    {
        return Motorcycle.Create(
            placa: placa,
            model: model,
            code: code,
            year: year,
            createdAt: createdAt,
            id: id)
            .RequiredResult;
    }
}
