using FluentValidation;
using Healthcare.Auth.Api.Core.Permisos.Dtos.Request;

namespace Healthcare.Auth.Api.Core.Permisos.Validators
{
    public class CrearPermisoRequestValidator : AbstractValidator<CrearPermisoRequest>
    {
        public CrearPermisoRequestValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Modulo).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Accion).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Descripcion).MaximumLength(255);
        }
    }

    public class ActualizarPermisoRequestValidator : AbstractValidator<ActualizarPermisoRequest>
    {
        public ActualizarPermisoRequestValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Modulo).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Accion).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Descripcion).MaximumLength(255);
        }
    }
}
