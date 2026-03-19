using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class Colloquiums : BaseEntity
{
    private int _orderNumber;

    public required int OrderNumber
    {
        get => _orderNumber;

        set
        {
            if (value is < 1 or > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(OrderNumber));
            }

            _orderNumber = value;
        }
    }

    public Grade Grade { get; set; }
    public DateTime Date { get; set; }
    public bool IsConfirmed { get; set; }

    public string StudentId { get; set; }
    public Student Student { get; set; }
    public string TaughtSubjectId { get; set; }
    public TaughtSubject TaughtSubject { get; set; }
}