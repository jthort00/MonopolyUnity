using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class LogicScript1 : MonoBehaviour
{
    public Text outcome;
    public Socket server;
    public GameObject Intro_screen;
    public GameObject Login_screen;
    public GameObject Register_screen;
    public InputField username;
    public InputField password;
    public Text loginerror;
    static readonly object lockObject = new object();
    string returnData = "";
    bool newData = false;


    void Update()
    {
        if (newData)
        {
            /*lock object to make sure there data is 
             *not being accessed from multiple threads at thesame time*/
            lock (lockObject)
            {
                newData = false;
                string[] trozos = returnData.Split('?');
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje = trozos[1].Split('\0')[0];
                //Process received data
                Debug.Log("Received: " + returnData);
                switch (codigo)
                {
                    //case 1: // Respuesta a registrarse 
                    //    mensaje = trozos[1];
                    //    if (mensaje == "-2")
                    //        MessageBox.Show("User already exists");
                    //    if (mensaje == "-1")
                    //        MessageBox.Show("Error!");
                    //    if (mensaje == "0")
                    //    {
                    //        MessageBox.Show("Successful registration");

                    //    }
                    //    break;

                    case 2: // Respuesta a iniciar sesión 
                        if (mensaje == "0")
                        {
                            loginerror.text = "Username or password are wrong";
                        }
                        if (mensaje == "1")
                        {
                            loginerror.text = "Muy bien, hagamos una fiesta";
                        }
                        break;
                }

                //Reset it for next read
                returnData = "";
            }
        }
    }

    public void serverConnectionInit()
    {
        //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
        //al que deseamos conectarnos
        IPAddress direc = IPAddress.Parse("192.168.56.102");
        IPEndPoint ipep = new IPEndPoint(direc, 7064);


        //Creamos el socket 
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            server.Connect(ipep);//Intentamos conectar el socket
            Intro_screen.gameObject.SetActive(false);
            Login_screen.gameObject.SetActive(true);
            Thread listening = new Thread(listenToServer);
            listening.Start();
        }
        catch (SocketException)
        {
            //Si hay excepcion imprimimos error y salimos del programa con return 
            outcome.text = "Connection error";
            return;
        }
    }

    public void userLogin()
    {
        if (username.text != "" && password.text != "")
        {
            string mensaje = "2/" + username.text + "$" + password.text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        else
        {
            loginerror.text = "Introduce username and password";   
        }
    }



   public void listenToServer()
    {   
        while (true)
        { 
            //Recibimos mensaje del servidor
            byte[] msg2 = new byte[500];
            server.Receive(msg2);
            Debug.Log("Received a message");

            lock (lockObject)
            {
                returnData = Encoding.ASCII.GetString(msg2);
                newData = true;
            }
        }
    }

    public void showRegistrationScreen()
    {
        Login_screen.SetActive(false);
        Register_screen.SetActive(true);
    }

}
