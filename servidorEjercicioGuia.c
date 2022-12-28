#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>

typedef struct {
	char name [20];
	int socket;
} Online;

typedef struct {
	Online online_users [100];
	int num;
} ListOnline;

typedef struct {
	int gameid;
	Online ingame_users [10];
	int num;
} Game;

typedef struct {
	Game game [100];
	int num;
}ListGames;

typedef struct {
	int socketnum;
	ListOnline* onlinelist;
	ListGames* listgames;
} TParam;

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
int ii = 0;
int sockets[100];
MYSQL *conn;
MYSQL_RES *resultado;
MYSQL_ROW row;

int AddOnline (ListOnline *list, char name[20], int socket){
	if (list->num == 100)
		return -1;
	else {
		strcpy(list->online_users[list->num].name, name);
		list->online_users[list->num].socket = socket;
		list->num++;
		return 0; 
	}
}

int GetPosition (ListOnline *list, char name[20]){
	int i = 0;
	int found = 0;
	while ((i<list->num) && !found)
	{
		if (strcmp(list->online_users[i].name, name) == 0)
			found =1;
		if (!found)
			i=i+1;
	}
	if (found)
		return i;
	else
		return -1;
}

int DeleteOnline (ListOnline *list, char name[20]){
	int pos = GetPosition (list, name);
	if (pos == -1)
		return -1;
	else {
		int i; 
		for (i=pos; i < list->num-1; i++)
		{
			list->online_users[i] = list->online_users[i+1];
			//strcpy(list->online_users[i].name, list ->online_users[i+1].name);
			//list->online_users[i].socket = list->online_users[i+1].socket;
		}
		list->num--;
		return 0;
	}
}

void GetOnline (ListOnline *list, char online[300]){
	sprintf (online, "%d", list->num);
	int i;
	if (list->num == 0)
		sprintf(online, "/null");
	else {
		for (i=0; i<list->num; i++)
			sprintf (online, "%s/%s", online, list->online_users[i].name);
	}
}

int NewAccount (char userdata[512])
{
	//MYSQL *conn;
	//MYSQL_RES *resultado;
	//MYSQL_ROW row;
	int err;
	int duplicated;
	char username[256];
	char password[256];
	char ages[4];
	char consulta [512];
	char consulta1 [512];
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Monopoly",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	char *p = strtok (userdata, "$");
	strcpy(username, p);
	p = strtok(NULL, "$");
	strcpy(password, p);
	p = strtok(NULL, "$");
	int age = atoi(p);
	
	strcpy (consulta, "SELECT * FROM Credentials WHERE playerID = '");
	strcat (consulta, username); 
	strcat (consulta, "' AND pass = '");
	strcat (consulta, password); 
	strcat (consulta, "';");
	
	printf("consulta = %s\n", consulta);
	
	err = mysql_query(conn, consulta);
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL){
		duplicated = 0;
	}
	else
		duplicated = 1;
	
	if (duplicated==0){
		strcpy (consulta1, "INSERT INTO Credentials VALUES ('");
		strcat (consulta1, username); 
		strcat (consulta1, "','");
		strcat (consulta1, password); 
		strcat (consulta1, "',");
		sprintf(ages, "%d", age);
		strcat (consulta1, ages); 
		strcat (consulta1, ");");
		
		printf("consulta = %s\n", consulta1);
		
		err = mysql_query(conn, consulta1);
		if (err!=0) {
			printf ("Error al introducir datos la base %u %s\n", 
					mysql_errno(conn), mysql_error(conn));
					return -1;
			exit (1);
		}
		
		return 0;
	}
	
	else
		printf("DID NOT WORK");
		return -2;
	
	//mysql_close (conn);
	exit(0);
	
}


int SignUp (char userpass[512])
{
	//MYSQL *conn;
	//MYSQL_RES *resultado;
	//MYSQL_ROW row;
	int err;
	char consulta [512];
	char username[256];
	char password[256];
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Monopoly",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	char *p = strtok (userpass, "$");
	strcpy(username, p);
	p = strtok(NULL, "$");
	strcpy(password, p);
	
	
	strcpy (consulta, "SELECT * FROM Credentials WHERE playerID = '");
	strcat (consulta, username); 
	strcat (consulta, "' AND pass = '");
	strcat (consulta, password); 
	strcat (consulta, "';");
	

	printf("consulta = %s\n", consulta);
	
	err = mysql_query(conn, consulta);
	if (err!=0) {
		printf ("Error al introducir datos la base %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return -1;
		exit (1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL){
		printf ("No se han obtenido datos en la consulta\n");
		return 0;
	}
	else
		return 1;
	//mysql_close (conn);
	exit(0);
	
}

char* CreateGame (char username[256], int socket, ListGames *listgames)
{
	//MYSQL *conn;
	//MYSQL_RES *resultado;
	//MYSQL_ROW row;
	int err;
	int currentid;
	int newgameid;
	char consulta [512];
	char consulta1 [512];
	char consulta2 [512];
	char consulta3 [512];
	Game game;
	
	
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return "-1";
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Monopoly",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return "-1";
		exit (1);
	}
	
	strcpy (consulta, "SELECT gameID FROM Games");
	printf("consulta = %s\n", consulta);
	
	err = mysql_query(conn, consulta);
	if (err!=0) {
		printf ("Error al introducir datos la base %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return "-1";
		exit (1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL){
		printf ("Primera partida en el servidor\n");
		newgameid = 0;
	}
	else
	{
		int highestgameid = 0;
		while (row !=NULL) {
			// la columna 2 contiene una palabra que es la edad
			// la convertimos a entero 
			currentid = atoi (row[0]);
			// las columnas 0 y 1 contienen DNI y nombre 
			if (currentid > highestgameid)
				highestgameid = currentid;
			// obtenemos la siguiente fila
			row = mysql_fetch_row (resultado);
		}
		
		newgameid = highestgameid+1;
		printf ("%d", newgameid);
	}
	char newgameids [16];
	strcpy (consulta1, "INSERT INTO Games (gameID, status, player1) VALUES (");
	sprintf(newgameids, "%d", newgameid);
	strcat (consulta1, newgameids);
	strcat (consulta1, ", 'Starting', '");
	strcat (consulta1, username);
	strcat (consulta1, "');");
	printf("consulta = %s\n", consulta1);
	
	
	err = mysql_query(conn, consulta1);
	if (err!=0) {
		printf ("Error al introducir datos la base %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		
		return "-1";
		exit (1);
	}
	else {
		printf("DID WORK");
		
	}
	
	strcpy (consulta2, "INSERT INTO PlayerStatus VALUES (");
	strcat (consulta2, newgameids);
	strcat (consulta2,", '");
	strcat (consulta2, username);
	strcat (consulta2, "', 2000000, 0, 0, 0, 1)");
	
	printf("consulta = %s\n", consulta2);
	
	err = mysql_query(conn, consulta2);
	if (err!=0) {
		printf ("Error al introducir datos la base %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		
		return "-1";
		exit (1);
	}
	else {
		printf("DID WORK\n");
	}
	
	strcpy (consulta3, "SELECT * FROM PlayerStatus WHERE playerID = '");
	strcat (consulta3, username);
	strcat (consulta3, "' AND gameID = ");
	strcat (consulta3, newgameids);
	strcat (consulta3, ";");
	
	printf("consulta = %s\n", consulta3);
	err = mysql_query(conn, consulta3);
	if (err!=0) {
		printf ("Error al introducir datos la base %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		
		return "-1";
		exit (1);
	}
	else {
		printf("DID WORK\n");
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	char* infop;
	char info[512];
		
	char returngameid[100];
	strcpy (returngameid, row[0]);

	char returnplayerid[100];
	strcpy (returnplayerid, row[1]);
	
	char returnmoney[8];
	strcpy (returnmoney, row[2]);
	
	char returnjail[2];
	strcpy (returnjail, row[5]);
	
	char returnposition[8];
	strcpy (returnposition, row[6]);
	
	strcpy (info, returngameid);
	strcat (info, "/");
	strcat (info, "Starting");
	strcat (info, "/");
	strcat (info, returnmoney);
	strcat (info, "/");
	strcat (info, returnjail);
	strcat (info, "/");
	strcat (info, returnposition);
	printf("info = %s\n", info);
	infop = info;
	
	Online PlayerInGame;
	strcpy(PlayerInGame.name, username);
	PlayerInGame.socket = socket;
	game.gameid = newgameid;
	game.ingame_users[0] = PlayerInGame;
	game.num=1;
	listgames->game[listgames->num]=game;
	listgames->num = listgames->num+1;
	
	return infop;
	
	//mysql_close (conn);
	exit(0);
}

char* GetGames(char username[256])
{
	//MYSQL *conn;
	//MYSQL_RES *resultado;
	//MYSQL_ROW row;
	int err;

	char consulta [512];

	
	
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return "-1";
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Monopoly",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return "-1";
		exit (1);
	}
	
	strcpy (consulta, "SELECT * FROM PlayerStatus WHERE playerID = '");
	strcat (consulta, username);
	strcat (consulta, "';");
	printf("consulta = %s\n", consulta);
	
	err = mysql_query(conn, consulta);
	if (err!=0) {
		printf ("Error al introducir datos la base %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		
		return "-1";
		exit (1);
	}
	else {
		printf("DID WORK\n");
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	char* infop;
	char info[2048];
	strcpy (info, "");
	if (row == NULL){
		strcpy(info, "-1");
		infop = info;
		printf("info = %s\n", info);
		return infop;
	}
	else
	{
		while (row !=NULL) {
			// la columna 2 contiene una palabra que es la edad
			// la convertimos a entero 
			char returngameid[100];
			strcpy (returngameid, row[0]);
			
			char returnplayerid[100];
			strcpy (returnplayerid, row[1]);
			
			char returnmoney[8];
			strcpy (returnmoney, row[2]);
			
			char returnjail[2];
			strcpy (returnjail, row[5]);
			
			char returnposition[8];
			strcpy (returnposition, row[6]);
			
			strcat (info, returngameid);
			strcat (info, "/");
			strcat (info, "Starting");
			strcat (info, "/");
			strcat (info, returnmoney);
			strcat (info, "/");
			strcat (info, returnjail);
			strcat (info, "/");
			strcat (info, returnposition);
			strcat (info, "$");
			
			row = mysql_fetch_row (resultado);
		}
		infop = info;
		printf("info = %s\n", info);
		//mysql_close (conn);
		return infop;
	}
	
	
}

int GetSocket (ListOnline *onlinelist, char nombre [512])
{
	int found = 0; 
	int i = 0;
	int k;
	while (i<onlinelist->num && found == 0)
	{
		k = strcmp(onlinelist->online_users[i].name, nombre);
		if (k==0)
		{
			found = 1;
			return onlinelist->online_users[i].socket;
		}
		else
			i=i+1;
	}
	if (found==0)
	{
		return -1;
	}
}

int AddUsertoGame (int gameid, char nombre[512], int socket, ListGames *listgames)
{
	//MYSQL *conn;
	//MYSQL_RES *resultado;
	//MYSQL_ROW row;

	int err;
	char consulta[512];
	char consulta1[512];
	
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Monopoly",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	else {
		printf("Sucessful connection to Mysql\n");
	}
	
	
	sprintf (consulta, "SELECT * FROM Games WHERE gameID = %d;", gameid);
	printf("consulta = %s\n", consulta);
	err = mysql_query(conn, consulta);
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	if (err!=0) {
		printf ("Error al introducir datos la base %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		
		return "-1";
		exit (1);
	}
	else {
		printf("DID WORK\n");
	
	int i = 3;
	int found = 0;
	while (i<8 && found ==0) {
		
		if (row[i] == NULL)
		{
			found = 1;
			int j=i-2;
			sprintf(consulta1, "UPDATE Games SET player%d = '%s' WHERE gameID = %d;",j,nombre,gameid);
			printf("consulta1 = %s\n", consulta1);
			
			err = mysql_query(conn, consulta1);
			if (err!=0) {
				printf ("Error al introducir datos la base %u %s\n", 
						mysql_errno(conn), mysql_error(conn));
				
				return "-1";
				exit (1);
			}
			else 
			{
				printf("DID WORK\n");
				int k = 0;
				int found1 = 0;
				while (k<listgames->num && found1==0)
				{
					if (listgames->game[k].gameid == gameid)
					{
						listgames->game[k].ingame_users[listgames->game[k].num].socket = socket;
						strcpy(listgames->game[k].ingame_users[listgames->game[k].num].name, nombre);
						listgames->game[k].num = listgames->game[k].num+1;
						found1 = 1;
					}
					
					else
					{
						k=k+1;
					}
				}	
				return 0;
			}
		}
		else
			i=i+1;
		
		
	}
	
	}	
}

int DeleteUserFromGame (char username[256], int gameid)
{
	int err;
	char consulta[512];
	char consulta1[512];
	char consulta2[512];
	
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Monopoly",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	
	sprintf (consulta, "SELECT * FROM Games WHERE gameID = %d;", gameid);
	printf("consulta = %s\n", consulta);
	err = mysql_query(conn, consulta);
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	if (err!=0) {
		printf ("Error al introducir datos la base %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		
		return -1;
		exit (1);
	}
	else {
		printf("DID WORK\n");
		
		int i = 3;
		while (i<8) {
			printf("We got here");
			if (strcmp(row[i],username) == 0){
				int j=i-2;
				sprintf(consulta1, "UPDATE Games SET player%d = '%s' WHERE gameID = %d;",j,NULL,gameid);
				printf("consulta1 = %s\n", consulta1);
				
				err = mysql_query(conn, consulta1);
				if (err!=0) {
					printf ("Error al introducir datos la base %u %s\n", 
							mysql_errno(conn), mysql_error(conn));
					
					return "-1";
					exit (1);
				}
				else {
					printf("DID WORK\n");
					return 0;
				}
			}
			else
				i=i+1;
			}
			sprintf(consulta2, "DELETE FROM PlayerStatus WHERE playerID = '%s' AND gameID = %d;", username, gameid);
			err = mysql_query(conn, consulta2);
		
		
			if (err!=0) {
				printf ("Error al introducir datos la base %u %s\n", 
						mysql_errno(conn), mysql_error(conn));
				
				return -1;
				exit (1);
			}
			else {
				printf("DID WORK\n");
				
			}
	}
		
		
}
	
	



void *AtenderCliente (TParam *par)
{
	int sock_conn;
	int *s;
	s=(int *) par->socketnum;
	sock_conn = s;
	
	char peticion[512];
	char respuesta[512];
	char respuesta1[512];
	char respuesta2[512];
	
	int ret;
	
	int end = 0;
	while (end==0){
		printf ("Escuchando \n");
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibida una petición\n");
		// Tenemos que añadirle la marca de fin de string 
		// para que no escriba lo que hay despues en el buffer
		peticion[ret]='\0';
		
		//Escribimos la peticion en la consola
		
		printf ("La petición es: %s\n",peticion);
		
		char *p = strtok(peticion, "/");
		int codigo =  atoi (p);

		p = strtok( NULL, "/");
		char nombre[512];
		strcpy (nombre, p);
		printf ("Codigo: %d, Nombre: %s\n", codigo, nombre);
		int sockt;
		
		if (codigo ==0) { //piden cerrar conexión
			
			close(sock_conn);
			end=1;
			
		}
		
		if (codigo ==1) { //piden añadir nuevo usuario a la base de datos
			int add = NewAccount(nombre);
			sprintf(respuesta, "1?%d", add);
			write (sock_conn,respuesta, strlen(respuesta));
			printf("%s\n", respuesta);
			//close(sock_conn);
			
		}
		
		if (codigo ==2) { //piden iniciar sesión 
			int signup = SignUp(nombre);
			sprintf(respuesta, "2?%d", signup);
			write (sock_conn,respuesta, strlen(respuesta));
			printf("%s\n", respuesta);
			if (signup == 1)
			{
				pthread_mutex_lock (&mutex);
				AddOnline(par->onlinelist, nombre, par->socketnum);
				pthread_mutex_unlock (&mutex);
				GetOnline (par->onlinelist, respuesta1);
				sprintf(respuesta2, "5?%s", respuesta1);
				printf("%s\n", respuesta2);
				int j;
				for (j=0; j<ii;j++)
					write (sockets[j], respuesta2, strlen(respuesta2));
			}
			//close(sock_conn);
			
		}
		
		if (codigo ==3) { //piden crear una partida 
			char* createdata = CreateGame(nombre, sock_conn, par->listgames);
			printf("El jugador %s ha creado la partida %d\n", par->listgames->game->ingame_users[0].name, par->listgames->game->gameid);
			strcpy(respuesta1, createdata);
			sprintf(respuesta, "3?%s", respuesta1);
			write (sock_conn,respuesta, strlen(respuesta));
			printf("%s\n", respuesta);
			//close(sock_conn);
			
		}
		
		if (codigo ==4) { //piden saber en que partidas estan  
			char* getdata = GetGames(nombre);
			strcpy(respuesta1, getdata);
			sprintf(respuesta, "4?%s", respuesta1);
			write (sock_conn,respuesta, strlen(respuesta));
			printf("%s\n", respuesta);
			//close(sock_conn);
			
		}
		
		if (codigo ==5) { //Obtener la lista de conectados 
			GetOnline (par->onlinelist, respuesta1);
			sprintf(respuesta, "5?%s", respuesta1);
			int j;
			for (j=0; j<ii;j++)
				write (sockets[j], respuesta, strlen(respuesta)); 
			
		}
		
		if (codigo ==6) { //Salir de la lista de conectados 
			pthread_mutex_lock (&mutex);
			int res = DeleteOnline (par->onlinelist, nombre);
			pthread_mutex_unlock (&mutex);
			sprintf(respuesta, "6?%d", res);
			int k;
			mysql_close (conn);
			for (k=0; k<ii;k++)
				write (sockets[k], respuesta, strlen(respuesta));
			
		}
		
		if (codigo ==7) { //Enviar invitación
			p = strtok( NULL, "/");
			char invitado[512];
			strcpy (invitado, p);
			printf("%s\n", invitado);
			sockt = GetSocket(par->onlinelist, invitado);
			p = strtok( NULL, "/");
			int gameid =  atoi (p);
			sprintf(respuesta, "7?%s/%d", nombre, gameid);
			write (sockt, respuesta, strlen(respuesta));
			printf("%s\n",respuesta);
			
		}
		if (codigo ==8) { //Respuesta invitación
			char u_envia[512];
			char u_recibe[512];
			strcpy (u_envia, nombre);
			p = strtok (NULL, "/");
			strcpy (u_recibe, p);
			p = strtok (NULL, "/");
			int gid = atoi(p);
			p = strtok (NULL, "/");
			int ad = atoi(p);
			int j=0;
			int i = 0;
			int found = 0;
			while (i<par->listgames->num && found==0)
			{
				if (par->listgames->game[i].gameid == gid)
				{
					printf("He encontrado la partida en la lista %d", par->listgames->game[i].gameid);
					found = 1;
				}
				else
					i=i+1;
			}
			
			if (ad==0)
			{
				sprintf(respuesta, "8?0/%s/", u_envia);
				//sockt = GetSocket(par->onlinelist, u_recibe);
				//write (sockt, respuesta, strlen(respuesta));
			}
			else if (ad==1)
			{
				
				int adduser = AddUsertoGame(gid, u_envia, sock_conn,par->listgames);
				sprintf(respuesta, "8?1/%s/", u_envia);
				//sockt = GetSocket(par->onlinelist, u_recibe);
				//write (sockt, respuesta, strlen(respuesta));
				j=0;
				while (j<par->listgames->game[i].num)
				{
					if (strcmp(u_envia, par->listgames->game[i].ingame_users[j].name)!=0)
					{
						strcat(respuesta, par->listgames->game[i].ingame_users[j].name);
						strcat(respuesta, "/");
					}
					printf("Añadido a la respuesta: %s\n", par->listgames->game[i].ingame_users[j].name);
					j=j+1;
					
				}
			}
			
			j=0;
			while (j<par->listgames->game[i].num)
			{
				write(par->listgames->game[i].ingame_users[j].socket, respuesta, strlen(respuesta));
				j=j+1;
			}
			printf("%s\n",respuesta);
			
		
			
		}
/*		if (codigo ==9) { *///Salir de una partida 
/*			p = strtok( NULL, "/");*/
/*			int gameid =  atoi (p);*/
/*			int result = DeleteUserFromGame(nombre, gameid);*/
/*			sprintf(respuesta, "9?%d", result);*/
/*			write (sock_conn, respuesta, strlen(respuesta));*/
/*			printf("%s\n",respuesta);*/
/*		}*/
		if (codigo =10) { //Siguiente turno mensaje recibido con la estructura 10/username/gameid
			p = strtok(NULL, "/");
			int gid = atoi(p);
			int i =0;
			int found =0;
			while (i<par->listgames->num && found==0)
			{
				if (par->listgames->game[i].gameid == gid)
				{
					printf("He encontrado la partida en la lista %d", par->listgames->game[i].gameid);
					found = 1;
				}
				else
					i=i+1;
			}
			
			found = 0; 
			int j = 0;
			int k;
			while (j<par->listgames->game[i].num && found == 0)
			{
				k = strcmp(par->listgames->game[i].ingame_users[j].name, nombre);
				if (k==0)
				{
					found = 1;
					if (j-1==par->listgames->game[i].num)
						j=0;
						
				}
				else
					j=j+1;
			}
			
			sprintf(respuesta, "10?%d/%s", gid, par->listgames->game[i].ingame_users[j].name);
			
			j=0;
			while (j<par->listgames->game[i].num)
			{
				write(par->listgames->game[i].ingame_users[j].socket, respuesta, strlen(respuesta));
				j=j+1;
			}
			printf("%s\n",respuesta);
		}
		
		if (codigo =11) { //Empieza la partida 
			p = strtok(NULL, "/");
			int gid = atoi(p);
			int i =0;
			int found =0;
			while (i<par->listgames->num && found==0)
			{
				if (par->listgames->game[i].gameid == gid)
				{
					printf("He encontrado la partida en la lista %d", par->listgames->game[i].gameid);
					found = 1;
				}
				else
					i=i+1;
			}
			
			sprintf(respuesta, "11?%d", gid);
			int j=0;
			while (j<par->listgames->game[i].num)
			{
				write(par->listgames->game[i].ingame_users[j].socket, respuesta, strlen(respuesta));
				j=j+1;
			}
			printf("%s\n",respuesta);
		}
		
	}
	
}

	


int main(int argc, char *argv[])
{
	ListOnline mylist;
	ListGames listgames;
	mylist.num = 0;	int sock_conn, sock_listen, ret;
	listgames.num = 0;
	struct sockaddr_in serv_adr;
	
	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	// Fem el bind al port
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	// asocia el socket a cualquiera de las IP de la m?quina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// escucharemos en el port 9050
	serv_adr.sin_port = htons(7064);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	//La cola de peticiones pendientes no podr? ser superior a 4
	if (listen(sock_listen, 4) < 0)
		printf("Error en el Listen");
	

	int max_threads = 5;
	pthread_t thread;
	TParam param [100];
	
	for (;;){
		printf("Escuchando...\n");
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexion\n");
		
		sockets[ii] = sock_conn;
		param[ii].socketnum = sockets[ii];
		param[ii].onlinelist = &mylist;
		param[ii].listgames = &listgames;
		
		pthread_create (&thread, NULL, AtenderCliente, &param[ii]);
		//char online[300];
		//GetOnline (&mylist, online);
		//printf ("Resultado: %s\n", online);
		ii=ii+1;
	}
	
	return 0;
}
	
	
	

