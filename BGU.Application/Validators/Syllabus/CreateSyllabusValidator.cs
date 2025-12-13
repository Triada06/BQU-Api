using BGU.Application.Contracts.Syllabus.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BGU.Application.Validators.Syllabus;

public class CreateSyllabusValidator : AbstractValidator<CreateSyllabusRequest>
{
    private const int MaxFileSizeBytes = 10 * 1024 * 1024; //10 MB

    public CreateSyllabusValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required.")
            .Must(BePdf).WithMessage("File must be a PDF.")
            .Must(BeWithinSizeLimit).WithMessage("File size exceeds 10 MB.");

        RuleFor(x => x.TaughtSubjectId)
            .NotEmpty().WithMessage("TaughtSubjectId is required.");
    }

    private bool BePdf(IFormFile file)
    {
        if (file == null || file.Length < 4)
            return false;

        //Do NOT trust ContentType alone
        using var stream = file.OpenReadStream();
        Span<byte> header = stackalloc byte[4];
        stream.Read(header);

        //%PDF
        return header.SequenceEqual(new byte[] { 0x25, 0x50, 0x44, 0x46 });
    }

    private bool BeWithinSizeLimit(IFormFile file)
        => file.Length <= MaxFileSizeBytes;
}