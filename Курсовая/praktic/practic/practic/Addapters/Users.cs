using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practic
{
    public class Users
    {
        public int UserID { get; set; }
        public string UserFirstName { get; set; }
        public string UserName { get; set; }
        public string UserThreeName { get; set; }
        public string UserPhone { get; set; }
        public string UserDateBirthday { get; set; }
        public string UserLogin {  get; set; }
        public string UserPassword { get; set; }
        public int UserRole { get; set; }


        public string Fio => $"{UserFirstName} {UserName} {UserThreeName}".Trim();
        public string RoleName
        {
            get
            {
                switch (UserRole)
                {
                    case 1: return "Администратор";
                    case 2: return "Менеджер";
                    case 3: return "Товаровед";
                    case 4: return "Директор";
                    case 5: return "Продавец";
                    default: return "Для этой роли еще нет формы!";
                }
            }
        }
    }
}
