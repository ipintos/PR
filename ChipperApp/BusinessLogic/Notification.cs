using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class Notification
    {

        private int notificationId;
        private Chip chip;

        public Notification(int id, Chip chip)
        {
            this.notificationId = id;
            this.chip = chip;
        }
        public Chip Chip { get { return chip; } set { chip = value; } }
        public int NotificationId { get { return notificationId; } set { notificationId = value; } }
    }
}