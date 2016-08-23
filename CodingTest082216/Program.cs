using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
//using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApplication1
{
    using System.Linq;
    using System.Collections;

    // I altered the person class, hope that's ok
    public class Person
    {
        public Person(ConsoleIO io)
        {
            Id = Db.people.Count + 1;
            io.WriteLine("What is the person's first name ?");
            FirstName = io.ReadLine();

            io.WriteLine("What is the person's last name ?");
            LastName = io.ReadLine();

            io.WriteLine("What is the person's age?");
            Age = io.ReadLineInt();

            // M or F
            while (Gender != "M" && Gender != "F")
            {
                io.WriteLine("What is the person's gender? [M or F]");
                Gender = io.ReadLine().ToUpper();
                if (Gender != "M" && Gender != "F")
                {
                    io.Write("Invalid input. Try again! ");
                }
            }
        
            //io.WriteLine("What is your dad's name ?");
            //p.DadName = Console.ReadLine();

            //io.WriteLine("What is your mom's name ?");
            //p.MomName = Console.ReadLine();
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public string Gender { get; set; }

        // Used to find individuals in the DB
        public int DadId { get; set; }
        public int MomId { get; set; }

        public virtual Person Dad { get; set; }
        public virtual Person Mom { get; set; }
    }

    // Interface for DI
    public interface IO
    {
        void WriteLine(string arg);

        string ReadLine();
    }

    // Service implementation
    public struct ConsoleIO : IO
    {
        public void WriteLine(string arg)
        {
            Console.WriteLine(arg);
        }

        public void WriteLine(string arg, object o)
        {
            Console.WriteLine(arg,o);
        }

        public void WriteLine(string arg, object o1, object o2)
        {
            Console.WriteLine(arg, o1, o2);
        }

        public void Write(string arg)
        {
            Console.Write(arg);
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public int ReadLineInt()
        {
            int i;
            String result = Console.ReadLine();
            while (!Int32.TryParse(result, out i))
            {
                Console.WriteLine("Not a valid number! Try again...");
                result = Console.ReadLine();
            }
            return int.Parse(result);
        }
    }

    public sealed class TestIO : IO
    {
        public void WriteLine(string arg)
        {
            Debug.WriteLine(arg);
        }

        public string ReadLine()
        {
            return string.Empty;
        }
    }

    public static class Db
    {
        public static List<Person> people = new List<Person>();
    }

    public class Solution
    {

        public static void Main(string[] args)
        {
            var io = new ConsoleIO();
            var solution = new Solution();
            var running = true;
            int input;

            while (running)
            {
                io.WriteLine("--- Select an action ---");
                io.WriteLine("1. Input a person's data");
                io.WriteLine("2. Set a person's parents");
                io.WriteLine("3. View a person's data");
                io.WriteLine("0. Exit");

                input = io.ReadLineInt();
                switch(input)
                {
                    case 0:
                        running = false;
                        break;
                    case 1:
                        Db.people.Add(new Person(io));
                        break;
                    case 2:
                        io.WriteLine("Whose parents are we setting?\nEnter a number:");
                        solution.ListAllPeople(io);
                        input = io.ReadLineInt();
                        solution.SetParents(input, io);
                        break;
                    case 3:
                        io.WriteLine("Select a person to view:");
                        solution.ListAllPeople(io);
                        input = io.ReadLineInt();
                        solution.ViewPerson(io, input);
                        break;
                    default:
                        break;
                }
                io.WriteLine("");
            }

            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }

        
        public void SetParents(int personId, ConsoleIO io)
        {
            if (Db.people.Count <= 1)
            {
                io.WriteLine("Not enough people are in the database!");
                return;
            }

            var person = Db.people.First(p => p.Id == personId);
            if (person == null)
            {
                io.WriteLine("No person found with that Id!");
                return;
            }

            io.WriteLine("Enter the ID number of {0}'s mother", person.FirstName);
            ListAllPeople(io);
            var id = io.ReadLineInt();
            var mother = Db.people.First(p => p.Id == id);
            if (mother == null)
            {
                io.WriteLine("No person with that Id exists.\n{0}'s mother has not been set!");
            }
            else
            {
                person.Mom = mother;
            }

            io.WriteLine("Enter the ID number of {0}'s father", person.FirstName);
            ListAllPeople(io);
            id = io.ReadLineInt();
            var father = Db.people.First(p => p.Id == id);
            if (father == null)
            {
                io.WriteLine("No person with that Id exists.\n{0}'s father has not been set!");
            }
            else
            {
                person.Dad = father;
            }
        }

        public void ListAllPeople(ConsoleIO io)
        {
            var sb = new StringBuilder();
            foreach (var person in Db.people)
            {
                sb.Append(person.Id)
                  .Append(": ")
                  .Append(person.FirstName)
                  .Append(" ")
                  .Append(person.LastName);

                io.WriteLine(sb.ToString());
                sb.Clear();
            }
        }


        public void ViewPerson(ConsoleIO io, int personId)
        {
            var p = Db.people.First(person => person.Id == personId);

            if (p == null)
            {
                io.WriteLine("No person with ID {0} exists in the database!", personId);
                return;
            }

            io.WriteLine("First Name is: {0}",p.FirstName);
            io.WriteLine("Last Name is: {0}", p.LastName);
            io.WriteLine("Age is: {0}", p.Age);
            io.WriteLine("You are a: {0}", p.Gender == "M" ? "Man" : "Woman");

            if (p.Dad != null)
            {
                io.WriteLine("This person's dad is {0} {1}", p.Dad.FirstName, p.Dad.LastName);
                io.WriteLine("This person's dad's age is {0}", p.Dad.Age);
            }
            else
            {
                io.WriteLine("No dad has been set for this person.");
            }

            if (p.Mom != null)
            {
                io.WriteLine("This person's dad is {0} {1}", p.Mom.FirstName, p.Mom.LastName);
                io.WriteLine("This person's dad's age is {0}", p.Mom.Age);
            }
            else
            {
                io.WriteLine("No mom has been set for this person.");
            }
        }
    }
}