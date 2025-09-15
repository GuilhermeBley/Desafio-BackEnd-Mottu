using System.ComponentModel.DataAnnotations;

namespace Bl.Mottu.Maintenance.Infrastructure.Config;

public class PostgreConfig
{
    [Required, StringLength(maximumLength: int.MaxValue, MinimumLength = 10)]
    public string ConnectionString { get; set; } = string.Empty;
}
