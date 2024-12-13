using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Defontana
{
    public class DocumentoDF
    {
        public string documentType { get; set; }
        public int firstFolio { get; set; }
        public int lastFolio { get; set; }
        public string externalDocumentID { get; set; }
        public EmissionDate emissionDate { get; set; }
        public EmissionDate firstFeePaid { get; set; }
        public string clientFile { get; set; }
        public string contactIndex { get; set; }
        public string rutMandante { get; set; }
        public string paymentCondition { get; set; }
        public string sellerFileId { get; set; }
        public Analysis clientAnalysis { get; set; }
        public string billingCoin { get; set; }
        public int billingRate { get; set; }
        public string shopId { get; set; }
        public string priceList { get; set; }
        public string giro { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public int contact { get; set; }
        public List<attachedDocument> attachedDocuments { get; set; }
        public Storage storage { get; set; }
        public List<Details> details { get; set; }
        public List<SaleTaxes> saleTaxes { get; set; }
        public List<string> ventaRecDesGlobal { get; set; }
        public string gloss { get; set; }
        public List<string> customFields { get; set; }
        public bool isTransferDocument { get; set; }


        public DocumentoDF()
        {
            firstFolio = 0;
            lastFolio = 0;
            externalDocumentID = "";
            rutMandante = "";
            shopId = "GVL000000000000";
            priceList = "1";
            contact = -1;
            isTransferDocument = true;
            clientAnalysis = new Analysis { accountNumber = "1110401001", businessCenter = "", classifier01 = "", classifier02 = "" };
            storage = new Storage { code = "", motive = "", storageAnalysis = new Analysis { accountNumber = "", businessCenter = "", classifier01 = "", classifier02 = "" } };
            saleTaxes = new List<SaleTaxes>();
            attachedDocuments = new List<attachedDocument>();
            ventaRecDesGlobal = new List<string>();
            customFields = new List<string>();
        }
    }
}
