namespace Communication
{
    public static class Protocol
    {
        public const int BACKLOG = 10;
        public const int FIXED_DATA_SIZE = 4; //bytes para el envío de un número, es un integer
        public const int TOKEN_DATA_SIZE = 8; //bytes para el envio del hash, también es un int por eso ocupa lo mismo
        public const int SEPARATOR_DATA_SIZE = 1; //bytes para el envio del separador, es un char
        public const int HEADER_DATA_SIZE = FIXED_DATA_SIZE + TOKEN_DATA_SIZE + SEPARATOR_DATA_SIZE; //bits para el envio del separador
        

        public const string CLIENT_EXIT_COMMAND = "quit";
        public const string SERVER_EXIT_COMMAND = "quit";
        public const string SERVER_SHOW_MENU_COMMAND = "menu";
        public const string CLIENT_SHOW_MENU_COMMAND = "menu";
        public const string CLEAR_CONSOLE_COMMAND = "clear";
        public const string CLIENT_END_CONNECTION_REQUEST_COMMAND = "exit";
        public const string ERROR_STATE = "ERROR";
        public const string OK_STATE = "OK";
        public const string METHOD_REQUEST = "REQ";
        public const string METHOD_RESPONSE = "RES";
        
        //constantes para las opciones del menú del cliente
        public const int ACTION_CLIENT_ADD_USER = 1;
        public const int ACTION_CLIENT_LOGIN = 2;
        public const int ACTION_SEARCH = 3;
        public const int ACTION_FOLLOW = 4;
        public const int ACTION_PUBLISH_CHIP = 5;
        public const int ACTION_NOTIFICATION = 6;
        public const int ACTION_NOTIFICATION_REPLY = 16;
        public const int ACTION_VIEW_PROFILE = 7;
        public const int ACTION_REPLY_CHIP_LIST = 8;
        public const int ACTION_REPLY_CHIP = 18;
        public const int ACTION_LOGOUT = 9;
        public const int ACTION_DISCONNECT = 10;

        //constantes para las opciones del menú del server
        public const int ACTION_SERVER_USERS = 1;
        public const int ACTION_SERVER_BLOCK_USER = 2;
        public const int ACTION_SERVER_UNLOCK_USER = 3;
        public const int ACTION_SERVER_SEARCH_CHIP = 4;
        public const int ACTION_SERVER_TOP_FOLLOWERS = 5;
        public const int ACTION_SERVER_TOP_ACTIVE = 6;

        public const int FIXED_FILE_SIZE = 8; //FixedFileSize = 8; //esto es poruqe la variable para el tamaño es de tipo long, que ocupa 8 bytes
        public const int MAX_PACKET_SIZE = 32768;//MaxPacketSize = 32768; // 32KB, pasarlo a 1500 bytes?
        public const string MESSAGE_SEPARATOR = "#";

        public static long CalculateFileParts(long fileSize)
        {
            var fileParts = fileSize / MAX_PACKET_SIZE;
            return fileParts * MAX_PACKET_SIZE == fileSize ? fileParts : fileParts + 1;
        }
    }
}
