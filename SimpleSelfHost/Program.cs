using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SimpleSelfHost
{
    class Program
    {

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        static IntPtr handle;
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static bool _estadoJanela;
        public static bool _execucao;


        [STAThread]
        static void Main(string[] args)
        {
            string baseAddress = "http://127.0.0.1:8080/";

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                //var endpoint = new Url("");


                Console.WriteLine($"Service Listening at {baseAddress}\n\n");

                Console.WriteLine($"É Possivel verificar o modelo JSON em: {baseAddress}api/impressora/modelojson");
                //Console.WriteLine($"Use o POST em {baseAddress}api/demo/\n" + "Enviando Um JSON com {\"Impressora\":\"impressora desejada\",\"conteudo\":\"string para impressao\"");
                Console.WriteLine("\n\n\n");

                var impressoras = CarregarImpressoras();
                Console.WriteLine("Impressoras disponíveis:\n");
                foreach (var a in impressoras)
                {
                    Console.WriteLine(a);
                }

                handle = GetConsoleWindow();

                _estadoJanela = HideConsole();

                Form1.Main2();

            }


        }


        public static bool ShowConsole()
        {
            return ShowWindow(handle, SW_SHOW);
        }
        public static bool HideConsole()
        {
            return ShowWindow(handle, SW_HIDE);
        }

        public static PrinterSettings.StringCollection CarregarImpressoras()
        {
            return PrinterSettings.InstalledPrinters;

        }
    }

    //todo: Mudar o icone de acordo com os problemas encontrados
    //habilitar opçoes no icone

    public class Form1 : System.Windows.Forms.Form
    {

        private bool allowVisible;
        private bool allowClose;
        protected override void SetVisibleCore(bool value)
        {
            if (!allowVisible)
            {
                value = false;
                if (!this.IsHandleCreated) CreateHandle();
            }
            base.SetVisibleCore(value);
        }

        protected override void OnFormClosing(System.Windows.Forms.FormClosingEventArgs e)
        {
            if (!allowClose)
            {
                this.Hide();
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }

        private System.Windows.Forms.NotifyIcon icone;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem MenuItemSair;
        private System.Windows.Forms.MenuItem MenuItemVisibilidadeConsole;
        private System.ComponentModel.IContainer components;


        public static void Main2()
        {
            var a = new Form1();
            a.SetVisibleCore(false);

            System.Windows.Forms.Application.Run(a);

            return;
        }

        public Form1()
        {


            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.MenuItemSair = new System.Windows.Forms.MenuItem();
            this.MenuItemVisibilidadeConsole = new System.Windows.Forms.MenuItem();


            // Inicializando o Menu no icone
            this.contextMenu.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { this.MenuItemSair, this.MenuItemVisibilidadeConsole });


            //area responsável pelas funçãoes que vão aparecer no menu de itens.

            this.MenuItemSair.Index = 0;
            this.MenuItemSair.Text = "Sair da Aplicação";
            this.MenuItemSair.Click += new System.EventHandler(this.SairDaAplicação);


            this.MenuItemVisibilidadeConsole.Index = 1;
            this.MenuItemVisibilidadeConsole.Text = "Mostrar ou Ocultar Console";
            this.MenuItemVisibilidadeConsole.Click += new System.EventHandler(this.VisibilidadeConsole);



            // Onde se Inicia o icone.
            this.icone = new System.Windows.Forms.NotifyIcon(this.components);


            //responsável pela imagem do ícone.
            icone.Icon = new Icon("printer.ico");



            // Seta o Icone como o Contexto do Menu.
            icone.ContextMenu = this.contextMenu;



            // Legenda da Aplicação .
            icone.Text = "SelfHost IMPRESSORAS";
            icone.Visible = true;


            // Lida com o evento responsável pelo doubleClick no Icone.
            icone.DoubleClick += new System.EventHandler(this.IconeDuploClick);
        }






        //area de metodos que serão usados nos Itens do Menu.
        private void IconeDuploClick(object Sender, EventArgs e)
        {

            VisibilidadeConsole(Sender, e);

        }

        private void SairDaAplicação(object Sender, EventArgs e)
        {
            this.Close();

            System.Windows.Forms.Application.Exit();

        }

        private void VisibilidadeConsole(object Sender, EventArgs e)
        {


            if (Program._estadoJanela)
            {
                icone.Icon = new Icon("printer.ico");
                Program._estadoJanela = Program.ShowConsole();
            }
            else
            {
                icone.Icon = new Icon("printerHidden.ico");
                Program._estadoJanela = Program.HideConsole();
            }

        }


    }
}
