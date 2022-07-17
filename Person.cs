using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WfpBeispiele.Listen
{
    public class Person : Bases.NotifyableBaseObject
    {
        private string firstname;
        private string lastname;
        private int iD;
        private decimal cash;

        public string Firstname
        {
            get => firstname; set
            {
                if (firstname != value)
                {
                    firstname = value;
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged(nameof(Fullname));
                }
            }
        }

        public string Lastname
        {
            get => lastname; set
            {
                if (lastname != value)
                {
                    lastname = value;
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged(nameof(Fullname));
                }
            }
        }
        public string Fullname => $"{Firstname} {Lastname}";
        public int ID
        {
            get => iD; set
            {
                if (iD != value)
                {
                    iD = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        public decimal Cash
        {
            get => cash; set
            {
                if (cash != value)
                {
                    cash = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Person(string firstname, string lastname, int iD, decimal cash)
        {
            Firstname = firstname;
            Lastname = lastname;
            ID = iD;
            Cash = cash;
        }
        public Person()
        {
            Firstname = "";
            Lastname = "";
            ID = 0;
            Cash = 0m;
        }
    }
}
