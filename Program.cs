using JsonConsole.Interfaces;
using JsonConsole.Storages;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace JsonConsole
{
    internal class Program
    {
        private const string Filename = "employees.json";
        private static IStorage<Employee> storage;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Hello();
                return;
            }

            try 
            {
                storage = new JsonStorage<Employee>(Filename);
            }
            catch (Exception ex)
            {
                WriteException(ex.Message);
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
                FirstName = parameters["FirstName"],
                LastName = parameters["LastName"],
                SalaryPerHour = salary
            };

            var addResult = storage.Add(employee);
            if (!addResult.Success)
            {
                WriteException(addResult.Message);
                return;
            }

            var saveResult = storage.Save();
            if (!saveResult.Success)
            {
                WriteException(saveResult.Message);
                return;
            }
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
            var getResult = storage.Get(id);
            if (!getResult.Success)
            {
                WriteException(getResult.Message);
                return;
            }
            Employee employee = getResult.Result!.First();
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

            var updateResult = storage.Update(employee);
            if (!updateResult.Success)
            {
                WriteException(updateResult.Message);
                return;
            }

            var saveResult = storage.Save();
            if (!saveResult.Success)
            {
                WriteException(saveResult.Message);
                return;
            }
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
            var getResult = storage.Get(id);
            if (!getResult.Success)
            {
                WriteException(getResult.Message);
                return;
            }

            Console.WriteLine(getResult.Result!.First());
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
            var getResult = storage.Get(id);
            if (!getResult.Success)
            {
                WriteException(getResult.Message);
                return;
            }

            var deleteResult = storage.Delete(id);
            if (!deleteResult.Success)
            {
                WriteException(deleteResult.Message);
                return;
            }

            var saveResult = storage.Save();
            if (!saveResult.Success)
            {
                WriteException(saveResult.Message);
                return;
            }
        }

        private static void GetAll()
        {
            var getAllResult = storage.GetAll();
            if (!getAllResult.Success)
            {
                WriteException(getAllResult.Message);
                return;
            }

            foreach (var employee in getAllResult.Result!)
                Console.WriteLine(employee);
        }
        #endregion

        #region ConsoleMessages
        private static void Hello()
            => Console.WriteLine("Hi, this program processes a text file containing a list of employees in JSON format. Call the -help command to get help.");
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
        private static void NoParametersToChange()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Specify the parameters to change");
            Console.ResetColor();
        }
        private static void WriteException(string exceptionMessasge)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exceptionMessasge);
            Console.ResetColor();
        }
        #endregion
    }
}