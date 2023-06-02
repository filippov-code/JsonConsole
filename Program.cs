using JsonConsole.Repository;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace JsonConsole
{
    internal class Program
    {
        private const string Filename = "employees.json";
        private static List<Employee> employees;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Hello();
                return;
            }

            try 
            {
                using (StreamReader reader = File.OpenText(Filename))
                {
                    string json = reader.ReadToEnd();
                    employees = JsonConvert.DeserializeObject<List<Employee>>(json) ?? new();
                }
            }
            catch (FileNotFoundException ex)
            {
                File.Create(Filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            
            var parameters = new Dictionary<string, string>();
            for (int i = 1; i < args.Length; i++)
            {
                if (!args[i].Contains("Id") && !args[i].Contains("FirstName") && !args[i].Contains("LastName") && !args[i].Contains("Salary"))
                {
                    UnknownParameter(args[i]);
                    return;
                }
                string[] pair = args[i].Split(':');
                if (pair.Length == 2 && !string.IsNullOrEmpty(pair[0]) && !string.IsNullOrEmpty(pair[1]))
                {
                    parameters.Add(pair[0], pair[1]);
                }
                else
                {
                    IncorrectParameterFormat(args[i]);
                    return;
                }
            }

            switch (args[0])
            {
                case "-help":
                    Help();
                    break;
                case "-add":
                    Add(parameters);
                    break;
                case "-update":
                    Update(parameters);
                    break;
                case "-get":
                    Get(parameters);
                    break;
                case "-delete":
                    Delete(parameters);
                    break;
                case "-getall":
                    if (parameters.Count > 0)
                    {
                        IncorrectNumberOfParameters();
                        return;
                    }
                    GetAll();
                    break;
                default:
                    UnknownCommand(args[0]);
                    break;
            }
        }

        #region Commands
        private static void Add(Dictionary<string, string> parameters)
        {
            if (parameters.Count != 3)
            {
                IncorrectNumberOfParameters();
                return;
            }
            if (!parameters.ContainsKey("FirstName"))
            {
                MissingParameter("FirstName");
                return;
            }
            if (!parameters.ContainsKey("LastName"))
            {
                MissingParameter("LastName");
                return;
            }
            if (!parameters.ContainsKey("Salary"))
            {
                MissingParameter("Salary");
                return;
            }
            if (!decimal.TryParse(parameters["Salary"], out decimal salary))
            {
                IncorrectParameterFormat("Salary");
                return;
            }

            Employee employee = new Employee()
            {
                Id = employees.Count > 0 ? employees.Last().Id + 1 : 1,
                FirstName = parameters["FirstName"],
                LastName = parameters["LastName"],
                SalaryPerHour = salary
            };

            employees.Add(employee);

            Save();
        }

        private static void Update(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Id"))
            {
                MissingParameter("Id");
                return;
            }
            if (!int.TryParse(parameters["Id"], out int id))
            {
                IncorrectParameterFormat("Id");
                return;
            }
            Employee? employee = employees.FirstOrDefault(x => x.Id == id);
            if (employee == null)
            {
                EmployeeNotFound(id);
                return;
            }
            if (parameters.Count < 2)
            {
                NoParametersToChange();
                return;
            }
            if (parameters.ContainsKey("FirstName"))
            {
                employee.FirstName = parameters["FirstName"];
            }
            if (parameters.ContainsKey("LastName"))
            {
                employee.LastName = parameters["LastName"];
            }
            if (parameters.ContainsKey("Salary"))
            {
                if (decimal.TryParse(parameters["Salary"], out decimal salary))
                {
                    employee.SalaryPerHour = salary;
                }
                else
                {
                    IncorrectParameterFormat("Salary");
                    return;
                }
            }

            Save();
        }

        private static void Get(Dictionary<string, string> parameters)
        {
            if (parameters.Count != 1)
            {
                IncorrectNumberOfParameters();
                return;
            }
            if (!parameters.ContainsKey("Id"))
            {
                MissingParameter("Id");
                return;
            }
            if (!int.TryParse(parameters["Id"], out int id))
            {
                IncorrectParameterFormat("Id");
                return;
            }
            Employee? employee = employees.FirstOrDefault(x => x.Id == id);
            if (employee == null)
            {
                EmployeeNotFound(id);
                return;
            }
            Console.WriteLine(employee);
        }

        private static void Delete(Dictionary<string, string> parameters)
        {
            if (parameters.Count != 1)
            {
                IncorrectNumberOfParameters();
                return;
            }
            if (!parameters.ContainsKey("Id"))
            {
                MissingParameter("Id");
                return;
            }
            if (!int.TryParse(parameters["Id"], out int id))
            {
                IncorrectParameterFormat("Id");
                return;
            }
            Employee? employee = employees.FirstOrDefault(x => x.Id == id);
            if (employee == null)
            {
                EmployeeNotFound(id);
                return;
            }

            employees.Remove(employee);

            Save();
        }

        private static void GetAll()
        {
            foreach (var employee in employees)
                Console.WriteLine(employee);
        }
        #endregion

        #region ConsoleMessages
        private static void Hello()
        {
            Console.WriteLine("Hi, this program processes a text file containing a list of employees in JSON format. Call the -help command to get help.");
        }
        private static void Help()
        {
            Console.WriteLine(
                "The file for storing employee data is searched for and saved in the program folder. Filename: employees.json\n\n" + 
                "Available commands:\n" +
                "\t1. -add\n" +
                "\t\tAdds a new entry to the file.\n" +
                "\t\tAvailable parameters:\n" +
                "\t\t\tFirst Name - string\n" +
                "\t\t\tLastName - string\n" +
                "\t\t\tSalary - decimal, delimiter - '.'\n" +
                "\t\tRequest example: -add FirstName:John LastName:Doe Salary:100.50\n" +
                "\t2. -update\n" +
                "\t\tUpdates the record with the specified Id.\n" +
                "\t\tAvailable parameters:\n" +
                "\t\t\tId - int\n" +
                "\t\t\tFirst Name - string\n" +
                "\t\t\tLastName - string\n" +
                "\t\t\tSalary - decimal, delimiter - '.'\n" +
                "\t\tRequest example: -update Id:123 FirstName:James\n" +
                "\t3. -get\n" +
                "\t\tOutputs data about the user with the specified Id.\n" +
                "\t\tAvailable parameters:\n" +
                "\t\t\tId - int\n" +
                "\t\tRequest example: -get Id:123\n" +
                "\t4. -delete\n" +
                "\t\tDeletes an entry with the specified Id.\n" +
                "\t\tAvailable parameters:\n" +
                "\t\t\tId - int\n" +
                "\t\tRequest example: -get Id:123\n" +
                "\t5. -getall\n" +
                "\t\tDisplays information about all existing employees.\n" +
                "\t\tThere are no parameters available.\n" +
                "\t\tRequest example: -getall"
                );
        }
        private static void UnknownCommand(string command)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Unknown command: {command}. Call the -help command to get help.");
            Console.ResetColor();
        }
        private static void UnknownParameter(string parameter)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Unknown parameter: {parameter}. Call the -help command to get help.");
            Console.ResetColor();
        }
        private static void IncorrectParameterFormat(string parameter)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"The \"{parameter}\" parameter has an incorrect format. Call the -help command to get help.");
            Console.ResetColor();
        }
        private static void MissingParameter(string parameterName)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"The required parameter was missed: {parameterName}.");
            Console.ResetColor();
        }
        private static void IncorrectNumberOfParameters()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid number of parameters.");
            Console.ResetColor();
        }
        private static void EmployeeNotFound(int id)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"The employee with id:{id} was not found.");
            Console.ResetColor();
        }
        private static void NoParametersToChange()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Specify the parameters to change");
            Console.ResetColor();
        }
        #endregion

        private static void Save()
        {
            Console.WriteLine(employees.Count);
            try
            {
                using (StreamWriter file = File.CreateText(Filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, employees);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}