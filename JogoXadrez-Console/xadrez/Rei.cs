using tabuleiro;

namespace xadrez
{
    class Rei : Peca
    {
        private PartidaDeXadrez partida;

        public Rei(Tabuleiro tabuleiro, Cor cor, PartidaDeXadrez partida) : base (tabuleiro, cor)
        {
            this.partida = partida;   
        }

        public Rei(Tabuleiro tabuleiro, Cor cor) : base(tabuleiro, cor)
        {
        }

        public override string ToString()
        {
            return "R";
        }

        private bool podeMover(Posicao pos)
        {
            Peca p = tabuleiro.Peca(pos);
            return p == null || p.cor != cor;
        }

        private bool TesteTorreParaRoque(Posicao pos)
        {
            Peca p = tabuleiro.Peca(pos);
            return p != null && p is Torre && p.cor == cor && p.qteMovimentos == 0;
        }

        public override bool[,] MovimentosPossiveis() 
        {
            bool[,] mat = new bool[tabuleiro.linhas, tabuleiro.colunas];

            Posicao pos = new Posicao(0, 0);

            //acima
            pos.DefinirValores(posicao.Linha - 1, posicao.Coluna + 1);
            if (tabuleiro.PosicaoValida(pos) && podeMover(pos))
            {
                mat[pos.Linha, pos.Coluna] = true;
            }

            //ne
            pos.DefinirValores(posicao.Linha - 1, posicao.Coluna);
            if (tabuleiro.PosicaoValida(pos) && podeMover(pos))
            {
                mat[pos.Linha, pos.Coluna] = true;
            }

            //direita
            pos.DefinirValores(posicao.Linha, posicao.Coluna + 1);
            if (tabuleiro.PosicaoValida(pos) && podeMover(pos))
            {
                mat[pos.Linha, pos.Coluna] = true;
            }

            //se
            pos.DefinirValores(posicao.Linha + 1, posicao.Coluna + 1);
            if (tabuleiro.PosicaoValida(pos) && podeMover(pos))
            {
                mat[pos.Linha, pos.Coluna] = true;
            }

            //abaixo
            pos.DefinirValores(posicao.Linha + 1, posicao.Coluna);
            if (tabuleiro.PosicaoValida(pos) && podeMover(pos))
            {
                mat[pos.Linha, pos.Coluna] = true;
            }

            //so
            pos.DefinirValores(posicao.Linha + 1, posicao.Coluna - 1);
            if (tabuleiro.PosicaoValida(pos) && podeMover(pos))
            {
                mat[pos.Linha, pos.Coluna] = true;
            }

            //esquerda
            pos.DefinirValores(posicao.Linha, posicao.Coluna - 1);
            if (tabuleiro.PosicaoValida(pos) && podeMover(pos))
            {
                mat[pos.Linha, pos.Coluna] = true;
            }

            //no
            pos.DefinirValores(posicao.Linha - 1, posicao.Coluna - 1);
            if (tabuleiro.PosicaoValida(pos) && podeMover(pos))
            {
                mat[pos.Linha, pos.Coluna] = true;
            }

            //#jogadaespecial roque
            if (qteMovimentos == 0 || !partida.Xeque)
            {
                //#jogadaespecial roque pequeno
                Posicao posicaoTorre1 = new Posicao(posicao.Linha, posicao.Coluna + 3);
                if (TesteTorreParaRoque(posicaoTorre1))
                {
                    Posicao p1 = new Posicao(posicao.Linha, posicao.Coluna + 1);
                    Posicao p2 = new Posicao(posicao.Linha, posicao.Coluna + 2);
                    if (tabuleiro.Peca(p1)==null || tabuleiro.Peca(p2) == null)
                    {
                        mat[posicao.Linha, posicao.Coluna + 2] = true;
                    }
                }

                //#jogadaespecial roque grande
                Posicao posicaoTorre2 = new Posicao(posicao.Linha, posicao.Coluna - 4);
                if (TesteTorreParaRoque(posicaoTorre2))
                {
                    Posicao p1 = new Posicao(posicao.Linha, posicao.Coluna - 1);
                    Posicao p2 = new Posicao(posicao.Linha, posicao.Coluna - 2);
                    Posicao p3 = new Posicao(posicao.Linha, posicao.Coluna - 3);
                    if (tabuleiro.Peca(p1) == null || tabuleiro.Peca(p2) == null || tabuleiro.Peca(p3) == null)
                    {
                        mat[posicao.Linha, posicao.Coluna - 2] = true;
                    }
                }
            }

            return mat;
        }
    }
}
