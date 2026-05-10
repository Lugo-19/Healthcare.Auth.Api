using FluentValidation;
using Healthcare.Auth.Api.Core.Roles.Dtos.Request;

namespace Healthcare.Auth.Api.Core.Roles.Validators
{
    public class CrearRolRequestValidator : AbstractValidator<CrearRolRequest>
    {
        public CrearRolRequestValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Descripcion).MaximumLength(255);
        }
    }

    public class ActualizarRolRequestValidator : AbstractValidator<ActualizarRolRequest>
    {
        public ActualizarRolRequestValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Descripcion).MaximumLength(255);
        }
    }

    public class AsignarPermisosRequestValidator : AbstractValidator<AsignarPermisosRequest>
    {
        public AsignarPermisosRequestValidator()
        {
            RuleFor(x => x.IdsPermisos).NotNull();
        }
    }
}
