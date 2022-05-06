using BusinessLogic;
using Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ClientFunctionalitiesResponse
    {
        private ChipperInstance _chipper;

        public ClientFunctionalitiesResponse()
        {
            _chipper = ChipperInstance.GetInstance();
        }

        public string Login(string username, string password)
        {
            string description = "Ocurrió un error al procesar la consulta.";
            string state = Protocol.ERROR_STATE;
            try
            {
                if (!_chipper.ValidateUser(username, password))
                {
                    description = "Usuario y contraseña no válidos.";
                    state = Protocol.ERROR_STATE;
                }
                else if (_chipper.IsBlockedUser(username))
                {
                    description = "El usuario se encuentra bloqueado";
                    state = Protocol.ERROR_STATE;
                }
                else
                {
                    description = _chipper.CreateSessionToken(username);
                    state = Protocol.OK_STATE;
                }
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_CLIENT_LOGIN, state, description);
            }
            catch (Exception)
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_CLIENT_LOGIN, state, description);
            }
        }

        public string CreateNewUser(string username, string password, string name, string lastname, string picture)
        {
            string description = "Ocurrió un error al procesar la consulta.";
            string state = Protocol.ERROR_STATE;
            try
            {
                User newUser = new(username, password, name, lastname, picture, new List<User>(), new List<User>(), new List<Chip>(), new List<Notification>());
                if (_chipper.Users.Contains(newUser))
                {
                    description = $"El usuario {username} ya existe.";
                    state = Protocol.ERROR_STATE;
                }
                else
                {
                    _chipper.AddUserToList(newUser);
                    description = $"El usuario {username} fue creado correctamente.";
                }
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_CLIENT_ADD_USER, Protocol.OK_STATE, description);
            }
            catch
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_CLIENT_ADD_USER, state, description);
            }
        }


        public string SearchUsers(string username, string name, string session)
        {
            string description = "Ocurrió un error al procesar la consulta.";
            string state = Protocol.ERROR_STATE;
            try
            {
                List<User> usersSearch = new List<User>();
                usersSearch = _chipper.Users.FindAll(u => (u.Name == name)); //agrega a la lista todos los que tienen el nombre name

                User userByUsername = new(username);
                if (_chipper.Users.Contains(userByUsername))
                {
                    usersSearch.Add(userByUsername);
                }

                if (usersSearch.Count > 0)
                {
                    description = string.Empty;
                    foreach (User u in usersSearch)
                    {
                        description += u.Username + "&";
                    }
                    state = Protocol.OK_STATE;
                }
                else
                {
                    description = "No se encontraron usuarios para los valores ingresados.";
                    state = Protocol.ERROR_STATE;
                }
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_SEARCH, state, description);
            }
            catch
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_CLIENT_ADD_USER, state, description);
            }
        }

        public string FollowUser(string username, string session)
        {
            try
            {
                User userFollow = _chipper.Users.Find(u => (u.Username == username));
                User user = GetLoggedUser(session);
                if (userFollow != null)
                {
                    user.Following.Add(userFollow); 
                    userFollow.Followers.Add(user);
                }
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_FOLLOW, Protocol.OK_STATE, "Siguiendo al usuario.");                 
            }
            catch
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_FOLLOW, Protocol.ERROR_STATE, "No se pudo seguir al usuario.");
            }
        }

        public string PublishChip(string chip, string session) 
        {
            try
            {
                User user = GetLoggedUser(session);                                
                Chip newChip = CreateNewChip(chip, user); //al crearlo ya lo agrega al usuario  logueado
                _chipper.Chips.Add(newChip); //lo agrega a la lista general de chips

                List<User> followers = _chipper.GetFollowers(user); //notificaciones para los seguidores
                foreach (User u in followers)
                {
                    int notificationid = _chipper.GetNotificationId() + 1;
                    Notification notification = new Notification(notificationid, newChip);

                    _chipper.GetNotifications(u).Add(notification);
                    _chipper.Notifications.Add(notification);

                    _chipper.SetNotificationId(notificationid);
                }

                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_PUBLISH_CHIP, Protocol.OK_STATE, "Chip publicado.");
            }
            catch
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_PUBLISH_CHIP, Protocol.ERROR_STATE, "Chip no pudo ser publicado.");
            }            
        }


        //create chip sin imagenes
        private Chip CreateNewChip (string chip, User user)
        {
            List<Chip> replies = new List<Chip>();
            
            int chipId;
            chipId = _chipper.GetChipId() + 1;
                       
            Chip newChip = new Chip(chipId, user, chip, DateTime.Now, replies);            
            user.Chips.Add(newChip); //lo agrega al user que lo publica

            _chipper.SetChipId(chipId);

            return newChip;
        }
        

        public string GetNotifications(string session)
        {
            try
            {               
                User user = GetLoggedUser(session);
                List<Notification> notifications = user.Notifications;                
                string reply = string.Empty;
                foreach (Notification n in notifications)
                {
                    reply = reply + n.NotificationId.ToString() + "|" + n.Chip.Content + "&";                                    
                }
                reply = reply.Remove(reply.Length - 1,1); //Elimna el ultimo &                
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_NOTIFICATION, Protocol.OK_STATE, reply);
            }
            catch
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_NOTIFICATION, Protocol.ERROR_STATE, "No se pudo obtener la lista de notificaciones.");
            }
        }

        public string GetUserProfile(string username)
        {
            try
            {
                string reply = string.Empty;
                int followers = 0;
                int following = 0;
                User userProfile = _chipper.Users.Find(u => (u.Username == username));
                if (userProfile != null)
                {
                    //los counts quedan vacios en lugar de cero, en caso que no tenga seguidores o seguidos. Hay que arreglar aca o del lado del processResponse
                    followers = followers + userProfile.Followers.Count;
                    following = following + userProfile.Following.Count;    
                    reply = userProfile.Username + "&" + userProfile.Name + "&" + userProfile.Lastname + "&" + followers + "&" + following + "&";
                    var orderedList = userProfile.Chips.OrderBy(c => c.DatePosted).Reverse().ToList();                        
                    foreach (Chip c in orderedList)
                    {
                        Console.WriteLine(c.Content);
                        reply = reply + c.Content + "|";
                    }
                    reply = reply.Remove(reply.Length - 1, 1); //Elimna el ultimo caracter, | si tiene chips, & sino tiene                
                }                
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_VIEW_PROFILE, Protocol.OK_STATE, reply);
            }
            catch
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_VIEW_PROFILE, Protocol.ERROR_STATE, "No se pudo obtener el perfil del usuario solicitado.");
            }            
        }

        public string ReplyChipList(string username)
        {
            try
            {
                string reply = string.Empty;
                User userReply = _chipper.Users.Find(u => (u.Username == username));
                if (userReply != null)
                {
                    reply = userReply.Username + "@"; //No devuelve el usuario logueado en el mensaje de respuesta.
                                                 //Se necesita el user al que se le responde, porque solo se envia chipid y contenido en la respuesta. Se necesita en la vuelta para 
                                                 //agregar al la lista de replies                                                 
                    foreach (Chip c in userReply.Chips)
                    {
                        reply = reply + c.ChipId + "|" + c.Content + "&";
                    }
                }
                reply = reply.Remove(reply.Length - 1, 1); //Elimna el ultimo &      
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_REPLY_CHIP_LIST, Protocol.OK_STATE, reply);
            }
            catch
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_REPLY_CHIP_LIST, Protocol.ERROR_STATE, "No se pudo obtener el perfil del usuario solicitado.");
            }
            
        }

        public string ReplyChip(string chipid, string chipreply, string session)
        {
            try
            {
                User user = GetLoggedUser(session);
                Chip chipReply = CreateNewChip(chipreply, user);
                _chipper.Chips.Add(chipReply); //lo agrega a la lista general de chips

                int chipIdOriginal = Convert.ToInt32(chipid);
                Chip chipOriginal = _chipper.Chips.Find(c => (c.ChipId == chipIdOriginal));
                chipOriginal.Replies.Add(chipReply); //lo agrega a la lista de replies del usario de la publiacacion original

                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_REPLY_CHIP, Protocol.OK_STATE, "Respuesta publicada.");
            }
            catch
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_REPLY_CHIP, Protocol.ERROR_STATE, "No se pudo publicar la respuesta.");
            }                     
        }

        public string Logout(string session)
        {
            try
            {
                User user = GetLoggedUser(session);
                _chipper.UsersLoggd.Remove(user);
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_LOGOUT, Protocol.OK_STATE, "Logout");
            }
            catch
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_LOGOUT, Protocol.ERROR_STATE, "Ocurrió un error al procesar la solicitud.");
            }
        }

        public string ReplyNotification(string notificationid, string chipreply, string session)
        {
            int notificationOriginal = Convert.ToInt32(notificationid);
            Notification notificationReplyChip = _chipper.Notifications.Find(n => (n.NotificationId == notificationOriginal));
            int chipOriginal = notificationReplyChip.Chip.ChipId;
            string chipid = Convert.ToString(chipOriginal);
            return ReplyChip(chipid, chipreply, session);            
        }

        public string CloseConnection()
        {
            return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_DISCONNECT, Protocol.OK_STATE, "La conexión fue finalizada correctamente.");
        }

        private static string BuildResponse(string method, int action, string state, string description)
        {
            return $"{method}{Protocol.MESSAGE_SEPARATOR}" +
                    $"{action}{Protocol.MESSAGE_SEPARATOR}" +
                    $"{state}{Protocol.MESSAGE_SEPARATOR}" +
                    $"{description}";
        }

        public bool IsLoggedUser(string token)
        {
            User user = _chipper.Users.Find(user => user.SessionToken == token);
            return _chipper.UsersLoggd.Contains(user);
        }

        private User GetLoggedUser(string token)
        {
            User user = _chipper.Users.Find(user => user.SessionToken == token);
            return user;
        }
    }
}

