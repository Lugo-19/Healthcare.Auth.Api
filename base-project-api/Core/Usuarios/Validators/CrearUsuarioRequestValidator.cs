using FluentValidation;
using Healthcare.Auth.Api.Core.Usuarios.Dtos.Request;

namespace Healthcare.Auth.Api.Core.Usuarios.Validators
{
    public class CrearUsuarioRequestValidator : AbstractValidator<CrearUsuarioRequest>
    {
        public CrearUsuarioRequestValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Apellido).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Correo).NotEmpty().EmailAddress().MaximumLength(255);
            RuleFor(x => x.Contrasena).NotEmpty().MinimumLength(6).MaximumLength(100);
            RuleFor(x => x.IdRol).NotEmpty();
        }
    }

    public class ActualizarUsuarioRequestValidator : AbstractValidator<ActualizarUsuarioRequest>
    {
        public ActualizarUsuarioRequestValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Apellido).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Correo).NotEmpty().EmailAddress().MaximumLength(255);
            RuleFor(x => x.IdRol).NotEmpty();
        }
    }

    public class CambiarEstadoUsuarioRequestValidator : AbstractValidator<CambiarEstadoUsuarioRequest>
    {
        public CambiarEstadoUsuarioRequestValidator()
        {
            RuleFor(x => x.IdEstado).InclusiveBetween((short)1, (short)3)
                .WithMessage("El estado debe ser 1 (Activo), 2 (Inactivo) o 3 (Eliminado).");
        }
    }
}
