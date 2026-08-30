using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Dtos.WorkingSchedule;

public class WorkingScheduleDayDto : IValidatableObject
{
    [EnumDataType(typeof(DayOfWeek))]
    public DayOfWeek DayOfWeek { get; set; }
    [JsonConverter(typeof(NullableTimeOnlyJsonConverter))]
    public TimeOnly? StartTime { get; set; }
    [JsonConverter(typeof(NullableTimeOnlyJsonConverter))]
    public TimeOnly? EndTime { get; set; }
    public bool IsWorkingDay { get; set; }
    [Range(0, 1440)]
    public int BreakMinutes { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!IsWorkingDay) yield break;

        if (!StartTime.HasValue)
            yield return new ValidationResult(
                $"Start time is required for {DayOfWeek}.",
                [nameof(StartTime)]);

        if (!EndTime.HasValue)
            yield return new ValidationResult(
                $"End time is required for {DayOfWeek}.",
                [nameof(EndTime)]);

        if (!StartTime.HasValue || !EndTime.HasValue) yield break;

        if (EndTime.Value <= StartTime.Value)
        {
            yield return new ValidationResult(
                $"End time must be after start time for {DayOfWeek}.",
                [nameof(EndTime)]);
            yield break;
        }

        var shiftMinutes = (EndTime.Value.ToTimeSpan() - StartTime.Value.ToTimeSpan()).TotalMinutes;
        if (BreakMinutes >= shiftMinutes)
            yield return new ValidationResult(
                $"Break minutes must be shorter than the shift for {DayOfWeek}.",
                [nameof(BreakMinutes)]);
    }
}

public class NullableTimeOnlyJsonConverter : JsonConverter<TimeOnly?>
{
    private static readonly string[] Formats = ["HH:mm", "HH:mm:ss", "HH:mm:ss.FFFFFFF"];

    public override TimeOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (TimeOnly.TryParseExact(value, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
            return time;
        throw new JsonException($"'{value}' is not a valid time. Use HH:mm or HH:mm:ss.");
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly? value, JsonSerializerOptions options)
    {
        if (!value.HasValue) writer.WriteNullValue();
        else writer.WriteStringValue(value.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
    }
}

public abstract class WorkingScheduleRequestDto : IValidatableObject
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    [Required, MinLength(7), MaxLength(7)]
    public List<WorkingScheduleDayDto> Days { get; set; } = [];

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EffectiveFrom == default)
            yield return new ValidationResult(
                "Effective from date is required.",
                [nameof(EffectiveFrom)]);

        if (EffectiveTo.HasValue && EffectiveFrom != default && EffectiveTo.Value.Date < EffectiveFrom.Date)
            yield return new ValidationResult(
                "Effective to date cannot be before effective from date.",
                [nameof(EffectiveTo)]);

        if (Days.Count == 7 && Days.Select(d => d.DayOfWeek).Distinct().Count() != 7)
            yield return new ValidationResult(
                "Exactly one entry for each day of the week is required.",
                [nameof(Days)]);
    }
}

public class CreateWorkingScheduleDto : WorkingScheduleRequestDto
{
    public Guid? CompanyId { get; set; }
}

public class UpdateWorkingScheduleDto : WorkingScheduleRequestDto
{
}

public class WorkingScheduleListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public int WorkingDays { get; set; }
}

public class WorkingScheduleDetailsDto : WorkingScheduleListDto
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<WorkingScheduleDayDto> Days { get; set; } = [];
}
