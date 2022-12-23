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
    public GameObject Main_user_screen;
    public GameObject row_stuff;
    public text_button_script text_button;
    public InputField username;
    public InputField password;
    public InputField newUsername;
    public InputField newPassword;
    public InputField confPassword;
    public InputField age;
    public Text loginerror;
    public Text registrationerror;
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
                    case 1: // Respuesta a registrarse 
                        if (mensaje == "-2")
                            registrationerror.text = "User already exists";
                        if (mensaje == "-1")
                            registrationerror.text = "Error!";
                        if (mensaje == "0")
                        {
                            registrationerror.text = "Successful registration";
                            Thread.Sleep(1000);
                            Register_screen.SetActive(false);
                            Login_screen.SetActive(true);
                        }
                        break;

                    case 2: // Respuesta a iniciar sesión 
                        if (mensaje == "0")
                        {
                            loginerror.text = "Username or password are wrong";
                        }
                        if (mensaje == "1")
                        {
                            Main_user_screen.SetActive(true);
                            Login_screen.SetActive(false);
                            mensaje = "4/" + username.text;
                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                            server.Send(msg);
                            
                        }
                        break;

                    case 4: // Conseguir todas las partidas en las que está el usuario 
                        if (mensaje != "-1")
                        {
                            string[] parts = mensaje.Split('$');
                            int i = 0;
                            while (i+1 < parts.Length)
                            {
                                string[] parts1 = parts[i].Split('/');
                                if (parts1[3] == "0")
                                    parts1[3] = "No";

                                row_stuff.GetComponent<populate_grid_script>().Populate(parts1[0]);
                                text_button = GameObject.FindGameObjectsWithTag("row_comp")[GameObject.FindGameObjectsWithTag("row_comp").Length - 1].GetComponent<text_button_script>();
                                text_button.clickable = 1;
                                row_stuff.GetComponent<populate_grid_script>().Populate(parts1[1]);
                                row_stuff.GetComponent<populate_grid_script>().Populate(parts1[2]);
                                row_stuff.GetComponent<populate_grid_script>().Populate(parts1[4]);
                                row_stuff.GetComponent<populate_grid_script>().Populate(parts1[3]);
                                Debug.Log("Added row");
                                i = i + 1;

                            }
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

    public void userRegistration()
    {
        if (confPassword.text == newPassword.text)
        {
            Boolean caracteres = newPassword.text.Contains("$");
            Boolean caracteres2 = newPassword.text.Contains("/");
            Boolean caracteres3 = newUsername.text.Contains("$");
            Boolean caracteres4 = newUsername.text.Contains("/");
            if (caracteres == false && caracteres2 == false && caracteres3 == false && caracteres4 == false)
            {
                string mensaje = "1/" + newUsername.text + "$" + newPassword.text + "$" + age.text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else
                registrationerror.text = "La contraseña no puede contener los símbolos /, $";
        }
        else
        {
            registrationerror.text = "La contraseña no coincide";
        }
    }

    public void GetGame(string gameID)
    {
        Debug.Log("you've clicked at " + gameID);
    }

  
}
