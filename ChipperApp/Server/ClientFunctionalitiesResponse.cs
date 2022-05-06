﻿using BusinessLogic;
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
            string description;
            try
            {
                if (!_chipper.ValidateUser(username, password))
                {
                    description = "Usuario y contraseña no válidos.";
                }
                else if (_chipper.IsBlockedUser(username))
                {
                    description = "El usuario se encuentra bloqueado";
                }
                else
                {
                    description = _chipper.CreateSessionToken(username);
                    Console.WriteLine($"Creó el token: {description}");
                }
                return BuildResponse(Protocol.METHOD_REQUEST, Protocol.ACTION_CLIENT_LOGIN, Protocol.OK_STATE, description);
            }
            catch (Exception)
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_LOGOUT, Protocol.ERROR_STATE, "Ocurrió un error al procesar la consulta.");
            }
        }

        public string CreateNewUser(string username, string password)
        {
            User newUser = new()
            {
                Username = username,
                Password = password,
            };
            _chipper.AddUserToList(newUser);
            return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_CLIENT_ADD_USER, Protocol.OK_STATE, $"El usuario {username} fue creado correctamente.");
        }

        public string SearchUsers(string username, string name, string session)
        {
            try
            {
                List<User> usersSearch = new List<User>();
                usersSearch = _chipper.users.FindAll(u => (u.Name == name)); //agrega a la lista todos los que tienen el nombre name
                User userFind = _chipper.users.Find(u => (u.Username == username)); //busca si hay usuario cuyo userName sea username
                if (userFind != null) //si existe el usuario con el userName buscado
                {
                    User userFindUnique = usersSearch.Find(u => (u.Username == userFind.Username)); //Verifica que no haya sido ingresado por nombre en la busqueda anterior
                    if (userFindUnique == null)
                        {
                            usersSearch.Add(userFind);
                        }
                }
                //string reply = "RES" + "#" + "03" + "#"; No se necesita armar la respuesta, lo hace el buildResponse
                string reply = String.Empty;
                foreach (User u in usersSearch)
                {                    
                    reply = reply + u.Username + "&";
                }
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_SEARCH, Protocol.OK_STATE, reply);
            }
            catch (Exception)
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_SEARCH, Protocol.ERROR_STATE, "Ocurrió un error en la búsqueda de usuario.");
            }
        }

        public string FollowUser(string username, string session)
        {
            try
            {
                User userFollow = _chipper.users.Find(u => (u.Username == username));
                if (userFollow != null)
                {
                    //userLogged.Following.Add(userFollow); //Necesito el usuario logueado para guardar en las listas de followes y following
                    //userFollow.Followers.Add(userLogged);
                }
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_FOLLOW, Protocol.OK_STATE, "Siguiendo al usuario."); 
                //debería responder con el usuario seguido o tal vez con la lista de usuarios seguidos?
            }
            catch
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_FOLLOW, Protocol.ERROR_STATE, "No se pudo seguir al usuario.");
            }
        }


        public string PublishChip(string username, string chip, string image, string session) //usuario logueado?
        {
            try
            {
                User user = _chipper.users.Find(u => (u.Username == username));
                Chip newChip = CreateNewChip(chip, session); //al crearlo ya lo agrega al usuario  logueado
                _chipper.chips.Add(newChip); //lo agrega a la lista general de chips

                List<User> followers = _chipper.GetFollowers(user); //notificaciones para los seguidores
                foreach (User u in followers)
                {
                    int notificationid = _chipper.GetNotificationId() + 1;
                    Notification notification = new Notification(notificationid, newChip);

                    _chipper.GetNotifications(u).Add(notification);
                    _chipper.Notification.Add(notification);

                    _chipper.SetNotificationId(notificationid);
                }

                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_PUBLISH_CHIP, Protocol.OK_STATE, "Chip publicado.");
            }
            catch
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_FOLLOW, Protocol.ERROR_STATE, "Chip no pudo ser publicado.");
            }            
        }


        //create chip sin imagenes
        public Chip CreateNewChip (string chip, string session)
        {
            List<Chip> replies = new List<Chip>();
            List<string> images = new List<string>();
            User user = GetLoggedUser(session);

            int chipId;
            chipId = _chipper.GetChipId() + 1;
            
            Chip newChip = new Chip(chipId, user, chip, images, DateTime.Now, replies);
            
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
                //string reply = "RES" + "#" + "06" + "#"; //No se necesita armar la respuesta, lo hace el buildResponse
                string reply = string.Empty;
                foreach (Notification n in notifications)
                {
                    reply = reply + n.NotificationId.ToString() + "|" + n.Chip.Content + "&";
                }
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
                User userProfile = _chipper.users.Find(u => (u.Username == username));
                if (userProfile != null)
                {
                    reply = reply + userProfile.Username + "&" + userProfile.Name + "&" + userProfile.Lastname + "&" + userProfile.Following.Count + "&" + userProfile.Followers.Count + "&";
                    var orderedList = userProfile.Chips.OrderBy(c => c.DatePosted).Reverse().ToList();                        
                    foreach (Chip c in orderedList)
                    {
                        Console.WriteLine(c.Content);
                        reply = reply + c.Content + "|";
                    }
                }
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_VIEW_PROFILE, Protocol.OK_STATE, reply);
            }
            catch
            {
                return BuildResponse(Protocol.METHOD_RESPONSE, Protocol.ACTION_NOTIFICATION, Protocol.ERROR_STATE, "No se pudo obtener el perfil del usuario solicitado.");
            }            
        }

        public string ReplyChipList(string username, string session)
        {
            try
            {
                string reply = string.Empty;
                User user = _chipper.users.Find(u => (u.Username == username));
                if (user != null)
                {
                    reply = user.Username + "@"; //No devuelve el usuario logueado en el mensaje de respuesta.
                                                 //Se necesita el user al que se le responde, porque solo se envia chipid y contenido en la respuesta. Se necesita en la vuelta para 
                                                 //agregar al la lista de replies                                                 
                    foreach (Chip c in user.Chips)
                    {
                        reply = reply + c.ChipId + "|" + c.Content + "&";
                    }
                }
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
                Chip chipReply = CreateNewChip(chipreply, session);
                _chipper.chips.Add(chipReply); //lo agrega a la lista general de chips

                int chipIdOriginal = Convert.ToInt32(chipid);
                Chip chipOriginal = _chipper.chips.Find(c => (c.ChipId == chipIdOriginal));
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

        public void CloseConnection()
        {
           /* return "CLIENT LOGOUT";*/
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
            Console.WriteLine($"el token es {token}");
            User user = _chipper.Users.Find(user => user.SessionToken == token);
            Console.WriteLine($"encuentra al usuario {_chipper.UsersLoggd.Contains(user)}");
            return _chipper.UsersLoggd.Contains(user);
        }

        public User GetLoggedUser(string token)
        {
            Console.WriteLine($"el token es {token}");
            User user = _chipper.Users.Find(user => user.SessionToken == token);
            Console.WriteLine($"encuentra al usuario {_chipper.UsersLoggd.Contains(user)}");
            return user;
        }
    }
}
