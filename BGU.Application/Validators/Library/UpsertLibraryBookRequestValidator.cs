using BGU.Application.Contracts.Library.Requests;
using FluentValidation;

namespace BGU.Application.Validators.Library;

public class UpsertLibraryBookRequestValidator : AbstractValidator<UpsertLibraryBookRequest>
{
    private static readonly string[] AllowedStatuses = ["available", "draft", "archived"];
    private static readonly string[] AllowedBookExtensions = [".pdf"];
    private static readonly string[] AllowedCoverExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public UpsertLibraryBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Authors)
            .NotEmpty().WithMessage("At least one author is required.");

        RuleForEach(x => x.Authors)
            .NotEmpty().WithMessage("Author cannot be empty.")
            .MaximumLength(100).WithMessage("Author must not exceed 100 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters.");

        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required.")
            .MaximumLength(50).WithMessage("Language must not exceed 50 characters.");

        RuleFor(x => x.Status)
            .Must(x => AllowedStatuses.Contains(x?.Trim().ToLowerInvariant()))
            .WithMessage("Status must be available, draft, or archived.");

        RuleFor(x => x.Isbn)
            .MaximumLength(30).When(x => !string.IsNullOrWhiteSpace(x.Isbn));

        RuleFor(x => x.Publisher)
            .MaximumLength(150).When(x => !string.IsNullOrWhiteSpace(x.Publisher));

        RuleFor(x => x.Edition)
            .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Edition));

        RuleFor(x => x.PublishedYear)
            .InclusiveBetween(1000, DateTime.UtcNow.Year + 1)
            .When(x => x.PublishedYear.HasValue)
            .WithMessage("Published year is invalid.");

        RuleForEach(x => x.Tags)
            .MaximumLength(50).WithMessage("Tag must not exceed 50 characters.");

        When(x => x.BookFile is not null, () =>
        {
            RuleFor(x => x.BookFile!.Length)
                .LessThanOrEqualTo(100 * 1024 * 1024)
                .WithMessage("Book file must not exceed 100 MB.");

            RuleFor(x => Path.GetExtension(x.BookFile!.FileName).ToLowerInvariant())
                .Must(x => AllowedBookExtensions.Contains(x))
                .WithMessage("Book file must be a PDF.");
        });

        When(x => x.CoverImage is not null, () =>
        {
            RuleFor(x => x.CoverImage!.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024)
                .WithMessage("Cover image must not exceed 5 MB.");

            RuleFor(x => Path.GetExtension(x.CoverImage!.FileName).ToLowerInvariant())
                .Must(x => AllowedCoverExtensions.Contains(x))
                .WithMessage("Cover image must be jpg, jpeg, png, or webp.");
        });
    }
}
