using DominoPontaDeQuina.Core.Models;
using DominoPontaDeQuina.Core.Regras;

namespace DominoPontaDeQuina.Laboratorio.Tests.Regras;

public class CalculadoraPontuacaoTests
{
    [Fact]
    public void Calcular_Soma10_Retorna2()
    {
        // Arrange
        const int somaPontas = 10;

        // Act
        var pontos = CalculadoraPontuacao.Calcular(somaPontas);

        // Assert
        pontos.Should().Be(2);
    }

    [Fact]
    public void Calcular_Soma15_Retorna3()
    {
        // Arrange
        const int somaPontas = 15;

        // Act
        var pontos = CalculadoraPontuacao.Calcular(somaPontas);

        // Assert
        pontos.Should().Be(3);
    }

    [Theory]
    [InlineData(5, 1)]
    [InlineData(10, 2)]
    [InlineData(15, 3)]
    [InlineData(20, 4)]
    [InlineData(25, 5)]
    public void Calcular_SomaMultiplaDe5_RetornaSomaDivididaPor5(int somaPontas, int pontosEsperados)
    {
        // Arrange (dados via InlineData)

        // Act
        var pontos = CalculadoraPontuacao.Calcular(somaPontas);

        // Assert
        pontos.Should().Be(pontosEsperados);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(7)]
    [InlineData(11)]
    [InlineData(12)]
    public void Calcular_SomaNaoMultiplaDe5_RetornaZero(int somaPontas)
    {
        // Arrange (dados via InlineData)

        // Act
        var pontos = CalculadoraPontuacao.Calcular(somaPontas);

        // Assert
        pontos.Should().Be(0);
    }

    [Fact]
    public void Calcular_SomaZero_RetornaZero()
    {
        // Arrange
        const int somaPontas = 0;

        // Act
        var pontos = CalculadoraPontuacao.Calcular(somaPontas);

        // Assert
        pontos.Should().Be(0);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Calcular_SomaNegativa_LancaArgumentOutOfRangeException(int somaPontas)
    {
        // Arrange
        var acao = () => CalculadoraPontuacao.Calcular(somaPontas);

        // Act + Assert
        acao.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("somaPontas");
    }

    [Fact]
    public void Calcular_TabuleiroVazio_RetornaZero()
    {
        // Arrange
        var tabuleiro = new Tabuleiro();

        // Act
        var pontos = CalculadoraPontuacao.Calcular(tabuleiro);

        // Assert
        pontos.Should().Be(0);
    }

    [Fact]
    public void Calcular_TabuleiroNulo_LancaArgumentNullException()
    {
        // Arrange
        Tabuleiro tabuleiro = null!;
        var acao = () => CalculadoraPontuacao.Calcular(tabuleiro);

        // Act + Assert
        acao.Should().Throw<ArgumentNullException>()
            .WithParameterName("tabuleiro");
    }
}
