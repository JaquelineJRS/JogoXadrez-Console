using System;
using tabuleiro;
using xadrez;

namespace JogoXadrez_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PosicaoXadrez posicao = new PosicaoXadrez('c', 7);

                Console.WriteLine(posicao);

                Console.WriteLine(posicao.toPosicao());
            }
            catch (TabuleiroException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
