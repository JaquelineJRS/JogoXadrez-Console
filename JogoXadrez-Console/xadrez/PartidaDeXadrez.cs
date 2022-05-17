using System;
using System.Collections.Generic;
using tabuleiro;

namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro tabuleiro { get; private set; }
        public int Turno { get; private set; }
        public Cor JogadorAtual { get; private set; }
        public bool Terminada { get; private set; }
        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;
        public bool Xeque { get; private set; }

        public Peca VulneravelEnPassant { get; private set; }

        public PartidaDeXadrez()
        {
            tabuleiro = new Tabuleiro(8, 8);
            Turno = 1;
            JogadorAtual = Cor.Branca;
            Terminada = false;
            VulneravelEnPassant = null;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            colocarPecas();
        }

        public Peca ExecutaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = tabuleiro.RetirarPeca(origem);
            p.IncrementarQteMovimentos();
            Peca pecaCapturada = tabuleiro.RetirarPeca(destino);
            tabuleiro.ColocarPeca(p, destino);
            if (pecaCapturada != null)
            {
                capturadas.Add(pecaCapturada);
            }

            //#jogadaespecial roque pequeno
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca T = tabuleiro.RetirarPeca(origemTorre);
                T.IncrementarQteMovimentos();
                tabuleiro.ColocarPeca(T, destinoTorre);
            }

            //#jogadaespecial roque grande
            if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca T = tabuleiro.RetirarPeca(origemTorre);
                T.IncrementarQteMovimentos();
                tabuleiro.ColocarPeca(T, destinoTorre);
            }

            //#jogadaespecial en passant
            if (p is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == null)
                {
                    Posicao posicaoPeao;
                    if (p.cor == Cor.Branca)
                    {
                        posicaoPeao = new Posicao(destino.Linha + 1, destino.Coluna);
                    }
                    else
                    {
                        posicaoPeao = new Posicao(destino.Linha - 1, destino.Coluna);
                    }
                    pecaCapturada = tabuleiro.RetirarPeca(posicaoPeao);
                    capturadas.Add(pecaCapturada);
                }
            }

            return pecaCapturada;
        }

        public void RealizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = ExecutaMovimento(origem, destino);

            if (EstaEmXeque(JogadorAtual))
            {
                DesfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em Xeque!");
            }

            if (EstaEmXeque(Adversaria(JogadorAtual)))
            {
                Xeque = true;
            }
            else
            {
                Xeque = false;
            }

            if (TesteXequeMate(Adversaria(JogadorAtual)))
            {
                Terminada = true;
            }
            else
            {
                Turno++;
                MudaJogardor();
            }

            Peca p = tabuleiro.Peca(destino);

            //#jogadaespecial en passant
            if (p is Peca && (destino.Linha == origem.Linha - 2 || destino.Linha == origem.Linha + 2))
            {
                VulneravelEnPassant = p;
            }
            else
            {
                VulneravelEnPassant = null;
            }
        }

        private void DesfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = tabuleiro.RetirarPeca(destino);
            p.DecrementarQteMovimentos();
            if (pecaCapturada != null)
            {
                tabuleiro.ColocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            tabuleiro.ColocarPeca(p, origem);

            //#jogadaespecial roque pequeno
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca T = tabuleiro.RetirarPeca(destinoTorre);
                T.DecrementarQteMovimentos();
                tabuleiro.ColocarPeca(T, origemTorre);
            }

            //#jogadaespecial roque grande
            if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca T = tabuleiro.RetirarPeca(destinoTorre);
                T.DecrementarQteMovimentos();
                tabuleiro.ColocarPeca(T, origemTorre);
            }

            //#jogadaespecial en passant
            if (p is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == VulneravelEnPassant)
                {
                    Peca peao = tabuleiro.RetirarPeca(destino);

                    Posicao posicaoPeao;
                    if (p.cor == Cor.Branca)
                    {
                        posicaoPeao = new Posicao(3, destino.Coluna);
                    }
                    else
                    {
                        posicaoPeao = new Posicao(4, destino.Coluna);
                    }

                    tabuleiro.ColocarPeca(peao, posicaoPeao);                    
                }
            }

        }

        public void ValidarPosicaoDeOrigem(Posicao pos)
        {
            if (tabuleiro.Peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            }

            if (JogadorAtual != tabuleiro.Peca(pos).cor)
            {
                throw new TabuleiroException("A peça de origem escolhida não é sua!");
            }

            if (!tabuleiro.Peca(pos).ExisteMovimentosPossiveis())
            {
                throw new TabuleiroException("Não há movimentos possível para a peça de origem escolhida!");
            }
        }

        public void ValidarPosicaoDeDestino(Posicao origem, Posicao destino)
        {
            if (!tabuleiro.Peca(origem).MovimentoPossivel(destino))
            {
                throw new TabuleiroException("Posição de destino inválida!");
            }
        }

        private void MudaJogardor()
        {
            if (JogadorAtual == Cor.Branca)
            {
                JogadorAtual = Cor.Preta;
            }
            else
            {
                JogadorAtual = Cor.Branca;
            }
        }

        public HashSet<Peca> PecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in capturadas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }
            return aux;
        }

        public HashSet<Peca> PecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in pecas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(PecasCapturadas(cor));
            return aux;
        }

        private Cor Adversaria(Cor cor)
        {
            if (cor == Cor.Branca)
            {
                return Cor.Preta;
            }
            else
            {
                return Cor.Branca;
            }
        }

        private Peca Rei(Cor cor)
        {
            foreach (Peca x in PecasEmJogo(cor))
            {
                if (x is Rei)
                {
                    return x;
                }
            }
            return null;
        }

        public bool EstaEmXeque(Cor cor)
        {
            Peca R = Rei(cor);

            if (R == null)
            {
                throw new TabuleiroException("Não tem rei da cor " + cor + " no tabuleiro!");

            }

            foreach(Peca x in PecasEmJogo(Adversaria(cor)))
            {
                bool[,] mat = x.MovimentosPossiveis();
                if (mat[R.posicao.Linha, R.posicao.Coluna])
                {
                    return true;
                }
            }
            return false;
        }

        public bool TesteXequeMate(Cor cor)
        {
            if (!EstaEmXeque(cor))
            {
                return false;
            }
            foreach(Peca x in PecasEmJogo(cor))
            {
                bool[,] mat = x.MovimentosPossiveis();
                for (int i = 0; i < tabuleiro.linhas; i++)
                {
                    for (int j = 0; j < tabuleiro.colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = ExecutaMovimento(origem, destino);
                            bool testeXeque = EstaEmXeque(cor);
                            DesfazMovimento(origem, destino, pecaCapturada);
                            if (!testeXeque)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void ColocarNovaPeca(char coluna, int linha, Peca peca)
        {
            tabuleiro.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        private void colocarPecas()
        {
            ColocarNovaPeca('a', 1, new Torre(tabuleiro, Cor.Branca));
            ColocarNovaPeca('b', 1, new Cavalo(tabuleiro, Cor.Branca));
            ColocarNovaPeca('c', 1, new Bispo(tabuleiro, Cor.Branca));
            ColocarNovaPeca('d', 1, new Dama(tabuleiro, Cor.Branca));
            ColocarNovaPeca('e', 1, new Rei(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('f', 1, new Bispo(tabuleiro, Cor.Branca));
            ColocarNovaPeca('g', 1, new Cavalo(tabuleiro, Cor.Branca));
            ColocarNovaPeca('h', 1, new Torre(tabuleiro, Cor.Branca));
            ColocarNovaPeca('a', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('b', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('c', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('d', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('e', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('f', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('g', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('h', 2, new Peao(tabuleiro, Cor.Branca, this));

            ColocarNovaPeca('a', 8, new Torre(tabuleiro, Cor.Preta));
            ColocarNovaPeca('b', 8, new Cavalo(tabuleiro, Cor.Preta));
            ColocarNovaPeca('c', 8, new Bispo(tabuleiro, Cor.Preta));
            ColocarNovaPeca('d', 8, new Dama(tabuleiro, Cor.Preta));
            ColocarNovaPeca('e', 8, new Rei(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('f', 8, new Bispo(tabuleiro, Cor.Preta));
            ColocarNovaPeca('g', 8, new Cavalo(tabuleiro, Cor.Preta));
            ColocarNovaPeca('h', 8, new Torre(tabuleiro, Cor.Preta));
            ColocarNovaPeca('a', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('b', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('c', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('d', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('e', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('f', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('g', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('h', 7, new Peao(tabuleiro, Cor.Preta, this));
        }
    }
}
