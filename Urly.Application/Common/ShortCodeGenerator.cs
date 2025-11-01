using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Urly.Application.Common;
public static class ShortCodeGenerator
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    private const int DefaultCodeLength = 7;

    public static string GenerateRandomShortCode(int length = DefaultCodeLength)
    {
        var codeBuilder = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            // 1. Obter um número aleatório criptograficamente seguro.
            // Usamos RandomNumberGenerator em vez de Random() porque Random()
            // pode gerar sequências repetidas se chamado muito rapidamente.
            // GetInt32(maxValue) gera um número entre 0 e (maxValue - 1).
            int randomIndex = RandomNumberGenerator.GetInt32(Alphabet.Length);

            // 2. Anexar o caractere aleatório do nosso alfabeto
            codeBuilder.Append(Alphabet[randomIndex]);
        }

        return codeBuilder.ToString();
    }
}
