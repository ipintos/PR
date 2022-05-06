using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class Chip
    {
        private int chipid;
        private User user;
        private string content; //TODO: controlar el largo del post en 280
        private List<string> image;//TODO: ver si poner 3 variables o una lista con tope 3 o que
        private DateTime datePosted;
        private List<Chip> replies;//TODO: se crea un arbol, ver si requiere ajustes adicionales

        public Chip(int chipid, User user, string content, List<string> image, DateTime datePosted, List<Chip> replies)
        {
            this.chipid = chipid;
            this.user = user;
            this.content = content;
            this.image = image;
            this.datePosted = datePosted;
            this.replies = replies;
        }
        
        //sin imagenes
        public Chip(int chipid, User user, string content, DateTime datePosted, List<Chip> replies)
        {
            this.chipid = chipid;
            this.user = user;
            this.content = content;
            //this.image = image;
            this.datePosted = datePosted;
            this.replies = replies;
        }



        public int ChipId { get { return chipid; } }

        public User User { get { return user; } }

        public string Content { get { return content; } }

        public List<string> Image { get { return image; } }

        public DateTime DatePosted { get { return datePosted; } }

        public List<Chip> Replies { get { return replies; } }
    }
}
