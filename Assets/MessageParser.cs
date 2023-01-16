using System;
//using LoginUI;
using System.Threading.Tasks;
using UnityEngine;
using NoGameFoundClient;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MessageParser : MonoBehaviour
{
/*
    public Animator infoLogAnimator;
    public Text infoLogText;

    private ServerConnection serverConnection;

    private LoginLogic loginLogic;
    private InformationLogic informationLogic;
    private RegisterLogin registerLogin;
    private MainMenuLogic mainMenuLogic;
    private GameCreationLogic gameCreationLogic;
    private InvitationLogic invitationLogic;
    private JoinAGameLogic joinAGameLogic;
    private ScoreBoardLogic scoreBoardLogic;
    private LoginUI.PersonalBoardLogic personalBoardLogic;



    bool invitationError = false;
    
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

   
    //Start is called before the first frame update
    //It gets all the unity objects needed on this script, makes the infoLog invisible
    //Get an instance of the server connection
    //set the button events
    //Executes the function to connect to the server for the first time
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loading Message_Parser");

        GameObject infoLogObject = GameObject.Find("InfoLog");
        infoLogObject.GetComponent<Canvas>().enabled = false;
        infoLogAnimator = infoLogObject.GetComponent<Animator>();
        

        GameObject loginWindow = GameObject.Find("LoginWindow");
        loginLogic = loginWindow.GetComponent<LoginLogic>();
        GameObject informationWindow = GameObject.Find("InformationWindow");
        informationLogic = informationWindow.GetComponent<InformationLogic>();
        GameObject registerWindow = GameObject.Find("RegisterWindow");
        registerLogin = registerWindow.GetComponent<RegisterLogin>();
        GameObject mainMenuWindow = GameObject.Find("MainMenuWindow");
        mainMenuLogic = mainMenuWindow.GetComponent<MainMenuLogic>();
        GameObject gameCreationWindow = GameObject.Find("GameCreationWindow");
        gameCreationLogic = gameCreationWindow.GetComponent<GameCreationLogic>();
        GameObject invitationWindow = GameObject.Find("InvitationWindow");
        invitationLogic= invitationWindow.GetComponent<InvitationLogic>();
        GameObject joinAGameWindow = GameObject.Find("JoinAGameWindow");
        joinAGameLogic= joinAGameWindow.GetComponent<JoinAGameLogic>();
        GameObject scoreboardWindow = GameObject.Find("ScoreboardWindow");
        scoreBoardLogic = scoreboardWindow.GetComponent<ScoreBoardLogic>();
        GameObject personalBoardWindow = GameObject.Find("PersonalBoardWindow");
        personalBoardLogic = personalBoardWindow.GetComponent<LoginUI.PersonalBoardLogic>();
        connectToServer();
    }

    //Asynchronous function that runs until we join a game.
    //It listens and parses all the messages sent by the server.
    //Sometimes messages come concatenated so we split them by ~
    async private void listenForServer()
    {
        bool run = true;
        while (run)
        {
            string serverResponse = await Task.Run(() => serverConnection.ListenForMessage());


            string[] commands = serverResponse.Split('~');

            int i = 0;
            while (commands[i] != "")
            {
                int prefix = int.Parse(commands[i].Split('/')[0]);
               Debug.Log(serverResponse);
                switch (prefix)
                {
                    //login accepted / denied
                    case 1:
                        if (commands[i] == "1/0")
                        {
                            globalData.userName = loginLogic.username.text;
                            if (loginLogic.credentialsError)
                            {
                                loginLogic.errorAnimator.SetBool("open", false);
                            }

                            mainMenuLogic.mainMenuCanvas.enabled = true;
                            loginLogic.loginCanvas.enabled = false;
                        }
                        else
                        {
                            loginLogic.credentialsError = true;
                            loginLogic.errorAnimator.SetBool("open", true);
                            loginLogic.error.text = "Incorrect user or password";
                        }
                        break;
                   
                    //register
                    case 2:
                        if (commands[i].Equals("2/0"))
                        {
                            registerLogin.registerWindow.enabled = false;
                            loginLogic.loginCanvas.enabled = true;
                            if (registerLogin.userError) loginLogic.errorAnimator.SetBool("open", false);

                        }

                        else
                        {
                            loginLogic.errorAnimator.SetBool("open", true);

                            loginLogic.error.text = "Register Error: User already exists";
                            Debug.Log("Register Error: User already exists");
                            registerLogin.userError = true;
                        }
                        break;
                    
                    //gets the age
                    case 3:
                        informationLogic.AgeText.text = commands[i].Replace("3/", "");
                        break;
                    
                    //gets the email
                    case 4:
                        informationLogic.EmailText.text = commands[i].Replace("4/", "");
                        break;
                    
                    //confirmation that spam has been changed
                    case 5:
                        if (commands[i] == "5/0")
                        {
                            Debug.Log("Change spam successful!");
                            if (informationLogic.canvasError) loginLogic.errorAnimator.SetBool("open", false);
                        }
                        else
                        {
                            loginLogic.errorAnimator.SetBool("open", true);

                            loginLogic.error.text = "Change spam error";
                            Debug.Log("Change spam error");

                        }
                        break;
                    
                    //spam getter
                    case 6:
                        informationLogic.spam.isOn = commands[i].Equals("6/1");
                        break;
                    
                    //list of connected users, calls the functions to update the displayed users
                    case 7:
                        informationLogic.connectedUsersNotificationListen(commands[i]);
                        gameCreationLogic.connectedUsersNotificationListen(commands[i]);
                        break;
                    
                    //gets the number of game that the user just created
                    case 8:
                        gameCreationLogic.gameNumber = int.Parse(commands[i].Split('/')[1]);
                        break;
                    
                    //confirmation that an invitation has been sent 
                    case 9:
                        if(commands[i].Equals("9/0"))
                        {
                            if(invitationError)
                            {
                                loginLogic.errorAnimator.SetBool("open",false);
                            }
                            infoLogAnimator.SetBool("open", true);
                            infoLogText.text = "Invitation sent!";
                            this.closeInfoLog();
                        }
                        else
                        {
                            loginLogic.errorAnimator.SetBool("open", true);
                            loginLogic.error.text = "Error sending invitation";
                            invitationError = true;
                        }
                        
                        break;
                    
                    //reciving an invitation, popping up the dialog with the information updated. Set the game number to the class
                    case 10:
                        
                        string user = commands[i].Split('/')[1].Split(',')[0];
                        int gameNumber = int.Parse(commands[i].Split('/')[1].Split(',')[1]);
                        invitationLogic.gameNumber = gameNumber;
                        gameCreationLogic.gameNumber = gameNumber;
                        Debug.Log("gamenumber: ");
                        Debug.Log(invitationLogic.gameNumber);
                        
                        invitationLogic.message.text = user + " has invited you to a game!";
                        invitationLogic.animator.SetBool("open", !invitationLogic.animator.GetBool("open"));
						
                        break;
                    
                    //message containing all the users of a game. Updates the displayed lists and the gameUserList.
                    case 11:
                        gameCreationLogic.gamePlayersNotificationListen(commands[i]);
                        
                        globalData.gameUserList = commands[i].Replace("11/", "").Split(',');
                        break;
                    
                    //message to start a game. Depending on the parameter we load a level or another
                    case 12:
                        if (invitationLogic.gameNumber != -1)
                        {
                            serverConnection.SendMessage("13/");
                            string[] values = commands[i].Split(',');
                            int level = int.Parse(values[1]);
                            globalData.level = level;
                            globalData.ability = int.Parse(values[2]);
                            switch (level)
                             {
                                case 1:
                                    SceneManager.LoadScene("GameScenePython", LoadSceneMode.Single);
                                    run = false;
                                    break;
                                case 2:
                                    SceneManager.LoadScene("GameSceneMatlab", LoadSceneMode.Single);
                                    run = false;
                                    break;

                            }
                         
                        }
                        break;
                    //receives list adll all started games
                    case 13:
                        joinAGameLogic.gamesNotificationUpdate(commands[i]);
                        break;
                    
                    //confirmation of joining a game, change the join game UI to create game UI
                    case 14:
                        if (commands[i].Equals("14/0"))
                        {
                            gameCreationLogic.gameCreationCanvas.enabled = true;
                            joinAGameLogic.mainCanvas.enabled = false;
                        }
                        else
                        {
                            Debug.Log("Error entering the game");
                        }
                        break;
                    
                    //confirmation of exiting a game
                    case 15:
                        if (commands[i].Equals("15/0"))
                        {
                            infoLogAnimator.SetBool("open", true);
                            infoLogText.text = "Exited game!";
                        }
                        else
                        {
                            loginLogic.errorAnimator.SetBool("open", true);

                            Debug.Log("Game exit Error");
                            loginLogic.error.text = "Game exit error Error";
                        }
                        
                        break;
                    case 16:
                        scoreBoardLogic.scoreBoardNotificationListen(commands[i]);
                        break;
                    case 17:
                        personalBoardLogic.usersPlayedWithNotificationListen(commands[i]);
                        break;
                    
                    case 18:
                        if (commands[i].Equals("18/0"))
                        {
                            Application.Quit();
                        }
                        else
                        {
                            loginLogic.errorAnimator.SetBool("open", true);

                            Debug.Log("Server error, account could not be deleted");
                            loginLogic.error.text = "Server error, account could not be deleted";
                        }
                        break;
                    case 19:
                        scoreBoardLogic.scoreBoardNotificationDATESListen(commands[i]);
                        break;
                }
                
                



                i++;
            }
        }
    }

    //Closes the info log window after 5 seconds
    private async Task closeInfoLog()
    {
        await Task.Delay(5000);
        infoLogAnimator.SetBool("open", false);
    }
    
    private async Task getConnected()
    {
        await Task.Delay(1000);
        Debug.Log("Get usrs");
        serverConnection.SendMessage("7/0");
       
        
    }

    //Connects to the server only if its is not connected yet. Notifies an error if not able to.
    async private void connectToServer()
    {
        serverConnection = ServerConnection.getInstance();
        if (!serverConnection.isConnected())
        {
            mainMenuLogic.mainMenuCanvas.enabled = false;


            int connectionSuccess = await Task.Run(() =>
            {
                return serverConnection.ConnectToServer();
            });


            if (connectionSuccess == 0)
            {
                Debug.Log("Connection with server successfull!");
                loginLogic.loginButton.enabled = true;
                loginLogic.registerButton.enabled = true;
                serverConnection.setConnected(true);

                //Listen forever
                listenForServer();
                
                
            }

            else if (connectionSuccess == -1)
            {

                loginLogic.errorAnimator.SetBool("open", true);

                Debug.Log("Connection Error");
                loginLogic.error.text = "Connection Error";
            }

        }
        //Scene has been loaded from when the game was already running, server was already connected.
        //Go directly to the main menu
        else
        {
            if (globalData.GamePaused)
            {
               
                globalData.GamePaused = false;
                loginLogic.loginCanvas.enabled = false;
                mainMenuLogic.mainMenuCanvas.enabled = true;
                getConnected();
                listenForServer();
                
                

            }
            
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Method executed when the application is quited. Sending a disconnection code to the server.
    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        serverConnection.SendMessage("0/");
    }

    //executed the scene is destroyed, for debugging purposes.
    void OnDestroy()
    {
        Debug.Log("Destroyed...");
    }
*/
}
