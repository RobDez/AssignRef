using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Requests.Games
{
    public class GameUpdateRequest : GameAddRequest, IModelIdentifier
    {
        public int Id { get; set; }
 
    }
}
