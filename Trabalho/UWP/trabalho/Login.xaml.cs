﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using Windows.Web.Http.Filters;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;





// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace login
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        public string Utilizador;
        public enum ModoDeJogo { online, offline }
        public ModoDeJogo modoDeJogo = ModoDeJogo.offline;

        string id;


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }


        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;

        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Frame view = new Frame();
            this.Frame.Navigate(typeof(increver.MainPage));
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            modoDeJogo = ModoDeJogo.offline;

            Frame view = new Frame();
            this.Frame.Navigate(typeof(jogo.MainPage));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //Frame view = new Frame();
            //this.Frame.Navigate(typeof(jogo.MainPage));

            //Prepara o pedido ao servidor com o URL adequado
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://prateleira.utad.priv:1234/LPDSW/2019-2020/Autentica");

            // Com o acesso usa HTTPS e o servidor usar cerificados autoassinados, temos de configurar o cliente para aceitar sempre o certificado.
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            // prepara os dados do pedido a partir de uma string só com a estrutura do XML (sem dados)
            XDocument xmlPedido = XDocument.Parse("<credenciais><username></username><password></password></credenciais>");
            //preenche os dados no XML
            xmlPedido.Element("credenciais").Element("username").Value = UserTextBox.Text; // colocar aqui o username do utilizador
            xmlPedido.Element("credenciais").Element("password").Value = PassTextBox.Password; // colocar aqui a palavra passe do utilizador

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

                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Erro",
                    Content = xmlResposta.Element("resultado").Element("contexto").Value,
                    PrimaryButtonText = "OK"
                };

                await dialog.ShowAsync();

            }
            else
            {
                // assume a autenticação e obtem o ID do resultado...para ser usado noutros pedidos
                id = Convert.ToString(xmlResposta.Element("resultado").Element("objeto").Element("ID").Value);
                modoDeJogo = ModoDeJogo.online;

                Utilizador = Convert.ToString(UserTextBox.Text);

                Frame.Navigate(typeof(jogo.MainPage));
            }
        }
    }
}
