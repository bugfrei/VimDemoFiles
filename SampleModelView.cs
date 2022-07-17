using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WfpBeispiele.Bases;

namespace WfpBeispiele.Listen
{
    public class ListenWindowViewModel : Bases.BaseViewModel
    {
        public ListenWindowViewModel()
        {
            AddPersonCommand = new DelegateCommand(AddPerson);
            Persons.Add(new Person("Max", "Muster", 1001, 123.4m));
            Persons.Add(new Person("Peter", "Lustig", 1025, 500.91m));
            Persons.Add(new Person("Hans", "Mayer", 2050, 100m));
            Persons.Add(new Person("Anna", "Schmidt", 1234, 99.99m));
        }

        private ObservableCollection<Person> persons = new ObservableCollection<Person>();
        public ObservableCollection<Person> Persons
        {
            get => persons;
            set
            {
                if (persons != value)
                {
                    persons = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        Person newPerson = new Person();
        public Person NewPerson
        {
            get => newPerson;
            set
            {
                if (newPerson != value)
                {
                    newPerson = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public DelegateCommand AddPersonCommand { get; set; }
        public void AddPerson(object parameter)
        {
            this.Persons.Add(newPerson);
            NewPerson = new Person();
        }
    }
}
