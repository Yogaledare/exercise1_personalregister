using System.Text.RegularExpressions;

namespace exercise1_personalregister;

class Program {
    static void Main(string[] args) {
        var registry = new EmployeeRegistry();

        while (true) {
            PrintMenu();
            Console.WriteLine();
            Console.Write("$ ");
            var input = Console.ReadLine();

            var parsedCommand = ValidateInput(input);

            if (parsedCommand == null) {
                continue;
            }

            if (parsedCommand is QuitCommand) {
                Console.WriteLine("Exiting...");
                break;
            }

            parsedCommand.Execute(registry);
            Console.WriteLine();
        }
    }


    private enum Command {
        Add,
        List,
        Quit
    }


    // simple print, 
    private static void PrintMenu() {
        Console.WriteLine("Available commands: ");
        Console.WriteLine("add <Name> <Salary>");
        Console.WriteLine("list");
        Console.WriteLine("quit");
    }


    // validate input and parse into a command, if possible
    private static ICommand? ValidateInput(string? input) {
        if (string.IsNullOrWhiteSpace(input)) {
            Console.WriteLine("null input");
            return null;
        }

        var words = input.Split(' ');

        if (words.Length == 0) {
            Console.WriteLine("empty input");
            return null;
        }

        var firstWord = words[0];

        if (!Enum.TryParse(firstWord, true, out Command commandType)) {
            Console.WriteLine("could not parse command");
            return null;
        }

        if (commandType == Command.Add) {
            if (words.Length != 3) {
                Console.WriteLine("wrong # of inputs");
                return null;
            }

            var name = words[1];

            var pattern = @"[a-zA-Z]+";

            if (!Regex.IsMatch(name, pattern)) {
                Console.WriteLine("disallowed characters in name");
            }

            var salary = words[2];

            if (!int.TryParse(salary, out int parsedValue)) {
                Console.WriteLine("could not read salary");
                return null;
            }

            return new AddCommand(name, parsedValue);
        }


        if (commandType == Command.List) {
            if (words.Length != 1) {
                Console.WriteLine("wrong # of inputs");
                return null;
            }

            return new ListCommand();
        }


        if (commandType == Command.Quit) {
            if (words.Length != 1) {
                Console.WriteLine("wrong # of inputs");
                return null;
            }

            return new QuitCommand();
        }


        return null;
    }


    // interface to allow for easier extension of commands 
    private interface ICommand {
        public void Execute(EmployeeRegistry employeeRegistry);
    }


    private record AddCommand(string Name, int Salary) : ICommand {
        public void Execute(EmployeeRegistry employeeRegistry) {
            employeeRegistry.AddEmployee(Name, Salary);
            Console.WriteLine($"Added New Employee. Name: {Name}, Salary: {Salary}");
        }
    }


    private record ListCommand : ICommand {
        public void Execute(EmployeeRegistry employeeRegistry) {
            var employees = employeeRegistry.GetEmployees();

            Console.WriteLine("---------Employees---------");
            foreach (var employeeRecord in employees) {
                Console.WriteLine(employeeRecord);
            }

            Console.WriteLine("---------------------------");
        }
    }


    private record QuitCommand : ICommand {
        public void Execute(EmployeeRegistry employeeRegistry) {
        }
    }


    // export class for employee
    private record EmployeeRecord(int Id, string Name, int Salary) {
        public override string ToString() {
            return $"Id: {Id}, Name: {Name}, Salary: {Salary}";
        }
    };


    // employee "repository"
    private class EmployeeRegistry {
        private SortedSet<Employee> _employees = new();


        // POST one
        public void AddEmployee(string name, int salary) {
            var newEmployee = new Employee(name, salary);
            _employees.Add(newEmployee);
        }


        // GET all
        public List<EmployeeRecord> GetEmployees() {
            var output = new List<EmployeeRecord>(_employees.Count);

            foreach (var employee in _employees) {
                output.Add(new EmployeeRecord(employee.Id, employee.Name, employee.Salary));
            }

            return output;
        }


        private class Employee : IComparable<Employee> {
            private static int _lastId = 0;

            public int Id { get; }
            public string Name { get; set; }
            public int Salary { get; set; }


            public Employee(string name, int salary) {
                Name = name;
                Salary = salary;

                _lastId++;
                Id = _lastId;
            }

            public int CompareTo(Employee? other) {
                if (other == null) {
                    return 1;
                }

                return this.Id.CompareTo(other.Id);
            }
        }
    }
}