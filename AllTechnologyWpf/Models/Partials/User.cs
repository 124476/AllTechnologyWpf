using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllTechnologyWpf.Models
{
    public partial class User
    {
        public string ColorPhoto
        {
            get
            {
                return "Gray8";
            }
        }

        public User MaxUser
        {
            get
            {
                var liderId = LiderId;
                User user = this;

                while(liderId != null)
                {
                    user = App.DB.User.FirstOrDefault(x => x.Id == liderId);
                    liderId = user.LiderId;
                }

                return user;
            }
        }
    }
}
