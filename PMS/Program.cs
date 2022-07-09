using System.ComponentModel.DataAnnotations;

using PMS.Contexts;
using PMS.Models;
using PMS.Lib;

using Sharprompt;

namespace PMS
{
    internal static class Program
    {
        private enum ParentCommand
        {
            Setup,
            Login
        }

        private enum SetupChildCommand
        {
            [Display(Name = "Create New User")]
            CreateUser,
            [Display(Name = "Migrate Database (creates any non-created tables)")]
            MigrateDatabase
        }
        
        static void Main(String[] args)
        {
            // cli prompt for migrate, etc - the arrow "dropdown"
            var parentCommand = Prompt.Select<ParentCommand>("What would you like to do?");
            var userContext = new UserContext();
            switch (parentCommand)
            {
                case ParentCommand.Setup:
                {
                    var setupChildCommand = Prompt.Select<SetupChildCommand>("What would you like to do?");
                    switch (setupChildCommand)
                    {
                        case SetupChildCommand.CreateUser:
                        {
                            // validate no spaces in username
                            var user = Prompt.Input<string>("Enter the user's name", validators: new[] {Validators.Required(), Validators.RegularExpression("^\\S*$", "No spaces are permitted in the username")});
                            var password = Prompt.Password("Enter the user's password", validators: new[] {Validators.Required(), Validators.MinLength(6)});
                            var confirmPassword = Prompt.Password("Confirm the user's password", validators: new[] {Validators.Required()});
                            if (password != confirmPassword)
                            {
                                Console.WriteLine("Passwords do not match");
                                return;
                            }
                            var rate = Prompt.Input<double>("Enter the user's hourly rate", validators: new[] {Validators.Required()});

                            password = Hash.GetHash(password);

                            try
                            {
                                var u = new User
                                {
                                    Username = user,
                                    HashedPassword = password,
                                    HourlyRate = rate,
                                };
                                userContext.User.Add(u);
                                userContext.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                return;
                            }

                            Console.WriteLine("User created");
                            break;
                        }
                        case SetupChildCommand.MigrateDatabase:
                        {
                            var result = Migration.RunMigration();
                            var message = result == -1 ? "Already migrated" : "Migrated";
                            Console.WriteLine(message);
                            break;
                        }
                    }
                    break;
                }
                case ParentCommand.Login:
                {
                    var username = Prompt.Input<string>("Enter the username", validators: new[] {Validators.Required()});
                    var password = Prompt.Password("Enter the password", validators: new[] {Validators.Required()});
                    password = Hash.GetHash(password);
                    var user = new User();
                    
                    if (userContext.User != null)
                    {
                        try
                        {
                            Login.LoginUser(ref user, username, password, userContext);
                            // find user based on username and hashed password
                            if (user == null)
                            {
                                Console.WriteLine("Invalid username or password");
                                return;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("An error occurred.");
                        return;
                    }

                    Console.WriteLine("You have been logged in. Enjoy your time!");
                    break;
                }
            }
        }
    }
}