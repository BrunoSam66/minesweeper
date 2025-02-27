﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace Minesweeper
{
    public partial class Jogo : Form
    {

        private enum Dificuldade { facil, medio }
        private Dificuldade dificuldade = Dificuldade.facil;

        private static readonly Random rnd = new Random();
        private int[] mines1 = new int[10];
        private int[] mines2 = new int[40];
        private string BotaoClicado;

        private int bombas = 10;
        private int quantidade_botoes;
        private int botoes_linha;

        private int time = 0;
        private bool isActive = false;

        string utilizador;
        string id;
        string ModoDeJogo;
        int PrimeiroClick = 0;
        string novadificuldade;

        string tempo_final;
        int vitoria = 0;

        public Jogo()
        {
            InitializeComponent();
        }

        public Jogo(string idUser, string user, string mododejogo)
        {
            InitializeComponent();
            id = idUser;
            utilizador = user;
            ModoDeJogo = Convert.ToString(mododejogo);
        }

        public void ChangeSize(int width, int height)
        {
            this.Size = new Size(width, height);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void LayoutPanel(object sender, LayoutEventArgs e)
        {
            if (dificuldade == Dificuldade.facil)
            {
                GerarMinas(mines1);

                DesenharPainel();
            }
            else
            {
                GerarMinas(mines2);

                DesenharPainel();
            }
        }

        private void DesenharPainel()
        {
            int i;

            for (i = 0; i < quantidade_botoes; i++)
            {
                Button button = new Button();
                button.Image = default(Image);
                button.FlatStyle = FlatStyle.Standard;
                button.Size = new Size(26, 26);
                button.Text = " ";
                button.Name = string.Format("Button{0}", i);
                button.Margin = new Padding(1, 1, 1, 1);
                button.Tag = i;
                button.Anchor = AnchorStyles.None;
                button.MouseDown += new MouseEventHandler(button_Click);
                Panel.Controls.Add(button);

                if (dificuldade == Dificuldade.facil && (i + 1) % 9 == 0)
                {
                    Panel.SetFlowBreak(button, true);
                }
                else if (dificuldade == Dificuldade.medio && (i + 1) % 16 == 0)
                {
                    Panel.SetFlowBreak(button, true);
                }
            }
        }

        public bool ExisteBomba(int posicao)
        {
            int i = 0;
            bool result = false;

            if (dificuldade == Dificuldade.facil)
            {
                for (i = 0; i < 10; i++)
                {
                    if (mines1[i] == Convert.ToInt32(posicao))
                    {
                        result = true;
                        break;
                    }
                }
            }
            else
            {
                for (i = 0; i < 40; i++)
                {
                    if (mines2[i] == Convert.ToInt32(posicao))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        private void GerarMinas(int[] mines)
        {
            int i;

            int random;

            for (i = 0; i < bombas; i++)
            {
                do
                {
                    random = rnd.Next(0, quantidade_botoes - 1);

                } while (ExisteBomba(random) == true);

                if (dificuldade == Dificuldade.facil)
                {
                    mines1[i] = random;
                }
                else
                {
                    mines2[i] = random;
                }
            }
        }

        private bool TemBomba(string botao)
        {
            int i;
            string mina;
            int resultado = 0;

            if (dificuldade == Dificuldade.facil)
            {
                for (i = 0; i < mines1.Length; i++)
                {
                    mina = "Button" + mines1[i];

                    if (mina == botao)
                    {
                        resultado++;
                    }
                }
            }
            else
            {
                for (i = 0; i < mines2.Length; i++)
                {
                    mina = "Button" + mines2[i];

                    if (mina == botao)
                    {
                        resultado++;
                    }
                }
            }

            if (resultado > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int NumBotao(string a)
        {
            string b = string.Empty;
            int val = 0;

            for (int i = 0; i < a.Length; i++)
            {
                if (Char.IsDigit(a[i]))
                    b += a[i];
            }

            if (b.Length > 0)
                val = int.Parse(b);

            return val;
        }

        private int BombasVolta(string botao)
        {
            int bombas = 0;
            int num_botao = NumBotao(Convert.ToString(botao));
            string lado_direito = "Button" + (num_botao + 1);
            string lado_esquerdo = "Button" + (num_botao - 1);
            string cima = "Button" + (num_botao - botoes_linha);
            string baixo = "Button" + (num_botao + botoes_linha);
            string diagonal_esquerda_cima = "Button" + (num_botao - botoes_linha - 1);
            string diagonal_esquerda_baixo = "Button" + (num_botao + botoes_linha - 1);
            string diagonal_direita_cima = "Button" + (num_botao - botoes_linha + 1);
            string diagonal_direita_baixo = "Button" + (num_botao + botoes_linha + 1);

            if ((num_botao - (botoes_linha - 1)) % botoes_linha != 0)
            {
                if (TemBomba(lado_direito) == true)
                {
                    bombas++;
                }
            }

            if (num_botao % botoes_linha != 0 && num_botao != 0)
            {
                if (TemBomba(lado_esquerdo) == true)
                {
                    bombas++;
                }
            }


            if (num_botao >= botoes_linha)
            {
                if (TemBomba(cima) == true)
                {
                    bombas++;
                }

            }

            if (num_botao <= (quantidade_botoes - botoes_linha))
            {
                if (TemBomba(baixo) == true)
                {
                    bombas++;
                }
            }

            if ((num_botao - (botoes_linha - 1)) % botoes_linha != 0 && num_botao <= (quantidade_botoes - botoes_linha))
            {
                if (TemBomba(diagonal_direita_baixo) == true)
                {
                    bombas++;
                }
            }

            if ((num_botao - (botoes_linha - 1)) % botoes_linha != 0 && num_botao >= 9)
            {
                if (TemBomba(diagonal_direita_cima) == true)
                {
                    bombas++;
                }
            }

            if (num_botao % botoes_linha != 0 && num_botao >= botoes_linha && num_botao != 0)
            {
                if (TemBomba(diagonal_esquerda_cima) == true)
                {
                    bombas++;
                }
            }

            if (num_botao % botoes_linha != 0 && num_botao <= (quantidade_botoes - botoes_linha) && num_botao != 0)
            {
                if (TemBomba(diagonal_esquerda_baixo) == true)
                {
                    bombas++;
                }
            }

            return bombas;
        }

        private void button_Click(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            isActive = true;

            if (PrimeiroClick == 0)
            {
                if (ModoDeJogo == "online")
                {
                    PedidoNovoJogo();
                }
                PrimeiroClick++;
            }

            if (e.Button == MouseButtons.Left)
            {
                BotaoClicado = button.Name;

                if (Victoria() == true)
                {
                    tempo_final = Convert.ToString(textBoxTime.Text);
                    vitoria = 1;
                    if (ModoDeJogo == "online")
                    {
                        GuardarJogo();
                    }
                    ResetTime();
                    MessageBox.Show("Parabéns, Ganhou o jogo!", " ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (dificuldade == Dificuldade.facil)
                    {
                        FacilToolStripMenuItem_Click(null, null);
                    }
                    else
                    {
                        MedioToolStripMenuItem1_Click(null, null);
                    }
                }
                else if (TemBomba(BotaoClicado) == true && Victoria() == false)
                {
                    button.Image = Image.FromFile("bomba.jpg");
                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.FlatStyle = FlatStyle.Flat;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Size = new Size(26, 26);

                    tempo_final = Convert.ToString(textBoxTime.Text);
                    vitoria = 0;
                    if (ModoDeJogo == "online")
                    {
                        GuardarJogo();
                    }
                    ResetTime();
                    isActive = false;

                    MessageBox.Show("   Game Over!", " ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    if (dificuldade == Dificuldade.facil)
                    {
                        FacilToolStripMenuItem_Click(null, null);
                    }
                    else
                    {
                        MedioToolStripMenuItem1_Click(null, null);
                    }
                }
                else if (BombasVolta(BotaoClicado) == 1)
                {
                    button.Image = Image.FromFile("1.png");
                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.FlatStyle = FlatStyle.Flat;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Size = new Size(26, 26);
                }
                else if (BombasVolta(BotaoClicado) == 2)
                {
                    button.Image = Image.FromFile("2.png");
                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.FlatStyle = FlatStyle.Flat;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Size = new Size(26, 26);
                }
                else if (BombasVolta(BotaoClicado) == 3)
                {
                    button.Image = Image.FromFile("3.png");
                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.FlatStyle = FlatStyle.Flat;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Size = new Size(26, 26);
                }
                else if (BombasVolta(BotaoClicado) == 4)
                {
                    button.Image = Image.FromFile("4.png");
                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.FlatStyle = FlatStyle.Flat;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Size = new Size(26, 26);
                }
                else if (BombasVolta(BotaoClicado) == 5)
                {
                    button.Image = Image.FromFile("5.png");
                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.FlatStyle = FlatStyle.Flat;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Size = new Size(26, 26);
                }
                else if (BombasVolta(BotaoClicado) == 6)
                {
                    button.Image = Image.FromFile("6.png");
                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.FlatStyle = FlatStyle.Flat;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Size = new Size(26, 26);
                }
                else if (BombasVolta(BotaoClicado) == 7)
                {
                    button.Image = Image.FromFile("7.png");
                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.FlatStyle = FlatStyle.Flat;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Size = new Size(26, 26);
                }
                else if (BombasVolta(BotaoClicado) == 8)
                {
                    button.Image = Image.FromFile("8.png");
                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.FlatStyle = FlatStyle.Flat;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Size = new Size(26, 26);
                }
                else
                {
                    button.Image = Image.FromFile("0.png");
                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.FlatStyle = FlatStyle.Flat;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Size = new Size(26, 26);

                    MostrarEspacos(BotaoClicado);
                    MostrarEspacos_y(BotaoClicado);
                    //MostrarEspacos_diagonal(BotaoClicado);

                }
            }
            else if (e.Button == MouseButtons.Right && button.Image == default(Image))
            {
                BotaoClicado = button.Name;

                button.Image = Image.FromFile("bandeira.png");
                button.ImageAlign = ContentAlignment.MiddleCenter;
                button.FlatStyle = FlatStyle.Flat;
                button.BackgroundImageLayout = ImageLayout.Stretch;
                button.Size = new Size(26, 26);
            }
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Jogo_Load(object sender, EventArgs e)
        {

            if (dificuldade == Dificuldade.facil)
            {
                bombas = 10;
                botoes_linha = 9;
                quantidade_botoes = 81;
            }
            else
            {
                bombas = 40;
                botoes_linha = 16;
                quantidade_botoes = 256;
            }

            textBoxBombas.Text = "Bombas: " + Convert.ToString(bombas);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i;
            isActive = false;
            ResetTime();

            GerarMinas(mines1);

            foreach (Button button in Panel.Controls)
            {
                for (i = 0; i < quantidade_botoes; i++)
                {
                    button.Image = default(Image);
                    button.ForeColor = default(Color);
                    button.UseVisualStyleBackColor = true;
                    button.FlatStyle = FlatStyle.Standard;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Size = new Size(26, 26);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isActive == true)
            {
                time++;
            }

            if (time != 0)
            {
                textBoxTime.Text = Convert.ToString(+time);
            }
            else
            {
                textBoxTime.Text = null;
            }
        }

        private void ResetTime()
        {
            time = 0;
        }

        private void textBoxTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void MedioToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PrimeiroClick = 0;
            isActive = false;
            ResetTime();

            dificuldade = Dificuldade.medio;
            Jogo_Load(null, null);

            ChangeSize(502, 570);
            Panel.Size = new Size(450, 450);

            menuStrip1.Size = new Size(502, 24);

            Panel.Controls.Clear();
        }

        private void FacilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrimeiroClick = 0;
            isActive = false;
            ResetTime();

            dificuldade = Dificuldade.facil;
            Jogo_Load(null, null);

            ChangeSize(310, 370);
            Panel.Size = new Size(270, 253);

            menuStrip1.Size = new Size(310, 24);

            Panel.Controls.Clear();
        }

        private void perfilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetTime();

            Perfil mostrarperfil = new Perfil(utilizador, Convert.ToString(ModoDeJogo));
            mostrarperfil.Show();
        }

        private void MostrarEspacos(string botaoC)
        {
            int i = 0;
            int j;
            int g;
            int num_botao = NumBotao(Convert.ToString(botaoC));
            string button;
            var botao = "Button" + num_botao;
            int x = x_botao(botao);
            int y = y_botao(botao);

            for (j = y; j >= 0; j--)
            {
                botao = "Button" + (num_botao - 9 * (y - j));
                if (TemBomba(botao) == false && BombasVolta(botao) == 0)
                {
                    for (i = x; i >= 0; i--)
                    {
                        button = "Button" + (i + (9 * j));
                        if (TemBomba(button) == false && BombasVolta(button) == 0)
                        {
                            show(button);
                        }
                        else
                        {
                            if (TemBomba(button) == false)
                            {
                                show(button);
                            }
                            break;
                        }
                    }

                    for (i = x; i <= 8; i++)
                    {
                        button = "Button" + (i + (9 * j));
                        if (TemBomba(button) == false && BombasVolta(button) == 0)
                        {
                            show(button);
                        }
                        else
                        {
                            if (TemBomba(button) == false)
                            {
                                show(button);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            for (g = y; g <= 8; g++)
            {
                botao = "Button" + (num_botao + 9 * (g - y));
                if (TemBomba(botao) == false && BombasVolta(botao) == 0)
                {
                    for (i = x; i >= 0; i--)
                    {
                        button = "Button" + (i + (9 * g));
                        if (TemBomba(button) == false && BombasVolta(button) == 0)
                        {
                            show(button);
                        }
                        else
                        {
                            if (TemBomba(button) == false)
                            {
                                show(button);
                            }
                            break;
                        }
                    }

                    for (i = x; i <= 8; i++)
                    {
                        button = "Button" + (i + (9 * g));
                        if (TemBomba(button) == false && BombasVolta(button) == 0)
                        {
                            show(button);
                        }
                        else
                        {
                            if (TemBomba(button) == false)
                            {
                                show(button);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

        }

        private void MostrarEspacos_y(string botaoC)
        {
            int i = 0;
            int j;
            int g;
            int num_botao = NumBotao(Convert.ToString(botaoC));
            string button;
            var botao = "Button" + num_botao;
            int x = x_botao(botao);
            int y = y_botao(botao);


            for (j = x; j >= 0; j--)
            {
                botao = "Button" + (num_botao - (x - j));
                if (TemBomba(botao) == false && BombasVolta(botao) == 0)
                {
                    for (i = y; i >= 0; i--)
                    {
                        button = "Button" + (j + (9 * i));
                        if (TemBomba(button) == false && BombasVolta(button) == 0)
                        {
                            show(button);
                        }
                        else
                        {
                            if (TemBomba(button) == false)
                            {
                                show(button);
                            }
                            break;
                        }
                    }

                    for (i = y; i <= 8; i++)
                    {
                        button = "Button" + (j + (9 * i));
                        if (TemBomba(button) == false && BombasVolta(button) == 0)
                        {
                            show(button);
                        }
                        else
                        {
                            if (TemBomba(button) == false)
                            {
                                show(button);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            for (g = x; g <= 8; g++)
            {
                botao = "Button" + (num_botao + (g - x));
                if (TemBomba(botao) == false && BombasVolta(botao) == 0)
                {
                    for (i = y; i >= 0; i--)
                    {
                        button = "Button" + (g + (9 * i));
                        if (TemBomba(button) == false && BombasVolta(button) == 0)
                        {
                            show(button);
                        }
                        else
                        {
                            if (TemBomba(button) == false)
                            {
                                show(button);
                            }
                            break;
                        }
                    }

                    for (i = y; i <= 8; i++)
                    {
                        button = "Button" + (g + (9 * i));
                        if (TemBomba(button) == false && BombasVolta(button) == 0)
                        {
                            show(button);
                        }
                        else
                        {
                            if (TemBomba(button) == false)
                            {
                                show(button);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void MostrarEspacos_diagonal(string botaoC)
        {
            int i = 0;
            int j;
            int num_botao = NumBotao(Convert.ToString(botaoC));
            string button;
            var botao = "Button" + num_botao;
            int x = x_botao(botao);
            int y = y_botao(botao);

            for (i = x - 1; i >= 0; i--) //diagonal cima-esquerda
            {
                j = y - 1;

                button = "Button" + ((j * 9) + i);
                if (TemBomba(button) == false && BombasVolta(button) == 0)
                {
                    show(button);
                }
                else
                {
                    if (TemBomba(button) == false)
                    {
                        show(button);
                    }
                    break;
                }

                j--;
            }

            for (i = x + 1; i <= 8; i++) //diagonal cima-direta
            {
                j = y - 1;

                button = "Button" + ((j * 9) + i);
                if (TemBomba(button) == false && BombasVolta(button) == 0)
                {
                    show(button);
                }
                else
                {
                    if (TemBomba(button) == false)
                    {
                        show(button);
                    }
                    break;
                }

                j--;
            }

            for (i = x - 1; i >= 0; i--) //diagonal baixo-esquerda
            {
                j = y + 1;

                button = "Button" + ((j * 9) + i);
                if (TemBomba(button) == false && BombasVolta(button) == 0)
                {
                    show(button);
                }
                else
                {
                    if (TemBomba(button) == false)
                    {
                        show(button);
                    }
                    break;
                }

                j++;
            }

            for (i = x + 1; i >= 8; i++) //diagonal baixo-direita
            {
                j = y + 1;

                button = "Button" + ((j * 9) + i);
                if (TemBomba(button) == false && BombasVolta(button) == 0)
                {
                    show(button);
                }
                else
                {
                    if (TemBomba(button) == false)
                    {
                        show(button);
                    }
                    break;
                }

                j++;

            }
        }

        private int y_botao(string botao)
        {
            int y = 0;

            int num_botao = NumBotao(Convert.ToString(botao));

            if (num_botao >= 0 && num_botao <= 8)
            {
                y = 0;
            }
            else if (num_botao >= 9 && num_botao <= 17)
            {
                y = 1;
            }
            else if (num_botao >= 18 && num_botao <= 26)
            {
                y = 2;
            }
            else if (num_botao >= 27 && num_botao <= 35)
            {
                y = 3;
            }
            else if (num_botao >= 36 && num_botao <= 44)
            {
                y = 4;
            }
            else if (num_botao >= 45 && num_botao <= 53)
            {
                y = 5;
            }
            else if (num_botao >= 54 && num_botao <= 62)
            {
                y = 6;
            }
            else if (num_botao >= 63 && num_botao <= 71)
            {
                y = 7;
            }
            else if (num_botao >= 72 && num_botao <= 80)
            {
                y = 8;
            }

            return y;
        }

        private int x_botao(string botao)
        {
            int x = 0;

            int num_botao = NumBotao(Convert.ToString(botao));

            if (num_botao == 0 || num_botao % 9 == 0)
            {
                x = 0;
            }
            else if (num_botao == 1 || (num_botao - 1) % 9 == 0)
            {
                x = 1;
            }
            else if (num_botao == 2 || (num_botao - 2) % 9 == 0)
            {
                x = 2;
            }
            else if (num_botao == 3 || (num_botao - 3) % 9 == 0)
            {
                x = 3;
            }
            else if (num_botao == 4 || (num_botao - 4) % 9 == 0)
            {
                x = 4;
            }
            else if (num_botao == 5 || (num_botao - 5) % 9 == 0)
            {
                x = 5;
            }
            else if (num_botao == 6 || (num_botao - 6) % 9 == 0)
            {
                x = 6;
            }
            else if (num_botao == 7 || (num_botao - 7) % 9 == 0)
            {
                x = 7;
            }
            else if (num_botao == 8 || (num_botao - 8) % 9 == 0)
            {
                x = 8;
            }

            return x;
        }

        private void show(string botao)
        {
            foreach (Button button in Panel.Controls)
            {
                if (button.Name == Convert.ToString(botao))
                {
                    if (TemBomba(botao) == true)
                    {
                        button.Image = Image.FromFile("bomba.jpg");
                        button.ImageAlign = ContentAlignment.MiddleCenter;
                        button.FlatStyle = FlatStyle.Flat;
                        button.BackgroundImageLayout = ImageLayout.Stretch;
                        button.Size = new Size(26, 26);

                        ResetTime();
                        isActive = false;

                        MessageBox.Show("   Game Over!", " ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (BombasVolta(botao) == 1)
                    {
                        button.Image = Image.FromFile("1.png");
                        button.ImageAlign = ContentAlignment.MiddleCenter;
                        button.FlatStyle = FlatStyle.Flat;
                        button.BackgroundImageLayout = ImageLayout.Stretch;
                        button.Size = new Size(26, 26);
                    }
                    else if (BombasVolta(botao) == 2)
                    {
                        button.Image = Image.FromFile("2.png");
                        button.ImageAlign = ContentAlignment.MiddleCenter;
                        button.FlatStyle = FlatStyle.Flat;
                        button.BackgroundImageLayout = ImageLayout.Stretch;
                        button.Size = new Size(26, 26);
                    }
                    else if (BombasVolta(botao) == 3)
                    {
                        button.Image = Image.FromFile("3.png");
                        button.ImageAlign = ContentAlignment.MiddleCenter;
                        button.FlatStyle = FlatStyle.Flat;
                        button.BackgroundImageLayout = ImageLayout.Stretch;
                        button.Size = new Size(26, 26);
                    }
                    else if (BombasVolta(botao) == 4)
                    {
                        button.Image = Image.FromFile("4.png");
                        button.ImageAlign = ContentAlignment.MiddleCenter;
                        button.FlatStyle = FlatStyle.Flat;
                        button.BackgroundImageLayout = ImageLayout.Stretch;
                        button.Size = new Size(26, 26);
                    }
                    else if (BombasVolta(botao) == 5)
                    {
                        button.Image = Image.FromFile("5.png");
                        button.ImageAlign = ContentAlignment.MiddleCenter;
                        button.FlatStyle = FlatStyle.Flat;
                        button.BackgroundImageLayout = ImageLayout.Stretch;
                        button.Size = new Size(26, 26);
                    }
                    else if (BombasVolta(botao) == 6)
                    {
                        button.Image = Image.FromFile("6.png");
                        button.ImageAlign = ContentAlignment.MiddleCenter;
                        button.FlatStyle = FlatStyle.Flat;
                        button.BackgroundImageLayout = ImageLayout.Stretch;
                        button.Size = new Size(26, 26);
                    }
                    else if (BombasVolta(botao) == 7)
                    {
                        button.Image = Image.FromFile("7.png");
                        button.ImageAlign = ContentAlignment.MiddleCenter;
                        button.FlatStyle = FlatStyle.Flat;
                        button.BackgroundImageLayout = ImageLayout.Stretch;
                        button.Size = new Size(26, 26);
                    }
                    else if (BombasVolta(botao) == 8)
                    {
                        button.Image = Image.FromFile("8.png");
                        button.ImageAlign = ContentAlignment.MiddleCenter;
                        button.FlatStyle = FlatStyle.Flat;
                        button.BackgroundImageLayout = ImageLayout.Stretch;
                        button.Size = new Size(26, 26);
                    }
                    else
                    {
                        button.Image = Image.FromFile("0.png");
                        button.ImageAlign = ContentAlignment.MiddleCenter;
                        button.FlatStyle = FlatStyle.Flat;
                        button.BackgroundImageLayout = ImageLayout.Stretch;
                        button.Size = new Size(26, 26);
                    }
                    break;
                }
                else
                {

                }
            }
        }

        private void top10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Top10 top = new Top10(ModoDeJogo);
            top.Show();
        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private bool Victoria()
        {
            int i = 0;

            foreach (Button button in Panel.Controls)
            {
                if (button.Image != null || TemBomba(button.Name) == false)
                {
                    i++;
                }
            }

            if (i == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void PedidoNovoJogo()
        {

            if (dificuldade == Dificuldade.facil)
            {
                novadificuldade = "Facil";
            }
            else
            {
                novadificuldade = "Medio";
            }

            //Prepara o pedido ao servidor com o URL adequado
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://prateleira.utad.priv:1234/LPDSW/2019-2020/novo/" + novadificuldade + "/" + id); // ou outro qualquer username

            // Com o acesso usa HTTPS e o servidor usar cerificados autoassinados, tempos de configurar o cliente para aceitar sempre o certificado.
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            request.Method = "GET"; // método usado para enviar o pedido
            HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // faz o envio do pedido

            Stream receiveStream = response.GetResponseStream(); // obtem o stream associado à resposta.
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8); // Canaliza o stream para um leitor de stream de nível superior com o formato de codificação necessário.
            string resultado = readStream.ReadToEnd();

            response.Close();
            readStream.Close();

            // converte para objeto XML para facilitar a extração da informação e ...
            XDocument xmlResposta = XDocument.Parse(resultado);
            // ...interpretar o resultado de acordo com a lógica da aplicação (exemplificativo)
            if (xmlResposta.Element("resultado").Element("status").Value == "ERRO")
            {
                // apresenta mensagem de erro usando o texto (contexto) da resposta
                MessageBox.Show(xmlResposta.Element("resultado").Element("contexto").Value, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FacilToolStripMenuItem_Click(null, null);
            }
            else
            {

            }
        }

        private void GuardarJogo()
        {
            //Prepara o pedido ao servidor com o URL adequado
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://prateleira.utad.priv:1234/LPDSW/2019-2020/resultado/" + id);

            // Com o acesso usa HTTPS e o servidor usar cerificados autoassinados, temos de configurar o cliente para aceitar sempre o certificado.
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            // prepara os dados do pedido a partir de uma string só com a estrutura do XML (sem dados)
            XDocument xmlPedido = XDocument.Parse("<resultado_jogo><nivel></nivel><tempo></tempo><vitoria></vitoria></resultado_jogo>");
            //preenche os dados no XML
            xmlPedido.Element("resultado_jogo").Element("nivel").Value = Convert.ToString(novadificuldade);
            xmlPedido.Element("resultado_jogo").Element("tempo").Value = Convert.ToString(tempo_final);

            string boolvitoria;

            if (vitoria == 0)
            {
                boolvitoria = "false";
            }
            else
            {
                boolvitoria = "true";
            }

            xmlPedido.Element("resultado_jogo").Element("vitoria").Value = Convert.ToString(boolvitoria);

            string mensagem = xmlPedido.Root.ToString();

            byte[] data = Encoding.Default.GetBytes(mensagem); // note: choose appropriate encoding
            request.Method = "POST";// método usado para enviar o pedido
            request.ContentType = "application/xml"; // tipo de dados que é enviado com o pedido
            request.ContentLength = data.Length; // comprimento dos dados enviado no pedido

            Stream newStream = request.GetRequestStream(); // obtem a referência do stream associado ao pedido...
            newStream.Write(data, 0, data.Length);// ... que permite escrever os dados a ser enviados ao servidor
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // faz o envio do pedido

            Stream receiveStream = response.GetResponseStream(); // obtem o stream associado à resposta.
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8); // Canaliza o stream para um leitor de stream de nível superior com o
                                                                                      //formato de codificação necessário.    
            string resultado = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
            // converte para objeto XML para facilitar a extração da informação e ...
            XDocument xmlResposta = XDocument.Parse(resultado);
            // ...interpretar o resultado de acordo com a lógica da aplicação (exemplificativo)
            if (xmlResposta.Element("resultado").Element("status").Value == "ERRO")
            {
                // apresenta mensagem de erro usando o texto (contexto) da resposta
                MessageBox.Show(xmlResposta.Element("resultado").Element("contexto").Value, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // assume a autenticação e obtem o ID do resultado...para ser usado noutros pedidos               

            }
        }

        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;

        }
    }
}