using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Domain.Repositories;

namespace DominoPontaDeQuina.Domain.Services;

/// <summary>
/// Regras de negocio de usuarios: decide quando consultar e quando salvar no repositorio.
/// </summary>
public class UsuarioService(IUsuarioRepository repositorio)
{
    private readonly IUsuarioRepository _repositorio =
        repositorio ?? throw new ArgumentNullException(nameof(repositorio));

    /// <summary>
    /// Cadastra um novo usuario.
    /// Dados invalidos nao chegam ao repositorio; e-mail ja cadastrado nao e salvo.
    /// </summary>
    public async Task<Usuario> CadastrarAsync(
        string nome, string email, string hashSenha, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(hashSenha);

        var emailNormalizado = email.Trim().ToLowerInvariant();

        if (await _repositorio.ExisteEmailAsync(emailNormalizado, cancellationToken))
        {
            throw new InvalidOperationException($"Ja existe um usuario com o e-mail '{emailNormalizado}'.");
        }

        var usuario = new Usuario
        {
            Nome = nome.Trim(),
            Email = emailNormalizado,
            HashSenha = hashSenha
        };

        await _repositorio.AdicionarAsync(usuario, cancellationToken);
        return usuario;
    }

    /// <summary>
    /// Obtem um usuario pelo identificador.
    /// </summary>
    /// <exception cref="ArgumentException">Quando o id for vazio (o repositorio nao e consultado).</exception>
    /// <exception cref="KeyNotFoundException">Quando o usuario nao existir.</exception>
    public async Task<Usuario> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("O id do usuario nao pode ser vazio.", nameof(id));
        }

        return await _repositorio.ObterPorIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Usuario '{id}' nao encontrado.");
    }
}
