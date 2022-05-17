using tabuleiro;

namespace xadrez
{

    class Bispo : Peca
    {

        public Bispo(Tabuleiro tabuleiro, Cor cor) : base(tabuleiro, cor)
        {
        }

        public override string ToString()
        {
            return "B";
        }

        private bool podeMover(Posicao pos)
        {
            Peca p = tabuleiro.Peca(pos);
            return p == null || p.cor != cor;
        }

        public override bool[,] MovimentosPossiveis()
        {
            bool[,] mat = new bool[tabuleiro.linhas, tabuleiro.colunas];

            Posicao pos = new Posicao(0, 0);

            // NO
            pos.DefinirValores(posicao.Linha - 1, posicao.Coluna - 1);
            while (tabuleiro.PosicaoValida(pos) && podeMover(pos))
            {
                mat[pos.Linha, pos.Coluna] = true;
                if (tabuleiro.Peca(pos) != null && tabuleiro.Peca(pos).cor != cor)
                {
                    break;
                }
                pos.DefinirValores(pos.Linha - 1, pos.Coluna - 1);
            }

            // NE
            pos.DefinirValores(posicao.Linha - 1, posicao.Coluna + 1);
            while (tabuleiro.PosicaoValida(pos) && podeMover(pos))
            {
                mat[pos.Linha, pos.Coluna] = true;
                if (tabuleiro.Peca(pos) != null && tabuleiro.Peca(pos).cor != cor)
                {
                    break;
                }
                pos.DefinirValores(pos.Linha - 1, pos.Coluna + 1);
            }

            // SE
            pos.DefinirValores(posicao.Linha + 1, posicao.Coluna + 1);
            while (tabuleiro.PosicaoValida(pos) && podeMover(pos))
            {
                mat[pos.Linha, pos.Coluna] = true;
                if (tabuleiro.Peca(pos) != null && tabuleiro.Peca(pos).cor != cor)
                {
                    break;
                }
                pos.DefinirValores(pos.Linha + 1, pos.Coluna + 1);
            }

            // SO
            pos.DefinirValores(posicao.Linha + 1, posicao.Coluna - 1);
            while (tabuleiro.PosicaoValida(pos) && podeMover(pos))
            {
                mat[pos.Linha, pos.Coluna] = true;
                if (tabuleiro.Peca(pos) != null && tabuleiro.Peca(pos).cor != cor)
                {
                    break;
                }
                pos.DefinirValores(pos.Linha + 1, pos.Coluna - 1);
            }

            return mat;
        }
    }
}