using BGU.Core.Entities;

namespace BGU.Application.Common.HelperServices.Interfaces;

public interface IAcademicHelper
{
    Task<bool> CreateAcademicRequirementsAsync(List<Class> classes, Student student,
        string taughtSubjectId);

    // Task<GenerateTranscriptResult> GenerateTranscript();
}