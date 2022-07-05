using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Net;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using PMS.Contexts;
using PMS.Models;
using PMS.Migrate;
using PMS.Lib;

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
                        var userContext = new UserContext();
                        userContext.User.Add(new User
                        {
                            Username = user,
                            HashedPassword = password,
                            HourlyRate = rate,
                        });
                        userContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }

                    Console.WriteLine("User created");
                }
                else if (setupChildCommand == _setupChildCommand.MigrateDatabase)
                {
                    var result = Migration.RunMigration();
                    var message = result == -1 ? "Already migrated" : "Migrated";
                    Console.WriteLine(message);
                }
            }
            else if (parentCommand == _parentCommand.Login)
            {
                
            }
        }
    }
}