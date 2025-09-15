using Bl.Mottu.Maintenance.Core.Entities;
using Bl.Mottu.Maintenance.Core.Primitive;
using System.Numerics;

namespace Bl.Mottu.Maintenance.Core.Tests.Entities;

public class VehicleRentTest
{
    [Fact]
    public void Create_ShouldBeValid()
    {
        var entity = Create();

        Assert.NotNull(entity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldBeInvalidPlan_WhenPlanIsZeroOrNegative(int plan)
    {
        var act = () => Create(plan: plan);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(60)]
    public void Create_ShouldBeInvalidPlan_WhenPlanNotInAllowedValues(int plan)
    {
        var act = () => Create(plan: plan);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Fact]
    public void Create_ShouldBeInvalidStartDate_WhenStartAtIsMinValue()
    {
        var act = () => Create(startAt: DateTime.MinValue);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Fact]
    public void Create_ShouldBeInvalidEstimatedEndDate_WhenExpectedEndingDateIsMinValue()
    {
        var act = () => Create(expectedEndingDate: DateTime.MinValue);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Fact]
    public void Create_ShouldBeInvalidRentalPeriod_WhenStartAtAfterExpectedEndingDate()
    {
        var startAt = DateTime.UtcNow;
        var expectedEndingDate = startAt.AddDays(-1);

        var act = () => Create(startAt: startAt, expectedEndingDate: expectedEndingDate);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Fact]
    public void Create_ShouldBeInvalidEndDate_WhenEndedAtBeforeStartAt()
    {
        var startAt = DateTime.UtcNow;
        var endedAt = startAt.AddDays(-1);

        var act = () => Create(startAt: startAt, endedAt: endedAt);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Fact]
    public void Create_ShouldBeLateReturn_WhenEndedAtAfterExpectedEndingDate()
    {
        var startAt = DateTime.UtcNow;
        var expectedEndingDate = startAt.AddDays(7);
        var endedAt = expectedEndingDate.AddDays(1);

        var act = () => Create(startAt: startAt, expectedEndingDate: expectedEndingDate, endedAt: endedAt);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Theory]
    [InlineData(7)]
    [InlineData(15)]
    [InlineData(30)]
    [InlineData(45)]
    [InlineData(50)]
    public void Create_ShouldBeValid_WhenPlanIsInAllowedValues(int plan)
    {
        var startAt = DateTime.UtcNow;
        var expectedEndingDate = startAt.AddDays(plan);

        var entity = Create(plan: plan, startAt: startAt, expectedEndingDate: expectedEndingDate);

        Assert.NotNull(entity);
        Assert.Equal(plan, entity.Plan);
    }

    [Fact]
    public void Create_ShouldBePlanMismatch_WhenExpectedEndingDateBeforeCalculatedEndDate()
    {
        var startAt = DateTime.UtcNow;
        var plan = 7;
        var expectedEndingDate = startAt.AddDays(plan - 1); // 1 day less than plan

        var act = () => Create(startAt: startAt, expectedEndingDate: expectedEndingDate, plan: plan);

        Assert.ThrowsAny<CoreException>(act);
    }

    [Fact]
    public void Create_ShouldBeValid_WhenExpectedEndingDateMatchesCalculatedEndDate()
    {
        var startAt = DateTime.UtcNow;
        var plan = 7;
        var expectedEndingDate = startAt.AddDays(plan);

        var entity = Create(startAt: startAt, expectedEndingDate: expectedEndingDate, plan: plan);

        Assert.NotNull(entity);
        Assert.Equal(expectedEndingDate.Date, entity.ExpectedEndingDate.Date);
    }

    [Fact]
    public void Create_ShouldBeValid_WhenExpectedEndingDateAfterCalculatedEndDate()
    {
        var startAt = DateTime.UtcNow;
        var plan = 7;
        var expectedEndingDate = startAt.AddDays(plan + 1); // 1 day more than plan

        var entity = Create(startAt: startAt, expectedEndingDate: expectedEndingDate, plan: plan);

        Assert.NotNull(entity);
        Assert.Equal(expectedEndingDate.Date, entity.ExpectedEndingDate.Date);
    }

    [Fact]
    public void Create_ShouldSetEndedAt_WhenProvided()
    {
        var startAt = DateTime.UtcNow;
        var expectedEndingDate = startAt.AddDays(7);
        var endedAt = startAt.AddDays(5);

        var entity = Create(startAt: startAt, expectedEndingDate: expectedEndingDate, endedAt: endedAt);

        Assert.NotNull(entity);
        Assert.Equal(endedAt.Date, entity.EndedAt?.Date);
    }

    [Fact]
    public void Create_ShouldNotSetEndedAt_WhenNotProvided()
    {
        var entity = Create();

        Assert.Null(entity.EndedAt);
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

    private static VehicleRent Create(
        Guid? deliveryDriverId = null,
        Guid? vehicleId = null,
        DateTime? startAt = null,
        DateTime? expectedEndingDate = null,
        int plan = 7,
        DateTime? endedAt = null,
        DateTime? createdAt = null,
        Guid? id = null)
    {
        var defaultStartAt = DateTime.UtcNow;
        var defaultExpectedEndingDate = defaultStartAt.AddDays(plan);

        return VehicleRent.Create(
            deliveryDriverId: deliveryDriverId ?? Guid.NewGuid(),
            vehicleId: vehicleId ?? Guid.NewGuid(),
            startAt: startAt ?? defaultStartAt,
            expectedEndingDate: expectedEndingDate ?? defaultExpectedEndingDate,
            plan: plan,
            endedAt: endedAt,
            createdAt: createdAt,
            id: id)
            .RequiredResult;
    }
}