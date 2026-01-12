using BGU.Application.Dtos.Teacher;

namespace BGU.Application.Services.Interfaces;

public interface IExcelCrudService
{
    Task<byte[]> ProcessTeachersAsync(List<TeacherDto> items);
}