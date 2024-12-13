using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Defontana
{
    public class TransacciondeInventario
    {
        public int folio { get; set; }
        public string documentTypeId { get; set; }
        public string fiscalYear { get; set; }
        public string clientId { get; set; }
        public string providerId { get; set; }
        public string gloss { get; set; }
        public string originStowageId { get; set; }
        public string destinationStowageId { get; set; }
        public string reasonId { get; set; }
        public decimal total { get; set; }
        public AnalisysAjuste analysis { get; set; }
        public int referenceDocumentFolio { get; set; }
        public string referenceDocumentType { get; set; }
        public DateTime date { get; set; }
        public string externalDocumentID { get; set; }
        public List<DetailAjuste> details { get; set; }

        public TransacciondeInventario()
        {
            folio = 0;
            clientId = "";
            providerId = "";
            originStowageId = "";
            destinationStowageId = "";

            analysis = new AnalisysAjuste { againstEBusinessCenter = ""};
            referenceDocumentFolio = 0;
            referenceDocumentType = "";
            externalDocumentID = "";
            details = new List<DetailAjuste>();
        }
    }

}
