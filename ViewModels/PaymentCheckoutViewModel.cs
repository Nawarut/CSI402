using Nawarut_8250_L005.Entities;

namespace Nawarut_8250_L005.ViewModels;

public class PaymentCheckoutViewModel
{
    public Booking Booking { get; set; } = null!;
    public string PaymentType { get; set; } = "deposit";
    public decimal Amount { get; set; }
}
