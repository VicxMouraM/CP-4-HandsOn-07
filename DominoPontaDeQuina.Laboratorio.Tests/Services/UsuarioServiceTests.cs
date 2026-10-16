using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Domain.Repositories;
using DominoPontaDeQuina.Domain.Services;

namespace DominoPontaDeQuina.Laboratorio.Tests.Services;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repositorio = new(MockBehavior.Strict);
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _service = new UsuarioService(_repositorio.Object);
    }

    // ---------- CadastrarAsync ----------

    [Fact]
    public async Task CadastrarAsync_EmailNovo_ConsultaESalvaUsuario()
    {
        // Arrange
        Usuario? usuarioSalvo = null;
        _repositorio
            .Setup(r => r.ExisteEmailAsync("ana@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .Callback<Usuario, CancellationToken>((u, _) => usuarioSalvo = u)
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.CadastrarAsync("  Ana  ", "  ANA@Email.com ", "hash123");

        // Assert
        resultado.Nome.Should().Be("Ana");
        resultado.Email.Should().Be("ana@email.com");
        resultado.HashSenha.Should().Be("hash123");
        usuarioSalvo.Should().BeSameAs(resultado);

        _repositorio.Verify(r => r.ExisteEmailAsync("ana@email.com", It.IsAny<CancellationToken>()), Times.Once);
        _repositorio.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CadastrarAsync_EmailJaCadastrado_LancaExcecaoENaoSalva()
    {
        // Arrange
        _repositorio
            .Setup(r => r.ExisteEmailAsync("ana@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var acao = () => _service.CadastrarAsync("Ana", "ana@email.com", "hash123");

        // Assert
        await acao.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ana@email.com*");

        _repositorio.Verify(r => r.ExisteEmailAsync("ana@email.com", It.IsAny<CancellationToken>()), Times.Once);
        _repositorio.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("", "ana@email.com", "hash")]
    [InlineData("Ana", " ", "hash")]
    [InlineData("Ana", "ana@email.com", "")]
    public async Task CadastrarAsync_DadosInvalidos_NaoConsultaNemSalva(string nome, string email, string hashSenha)
    {
        // Arrange
        var acao = () => _service.CadastrarAsync(nome, email, hashSenha);

        // Act + Assert
        await acao.Should().ThrowAsync<ArgumentException>();

        _repositorio.VerifyNoOtherCalls();
    }

    // ---------- ObterPorIdAsync ----------

    [Fact]
    public async Task ObterPorIdAsync_UsuarioExistente_RetornaUsuarioDoRepositorio()
    {
        // Arrange
        var usuario = new Usuario { Nome = "Bia", Email = "bia@email.com" };
        _repositorio
            .Setup(r => r.ObterPorIdAsync(usuario.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        // Act
        var resultado = await _service.ObterPorIdAsync(usuario.Id);

        // Assert
        resultado.Should().BeSameAs(usuario);
        _repositorio.Verify(r => r.ObterPorIdAsync(usuario.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_UsuarioInexistente_LancaKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositorio
            .Setup(r => r.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        // Act
        var acao = () => _service.ObterPorIdAsync(id);

        // Assert
        await acao.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{id}*");
    }

    [Fact]
    public async Task ObterPorIdAsync_IdVazio_LancaArgumentExceptionSemConsultar()
    {
        // Arrange
        var acao = () => _service.ObterPorIdAsync(Guid.Empty);

        // Act + Assert
        await acao.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("id");

        _repositorio.VerifyNoOtherCalls();
    }

    [Fact]
    public void Construtor_RepositorioNulo_LancaArgumentNullException()
    {
        // Arrange
        var acao = () => new UsuarioService(null!);

        // Act + Assert
        acao.Should().Throw<ArgumentNullException>()
            .WithParameterName("repositorio");
    }
}
