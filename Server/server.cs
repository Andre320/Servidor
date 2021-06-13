using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace servidor
{
    class Servidor
    {
        private TcpListener servidor;
        private TcpClient cliente = new TcpClient();
        private List<Cliente> clientesConectados = new List<Cliente>();
        Cliente nuevoCliente = new();
        public string ip;
        public string puerto;

        private struct Cliente
        {
            public NetworkStream stream;
            public StreamReader reader;
            public StreamWriter writter;


            public string color;
            public string nombre;
            public string data;
        }
        public void Conexion()
        {
            IPEndPoint IPconnection = new IPEndPoint(IPAddress.Parse(ip), int.Parse(puerto));
            Console.WriteLine("Inciando servidor...");
            servidor = new TcpListener(IPconnection);
            servidor.Start();

            while (true)
            {
                cliente = servidor.AcceptTcpClient();
                nuevoCliente.stream = cliente.GetStream();
                nuevoCliente.reader = new StreamReader(nuevoCliente.stream);
                nuevoCliente.writter = new StreamWriter(nuevoCliente.stream);
                nuevoCliente.color = nuevoCliente.reader.ReadLine();
                nuevoCliente.nombre = nuevoCliente.reader.ReadLine();
                nuevoCliente.data = nuevoCliente.reader.ReadLine();

                // Añadir cliente a lista para saber que esta conectado
                clientesConectados.Add(nuevoCliente);

                Console.WriteLine(nuevoCliente.nombre + " se ha conectado, con el color "+ nuevoCliente.color);

                // Se inicializa un hilo con el cliente
                Thread thread = new Thread(EscucharConexion);
               
                thread.Start();
               

            }
        }

        public void EscucharConexion()
        {
            Cliente copiaCliente = nuevoCliente;
            do
            {
                Console.WriteLine("do");
                try
                {
                    Console.WriteLine("1 try");
                    string tmp = copiaCliente.reader.ReadLine();
                    Console.WriteLine("mensaje enviado por el cliente " + copiaCliente.nombre + ": " + tmp);

                    try
                    {
                        for (int i= 0; i< clientesConectados.Count; i++)
                        {
                           clientesConectados[i].writter.WriteLine(tmp);
                           clientesConectados[i].writter.Flush();
                        }
                    }
                    catch{
                        Console.WriteLine("error en el cliente");
                    }

                }
                catch
                {
                    clientesConectados.Remove(nuevoCliente);
                    Console.WriteLine(nuevoCliente.nombre + " se ha desconectado");
                    break;
                }
            } while (true);
        }

        static void Main(string[] args)
        {
            Servidor servidor = new Servidor();
            Console.WriteLine("Escriba el IP");
            servidor.ip = "25.107.59.124";
            Console.WriteLine("Escriba el puerto");
            servidor.puerto = "46000";
            servidor.Conexion();
        }
    }
}
