using LabPat.Domain.Entities;

namespace LabPat.Application.Common;

public interface ITokenGenerator
{
    string Generate(Usuario usuario);
}
