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
    public GameObject Online_list_screen;
    public GameObject Lobby_screen;
    public GameObject Invitation_screen;
    public GameObject Main_game_screen;

    public GameObject row_stuff;
    public GameObject online_list_row;
    public GameObject ingame_list_row;
    public text_button_script text_button;

    public InputField username;
    public InputField password;
    public InputField newUsername;
    public InputField newPassword;
    public InputField confPassword;
    public InputField age;

    public Text loginerror;
    public Text registrationerror;
    public Text inviteerror;
    public Text gamelobbytitle;
    public Text invitationtext;
 

    public Button launchgame_button;
    public Button rolldice_button;
    public Button nexturn_button;

    private int current_gameid;
    private int invitation_gameid;
    private int host = 0;
    private int launched = 0; 
    private string invitation_player;

    static readonly object lockObject = new object();
    string returnData = "";
    bool newData = false;
    Thread listening;

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

                    case 3: // Respuesta a crear partida 
                        string[] parts = mensaje.Split('/');
                        if (parts[3] == "0")
                            parts[3] = "No";
                        row_stuff.GetComponent<populate_grid_script>().Populate(parts[0],1);
                        row_stuff.GetComponent<populate_grid_script>().Populate(parts[1],0);
                        row_stuff.GetComponent<populate_grid_script>().Populate(parts[2],0);
                        row_stuff.GetComponent<populate_grid_script>().Populate(parts[4],0);
                        row_stuff.GetComponent<populate_grid_script>().Populate(parts[3],0);
                        current_gameid = Convert.ToInt32(parts[0]);
                        gamelobbytitle.text = "Game " + current_gameid + " lobby";
                        host = 1; 
                        break;

                    case 4: // Conseguir todas las partidas en las que está el usuario 
                        if (mensaje != "-1")
                        {
                            parts = mensaje.Split('$');
                            int i = 0;
                            while (i+1 < parts.Length)
                            {
                                string[] parts1 = parts[i].Split('/');
                                if (parts1[3] == "0")
                                    parts1[3] = "No";

                                row_stuff.GetComponent<populate_grid_script>().Populate(parts1[0],1);
                                row_stuff.GetComponent<populate_grid_script>().Populate(parts1[1],0);
                                row_stuff.GetComponent<populate_grid_script>().Populate(parts1[2],0);
                                row_stuff.GetComponent<populate_grid_script>().Populate(parts1[4],0);
                                row_stuff.GetComponent<populate_grid_script>().Populate(parts1[3],0);
                                i = i + 1;

                            }
                        }
                        break;

                    case 5: // Lista de conectados actualizada
                        parts = mensaje.Split('/');
                        if (mensaje != "0/null")
                        {
                            online_list_row.GetComponent<populate_online_grid_script>().Delete();
                            int i = 1;
                            while (i < parts.Length)
                            {
                                online_list_row.GetComponent<populate_online_grid_script>().Populate(parts[i],2);
                                i = i + 1;
                            }

                        }
                        break;

                    case 7: // Notificación de que un jugador te ha invitado 
                        parts = mensaje.Split('/');
                        invitation_player = parts[0];
                        invitation_gameid = Convert.ToInt32(parts[1]);
                        Main_user_screen.SetActive(false);
                        Invitation_screen.SetActive(true);
                        invitationtext.text = invitation_player + " has invited you to a game";
                        break;

                    case 8: // Respuesta a una invitación. Se recibe una lista actualizada de los jugadores en la partida 
                        parts = mensaje.Split('/');
                        Debug.Log("He recibido respuesta a la invitación " + mensaje);
                        ingame_list_row.GetComponent<populate_ingame_list_script>().Delete();
                        if (parts[0] == "1")
                        {
                            inviteerror.text = parts[1] + " has joined the game";
                            inviteerror.color = Color.green;
                            int i = 1;
                            while (i < parts.Length)
                            {
                                ingame_list_row.GetComponent<populate_ingame_list_script>().Populate(parts[i], 0);
                                i = i + 1;
                            }
                        }

                        if (parts[0] == "0")
                        {
                            inviteerror.text = parts[1] + " has rejected the invitation";
                            inviteerror.color = Color.red;
                        }
                        break;

                    case 10: //Es tu turno

                        rolldice_button.gameObject.SetActive(true);
                        break;

                    case 11: //Ha empezado la partida 
                        Main_game_screen.SetActive(true);
                        Lobby_screen.SetActive(false);
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
            listening = new Thread(listenToServer);
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
        Debug.Log("you've clicked at " + gameID + "work in progress");
    }

    public void CreateGame()
    {
        string mensaje = "3/" + username.text;
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);
        Lobby_screen.SetActive(true);
        Main_user_screen.SetActive(false);
        Online_list_screen.SetActive(true);
    }

    public void LogOut()
    {
        string mensaje = "6/" + username.text;
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);
        row_stuff.GetComponent<populate_grid_script>().Delete();
        Main_user_screen.SetActive(false);
        Login_screen.SetActive(true);
        Register_screen.SetActive(false);
        Online_list_screen.SetActive(false);
    }

    public void Disconnect()
    {
        string mensaje = "0/" + "Bye";
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);
        listening.Abort();
        server.Shutdown(SocketShutdown.Both);
        server.Close();
        Login_screen.SetActive(false);
        Intro_screen.SetActive(true);

    }

    public void ShowOnlineList()
    {
        if (Online_list_screen.activeSelf == true)
            Online_list_screen.SetActive(false);
        else
            Online_list_screen.SetActive(true);
    }

    public void InviteUser(string username)
    {
        if (Lobby_screen.activeSelf==true && host == 1 && launched ==0)
        {
            if (string.Equals(this.username.text, username) == false)
            {
                string mensaje = "7/" + username + "/" + username + "/" + current_gameid;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else
                inviteerror.text = "You can't invite yourself";
        }
        
    }

    public void AcceptInvitation()
    {
        string mensaje = "8/" + invitation_player + "/" + username.text + "/" + invitation_gameid + "/1";
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);
        Invitation_screen.SetActive(false);
        Lobby_screen.SetActive(true);
        current_gameid = invitation_gameid;
        gamelobbytitle.text = "Game " + current_gameid + " lobby";
        launchgame_button.gameObject.SetActive(false);
    }

    public void RejectInvitation()
    {
        string mensaje = "8/" + invitation_player + "/" + username + "/" + invitation_gameid + "/0";
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);
        Invitation_screen.SetActive(false);
    }

    public void LaunchGame()
    {
        launched = 1;
        Main_game_screen.SetActive(true);
        Lobby_screen.SetActive(false);

        string mensaje = "11/" + current_gameid;
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);
    }

    public void ShowLobbyScreen()
    {
        if (Lobby_screen.activeSelf == true)
            Lobby_screen.SetActive(false);
        else
            Lobby_screen.SetActive(true);
    }

    public void RollDice()
    {
        //funció roll dice aquí
        nexturn_button.gameObject.SetActive(true);
    }

    public void NextTurn()
    {
        rolldice_button.gameObject.SetActive(false);
        nexturn_button.gameObject.SetActive(false);
        string mensaje = "10/" + username.text + "/" + current_gameid;
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);
    }

    //Associar cada gameid a una peça en la partida. IMPORTANT!
}
