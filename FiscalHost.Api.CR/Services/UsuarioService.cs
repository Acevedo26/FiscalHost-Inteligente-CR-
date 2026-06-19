using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> ObtenerTodosAsync();
}

public class UsuarioService(IUsuarioRepository usuarioRepo) : IUsuarioService
{
    public async Task<IEnumerable<UsuarioDto>> ObtenerTodosAsync()
    {
        var usuarios = await usuarioRepo.GetAllAsync();
        
        return usuarios.Select(u => new UsuarioDto
        {
            UsuarioId = u.UsuarioId,
            TipoIdentificacion = u.TipoIdentificacion,
            NumeroIdentificacion = u.NumeroIdentificacion,
            NombreCompleto = u.NombreCompleto,
            RazonSocial = u.RazonSocial,
            CorreoElectronico = u.CorreoElectronico,
            Estado = u.Estado,
            RolPrincipal = u.RolPrincipal,
            EsUsuarioNuevo = u.EsUsuarioNuevo,
            CorreoVerificado = u.CorreoVerificado,
            FechaActivacion = u.FechaActivacion,
            UltimoAcceso = u.UltimoAcceso
        });
    }
}
