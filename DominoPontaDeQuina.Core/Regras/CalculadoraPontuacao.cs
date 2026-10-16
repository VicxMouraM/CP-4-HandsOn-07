using DominoPontaDeQuina.Core.Models;

namespace DominoPontaDeQuina.Core.Regras;

/// <summary>
/// Regra de pontuacao do domino "ponta de quina":
/// quando a soma das pontas externas for multipla de 5, a jogada vale soma / 5 pontos;
/// caso contrario, nao pontua.
/// </summary>
public static class CalculadoraPontuacao
{
    /// <summary>
    /// Divisor que define a "quina" (multiplos de 5).
    /// </summary>
    public const int Divisor = 5;

    /// <summary>
    /// Calcula os pontos obtidos a partir da soma das pontas externas.
    /// </summary>
    /// <param name="somaPontas">A soma das pontas externas do tabuleiro.</param>
    /// <returns><c>somaPontas / 5</c> quando a soma for multipla de 5; caso contrario, 0.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Quando a soma for negativa.</exception>
    public static int Calcular(int somaPontas)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(somaPontas);

        return somaPontas % Divisor == 0
            ? somaPontas / Divisor
            : 0;
    }

    /// <summary>
    /// Calcula os pontos obtidos com base no estado atual do tabuleiro.
    /// </summary>
    /// <param name="tabuleiro">O tabuleiro da rodada.</param>
    /// <returns>Os pontos correspondentes a soma das pontas externas.</returns>
    /// <exception cref="ArgumentNullException">Quando o tabuleiro for nulo.</exception>
    public static int Calcular(Tabuleiro tabuleiro)
    {
        ArgumentNullException.ThrowIfNull(tabuleiro);
        return Calcular(tabuleiro.SomarPontasExternas());
    }
}
