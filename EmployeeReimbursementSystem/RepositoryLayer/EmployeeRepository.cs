/* JASON TEJADA    PROJECT 1 REPOSITORY LAYER CLASS    REVATURE
 * Desc:
 *          This class contains functions to interact with our database.
 *          GetEmployees returns a deserialized json employee database as a list
 *          of employees. PostEmployees serializes a list of employees into
 *          a json employee database. We will work with the list in the
 *          service class.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Data.SqlClient;

// Importing necessary layers
using ModelLayer;

// TODO Refactor
namespace RepositoryLayer
{
    public interface IEmployeeRepository {
        List<Employee> GetEmployees();
        void PostEmployees(List<Employee> employeeDB);
        Employee PostEmployee(string email, string password);
        Employee PostEmployee(string email, string password, int roleId);
        Employee GetEmployee(string email);
        Employee GetEmployee(int id);
        Employee LoginEmployee(string email, string password);
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        public List<Employee> GetEmployees() {
            if(File.Exists("EmployeeDatabase.json")) {
                return JsonSerializer.Deserialize<List<Employee>>(File.ReadAllText("EmployeeDatabase.json"))!;
            } else {
                return new List<Employee>();
            }
        }

        public void PostEmployees(List<Employee> employeeDb) {
            string serializedDb = JsonSerializer.Serialize(employeeDb);
            File.WriteAllText("EmployeeDatabase.json", serializedDb);
        }

        public Employee PostEmployee(string email, string password) {
            string conStirng = File.ReadAllText("../../ConString.txt");
            using(SqlConnection connection = new SqlConnection(conStirng)) {
                string insertEmployeeQuery = "INSERT INTO Employee (Email, Password, RoleId) VALUES (@email, @password, @RoleId);";
                SqlCommand command = new SqlCommand(insertEmployeeQuery, connection);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@RoleId", 0);
                try {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if(rowsAffected == 1) {
                        Console.WriteLine("Post Success");
                        return GetEmployee(email);
                    } else return null!;
                } catch (Exception e) {
                    Console.WriteLine("Insertion Failure\n" + e.Message);
                    return null!;
                }
            }
        }

        public Employee PostEmployee(string email, string password, int roleId) {
            string conStirng = File.ReadAllText("../../ConString.txt");
            using(SqlConnection connection = new SqlConnection(conStirng)) {
                string insertEmployeeQuery = "INSERT INTO Employee (Email, Password, RoleId) VALUES (@email, @password, @RoleId);";
                SqlCommand command = new SqlCommand(insertEmployeeQuery, connection);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@RoleId", roleId);
                try {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if(rowsAffected == 1) {
                        Console.WriteLine("Post Success");
                        return GetEmployee(email);
                    } else return null!;
                } catch (Exception e) {
                    Console.WriteLine("Insertion Failure\n" + e.Message);
                    return null!;
                }
            }
        }

        #region  // Get Methods: retrieve unique employee by email, id, or email & password
        public Employee GetEmployee(string email) {
            string conString = File.ReadAllText("../../ConString.txt");
            using(SqlConnection connection = new SqlConnection(conString)) {
                string queryEmployeeByEmail = "SELECT * FROM Employee WHERE Email = @email";
                SqlCommand command = new SqlCommand(queryEmployeeByEmail, connection);
                command.Parameters.AddWithValue("@Email", email);
                try {
                    connection.Open();
                    
                    using(SqlDataReader reader = command.ExecuteReader()) {
                        if(!reader.HasRows) return null!;
                        else {
                            reader.Read();
                            return new Employee(
                                (int)reader[0], 
                                (string)reader[1], 
                                (string)reader[2], 
                                (int)reader[3]
                            );
                        }
                    }
                } catch(Exception e) {
                    Console.WriteLine(e.Message);
                    return null!;
                }
            }
        }

        public Employee GetEmployee(int id) {
            string conString = File.ReadAllText("../../ConString.txt");
            using(SqlConnection connection = new SqlConnection(conString)) {
                string queryEmployeeByEmail = "SELECT * FROM Employee WHERE EmployeeId = @id";
                SqlCommand command = new SqlCommand(queryEmployeeByEmail, connection);
                command.Parameters.AddWithValue("@id", id);
                try {
                    connection.Open();
                    
                    using(SqlDataReader reader = command.ExecuteReader()) {
                        if(!reader.HasRows) return null!;
                        else {
                            reader.Read();
                            return new Employee(
                                (int)reader[0], 
                                (string)reader[1], 
                                (string)reader[2], 
                                (int)reader[3]
                            );
                        }
                    }
                } catch(Exception e) {
                    Console.WriteLine(e.Message);
                    return null!;
                }
            }
        }
        #endregion

        public Employee LoginEmployee(string email, string password) {
            string conString = File.ReadAllText("../../ConString.txt");
            using(SqlConnection connection = new SqlConnection(conString)) {
                string queryEmployeeByEmail = "SELECT * FROM Employee WHERE Email = @email AND Password = @password";
                SqlCommand command = new SqlCommand(queryEmployeeByEmail, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);
                try {
                    connection.Open();
                    
                    using(SqlDataReader reader = command.ExecuteReader()) {
                        if(!reader.HasRows) return null!;
                        else {
                            reader.Read();
                            return new Employee(
                                (int)reader[0], 
                                (string)reader[1], 
                                (string)reader[2], 
                                (int)reader[3]
                            );
                        }
                    }
                } catch(Exception e) {
                    Console.WriteLine(e.Message);
                    return null!;
                }
            }
        }
    }
}