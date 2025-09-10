namespace Bl.Mottu.Maintenance.Core.Primitive;

public enum CoreExceptionCode
{
    Unauthorized = 401,

    Forbbiden = 403,

    NotFound = 404,

    Conflict = 409,

    BadRequest = 400,
    InvalidPlate = 400_1,
    InvalidYear = 400_2,
    InvalidModel = 400_3,
    InvalidName = 400_4,
    InvalidCnpj = 400_5,
    InvalidBirthDate = 400_6,
    InvalidCnhNumber = 400_7,
    InvalidCnhType = 400_8,
    InvalidCnhImage = 400_9,
    InvalidStartDate = 400_10,
    InvalidEstimatedEndDate = 400_11,
    InvalidRentalPeriod = 400_12,
    InvalidEndDate = 400_13,
    LateReturn = 400_13,
    InvalidRentalPlan = 400_13,
    PlanMismatch = 400_14,
}
