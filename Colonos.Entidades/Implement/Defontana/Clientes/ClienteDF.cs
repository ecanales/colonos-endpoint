using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Defontana
{
    public class ClienteDF
    {
     
        public string active {get;set;}
        public string isProvider {get;set;}
        public string lastName1 {get;set;}
        public string lastName2 {get;set;}
        public string city {get;set;}
        public string client {get;set;}
        public string legalCode {get;set;}
        public decimal? agreedDiscount { get; set; }
        public string address {get;set;}
        public string district {get;set;}
        public string email {get;set;}
        public string sendEmailDTE {get;set;}
        public string state {get;set;}
        public string fax {get;set;}
        public string business {get;set;}
        public string companyID {get;set;}
        public string fileID {get;set;}
        public string localID {get;set;}
        public string coinID {get;set;}
        public string paymentID {get;set;}
        public string productID {get;set;}
        public string rubroId {get;set;}
        public string docTypeID {get;set;}
        public string sellerID {get;set;}
        public string priceList {get;set;}
        public string name {get;set;}
        public string fantasyname {get;set;}
        public string country {get;set;}
        public string poBox {get;set;}
        public string provider {get;set;}
        public string phone {get;set;}
        public string productType {get;set;}
        public string tipRcgoDctoDocVta {get;set;}
        public string usaRcgoDctoDocVta {get;set;}
        public string webSite {get;set;}
        public string zipCode { get; set; }
        public paymentElectronicDF paymentElectronic { get; set; }

    }
}
