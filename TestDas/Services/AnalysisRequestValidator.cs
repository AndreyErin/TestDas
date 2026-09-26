using FluentValidation;
using TestDas.Models;

namespace TestDas.Services;

public class AnalysisRequestValidator : AbstractValidator<AnalysisRequest>
{
    private const int AesKeyLength = 32;

    public AnalysisRequestValidator()
    {
        RuleFor(x => x.Selector)
            .NotEmpty().WithMessage("Selector обязателен")
            .MaximumLength(500);

        RuleFor(x => x.Attribute)
            .NotEmpty().WithMessage("Attribute обязателен")
            .MaximumLength(100);

        RuleFor(x => x.Url_B64)
            .NotEmpty().WithMessage("url_b64 обязателен")
            .Must(s => BeValidBase64(s)).WithMessage("url_b64 не является корректным Base64");

        RuleFor(x => x.Page_B64)
            .NotEmpty().WithMessage("page_b64 обязателен")
            .Must(s => BeValidBase64(s)).WithMessage("page_b64 не является корректным Base64");

        RuleFor(x => x.Key_Bytes_B64)
            .NotEmpty().WithMessage("key_bytes_b64 обязателен")
            .Must(s => BeValidBase64(s, true)).WithMessage("key_bytes_b64 не является корректным Base64. Ключ должен быть 32 байта (AES-256)");

        RuleFor(x => x.Encrypted_Text_Bytes_B64)
            .NotEmpty().WithMessage("encrypted_text_bytes_b64 обязателен")
            .Must(s => BeValidBase64(s)).WithMessage("encrypted_text_bytes_b64 не является корректным Base64");
    }

    private static bool BeValidBase64(string value, bool checkAesKeyLength = false)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        Span<byte> buffer = new byte[value.Length];
        if (!Convert.TryFromBase64String(value, buffer, out var written)) return false;

        return  !checkAesKeyLength || written == AesKeyLength;
    }
}