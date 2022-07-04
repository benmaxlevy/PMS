using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Net;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using PMS.Contexts;
using PMS.Models;

using Sharprompt;

namespace PMS.Main
{
    internal class Program
    {
        private enum _parentCommand
        {
            Setup,
            Login,
        }

        private enum _setupChildCommand
        {
            [Display(Name = "Create New User")]
            CreateUser,
            [Display(Name = "Migrate Database (creates any non-created tables)")]
            MigrateDatabase,
        }
        
        static void Main(String[] args)
        {
            // cli prompt for migrate, etc - the arrow "dropdown"
            var parentCommand = Prompt.Select<_parentCommand>("What would you like to do?");
            if (parentCommand == _parentCommand.Setup)
            {
                var setupChildCommand = Prompt.Select<_setupChildCommand>("What would you like to do?");
                if (setupChildCommand == _setupChildCommand.CreateUser)
                {
                    // validate no spaces in username
                    var user = Prompt.Input<string>("Enter the user's name", validators: new[] {Validators.Required(), Validators.RegularExpression("^\\S*$", "damn bro, ok")});
                    var password = Prompt.Password("Enter the user's password", validators: new[] {Validators.Required(), Validators.MinLength(6)});
                    var confirmPassword = Prompt.Password("Confirm the user's password", validators: new[] {Validators.Required()});
                    if (password != confirmPassword)
                    {
                        Console.WriteLine("Passwords do not match");
                        return;
                    }
                    var rate = Prompt.Input<double>("Enter the user's hourly rate", validators: new[] {Validators.Required()});
                    
                    using(SHA256 sha256Hash = SHA256.Create())
                    {
                        var passwordHash = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                        password = BitConverter.ToString(passwordHash).Replace("-", "").ToLower();
                    }
                    
                    var userContext = new UserContext();
                    userContext.User.Add(new User
                    {
                        Username = user,
                        HashedPassword = password,
                        HourlyRate = rate,
                    });
                    userContext.SaveChanges();
                    Console.WriteLine("User created");
                }
                else if (setupChildCommand == _setupChildCommand.MigrateDatabase)
                {
                    var result = Migrate.Migrate.RunMigration();
                    var message = result == -1 ? "Already migrated" : "Migrated";
                    Console.WriteLine(message);
                    return;
                }
            }
        }
    }
}